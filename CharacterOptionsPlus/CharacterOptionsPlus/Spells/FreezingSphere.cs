using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Assets;
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
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class FreezingSphere
  {
    private const string FeatureName = "FreezingSphere";

    internal const string DisplayName = "FreezingSphere.Name";
    private const string Description = "FreezingSphere.Description";

    private const string ProjectileName = "FreezingSphere.Projectile";

    // Effect from Kinetic_IceSphere00
    private const string AreaEffectFxSource = "b23302f818cab9a4f9b57b821195ed01";
    private const string AreaEffectFx = "8bb1eeb9-1255-4c36-bf22-7c01a7af4ad7";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

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

      ProjectileConfigurator.New(ProjectileName, Guids.FreezingSphereProjectile).Configure();
      AbilityConfigurator.New(FeatureName, Guids.FreezingSphereSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AssetTool.RegisterDynamicPrefabLink(AreaEffectFx, AreaEffectFxSource, ModifyFx);
      var projectile = ProjectileConfigurator.New(ProjectileName, Guids.FreezingSphereProjectile)
        .CopyFrom(ProjectileRefs.Kinetic_IceSphere00_Projectile)
        .ModifyProjectileHit(hit => hit.HitFx = new() { AssetId = AreaEffectFx})
        .SetSpeed(10f)
        .Configure();

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
      {
        Logger.Verbose("Enabling Selective Metamagic");
        availableMetamagic.Add(Metamagic.Selective);
      }

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
        .SetIcon(AbilityRefs.BlizzardBlastAbility.Reference.Get().Icon)
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
        .AddAbilityTargetsAround(
          radius: 40.Feet(),
          targetType:  alwaysSelective ? TargetType.Enemy : TargetType.Any,
          spreadSpeed: 45.Feet())
        .AddAbilityEffectRunAction(attack)
        .Configure();
    }

    private static void ModifyFx(GameObject iceSphere)
    {
      iceSphere.transform.localScale = new(1.75f, 1.0f, 1.75f);
      iceSphere.transform.Find("StonesBig").localScale = new(1.5f, 1.0f, 1.5f);
      iceSphere.transform.Find("IceSpike").localScale = new(3.5f, 1.0f, 3.5f);
      iceSphere.transform.Find("Shockwave_FastBottom").localScale = new(3.5f, 1.0f, 3.5f);
      //iceSphere.transform.Find("ProjectorDecal (1)_Start").localScale = new(1.5f, 1.0f, 1.5f);
      iceSphere.transform.Find("Splashes (1)").localScale = new(1.75f, 1.0f, 1.75f);
      iceSphere.transform.Find("DropsSmall").localScale = new(1.75f, 1.0f, 1.75f);
      iceSphere.transform.Find("Rescale").localScale = new(1.75f, 1.0f, 1.75f);
    }
  }
}
