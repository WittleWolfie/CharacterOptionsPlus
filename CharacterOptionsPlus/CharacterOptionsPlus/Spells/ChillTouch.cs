using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using static CharacterOptionsPlus.UnitParts.UnitPartTouchCharges;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class ChillTouch
  {
    private const string FeatureName = "ChillTouch";
    private const string EffectName = "ChillTouch.Effect";

    internal const string DisplayName = "ChillTouch.Name";
    private const string Description = "ChillTouch.Description";
    private const string TouchName = "ChillTouch.Touch.Name";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ChillTouchSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ChillTouch.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(EffectName, Guids.ChillTouchEffect).Configure();
      AbilityConfigurator.New(FeatureName, Guids.ChillTouchSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = AbilityRefs.ArmyMagusChillTouch.Reference.Get().Icon;
      var effectAbility = AbilityConfigurator.NewSpell(
          EffectName, Guids.ChillTouchEffect, SpellSchool.Necromancy, canSpecialize: false)
        .SetDisplayName(TouchName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetLocalizedSavingThrow(Common.SavingThrowFortPartialOrWillNegates)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalCold,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Piercing)
        .AddComponent(new TouchCharges(ContextValues.Rank()))
        .AddAbilityDeliverTouch(touchWeapon: ItemWeaponRefs.TouchItem.ToString())
        .AddAbilityEffectRunAction(
          actions:
            ActionsBuilder.New()
              .Conditional(
                ConditionsBuilder.New().HasFact(FeatureRefs.UndeadType.ToString()),
                ifTrue: ActionsBuilder.New()
                  .SavingThrow(
                    SavingThrowType.Will,
                    onResult: ActionsBuilder.New()
                      .ConditionalSaved(
                        failed: ActionsBuilder.New()
                          .ApplyBuff(
                            Guids.PanickedBuff,
                            ContextDuration.VariableDice(DiceType.D4, diceCount: 1, bonus: ContextValues.Rank())))),
                ifFalse: ActionsBuilder.New()
                  .DealDamage(
                    DamageTypes.Energy(DamageEnergyType.NegativeEnergy),
                    ContextDice.Value(DiceType.D6, diceCount: ContextValues.Constant(1)))
                  .SavingThrow(
                    SavingThrowType.Fortitude,
                    onResult: ActionsBuilder.New()
                      .ConditionalSaved(
                        failed: ActionsBuilder.New()
                          .DealDamageToAbility(StatType.Strength, ContextDice.Value(DiceType.One, diceCount: 1))))))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.ChillTouchSpell, SpellSchool.Necromancy, canSpecialize: true)
        .CopyFrom(effectAbility)
        .SetDisplayName(DisplayName)
        .SetShouldTurnToTarget(true)
        .AddToSpellLists(
          level: 1, SpellList.Bloodrager, SpellList.Magus, SpellList.Shaman, SpellList.Witch, SpellList.Wizard)
        .AddAbilityEffectStickyTouch(touchDeliveryAbility: effectAbility)
        .Configure(delayed: true);
    }
  }
}
