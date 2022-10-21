using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using CharacterOptionsPlus.Util;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class IceSlick
  {
    // c591b2c6606714d4ebdf0c2fc05cafec:kinetic_iceaoe00_20feet_aoe.fx ?
    private const string FeatureName = "IceSlick";

    internal const string DisplayName = "IceSlick.Name";
    private const string Description = "IceSlick.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

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

      FeatureConfigurator.New(FeatureName, Guids.IceSlickSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      FeatureConfigurator.New(FeatureName, Guids.IceSlickSpell)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetIsClassFeature()
        .Configure(delayed: true);
    }
  }
}
