using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
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
  internal class Desecrate
  {
    private const string FeatureName = "Desecrate";

    internal const string DisplayName = "Desecrate.Name";
    private const string Description = "Desecrate.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "Desecrate.png";

    private const string AreaEffect = "Desecrate.AoE";

    // A 25 ft "negative puddle"
    private const string AreaEffectFxSource = "b56b39f94af1bb04da24ba4206cc9140";
    private const string AreaEffectFx = "d9538102-91af-44d5-a96b-f234d966fec3";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

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

      AbilityAreaEffectConfigurator.New(AreaEffect, Guids.DesecrateAoE).Configure();
      AbilityConfigurator.New(FeatureName, Guids.DesecrateSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      // This handles updating the look of the effect
      AssetTool.RegisterDynamicPrefabLink(AreaEffectFx, AreaEffectFxSource, ModifyFx);
      var area = AbilityAreaEffectConfigurator.New(AreaEffect, Guids.DesecrateAoE)
        .CopyFrom(AbilityAreaEffectRefs.GreaseArea)
        .SetFx(AreaEffectFx)
        .AddSpellDescriptorComponent(SpellDescriptor.Evil)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.DesecrateSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Evil)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetLocalizedDuration(AbilityRefs.MageArmor.Reference.Get().LocalizedDuration)
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
          Metamagic.Selective)
        .AddToSpellLists(
          level: 2,
          SpellList.Cleric,
          SpellList.Inquisitor,
          SpellList.LichInquisitorMinor)
        .AddAbilityAoERadius(radius: 20.Feet())
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .SpawnAreaEffect(area, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Hours)))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .Configure();
    }

    private static void ModifyFx(GameObject puddle)
    {
     // UnityEngine.Object.DestroyImmediate(puddle.transform.Find("Transform/ProjectorCollision_big").gameObject); // Remove unwanted particle effects
      puddle.transform.localScale = new(0.85f, 1.0f, 0.85f); // Scale from 25ft to 20ft
    }
  }
}
