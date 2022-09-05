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

      public void AfterBlueprintCachePatches() { }

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


          // Must init settings first
          Settings.Init();

          PatchArchetypes();
          PatchFeats();
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to initialize.", e);
        }
      }

      private static void PatchArchetypes()
      {
        Logger.Log("Patching archetypes.");

        ArrowsongMinstrel.Configure();
      }

      private static void PatchFeats()
      {
        Logger.Log("Patching feats.");

        FuriousFocus.Configure();
        Hurtful.Configure();
        SkaldsVigor.Configure();
      }
    }
  }
}
