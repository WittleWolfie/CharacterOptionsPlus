using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Parts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CharacterOptionsPlus.UnitParts
{
  internal class UnitPartTouchCharges : OldStyleUnitPart
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(UnitPartTouchCharges));

    [JsonProperty]
    private int Charges = 0;

    internal void Init(int charges)
    {
      Charges = charges;
    }

    internal void UseCharge() { Charges--; }
    internal bool HasCharge() { return Charges > 0; }
    internal void Remove() { Charges = 0; }

    [HarmonyPatch(typeof(TouchSpellsController))]
    static class TouchSpellsController_Patch
    {
      static readonly MethodInfo RemoveUnitPart =
        AccessTools.Method(typeof(EntityDataBase), "Remove", generics: new[] { typeof(UnitPartTouch) });

      private static void ConsumeCharge(UnitEntityData caster)
      {
        var charges = caster.Get<UnitPartTouchCharges>();
        if (charges is null)
        {
          Logger.Verbose("No charges to consume");
          caster.Remove<UnitPartTouch>();
          return;
        }

        Logger.Verbose($"Consuming touch charge on {caster.CharacterName}");
        charges.UseCharge();
        if (!charges.HasCharge())
        {
          Logger.Verbose("No charges remaining");
          caster.Remove<UnitPartTouch>();
        }
      }

      [HarmonyPatch(nameof(TouchSpellsController.OnAbilityEffectApplied)), HarmonyTranspiler]
      static IEnumerable<CodeInstruction> OnAbilityEffectApplied(IEnumerable<CodeInstruction> instructions)
      {
        try
        {
          var code = new List<CodeInstruction>(instructions);

          // Find the removal of the unit part
          var removalIndex = -1;
          for (int i = 0; i < code.Count; i++)
          {
            if (code[i].Calls(RemoveUnitPart))
            {
              removalIndex = i;
              break;
            }
          }
          if (removalIndex < 0)
            throw new InvalidOperationException("Unable to find touch part removal.");

          code.RemoveAt(removalIndex);
          code.Insert(removalIndex, CodeInstruction.Call(typeof(TouchSpellsController_Patch), nameof(ConsumeCharge)));
          return code;
        }
        catch (Exception e)
        {
          Logger.LogException("TouchSpellsController_Patch.OnAbilityEffectApplied", e);
        }
        return instructions;
      }
    }

    [HarmonyPatch(typeof(UnitPartTouch))]
    static class UnitPartTouch_Patch
    {
      [HarmonyPatch(nameof(UnitPartTouch.Init)), HarmonyPostfix]
      static void Init(BlueprintAbility ability, AbilityExecutionContext context)
      {
        try
        {
          var caster = context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var touchCharges = ability.GetComponent<TouchCharges>();
          if (touchCharges is null)
          {
            Logger.Verbose("Not a touch ability");
            return;
          }

          var count = touchCharges.GetChargeCount(context);
          var charges = caster.Ensure<UnitPartTouchCharges>();
          charges.Init(count);
          Logger.Verbose($"Added {count} touch charges for {caster.CharacterName}");
        }
        catch (Exception e)
        {
          Logger.LogException("UnitPartTouch_Patch.Init", e);
        }
      }

      [HarmonyPatch(nameof(UnitPartTouch.OnDispose)), HarmonyPostfix]
      static void OnDispose(UnitPartTouch __instance)
      {
        try
        {
          __instance.Owner.Remove<UnitPartTouchCharges>();
        }
        catch (Exception e)
        {
          Logger.LogException("UnitPartTouch_Patch.OnDispose", e);
        }
      }
    }

    [AllowedOn(typeof(BlueprintAbility))]
    [TypeId("10e90122-6ef9-4967-a284-259aed042cd7")]
    internal class TouchCharges : UnitFactComponentDelegate
    {
      private readonly ContextValue Charges;

      internal TouchCharges(ContextValue charges)
      {
        Charges = charges;
      }

      internal int GetChargeCount(MechanicsContext context)
      {
        return Charges.Calculate(context);
      }
    }
  }
}
