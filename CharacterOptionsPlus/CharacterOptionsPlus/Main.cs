using BlueprintCore.Utils;
using CharacterOptionsPlus.Feats;
using HarmonyLib;
using Kingmaker.PubSubSystem;
using System;
using TabletopTweaks.Core.NewEvents;
using UnityModManagerNet;

namespace CharacterOptionsPlus
{
  public static class Main
  {
    private static readonly LogWrapper Logger = LogWrapper.Get("CharacterOptionsPlus");

    public static bool Load(UnityModManager.ModEntry modEntry)
    {
      try
      {
        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll();
        EventBus.Subscribe(new BlueprintCacheInitHandler());
        Logger.Info("Finished patching.");
      }
      catch (Exception e)
      {
        Logger.Error("Failed to patch", e);
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
            Logger.Info("Already initialized blueprints cache.");
            return;
          }
          Initialized = true;

          PatchFeats();
        }
        catch (Exception e)
        {
          Logger.Error("Failed to initialize.", e);
        }
      }

      private static void PatchFeats()
      {
        Logger.Info("Patching feats.");

        FuriousFocus.Configure();
        Hurtful.Configure();
        SkaldsVigor.Configure();
      }
    }
  }
}
