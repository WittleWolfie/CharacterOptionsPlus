using BlueprintCore.Utils;
using Kingmaker.Localization;
using ModMenu.Settings;
using static UnityModManagerNet.UnityModManager.ModEntry;
using Menu = ModMenu.ModMenu;

namespace CharacterOptionsPlus.Util
{
  internal static class Settings
  {
    private static readonly string RootKey = "cop.settings";

    private static readonly ModLogger Logger = Logging.GetLogger(nameof(Settings));

    internal static bool IsEnabled(string guid)
    {
      return Menu.GetSettingValue<bool>(GetKey(guid));
    }

    internal static void Init()
    {
      Logger.Log("Initializing settings.");
      var settings = SettingsBuilder.New(RootKey, GetString("Settings.Title"));

      settings.AddSubHeader(GetString("Settings.Feats.Title"), startExpanded: true);
      foreach (var (guid, name) in Guids.Feats)
      {
        settings.AddToggle(
          Toggle.New(GetKey(guid), defaultValue: true, GetString(name))
            .WithLongDescription(GetString("Settings.EnableFeature")));
      }

      Menu.AddSettings(settings);
    }

    private static LocalizedString GetString(string key)
    {
      return LocalizationTool.GetString(key);
    }

    private static string GetKey(string partialKey)
    {
      return $"{RootKey}.{partialKey}";
    }
  }
}
