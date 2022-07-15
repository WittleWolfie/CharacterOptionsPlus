using BlueprintCore.Utils;
using CharacterOptionsPlus.Feats;
using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System;
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
        Logger.Info("Finished patching.");
      }
      catch (Exception e)
      {
        Logger.Error("Failed to patch", e);
      }
      return true;
    }

    [HarmonyPatch(typeof(BlueprintsCache))]
    static class BlueprintsCaches_Patch
    {
      private static bool Initialized = false;

      [HarmonyPriority(Priority.Last)]
      [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
      static void Postfix()
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

        SkaldsVigor.Configure();
      }
    }
  }
}
