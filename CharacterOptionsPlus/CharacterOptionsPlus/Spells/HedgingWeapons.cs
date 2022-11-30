using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using System;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
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

    private const string WeaponEffect = "d65d6515-0513-4962-91db-d32618fb8fc6";
    private const string WeaponEffectSource = "ad5acf45026b1ac46911fbef2ab79fb5";

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

    // TODO: So here's how it works:
    // - Use Controllable Projectile to spawn ??
    //    - Maybe an FX just re-use the bone lance thing from lich
    //    - Alternatively a Weapon Prefab directly
    // - FX Approach:
    //    - Set "DontAttach" and related variables on SnapToLocator
    //    - Spawn Weapon Prefab and add to FX
    // - Weapon Approach:
    //    - Create SnapToLocator and attach OR mess w/ WeaponParticlesSnapMap OR LocatorPositionTracker
    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.HedgingWeaponsBuff)
        .Configure();

      AssetTool.RegisterDynamicPrefabLink(WeaponEffect, WeaponEffectSource, ModifyFx);
      var projectile = ControllableProjectileConfigurator.New(ProjectileName, Guids.HedgingWeaponsProjectile)
        .SetOnCreaturePrefab(WeaponEffect)
        .SetHeightOffset(1.2f)
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
            .SpawnControllableProjectile(buff, projectile))
        .Configure();
    }

    private static void ModifyFx(GameObject obj)
    {

    }
  }
}
