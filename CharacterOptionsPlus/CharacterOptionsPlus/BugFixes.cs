using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus
{
  internal class BugFixes
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(BugFixes));

    internal static void Configure()
    {
      if (Settings.IsEnabled(PackRagerTeamworkSelection))
        FixPackRagerTeamworkSelection();
      if (Settings.IsEnabled(HeavenlyFireResourceAmount))
        FixHeavenlyFireResourceAmount();
    }

    internal static string PackRagerTeamworkSelection = "pack-rager-teamwork-selection-fix";
    internal static void FixPackRagerTeamworkSelection()
    {
      Logger.Log("Patching PackRager Teamwork Selection");
      FeatureSelectionConfigurator.For(FeatureSelectionRefs.PackRagerTeamworkFeatSelection)
        .SetGroup(FeatureGroup.TeamworkFeat)
        .Configure();
    }

    internal static string HeavenlyFireResourceAmount = "heavenly-fire-resource-amount-fix";
    internal static void FixHeavenlyFireResourceAmount()
    {
      Logger.Log("Patching Heavenly Fire resource amount");
      AbilityResourceConfigurator.For(AbilityResourceRefs.BloodlineCelestialHeavenlyFireResource)
        .SetMaxAmount(ResourceAmountBuilder.New(3).IncreaseByStat(StatType.Charisma))
        .Configure();
    }

    internal static readonly List<(string key, string name, string description)> Entries =
      new()
      {
        (PackRagerTeamworkSelection, "PackRagerTeamworkSelection.Name", "PackRagerTeamworkSelection.Description"),
        (HeavenlyFireResourceAmount, "HeavenlyFireResourceAmount.Name", "HeavenlyFireResourceAmount.Description"),
      };
  }
}
