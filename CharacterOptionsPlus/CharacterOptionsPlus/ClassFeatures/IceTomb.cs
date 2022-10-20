using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class IceTomb
  {
    private const string FeatureName = "IceTomb";

    internal const string DisplayName = "IceTomb.Name";
    private const string Description = "IceTomb.Description";

    private const string AbilityName = "IceTomb.Ability";

    private const string BuffName = "IceTomb.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "IceTomb.png"; // TODO

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.IceTombHex))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("IceTomb.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      FeatureConfigurator.New(FeatureName, Guids.IceTombHex).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      FeatureConfigurator.New(FeatureName, Guids.IceTombHex, FeatureGroup.WitchHex)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .Configure(delayed: true);
    }
  }
}
