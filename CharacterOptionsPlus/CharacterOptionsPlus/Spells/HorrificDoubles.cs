using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Craft;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Abilities.Blueprints.BlueprintAbilityAreaEffect;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class HorrificDoubles
  {
    private const string FeatureName = "HorrificDoubles";

    internal const string DisplayName = "HorrificDoubles.Name";
    private const string Description = "HorrificDoubles.Description";

    private const string AreaName = "HorrificDoubles.Area";
    private const string BuffName = "HorrificDoubles.Buff";
    private const string FailedSaveName = "HorrificDoubles.Save.Failed";
    private const string ShakenName = "HorrificDoubles.Shaken";
    private const string ShakenImmunityName = "HorrificDoubles.Shaken.Immunity";
    private const string ShakenDescription = "HorrificDoubles.Shaken.Description";
    private const string FrightenedImmunityName = "HorrificDoubles.Frightened.Immunity";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.HorrificDoublesSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("HorrificDoubles.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(ShakenName, Guids.HorrificDoublesShaken).Configure();
      BuffConfigurator.New(FailedSaveName, Guids.HorrificDoublesFailedSave).Configure();
      BuffConfigurator.New(ShakenImmunityName, Guids.HorrificDoublesShakenImmunity).Configure();
      BuffConfigurator.New(FrightenedImmunityName, Guids.HorrificDoublesFrightenedImmunity).Configure();
      AbilityAreaEffectConfigurator.New(AreaName, Guids.HorrificDoublesArea).Configure();
      BuffConfigurator.New(BuffName, Guids.HorrificDoublesBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.HorrificDoublesSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = AbilityRefs.Lich1FalseGraceAbility.Reference.Get().Icon;
      var shaken = BuffConfigurator.New(ShakenName, Guids.HorrificDoublesShaken)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Stack)
        .AddSpellDescriptorComponent(SpellDescriptor.MindAffecting)
        .AddFactContextActions(activated: ActionsBuilder.New().ApplyBuffPermanent(Shaken, asChild: true))
        .Configure();

      var failedSave = BuffConfigurator.New(FailedSaveName, Guids.HorrificDoublesFailedSave)
        .SetDisplayName(DisplayName)
        .SetDescription(ShakenDescription)
        .SetIcon(icon)
        .SetStacking(StackingType.Stack)
        .AddSpellDescriptorComponent(SpellDescriptor.MindAffecting)
        .AddComponent<HorrifiedComponent>()
        .Configure();

      var shakenImmunity = BuffConfigurator.New(ShakenImmunityName, Guids.HorrificDoublesShakenImmunity)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Stack)
        .Configure();

      BuffConfigurator.New(FrightenedImmunityName, Guids.HorrificDoublesFrightenedImmunity)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Stack)
        .Configure();

      var area = AbilityAreaEffectConfigurator.New(AreaName, Guids.HorrificDoublesArea)
        .SetAffectEnemies(true)
        .SetTargetType(TargetType.Enemy)
        .SetSize(60.Feet())
        .SetShape(AreaEffectShape.Cylinder)
        .AddAbilityAreaEffectRunAction(
          unitExit: ActionsBuilder.New().RemoveBuff(shaken),
          unitEnter: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New().HasBuffFromThisAreaEffect(buff: failedSave),
              ifTrue: ActionsBuilder.New().ApplyBuffPermanent(shaken),
              ifFalse: ActionsBuilder.New()
                .Conditional(
                  ConditionsBuilder.New().HasBuffFromThisAreaEffect(buff: shakenImmunity),
                  ifFalse: ActionsBuilder.New()
                    .SavingThrow(
                      SavingThrowType.Will,
                      onResult: ActionsBuilder.New()
                        .ConditionalSaved(
                          succeed: ActionsBuilder.New().ApplyBuffPermanent(shakenImmunity),
                          failed: ActionsBuilder.New()
                            .ApplyBuffPermanent(failedSave)
                            .ApplyBuffPermanent(shaken))))))
        .Configure();

      var buff = BuffConfigurator.New(BuffName, Guids.HorrificDoublesBuff)
        .CopyFrom(BuffRefs.MirrorImageBuff, c => true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .AddAreaEffect(areaEffect: area)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.HorrificDoublesSpell,
          SpellSchool.Illusion,
          canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetLocalizedDuration(Duration.MinutePerLevel)
        .SetLocalizedSavingThrow(SavingThrow.WillNegates)
        .SetRange(AbilityRange.Personal)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.SelfTouch)
        .SetActionType(CommandType.Standard)
        .AllowTargeting(self: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(level: 4, SpellList.Bard, SpellList.Bloodrager, SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes)))
        .AddCraftInfoComponent(
          aOEType: CraftAOE.AOE,
          savingThrow: CraftSavingThrow.Will,
          spellType: CraftSpellType.Buff)
        .Configure();
    }

    private static BlueprintBuff _shaken;
    private static BlueprintBuff Shaken
    {
      get
      {
        _shaken ??= BuffRefs.Shaken.Reference.Get();
        return _shaken;
      }
    }
    private static BlueprintBuff _frightenedImmunity;
    private static BlueprintBuff FrightenedImmunity
    {
      get
      {
        _frightenedImmunity ??= BlueprintTool.Get<BlueprintBuff>(Guids.HorrificDoublesFrightenedImmunity);
        return _frightenedImmunity;
      }
    }

    [TypeId("8f520a35-dbc0-4d3d-9650-dba44f14a40f")]
    private class HorrifiedComponent : UnitBuffComponentDelegate, IInitiatorRulebookHandler<RuleAttackRoll>
    {
      public void OnEventAboutToTrigger(RuleAttackRoll evt) { }

      public void OnEventDidTrigger(RuleAttackRoll evt)
      {
        try
        {
          if (evt.Target != Context.MaybeCaster)
          {
            Logger.Verbose(() => "Target is not caster");
            return;
          }

          if (evt.HitMirrorImageIndex < 1)
          {
            Logger.Verbose(() => "Missed images");
            return;
          }

          if (Owner.HasFact(FrightenedImmunity))
          {
            Logger.Verbose(() => "Already hit an image");
          }

          // Hit an image, make a will save (but only the first time)
          Buff.StoreFact(Owner.AddBuff(FrightenedImmunity, Buff.Context));

          if (Rulebook.Trigger(CreateSavingThrow(Owner)).IsPassed
              && (!Buff.Context.HasMetamagic(Metamagic.Persistent)
                || Rulebook.Trigger(CreateSavingThrow(Owner, persistent: true)).IsPassed))
          {
            Logger.Verbose(() => $"{Owner} passed their save");
            return;
          }

          Buff.StoreFact(Owner.AddBuff(BuffRefs.Frightened.Reference.Get(), Buff.Context, 1.Rounds().Seconds));
          Context.TriggerRule<RuleDealStatDamage>(
            new(Context.MaybeCaster, Owner, StatType.Wisdom, new DiceFormula(1, DiceType.D3), bonus: 0));
        }
        catch (Exception e)
        {
          Logger.LogException("HorrifiedComponent.OnEventDidTrigger", e);
        }
      }

      private RuleSavingThrow CreateSavingThrow(UnitEntityData target, bool persistent = false)
      {
        return
          new(target, SavingThrowType.Will, Buff.Context.Params.DC)
          {
            Buff = Buff.Blueprint,
            PersistentSpell = persistent
          };
      }
    }
  }
}
