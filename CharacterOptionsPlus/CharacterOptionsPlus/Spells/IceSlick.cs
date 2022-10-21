using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
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

      AbilityConfigurator.New(FeatureName, Guids.IceSlickSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.IceSlickSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Cold,
          SpellDescriptor.MovementImpairing,
          SpellDescriptor.Ground)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddToSpellLists(
          level: 2,
          SpellList.Druid, 
          SpellList.Magus,
          SpellList.Ranger,
          SpellList.Wizard,
          SpellList.Witch,
          SpellList.Lich)
        .Configure();
    }
  }
}
