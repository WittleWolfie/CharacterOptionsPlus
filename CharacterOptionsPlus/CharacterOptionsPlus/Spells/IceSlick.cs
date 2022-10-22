using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
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
    private const string IconName = IconPrefix + "gloriousheat.png";

    private const string AreaEffect = "IceSlick.AoE";
    // A 20 ft "cold puddle"
    private const string AreaEffectFx = "fd21d914e9f6f5e4faa77365549ad0a7";

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

      AbilityConfigurator.New(FeatureName, Guids.IceSlickSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      // This handles updating the look of the effect
      FxTool.RegisterAoESpawnHandler(AreaEffect, ModifyFx);
      var area = AbilityAreaEffectConfigurator.New(AreaEffect, Guids.IceSlickAoE)
        .SetAffectEnemies()
        .SetAggroEnemies()
        .SetSize(10.Feet())
        .SetFx(new PrefabLink() { AssetId = AreaEffectFx })
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
      // TODO: Shrink size and remove spikes
    }
  }
}
