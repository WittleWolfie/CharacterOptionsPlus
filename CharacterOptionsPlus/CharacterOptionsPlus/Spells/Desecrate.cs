﻿using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Craft;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System;
using System.Linq;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Spells
{
  internal class Desecrate
  {
    private const string FeatureName = "Desecrate";

    internal const string DisplayName = "Desecrate.Name";
    private const string Description = "Desecrate.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "desecrate.png";

    private const string AreaEffect = "Desecrate.AoE";
    private const string BuffName = "Desecrate.Buff";

    // A 25 ft "negative puddle"
    private const string AreaEffectFxSource = "b56b39f94af1bb04da24ba4206cc9140";
    private const string AreaEffectFx = "d9538102-91af-44d5-a96b-f234d966fec3";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.DesecrateSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("Desecrate.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.DesecrateBuff).Configure();
      AbilityAreaEffectConfigurator.New(AreaEffect, Guids.DesecrateAoE).Configure();
      AbilityConfigurator.New(FeatureName, Guids.DesecrateSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      DoubleSummons(AbilityRefs.AnimateDead.ToString());
      DoubleSummons(AbilityRefs.AnimateDeadLesser.ToString());
      DoubleSummons(AbilityRefs.DirgeBardAnimateDead.ToString());
      DoubleSummons(AbilityRefs.OracleBonesAnimateDead.ToString());

      var isUndead = ConditionsBuilder.New().HasFact(FeatureRefs.UndeadType.ToString());
      var buff = BuffConfigurator.New(BuffName, Guids.DesecrateBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddSavingThrowBonusAgainstDescriptor(
          spellDescriptor: SpellDescriptor.ChannelNegativeHarm,
          modifierDescriptor: ModifierDescriptor.Profane,
          value: -3)
        .AddAttackBonusConditional(
          conditions: isUndead, bonus: 1, descriptor: ModifierDescriptor.Profane, checkWielder: true)
        .AddDamageBonusConditional(
          conditions: isUndead, bonus: 1, descriptor: ModifierDescriptor.Profane, checkWielder: true)
        .AddComponent<DesecrateComponent>()
        .Configure();

      // This handles updating the look of the effect
      AssetTool.RegisterDynamicPrefabLink(AreaEffectFx, AreaEffectFxSource, ModifyFx);
      var area = AbilityAreaEffectConfigurator.New(AreaEffect, Guids.DesecrateAoE)
        .CopyFrom(AbilityAreaEffectRefs.GreaseArea)
        .SetSize(20.Feet())
        .SetFx(AreaEffectFx)
        .AddSpellDescriptorComponent(SpellDescriptor.Evil)
        .AddAbilityAreaEffectBuff(buff: buff)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.DesecrateSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Evil)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.HourPerLevel)
        .SetActionType(CommandType.Standard)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(friends: true, enemies: true, self: true, point: true)
        .SetSpellResistance()
        .SetShouldTurnToTarget()
        .SetEffectOnAlly(AbilityEffectOnUnit.Harmful)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          Metamagic.Selective)
        .AddToSpellLists(
          level: 2,
          SpellList.Cleric,
          SpellList.Inquisitor,
          SpellList.Warpriest,
          SpellList.LichInquisitorMinor,
          SpellList.Lich)
        .AddAbilityAoERadius(radius: 20.Feet())
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .SpawnAreaEffect(area, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Hours)))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .SetMaterialComponent(
          new()
          {
            m_Item = ItemRefs.GoldCoins.Cast<BlueprintItemReference>().Reference,
            Count = 25
          })
        .AddCraftInfoComponent(
          aOEType: CraftAOE.AOE,
          savingThrow: CraftSavingThrow.None,
          spellType: CraftSpellType.Buff)
        .Configure();
    }

    private static void DoubleSummons(string ability)
    {
      AbilityConfigurator.For(ability)
        .EditComponent<AbilityEffectRunAction>(
          c =>
          {
            Logger.Verbose(() => $"Editing {ability} to increase spawn count");
            var spawnMonster =
              c.Actions.Actions.Where(
                a => a is ContextActionSpawnMonster).Cast<ContextActionSpawnMonster>().FirstOrDefault();
            var spawn = ActionsBuilder.New().AddAll(c.Actions);
            var augmentedSpawn = ActionsBuilder.New()
              .SpawnMonsterUsingSummonPool(
                countValue: ContextDice.Value(DiceType.D4, diceCount: 2, bonus: 4),
                durationValue: spawnMonster.DurationValue,
                monster: spawnMonster.m_Blueprint,
                afterSpawn: spawnMonster.AfterSpawn,
                summonPool: spawnMonster.SummonPool,
                levelValue: spawnMonster.LevelValue);
            c.Actions = ActionsBuilder.New()
              .Conditional(
                conditions: ConditionsBuilder.New().CasterHasFact(Guids.DesecrateBuff),
                ifTrue: augmentedSpawn,
                ifFalse: spawn)
              .Build();
          })
        .Configure();
    }

    private static void ModifyFx(GameObject puddle)
    {
      // Destroy everything except the fog cloud
      UnityEngine.Object.DestroyImmediate(puddle.transform.Find("Ground/Sparks").gameObject);
      UnityEngine.Object.DestroyImmediate(puddle.transform.Find("Ground/Smoke").gameObject);
      UnityEngine.Object.DestroyImmediate(puddle.transform.Find("Ground/Decal").gameObject);
      puddle.transform.localScale = new(0.85f, 1.0f, 0.85f); // Scale from 25ft to 20ft
    }

    [TypeId("e2dd3665-24d8-4db9-a192-2434529c10bd")]
    private class DesecrateComponent : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleSavingThrow>
    {
      private static BlueprintFeature _undead;
      private static BlueprintFeature Undead
      {
        get
        {
          _undead ??= FeatureRefs.UndeadType.Reference.Get();
          return _undead;
        }
      }

      public override void OnTurnOn()
      {
        try
        {
          if (!Owner.HasFact(Undead))
          {
            Logger.Verbose(() => $"Skipping hit point bonus for {Owner.CharacterName}, not undead.");
            return;
          }
           
          if (!Owner.HasFact(Game.Instance.BlueprintRoot.SystemMechanics.SummonedUnitBuff))
          {
            Logger.Verbose(() => $"Skipping hit point bonus for {Owner.CharacterName}, not summoned.");
            return;
          }

          var bonusHP = Owner.Descriptor.Progression.CharacterLevel;

          Logger.Verbose(() => $"Adding +{bonusHP} hit points to {Owner.CharacterName}");
          Owner.Stats.HitPoints.RemoveModifiersFrom(Runtime);
          Owner.Stats.HitPoints.AddModifier(bonusHP, Runtime, ModifierDescriptor.UntypedStackable);
        }
        catch (Exception e)
        {
          Logger.LogException("DesecrateComponent.OnTurnOn", e);
        }
      }

      public override void OnTurnOff()
      {
        try
        {
          Owner.Stats.HitPoints.RemoveModifiersFrom(Runtime);
        }
        catch (Exception e)
        {
          Logger.LogException("DesecrateComponent.OnTurnOff", e);
        }
      }

      public void OnEventAboutToTrigger(RuleSavingThrow evt)
      {
        try
        {
          if (!Owner.HasFact(Undead))
          {
            Logger.Verbose(() => $"Skipping saving throw bonus for {Owner.CharacterName}, not undead.");
            return;
          }

          Logger.Verbose(() => $"Adding +1 Profane bonus to saving throw for {Owner.CharacterName}");
          evt.AddModifier(1, source: Fact, descriptor: ModifierDescriptor.Profane);
        }
        catch (Exception e)
        {
          Logger.LogException("DesecrateComponent.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleSavingThrow evt) { }
    }
  }
}
