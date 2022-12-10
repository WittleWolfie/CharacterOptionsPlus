using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.UnitParts;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Visual.Particles;
using Kingmaker.Visual.Particles.FxSpawnSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.NewActions;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class HedgingWeapons
  {
    private const string FeatureName = "HedgingWeapons";

    internal const string DisplayName = "HedgingWeapons.Name";
    private const string Description = "HedgingWeapons.Description";

    private const string BuffName = "HedgingWeapons.Buff";
    private const string ThrowName = "HedgingWeapons.Throw";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.HedgingWeaponsSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("HedgingWeapons.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(ThrowName, Guids.HedgingWeaponsThrow).Configure();
      BuffConfigurator.New(BuffName, Guids.HedgingWeaponsBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.HedgingWeaponsSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = FeatureRefs.DevotedBladeFeature.Reference.Get().Icon;
      var buff = BuffConfigurator.New(BuffName, Guids.HedgingWeaponsBuff)
        .SetRanks(5)
        .SetStacking(StackingType.Rank)
        .AddComponent<HedgingWeaponsController>()
        .AddComponent<ControlledProjectilesHolder>()
        .AddFacts(new() { Guids.HedgingWeaponsThrow })
        .AddACBonusAgainstAttacks(value: ContextValues.Rank(), descriptor: ModifierDescriptor.Deflection)
        .AddContextRankConfig(ContextRankConfigs.BuffRank(Guids.HedgingWeaponsBuff))
        .Configure();

      AbilityConfigurator.New(ThrowName, Guids.HedgingWeaponsThrow)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetAnimation(CastAnimationStyle.Thrown)
        .SetActionType(CommandType.Standard)
        .AddComponent(new AbilityDeliverControlledProjectile(buff.ToReference<BlueprintBuffReference>()))
        .AddComponent<AlwaysAddToActionBar>()
        .AddAbilityEffectRunAction(
          ActionsBuilder.New().DealDamage(DamageTypes.Force(), ContextDice.Value(DiceType.D6, diceCount: 2)))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.HedgingWeaponsSpell, SpellSchool.Abjuration, canSpecialize: true, SpellDescriptor.Force)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetRange(AbilityRange.Personal)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Quicken)
        .AddToSpellLists(
          level: 1, SpellList.Cleric, SpellList.Inquisitor, SpellList.LichInquisitorMinor, SpellList.Paladin)
        .AddToSpellList(level: 1, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddContextRankConfig(
          ContextRankConfigs.CasterLevel(max: 5, type: AbilityRankType.ProjectilesCount)
            .WithStartPlusDivStepProgression(4, start: 2))
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .RemoveBuff(buff) // Clear the old buff first
            .Add<ContextActionApplyBuffRanks>(
              a =>
                {
                  a.m_Buff = buff.ToReference<BlueprintBuffReference>();
                  a.Rank = ContextValues.Rank(AbilityRankType.ProjectilesCount);
                  a.DurationValue = ContextDuration.Variable(ContextValues.Rank(), DurationRate.Minutes);
                }))
        .Configure(delayed: true);
    }

    [TypeId("45444d3f-4e06-4294-a6d5-7905a77f54b4")]
    private class HedgingWeaponsController : ProjectileControllerComponent
    {
      private static readonly BlueprintBuffReference HedgingWeaponsBuff =
        BlueprintTool.GetRef<BlueprintBuffReference>(Guids.HedgingWeaponsBuff);

      public override RuleAttackRoll RollAttack(AbilityExecutionContext context)
      {
        try
        {
          var caster = context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return null;
          }

          var weapon = GetWeapon(caster).Blueprint.Get().CreateEntity<ItemEntityWeapon>();
          using (ContextData<AttackBonusStatReplacement>.Request().Setup(StatType.Dexterity))
          {
            var attackRule = new RuleAttackRoll(caster, context.MainTarget.Unit, weapon, attackBonusPenalty: 0);
            attackRule.SuspendCombatLog = true;
            return context.TriggerRule(attackRule);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("HedgingWeaponsSpawner.RollAttack", e);
        }
        return null;
      }

      #region Asset Stuff
      private static readonly Vector3[] Offsets =
        new Vector3[]
        {
          new(0.5f, 1f, 0.5f),
          new(0.5f, 1f, -0.5f),
          new(-0.5f, 1f, 0.5f),
          new(-0.5f, 1f, -0.5f),
          new(-0.86f, 1f, 0f),
        };

      public override void OnActivate()
      {
        try
        {
          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          if (caster.Get<UnitPartControlledProjectiles>()?.Get(HedgingWeaponsBuff)?.Any() == true)
            return;

          var enchant = Enchant.Load();
          var weapon = GetWeapon(caster);
          var prefab = weapon.Prefab.Load();
          prefab.SetActive(false);

          var projectiles = new List<ControlledProjectile>();
          var numWeapons = Buff.GetRank();
          for (int i = 0; i < numWeapons; i++)
            projectiles.Add(new(SpawnWeapon(prefab, enchant, caster, Offsets[i], weapon.Scale)));

          caster.Ensure<UnitPartControlledProjectiles>().Register(HedgingWeaponsBuff, projectiles);
        }
        catch (Exception e)
        {
          Logger.LogException("HedgingWeaponsSpawner.OnActivate", e);
        }
      }

      public override void SpawnProjectiles(UnitEntityData caster)
      {
        try
        {
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var enchant = Enchant.Load();
          var weapon = GetWeapon(caster);
          var prefab = weapon.Prefab.Load();
          prefab.SetActive(false);

          var projectiles = caster.Ensure<UnitPartControlledProjectiles>().Get(HedgingWeaponsBuff);
          int i = 0;
          foreach (var projectile in projectiles)
          {
            projectile.Handle = SpawnWeapon(prefab, enchant, caster, Offsets[i], weapon.Scale);
            i++;
          }
        }
        catch(Exception e)
        {
          Logger.LogException("HedgingWeaponsSpawner.SpawnProjectiles", e);
        }
      }

      private IFxHandle SpawnWeapon(
        GameObject prefab, GameObject enchant, UnitEntityData unit, Vector3 offset, float scale = 1.0f)
      {
        var handle = FxHelper.SpawnFxOnUnit(prefab, unit.View, partyRelated: true);
        handle.RunAfterSpawn(
          weapon =>
          {
            weapon.AnchorToUnit(unit, offset, Quaternion.AngleAxis(90, Vector3.up));
            weapon.SetActive(true);
            weapon.transform.localScale = new(scale, scale, scale);
            FxHelper.SpawnFxOnGameObject(enchant, weapon, partySource: true);
          });
        return handle;
      }

      public override void OnDeactivate()
      {
        try
        {
          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var consumeAll = ContextData<BuffCollection.RemoveByRank>.Current ? Fact.GetRank() <= 1 : true;
          if (consumeAll)
            caster.Get<UnitPartControlledProjectiles>()?.ConsumeAll(HedgingWeaponsBuff);
        }
        catch (Exception e)
        {
          Logger.LogException("HedgingWeaponsSpawner.OnDeactivate", e);
        }
      }
      #endregion

      #region God Stuff
      private struct Weapon
      {
        public BlueprintItemWeaponReference Blueprint;
        public PrefabLink Prefab;
        public float Scale;

        public Weapon(string blueprintId, string assetId, float scale = 1.0f)
        {
          Blueprint = BlueprintTool.GetRef<BlueprintItemWeaponReference>(blueprintId);
          Prefab = new() { AssetId = assetId };
          Scale = scale;
        }
      }

      private static readonly PrefabLink Enchant = new() { AssetId = "fdc7f8f37d3f8da42be2a1d35a617001" };

      private static readonly Weapon AbadarWeapon =
        new(ItemWeaponRefs.StandardHeavyCrossbow.ToString(), "f4ef679dee9518b40806cea527b62958", scale: 0.7f);
      private static readonly Weapon AsmodeusWeapon =
        new(ItemWeaponRefs.StandardHeavyMace.ToString(), "949c76c2c0141264e9885d939a3e09e4");
      private static readonly Weapon DesnaWeapon =
        new(ItemWeaponRefs.StandardStarknife.ToString(), "0c8388fec791b844587490913639146b");
      private static readonly Weapon ErastilWeapon =
        new(ItemWeaponRefs.StandardLongbow.ToString(), "bc10b2b632dcc4c4fbc157a8b9e32114", scale: 0.7f);
      private static readonly Weapon GorumWeapon =
        new(ItemWeaponRefs.StandardGreatsword.ToString(), "5f14f60fffc49c843affe14fcb55fa04", scale: 0.6f);
      private static readonly Weapon IomedaeWeapon =
        new(ItemWeaponRefs.StandardLongsword.ToString(), "7545e752a43edac47a2af12fd8cb79f5");
      private static readonly Weapon LamashtuWeapon =
        new(ItemWeaponRefs.StandardKukri.ToString(), "5464bdca2bddbab439369d03f6383f41");
      private static readonly Weapon NorgorberWeapon =
        new(ItemWeaponRefs.StandardDagger.ToString(), "8a0684581898ee642aa771427d77f9ab");
      private static readonly Weapon SarenraeWeapon =
        new(ItemWeaponRefs.StandardScimitar.ToString(), "ad5acf45026b1ac46911fbef2ab79fb5");
      private static readonly Weapon ToragWeapon =
        new(ItemWeaponRefs.StandardWarhammer.ToString(), "ef203bf6ef89c19409394731d72955d3");
      private static readonly Weapon UrgathoaWeapon =
        new(ItemWeaponRefs.StandardScythe.ToString(), "88fd281bc9977bf448246469c85477b6", scale: 0.6f);

      private static Weapon GetWeapon(UnitEntityData unit)
      {
        if (unit.HasFact(FeatureRefs.AbadarFeature.Reference.Get()))
          return AbadarWeapon;
        if (unit.HasFact(FeatureRefs.AsmodeusFeature.Reference.Get()))
          return AsmodeusWeapon;
        if (unit.HasFact(FeatureRefs.DesnaFeature.Reference.Get()))
          return DesnaWeapon;
        if (unit.HasFact(FeatureRefs.ErastilFeature.Reference.Get()))
          return ErastilWeapon;
        if (unit.HasFact(FeatureRefs.GorumFeature.Reference.Get()))
          return GorumWeapon;
        if (unit.HasFact(FeatureRefs.IomedaeFeature.Reference.Get()))
          return IomedaeWeapon;
        if (unit.HasFact(FeatureRefs.LamashtuFeature.Reference.Get()))
          return LamashtuWeapon;
        if (unit.HasFact(FeatureRefs.NorgorberFeature.Reference.Get()))
          return NorgorberWeapon;
        if (unit.HasFact(FeatureRefs.SarenraeFeature.Reference.Get()))
          return SarenraeWeapon;
        if (unit.HasFact(FeatureRefs.ToragFeature.Reference.Get()))
          return ToragWeapon;
        if (unit.HasFact(FeatureRefs.UrgathoaFeature.Reference.Get()))
          return UrgathoaWeapon;
        return IomedaeWeapon;
      }
      #endregion
    }
  }
}
