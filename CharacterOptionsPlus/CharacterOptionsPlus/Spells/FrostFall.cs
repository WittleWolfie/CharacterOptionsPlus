using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Actions;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Utility;
using System;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class FrostFall
  {
    private const string FeatureName = "FrostFall";

    internal const string DisplayName = "FrostFall.Name";
    private const string Description = "FrostFall.Description";

    private const string AreaName = "FrostFall.Area";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "frostfall.png";

    // A 20 ft "cold puddle"
    private const string AreaEffectFxSource = "fd21d914e9f6f5e4faa77365549ad0a7";
    private const string AreaEffectFx = "53dc3c4b-055d-4150-bc98-6940f910b9ab";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.FrostFallSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("FrostFall.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityAreaEffectConfigurator.New(AreaName, Guids.FrostFallAoE).Configure();
      AbilityConfigurator.New(FeatureName, Guids.FrostFallSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AssetTool.RegisterDynamicPrefabLink(AreaEffectFx, AreaEffectFxSource, ModifyFx);
      var area = AbilityAreaEffectConfigurator.New(AreaName, Guids.FrostFallAoE)
        .CopyFrom(AbilityAreaEffectRefs.GreaseArea)
        .SetFx(AreaEffectFx)
        .AddSpellDescriptorComponent(SpellDescriptor.Cold | SpellDescriptor.Ground)
        .AddComponent(
          new AbilityAreaEffectRunActionExtended(
            onSpawn: ActionsBuilder.New()
              .Add<SpellResistanceCheck>(
                c => c.OnResistFail =
                  ActionsBuilder.New()
                    .DealDamage(
                      DamageTypes.Energy(DamageEnergyType.Cold), ContextDice.Value(DiceType.D6, diceCount: 2))
                    .SavingThrow(
                      SavingThrowType.Fortitude,
                      onResult: ActionsBuilder.New()
                        .ConditionalSaved(
                          failed: ActionsBuilder.New()
                            .ApplyBuff(BuffRefs.Staggered.ToString(), ContextDuration.Fixed(1))))
                    .Build()),
            newRound: ActionsBuilder.New()
              .Add<SpellResistanceCheck>(
                c => c.OnResistFail =
                  ActionsBuilder.New()
                    .SavingThrow(
                      SavingThrowType.Fortitude,
                      onResult: ActionsBuilder.New()
                        .DealDamage(
                          DamageTypes.Energy(DamageEnergyType.Cold),
                          ContextDice.Value(DiceType.D6, diceCount: 1),
                          halfIfSaved: true))
                    .Build())))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.FrostFallSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Cold,
          SpellDescriptor.Ground)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(friends: true, enemies: true, self: true, point: true)
        .SetSpellResistance()
        .SetShouldTurnToTarget()
        .SetAnimation(CastAnimationStyle.Point)
        .SetLocalizedDuration(Common.DurationRoundPerTwoLevels)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.Bolstered,
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          Metamagic.Selective,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Piercing,
          (Metamagic)CustomMetamagic.Rime)
        .AddToSpellLists(level: 2, SpellList.Druid, SpellList.Witch, SpellList.Wizard)
        .AddAbilityAoERadius(radius: 10.Feet())
        .AddContextRankConfig(ContextRankConfigs.CasterLevel().WithDiv2Progression())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New().SpawnAreaEffect(area, ContextDuration.Variable(ContextValues.Rank())))
        .Configure();
    }

    private static void ModifyFx(GameObject puddle)
    {
      UnityEngine.Object.DestroyImmediate(puddle.transform.Find("Transform/ProjectorCollision_big").gameObject); // Remove unwanted particle effects
      puddle.transform.localScale = new(0.55f, 1.0f, 0.55f); // Scale from 20ft to 10ft
    }
  }
}
