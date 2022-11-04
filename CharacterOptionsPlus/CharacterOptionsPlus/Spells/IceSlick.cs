using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Actions;
using CharacterOptionsPlus.Util;
using Kingmaker;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class IceSlick
  {
    private const string FeatureName = "IceSlick";

    internal const string DisplayName = "IceSlick.Name";
    private const string Description = "IceSlick.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "iceslick.png";

    private const string AreaEffect = "IceSlick.AoE";
    private const string BuffName = "IceSlick.Buff";

    // A 20 ft "cold puddle"
    private const string AreaEffectFxSource = "fd21d914e9f6f5e4faa77365549ad0a7";
    private const string AreaEffectFx = "c1ef4fc5-e5ea-43b7-a9d4-cbb4be41516a";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.IceSlickSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("IceSlick.Configure", e);
      }
    }
    
    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.IceSlickAoEBuff).Configure();
      AbilityAreaEffectConfigurator.New(AreaEffect, Guids.IceSlickAoE).Configure();
      AbilityConfigurator.New(FeatureName, Guids.IceSlickSpell).Configure();
    }

    private static readonly ActionsBuilder ApplyProne =
      ActionsBuilder.New()
        .ApplyBuff(BuffRefs.Prone.ToString(), durationValue: ContextDuration.Fixed(1), isNotDispelable: true);

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      BuffConfigurator.New(BuffName, Guids.IceSlickAoEBuff)
        .CopyFrom(BuffRefs.GreaseBuff)
        .AddSpellDescriptorComponent(SpellDescriptor.Ground | SpellDescriptor.MovementImpairing)
        .AddFactContextActions(
          newRound:
            ActionsBuilder.New()
              .SavingThrow(
                SavingThrowType.Reflex, onResult: ActionsBuilder.New().ConditionalSaved(failed: ApplyProne)))
        .Configure();

      // This handles updating the look of the effect
      AssetTool.RegisterDynamicPrefabLink(AreaEffectFx, AreaEffectFxSource, ModifyFx);
      var area = AbilityAreaEffectConfigurator.New(AreaEffect, Guids.IceSlickAoE)
        .CopyFrom(AbilityAreaEffectRefs.GreaseArea)
        .SetFx(AreaEffectFx)
        .AddComponent<IceSlickEffect>()
        .AddSpellDescriptorComponent(SpellDescriptor.Cold | SpellDescriptor.Ground | SpellDescriptor.MovementImpairing)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 10))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.IceSlickSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Cold,
          SpellDescriptor.MovementImpairing,
          SpellDescriptor.Ground)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetActionType(CommandType.Standard)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(friends: true, enemies: true, self: true, point: true)
        .SetSpellResistance()
        .SetShouldTurnToTarget()
        .SetEffectOnAlly(AbilityEffectOnUnit.Harmful)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetAvailableMetamagic(
          Metamagic.Quicken,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Reach,
          Metamagic.CompletelyNormal,
          Metamagic.Persistent,
          Metamagic.Selective,
          Metamagic.Empower,
          Metamagic.Maximize)
        .AddToSpellLists(
          level: 2,
          SpellList.Druid, 
          SpellList.Magus,
          SpellList.Ranger,
          SpellList.Wizard,
          SpellList.Witch,
          SpellList.Lich)
        .AddAbilityAoERadius(radius: 10.Feet())
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .SpawnAreaEffect(area, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes)))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .Configure();
    }

    private static void ModifyFx(GameObject puddle)
    {
      UnityEngine.Object.DestroyImmediate(puddle.transform.Find("Transform/ProjectorCollision_big").gameObject); // Remove unwanted particle effects
      puddle.transform.localScale = new(0.55f, 1.0f, 0.55f); // Scale from 20ft to 10ft
    }

    [TypeId("998be8d7-fd17-4474-a440-baa5ac051fee")]
    private class IceSlickEffect : AbilityAreaEffectRunAction
    {
      private readonly ActionList OnSpawn;

      public IceSlickEffect()
      {
        OnSpawn =
          ActionsBuilder.New()
            .ApplyBuffPermanent(Guids.IceSlickAoEBuff, isNotDispelable: true)
            .SavingThrow(
              SavingThrowType.Reflex,
              onResult:
                ActionsBuilder.New()
                  // Damage is only dealt if spell resistance passes
                  .Add<SpellResistanceCheck>(
                    c =>
                      c.OnResistFail =
                        ActionsBuilder.New()
                          .DealDamage(
                            DamageTypes.Energy(DamageEnergyType.Cold),
                            value: ContextDice.Value(DiceType.D6, bonus: ContextValues.Rank()),
                            halfIfSaved: true)
                          .Build())
                  // If they failed the save apply prone
                  .ConditionalSaved(failed: ApplyProne))
            .Build();

        UnitEnter =
            ActionsBuilder.New()
              .ApplyBuffPermanent(Guids.IceSlickAoEBuff, isNotDispelable: true)
              .SavingThrow(
                SavingThrowType.Reflex, onResult: ActionsBuilder.New().ConditionalSaved(failed: ApplyProne))
              .Build();
        Round = Constants.Empty.Actions;
        UnitExit = ActionsBuilder.New().RemoveBuff(Guids.IceSlickAoEBuff).Build();
      }

      public override void OnUnitEnter(MechanicsContext context, AreaEffectEntityData areaEffect, UnitEntityData unit)
      {
        try
        {
          if (context.MaybeCaster is null)
          {
            Logger.Warning("No caster!");
            return;
          }

          // Assume this is right when the effect spawned
          if (Game.Instance.TimeController.GameTime - areaEffect.m_CreationTime < 0.25f.Seconds())
          {
            Logger.Log($"Running spawn actions on {unit.CharacterName}");
            using (ContextData<AreaEffectContextData>.Request().Setup(areaEffect))
            {
              using (context.GetDataScope(unit))
                OnSpawn.Run();
            }
          }
          else
          {
            base.OnUnitEnter(context, areaEffect, unit);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("IceSlickEffect.OnUnitEnter", e);
        }
      }
    }
  }
}
