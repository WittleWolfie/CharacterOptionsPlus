using HarmonyLib;
using Kingmaker.View.MapObjects;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Util
{
  internal static class FxTool
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(FxTool));

    private static readonly Dictionary<string, Action<GameObject>> AoESpawnHandlers;

    /// <summary>
    /// Only a single handler can be registered per <c>BlueprintAbilityAreaEffect</c>.
    /// </summary>
    internal static void RegisterAoESpawnHandler(string blueprintName, Action<GameObject> spawnHandler)
    {
      if (AoESpawnHandlers.ContainsKey(blueprintName))
        throw new InvalidOperationException($"A handler has already been registered for {blueprintName}. Only one supported.");

      AoESpawnHandlers[blueprintName] = spawnHandler;
    }

    [HarmonyPatch(typeof(AreaEffectView))]
    static class AreaEffectView_Patch
    {
      [HarmonyPatch(nameof(AreaEffectView.SpawnFxs)), HarmonyPostfix]
      static void SpawnFxs(AreaEffectView __instance)
      {
        try
        {
          var bpName = __instance.m_Blueprint.Get().name;
          if (AoESpawnHandlers.ContainsKey(bpName))
            __instance.m_SpawnedFx?.RunAfterSpawn(AoESpawnHandlers[bpName]);
        }
        catch (Exception e)
        {
          Logger.LogException("AreaEffectView_Patch.SpawnFxs", e);
        }
      }
    }
  }
}
