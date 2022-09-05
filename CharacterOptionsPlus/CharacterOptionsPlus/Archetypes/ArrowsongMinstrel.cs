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
        .Configure();
    }
  }
}
