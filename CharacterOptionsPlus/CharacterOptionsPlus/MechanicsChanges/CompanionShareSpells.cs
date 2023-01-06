using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using System;

namespace CharacterOptionsPlus.MechanicsChanges
{
  internal class CompanionShareSpells
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(CompanionShareSpells));

    [HarmonyPatch(typeof(AbilityData))]
    static class CompanionShareSpells_Patch
    {
      [HarmonyPatch(nameof(AbilityData.CanTarget)), HarmonyPrefix]
      static bool CanTarget(AbilityData __instance, TargetWrapper target, ref bool __result)
      {
        try
        {
          if (__instance.Range != AbilityRange.Personal)
            return true;

          if (__instance.Caster.Unit == target.Unit)
            return true;

          if (!Settings.IsEnabled(Homebrew.CompanionShareSpells))
            return true;

          if (target.Unit is null)
          {
            Logger.Warning($"No target for personal spell: {__instance.Blueprint.Name}");
            return true;
          }

          var petType = target.Unit.Get<UnitPartPet>()?.Type;
          if (petType is null || petType != PetType.AnimalCompanion)
          {
            Logger.Verbose($"Target is not an animal companion: {petType}");
            __result = false;
            return false;
          }

          var master = target.Unit.Master;
          if (master != __instance.Caster.Unit)
          {
            Logger.Verbose($"Target is not the caster's pet: {target.Unit.CharacterName} is owned by {master.CharacterName}");
            __result = false;
            return false;
          }

          __result = true;
          return false;
        }
        catch (Exception e)
        {
          Logger.LogException("CompanionShareSpells_Patch.CanTarget", e);
        }
        return true;
      }

      [HarmonyPatch(nameof(AbilityData.TargetAnchor), MethodType.Getter), HarmonyPrefix]
      static bool GetTargetAnchor(AbilityData __instance, ref AbilityTargetAnchor __result)
      {
        try
        {
          if (__instance.Range != AbilityRange.Personal)
            return true;

          if (!Settings.IsEnabled(Homebrew.CompanionShareSpells))
            return true;

          if (__instance.Caster.Unit.Get<UnitPartPetMaster>() == null)
          {
            Logger.Verbose($"Caster has no animal companion: {__instance.Caster.Unit}");
            return true;
          }

          __result = AbilityTargetAnchor.Unit;
          return false;
        }
        catch (Exception e)
        {
          Logger.LogException("CompanionShareSpells_Patch.CanTarget", e);
        }
        return true;
      }
    }
  }
}
