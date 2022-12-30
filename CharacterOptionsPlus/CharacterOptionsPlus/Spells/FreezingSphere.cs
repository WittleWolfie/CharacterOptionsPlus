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
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class FreezingSphere
  {
    private const string FeatureName = "FreezingSphere";

    internal const string DisplayName = "FreezingSphere.Name";
    private const string Description = "FreezingSphere.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "burningdisarm.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.FreezingSphereSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("FreezingSphere.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.FreezingSphereSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var availableMetamagic =
        new List<Metamagic>()
        {
          Metamagic.Bolstered,
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Intensified,
          (Metamagic)CustomMetamagic.Piercing,
          (Metamagic)CustomMetamagic.Rime,
        };

      var alwaysSelective = Settings.IsEnabled(Homebrew.SelectiveFreezingSphere);
      if (!alwaysSelective)
        availableMetamagic.Add(Metamagic.Selective);

      var attack = ActionsBuilder.New()
        .SavingThrow(
          SavingThrowType.Reflex,
          onResult: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New().HasFact(fact: FeatureRefs.SubtypeWater.ToString()),
              ifTrue: ActionsBuilder.New()
                .DealDamage(
                  DamageTypes.Energy(DamageEnergyType.Cold),
                  ContextDice.Value(DiceType.D8, diceCount: ContextValues.Rank()),
                  halfIfSaved: true)
                .ApplyBuff(BuffRefs.Staggered.ToString(), ContextDuration.FixedDice(DiceType.D4)),
              ifFalse: ActionsBuilder.New()
                .DealDamage(
                  DamageTypes.Energy(DamageEnergyType.Cold),
                  ContextDice.Value(DiceType.D6, diceCount: ContextValues.Rank()),
                  halfIfSaved: true)));
      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.FreezingSphereSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Cold)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetRange(AbilityRange.Long)
        .AllowTargeting(enemies: true, point: true)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetSpellResistance()
        .SetShouldTurnToTarget()
        .SetAvailableMetamagic(availableMetamagic.ToArray())
        .AddToSpellLists(level: 6, SpellList.Magus, SpellList.Wizard)
        .AddAbilityAoERadius(radius: 40.Feet())
        .AddAbilityDeliverProjectile(
          type: AbilityProjectileType.Simple,
          projectiles: new() { ProjectileRefs.Kinetic_IceSphere00_Projectile.ToString() })
        .AddAbilityEffectRunAction(
          alwaysSelective
            ? ActionsBuilder.New().Conditional(ConditionsBuilder.New().IsEnemy(), ifTrue: attack)
            : attack)
        .Configure();
    }

  }
}
