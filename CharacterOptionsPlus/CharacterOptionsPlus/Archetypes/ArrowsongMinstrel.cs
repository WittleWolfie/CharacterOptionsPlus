using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Archetypes
{
  internal class ArrowsongMinstrel
  {
    private const string ArchetypeName = "ArrowsongMinstrel";

    internal const string ArchetypeDisplayName = "ArrowsongMinstrel.Name";
    private const string ArchetypeDescription = "ArrowsongMinstrel.Description";

    private static readonly ModLogger Logger = Logging.GetLogger(ArchetypeName);

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "hurtful.png"; // TODO: Replace with new icon

    internal static void Configure()
    {
      if (Settings.IsEnabled(Guids.ArrowsingMinstrelArchetype))
        ConfigureEnabled();
      else
        ConfigureDisabled();
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {ArchetypeName} (disabled)");
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {ArchetypeName}");

      ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsingMinstrelArchetype, CharacterClassRefs.BardClass)
        .SetLocalizedName(ArchetypeDisplayName)
        .SetLocalizedDescription(ArchetypeDescription)

        // TODO: Diminished spellcasting

        // First remove the replaced features
        .AddToRemoveFeatures(1, FeatureRefs.BardProficiencies.ToString())
        .AddToRemoveFeatures(1, FeatureRefs.BardicKnowledge.ToString())

        .AddToRemoveFeatures(2, FeatureRefs.BardWellVersed.ToString())
        .AddToRemoveFeatures(2, FeatureSelectionRefs.BardTalentSelection.ToString()) // Level 2 Versatile Performance

        .AddToRemoveFeatures(5, FeatureRefs.BardLoreMaster.ToString())
        .AddToRemoveFeatures(6, FeatureRefs.FascinateFeature.ToString())
        .AddToRemoveFeatures(8, FeatureRefs.DirgeOfDoomFeature.ToString())
        .AddToRemoveFeatures(12, FeatureRefs.SoothingPerformanceFeature.ToString())

        // All Inspire Competence
        .AddToRemoveFeatures(3, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(7, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(11, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(15, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(19, FeatureRefs.InspireCompetenceFeature.ToString())

        // TODO: Add replacement features
        .Configure();
    }
  }
}
