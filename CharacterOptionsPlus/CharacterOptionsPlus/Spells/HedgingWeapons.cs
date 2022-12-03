using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Visual.Particles;
using System;
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
    private const string ProjectileName = "HedgingWeapons.Projectile";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

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

      BuffConfigurator.New(BuffName, Guids.HedgingWeaponsBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.HedgingWeaponsSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.HedgingWeaponsBuff)
        .Configure();

      var projectile = ControllableProjectileConfigurator.New(ProjectileName, Guids.HedgingWeaponsProjectile)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.HedgingWeaponsSpell, SpellSchool.Abjuration, canSpecialize: true, SpellDescriptor.Force)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
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
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes))
            .Add<SpawnHedgingWeapons>())
        .Configure();
    }

    private static readonly Vector3[] Offsets =
      new Vector3[]
      {
        new(0.5f, 1f, 0.5f),
        new(0.5f, 1f, -0.5f),
        new(-0.5f, 1f, 0.5f),
        new(-0.5f, 1f, -0.5f),
        new(-8.6f, 1f, 0f),
      };

    [TypeId("45444d3f-4e06-4294-a6d5-7905a77f54b4")]
    private class SpawnHedgingWeapons : ContextAction
    {
      public override string GetCaption()
      {
        return "Custom action for Hedging Weapons";
      }

      public override void RunAction()
      {
        try
        {
          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var enchantPrefab = Enchant.Load();
          var weaponPrefab = GetWeapon(caster).Load();
          weaponPrefab.SetActive(false);

          var numWeapons = Math.Min(5, 1 + (Context.Params.CasterLevel - 2) / 4);
          for (int i = 0; i < numWeapons; i++)
          {
            var weapon = GameObject.Instantiate(weaponPrefab);
            weapon.AnchorToUnit(caster, Offsets[i], Quaternion.AngleAxis(90, Vector3.up));
            weapon.SetActive(true);
            FxHelper.SpawnFxOnGameObject(enchantPrefab, weapon, partySource: true);
          }
        }
        catch(Exception e)
        {
          Logger.LogException("SpawnHedgingWeapons.RunAction", e);
        }
      }

      private static readonly PrefabLink Enchant = new() { AssetId = "fdc7f8f37d3f8da42be2a1d35a617001" };
      private static readonly PrefabLink Abadar = new() { AssetId = "f4ef679dee9518b40806cea527b62958" };
      private static readonly PrefabLink Asmodeus = new() { AssetId = "949c76c2c0141264e9885d939a3e09e4" };
      private static readonly PrefabLink Desna = new() { AssetId = "0c8388fec791b844587490913639146b" };
      private static readonly PrefabLink Erastil = new() { AssetId = "8082c39628c82e34abf1dbc920624036" };
      private static readonly PrefabLink Gorum = new() { AssetId = "5f14f60fffc49c843affe14fcb55fa04" };
      private static readonly PrefabLink Iomedae = new() { AssetId = "7545e752a43edac47a2af12fd8cb79f5" };
      private static readonly PrefabLink Lamashtu = new() { AssetId = "5464bdca2bddbab439369d03f6383f41" };
      private static readonly PrefabLink Norgorber = new() { AssetId = "8a0684581898ee642aa771427d77f9ab" };
      private static readonly PrefabLink Sarenrae = new() { AssetId = "ad5acf45026b1ac46911fbef2ab79fb5" };
      private static readonly PrefabLink Torag = new() { AssetId = "ef203bf6ef89c19409394731d72955d3" };
      private static readonly PrefabLink Urgathoa = new() { AssetId = "88fd281bc9977bf448246469c85477b6" };

      private static PrefabLink GetWeapon(UnitEntityData unit)
      {
        if (unit.HasFact(FeatureRefs.AbadarFeature.Reference.Get()))
          return Abadar;
        if (unit.HasFact(FeatureRefs.AsmodeusFeature.Reference.Get()))
          return Asmodeus;
        if (unit.HasFact(FeatureRefs.DesnaFeature.Reference.Get()))
          return Desna;
        if (unit.HasFact(FeatureRefs.ErastilFeature.Reference.Get()))
          return Erastil;
        if (unit.HasFact(FeatureRefs.GorumFeature.Reference.Get()))
          return Gorum;
        if (unit.HasFact(FeatureRefs.IomedaeFeature.Reference.Get()))
          return Iomedae;
        if (unit.HasFact(FeatureRefs.LamashtuFeature.Reference.Get()))
          return Lamashtu;
        if (unit.HasFact(FeatureRefs.NorgorberFeature.Reference.Get()))
          return Norgorber;
        if (unit.HasFact(FeatureRefs.SarenraeFeature.Reference.Get()))
          return Sarenrae;
        if (unit.HasFact(FeatureRefs.ToragFeature.Reference.Get()))
          return Torag;
        if (unit.HasFact(FeatureRefs.UrgathoaFeature.Reference.Get()))
          return Urgathoa;
        return Iomedae;
      }
    }
  }
}
