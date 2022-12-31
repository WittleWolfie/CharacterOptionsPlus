using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Actions;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class FrozenNote
  {
    private const string FeatureName = "FrozenNote";

    internal const string DisplayName = "FrozenNote.Name";
    private const string Description = "FrozenNote.Description";

    private const string BuffName = "FrozenNote.Buff";
    private const string AreaName = "FrozenNote.Area";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "frozennote.png";

    // Battle Medication
    private const string AreaEffect = "3a0228650295f6a40bc335385a929a07";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.FrozenNoteSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("FrozenNote.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityAreaEffectConfigurator.New(AreaName, Guids.FrozenNoteArea).Configure();
      BuffConfigurator.New(BuffName, Guids.FrozenNoteBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.FrozenNoteSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var paralyzeBuff = BuffRefs.ParalyzedMindAffecting.ToString();
      var applyParalyze = ActionsBuilder.New().ApplyBuffPermanent(paralyzeBuff, isFromSpell: true);
      var areaEffect = ActionsBuilder.New()
        .Conditional(
          ConditionsBuilder.New().UseOr().IsCaster().HasBuffFromThisAreaEffect(paralyzeBuff),
          ifFalse: ActionsBuilder.New()
            .Add<SpellResistanceCheck>(
              a => a.OnResistFail = ActionsBuilder.New()
                .Add<ConditionalHitDice>(
                  a =>
                  {
                    a.HighValue = ContextValues.Rank();
                    a.LowValue = ContextValues.Rank(AbilityRankType.StatBonus);
                    a.OnInBetween = ActionsBuilder.New()
                      .SavingThrow(
                        SavingThrowType.Will, onResult: ActionsBuilder.New().ConditionalSaved(failed: applyParalyze))
                      .Build();
                    a.OnLower = applyParalyze.Build();
                  })
                .Build()));
      var area = AbilityAreaEffectConfigurator.New(AreaName, Guids.FrozenNoteArea)
        .SetSize(30.Feet())
        .SetShape(AreaEffectShape.Cylinder)
        .SetFx(AreaEffect)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel().WithBonusValueProgression(4))
        .AddContextRankConfig(
          ContextRankConfigs.CasterLevel(type: AbilityRankType.StatBonus).WithBonusValueProgression(-4))
        .AddAbilityAreaEffectRunAction(
          unitEnter: areaEffect,
          round: areaEffect,
          unitExit: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New().HasBuffFromThisAreaEffect(paralyzeBuff),
              ifTrue: ActionsBuilder.New().RemoveBuff(paralyzeBuff)))
        .Configure();

      var buff = BuffConfigurator.New(BuffName, Guids.FrozenNoteBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddAreaEffect(areaEffect: area)
        .AddComponent<FrozenNoteComponent>()
        .AddNotDispelable()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.FrozenNoteSpell,
          SpellSchool.Enchantment,
          canSpecialize: true,
          SpellDescriptor.Compulsion,
          SpellDescriptor.MindAffecting,
          SpellDescriptor.Sonic)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(self: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetLocalizedDuration(Duration.RoundPerLevel)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Selective,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(level: 5, SpellList.Bard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank()), toCaster: true))
        .Configure();
    }

    private class FrozenNoteComponent :
      UnitBuffComponentDelegate,
      IUnitCommandStartHandler,
      ITargetRulebookHandler<RuleDealDamage>
    {
      public void HandleUnitCommandDidStart(UnitCommand command)
      {
        try
        {
          if (command.Type == CommandType.Free || !command.CreatedByPlayer)
            return;

          Logger.NativeLog($"{Owner.CharacterName} took an action {command.Type}, interrupting Frozen Note");
          Buff.Remove();
        }
        catch (Exception e)
        {
          Logger.LogException("FrozenNoteComponent.HandleUnitCommandDidStart", e);
        }
      }

      public void OnEventAboutToTrigger(RuleDealDamage evt) { }

      public void OnEventDidTrigger(RuleDealDamage evt)
      {
        try
        {
          if (evt.Result > 0)
          {
            Logger.NativeLog($"{Owner.CharacterName} took damage, interrupting Frozen Note");
            Buff.Remove();
          }
        }
        catch (Exception e)
        {
          Logger.LogException("FrozenNoteComponent.HandleUnitMovement", e);
        }
      }
    }
  }
}