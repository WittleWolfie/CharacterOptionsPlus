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
      ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsingMinstrelArchetype)
        .SetLocalizedName(ArchetypeDisplayName)
        .SetLocalizedDescription(ArchetypeDescription)
        .Configure(); 
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {ArchetypeName}");

      var archetype =
        ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsingMinstrelArchetype, CharacterClassRefs.BardClass)
          .SetLocalizedName(ArchetypeDisplayName)
          .SetLocalizedDescription(ArchetypeDescription);

      // Remove features
      archetype
        .AddToRemoveFeatures(1, FeatureRefs.BardProficiencies.ToString(), FeatureRefs.BardicKnowledge.ToString())
        // Remove the bard talent which is a stand-in for Versatile Performance
        // Notably BardTalentSelection is the incorrect reference!
        .AddToRemoveFeatures(
          2, FeatureRefs.BardWellVersed.ToString(), FeatureSelectionRefs.BardTalentSelection_0.ToString())
        .AddToRemoveFeatures(5, FeatureRefs.BardLoreMaster.ToString())
        .AddToRemoveFeatures(6, FeatureRefs.FascinateFeature.ToString())
        .AddToRemoveFeatures(8, FeatureRefs.DirgeOfDoomFeature.ToString())
        .AddToRemoveFeatures(12, FeatureRefs.SoothingPerformanceFeature.ToString())
        .AddToRemoveFeatures(3, FeatureRefs.InspireCompetenceFeature.ToString()) // All Inspire Competence
        .AddToRemoveFeatures(7, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(11, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(15, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(19, FeatureRefs.InspireCompetenceFeature.ToString());

     // TODO: Add features

      archetype.Configure();
    }
  }
}
