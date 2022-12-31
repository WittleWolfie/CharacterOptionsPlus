using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.Utility;
using System;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Actions.Builder.ContextEx;

namespace CharacterOptionsPlus.Spells
{
  internal class ScreamingFlames
  {
    private const string FeatureName = "ScreamingFlames";

    internal const string DisplayName = "ScreamingFlames.Name";
    private const string Description = "ScreamingFlames.Description";

    private const string ProjectileName = "ScreamingFlames.Projectile";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ScreamingFlamesSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ScreamingFlames.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      ProjectileConfigurator.New(ProjectileName, Guids.ScreamingFlamesProjectile).Configure();
      AbilityConfigurator.New(FeatureName, Guids.ScreamingFlamesSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var projectile = ProjectileConfigurator.New(ProjectileName, Guids.ScreamingFlamesProjectile)
        .CopyFrom(ProjectileRefs.Fireball00)
        .SetView("cb13d9f65003c1f4497a3919f507fb26") // ScreamingFlames00
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.ScreamingFlamesSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Evil,
          SpellDescriptor.MindAffecting,
          SpellDescriptor.Fire)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.Demon2FlamesOfTheAbyssAbility.Reference.Get().Icon)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true, point: true, friends: true)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetSpellResistance()
        .SetShouldTurnToTarget()
        .SetAvailableMetamagic(
          Metamagic.Bolstered,
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Burning,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalCold,
          (Metamagic)CustomMetamagic.Intensified,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(level: 3, SpellList.Cleric)
        .AddToSpellList(level: 3, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddAbilityAoERadius(radius: 15.Feet())
        .AddAbilityDeliverProjectile(type: AbilityProjectileType.Simple, projectiles: new() { projectile })
        .AddAbilityTargetsAround(
          radius: 15.Feet(),
          spreadSpeed: 20.Feet(),
          targetType: TargetType.Any)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 5).WithDivStepProgression(2))
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .DealDamage(
              DamageTypes.Energy(DamageEnergyType.Fire),
              ContextDice.Value(DiceType.D8, diceCount: ContextValues.Rank()),
              halfIfSaved: true)
            .SavingThrow(
              SavingThrowType.Will,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  failed: ActionsBuilder.New()
                    .DealDamageToAbility(StatType.Wisdom, ContextDice.Value(DiceType.D3, diceCount: 1)))),
          savingThrowType: SavingThrowType.Reflex)
        .Configure();
    }
  }
}
