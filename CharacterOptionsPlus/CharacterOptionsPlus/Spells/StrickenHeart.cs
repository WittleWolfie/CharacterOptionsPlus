using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Conditions;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Craft;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class StrickenHeart
  {
    private const string FeatureName = "StrickenHeart";

    internal const string DisplayName = "StrickenHeart.Name";
    private const string Description = "StrickenHeart.Description";

    private const string TouchEffectName = "StricknHeart.Touch";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.StrickenHeartSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("StrickenHeart.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(TouchEffectName, Guids.StrickenHeartEffect).Configure();
      AbilityConfigurator.New(FeatureName, Guids.StrickenHeartSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var staggered = BuffRefs.Staggered.ToString();
      var effect = AbilityConfigurator.NewSpell(
          TouchEffectName,
          Guids.StrickenHeartEffect,
          SpellSchool.Necromancy,
          canSpecialize: true,
          SpellDescriptor.Death)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.DeathClutch.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.OneRound)
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
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalCold,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Piercing)
        .AddAbilityDeliverTouch(touchWeapon: ItemWeaponRefs.TouchItem.ToString())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .DealDamage(
              DamageTypes.Energy(DamageEnergyType.NegativeEnergy), ContextDice.Value(DiceType.D6, diceCount: 2))
            .Conditional(
              ConditionsBuilder.New().Add<IsImmuneToPrecisionDamage>(),
              ifFalse: ActionsBuilder.New()
                .Conditional(
                  ConditionsBuilder.New().Add<DidAttackCrit>(),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(staggered, ContextDuration.Fixed(1, rate: DurationRate.Minutes)),
                  ifFalse: ActionsBuilder.New().ApplyBuff(staggered, ContextDuration.Fixed(1)))))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.StrickenHeartSpell,
          SpellSchool.Necromancy,
          canSpecialize: true,
          SpellDescriptor.Death)
        .CopyFrom(effect)
        .SetShouldTurnToTarget(true)
        .AddToSpellLists(
          level: 2,
          SpellList.Inquisitor,
          SpellList.LichInquisitorMinor,
          SpellList.Shaman,
          SpellList.Wizard,
          SpellList.Witch)
        .AddAbilityEffectStickyTouch(touchDeliveryAbility: effect)
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.None,
          spellType: CraftSpellType.Damage)
        .Configure(delayed: true);
    }

    [TypeId("0b243cd9-bfe7-4445-847e-bb4a7370cc65")]
    private class DidAttackCrit : ContextCondition
    {
      public override bool CheckCondition()
      {
        try
        {
          var attack = Context.SourceAbilityContext.RulebookContext.LastEvent<RuleAttackRoll>();
          if (attack is null)
          {
            Logger.Warning("No attack roll");
            return false;
          }

          return attack.IsCriticalConfirmed;
        }
        catch (Exception e)
        {
          Logger.LogException("DidAttackCrit.CheckCondition", e);
        }
        return false;
      }

      public override string GetConditionCaption()
      {
        return "Checks if the last attack was a crit";
      }
    }
  }
}
