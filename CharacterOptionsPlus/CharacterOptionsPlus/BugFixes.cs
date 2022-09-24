using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus
{
  internal class BugFixes
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(BugFixes));

    internal static readonly List<(string key, string name, string description)> Entries =
      new()
      {
        (PackRagerTeamworkSelection, "PackRagerTeamworkSelection.Name", "PackRagerTeamworkSelection.Description"),
      };

    internal static void Configure()
    {
      if (Util.Settings.IsEnabled(PackRagerTeamworkSelection))
        FixPackRagerTeamworkSelection();
    }

    internal static string PackRagerTeamworkSelection = "pack-rager-teamwork-selection-fix";
    internal static void FixPackRagerTeamworkSelection()
    {
      Logger.Log("Patching PackRager Teamwork Selection");
      FeatureSelectionConfigurator.For(FeatureSelectionRefs.PackRagerTeamworkFeatSelection)
        .SetGroup(FeatureGroup.TeamworkFeat)
        .Configure();
    }
  }
}
