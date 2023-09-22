using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.BasicEx;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Craft;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class FleshwormInfestation
  {
    private const string FeatureName = "FleshwormInfestation";

    internal const string DisplayName = "FleshwormInfestation.Name";
    private const string Description = "FleshwormInfestation.Description";

    private const string BuffName = "FleshwormInfestation.Buff";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.FleshwormInfestationSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("FleshwormInfestation.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.FleshwormInfestationBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.FleshwormInfestationSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.FleshwormInfestationBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .AddFactContextActions(
          newRound: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New()
                .HasFact(BuffRefs.ProtectionFromEvilBuff.ToString(), negate: true)
                .HasFact(BuffRefs.AuraOfProtectionFromEvilEffectBuff.ToString(), negate: true),
              ifTrue: ActionsBuilder.New()
                .SavingThrow(
                  SavingThrowType.Fortitude,
                  onResult: ActionsBuilder.New()
                    .ConditionalSaved(
                      succeed: ActionsBuilder.New().ApplyBuff(BuffRefs.Sickened.ToString(), ContextDuration.Fixed(1)),
                      failed: ActionsBuilder.New()
                        .ApplyBuff(BuffRefs.Staggered.ToString(), ContextDuration.Fixed(1))
                        .DealDamage(DamageTypes.Untyped(), ContextDice.Value(DiceType.D6, diceCount: 1))
                        .DealDamageToAbility(StatType.Dexterity, ContextDice.Value(DiceType.One, diceCount: 2))))))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.FleshwormInfestationSpell,
          SpellSchool.Conjuration,
          canSpecialize: true,
          SpellDescriptor.Summoning,
          SpellDescriptor.Evil)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.Contagion.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.RoundPerLevel)
        .SetLocalizedSavingThrow(SavingThrow.FortPartial)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Extend,
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
        .AddToSpellLists(
          level: 4,
          SpellList.Cleric,
          SpellList.Inquisitor,
          SpellList.Warpriest,
          SpellList.Witch,
          SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank())))
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.Fortitude,
          spellType: CraftSpellType.Damage)
        .Configure();
    }
  }
}
