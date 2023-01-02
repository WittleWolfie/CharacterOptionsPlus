using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class Wrath
  {
    private const string FeatureName = "Wrath";

    internal const string DisplayName = "Wrath.Name";
    private const string Description = "Wrath.Description";

    private const string BuffName = "Wrath.Buff";
    private const string BuffSelfName = "Wrath.Buff.Self";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "wrath.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.WrathSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("Wrath.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.WrathBuff).Configure();
      BuffConfigurator.New(BuffSelfName, Guids.WrathSelfBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.WrathSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.WrathBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 3).WithStartPlusDivStepProgression(3))
        .AddAttackBonusAgainstTarget(
          value: ContextValues.Rank(), checkCaster: true, descriptor: ModifierDescriptor.Morale)
        .AddDamageBonusAgainstTarget(value: ContextValues.Rank(), checkCaster: true)
        .AddComponent(new SpellPenBonusAgainstTarget(ContextValues.Rank()))
        .Configure();

      var selfBuff = BuffConfigurator.New(BuffSelfName, Guids.WrathSelfBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddComponent<WrathfulCritical>()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.WrathSpell,
          SpellSchool.Enchantment,
          canSpecialize: true,
          SpellDescriptor.Compulsion,
          SpellDescriptor.Emotion,
          SpellDescriptor.MindAffecting)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.OneMinute)
        .SetRange(AbilityRange.Long)
        .AllowTargeting(enemies: true)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Quicken,
          Metamagic.Extend,
          (Metamagic)CustomMetamagic.Encouraging)
        .AddToSpellLists(level: 1, SpellList.Inquisitor, SpellList.LichInquisitorMinor)
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff: buff, ContextDuration.Fixed(1, rate: DurationRate.Minutes), isNotDispelable: true)
            .ApplyBuff(buff: selfBuff, ContextDuration.Fixed(1, rate: DurationRate.Minutes), toCaster: true))
        .Configure();
    }

    [TypeId("e004e2f3-84f7-4a74-923e-c2d97397bc8e")]
    private class WrathfulCritical : UnitBuffComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>
    {
      private static BlueprintBuff _wrathBuff;
      private static BlueprintBuff WrathBuff
      {
        get
        {
          _wrathBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.WrathBuff);
          return _wrathBuff;
        }
      }

      public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
      {
        try
        {
          if (Context.Params.CasterLevel < 12 || evt.AttackWithWeapon?.Target?.HasFact(WrathBuff) != true)
          {
            Logger.Verbose("Critical edge does not apply");
            return;
          }

          evt.DoubleCriticalEdge = true;
        }
        catch (Exception e)
        {
          Logger.LogException("WrathfulCritical.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }
    }
  }
}
