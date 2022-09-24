using BlueprintCore.Blueprints.Configurators.Root;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Archetypes;
using CharacterOptionsPlus.Feats;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.PubSubSystem;
using System;
using TabletopTweaks.Core.NewEvents;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus
{
  public static class Main
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(Main));

    public static bool Load(UnityModManager.ModEntry modEntry)
    {
      try
      {
        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll();

        EventBus.Subscribe(new BlueprintCacheInitHandler());

        Logger.Log("Finished patching.");
      }
      catch (Exception e)
      {
        Logger.LogException("Failed to patch", e);
      }
      return true;
    }

    class BlueprintCacheInitHandler : IBlueprintCacheInitHandler
    {
      private static bool Initialized = false;
      private static bool InitializeDelayed = false;

      public void AfterBlueprintCachePatches()
      {
        try
        {
          if (InitializeDelayed)
          {
            Logger.Log("Already initialized blueprints cache.");
            return;
          }
          InitializeDelayed = true;

          RootConfigurator.ConfigureDelayedBlueprints();
        }
        catch (Exception e)
        {
          Logger.LogException("Delayed blueprint configuration failed.", e);
        }
      }

      public void BeforeBlueprintCacheInit() { }

      public void BeforeBlueprintCachePatches() { }

      public void AfterBlueprintCacheInit()
      {
        try
        {
          if (Initialized)
          {
            Logger.Log("Already initialized blueprints cache.");
            return;
          }
          Initialized = true;

          // First strings
          LocalizationTool.LoadEmbeddedLocalizationPacks(
            "CharacterOptionsPlus.Strings.Archetypes.json",
            "CharacterOptionsPlus.Strings.BugFixes.json",
            "CharacterOptionsPlus.Strings.Feats.json",
            "CharacterOptionsPlus.Strings.Settings.json");

          // Then settings
          Settings.Init();

          BugFixes.Configure();

          ConfigureArchetypes();
          ConfigureFeats();
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to initialize.", e);
        }
      }

      private static void ConfigureArchetypes()
      {
        Logger.Log("Configuring archetypes.");

        ArrowsongMinstrel.Configure();
      }

      private static void ConfigureFeats()
      {
        Logger.Log("Configuring feats.");

        FuriousFocus.Configure();
        GloriousHeat.Configure();
        Hurtful.Configure();
        PairedOpportunists.Configure();
        SkaldsVigor.Configure();
      }
    }
  }
}
