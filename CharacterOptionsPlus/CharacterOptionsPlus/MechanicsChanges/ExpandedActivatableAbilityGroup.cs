using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Parts;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.MechanicsChanges
{
  internal class ExpandedActivatableAbilityGroup
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(ExpandedActivatableAbilityGroup));

    internal const ActivatableAbilityGroup AsmodeusTechnique = (ActivatableAbilityGroup)1050;

    [HarmonyPatch(typeof(UnitPartActivatableAbility))]
    static class UnitPartActivatableAbility_Patch
    {
      [HarmonyPatch(nameof(UnitPartActivatableAbility.GetGroupSize)), HarmonyPrefix]
      static bool GetGroupSize(ActivatableAbilityGroup group, ref int __result)
      {
        try
        {
          if (group == AsmodeusTechnique)
          {
            __result = 1;
            return false;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("UnitPartActivatableAbility_Patch.GetGroupSize", e);
        }
        return true;
      }
    }
  }
}
