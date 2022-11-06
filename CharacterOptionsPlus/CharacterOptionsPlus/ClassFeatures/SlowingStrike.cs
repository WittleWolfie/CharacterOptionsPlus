using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class SlowingStrike
  {
    private const string FeatureName = "SlowingStrike";

    internal const string DisplayName = "SlowingStrike.Name";
    private const string Description = "SlowingStrike.Description";

    private const string BuffName = "SlowingStrike.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "burningdisarm.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.SlowingStrikeTalent))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("SlowingStrike.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.SlowingStrikeBuff).Configure();
      FeatureConfigurator.New(FeatureName, Guids.SlowingStrikeTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.SlowingStrikeBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .Configure();

      FeatureConfigurator.New(FeatureName, Guids.SlowingStrikeTalent, FeatureGroup.SlayerTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetIsClassFeature()
        .Configure(delayed: true);
    }
  }
}
