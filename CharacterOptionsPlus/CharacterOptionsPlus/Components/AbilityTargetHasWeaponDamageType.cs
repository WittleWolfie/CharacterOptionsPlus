using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowedOn(typeof(BlueprintAbility))]
  [TypeId("065f5e6a-f98f-418b-b701-ff6e7488a584")]
  internal class AbilityTargetHasWeaponDamageType : BlueprintComponent, IAbilityTargetRestriction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(AbilityTargetHasWeaponDamageType));

    private readonly List<PhysicalDamageForm> DamageTypes = new();
    private readonly bool Exclude;

    internal AbilityTargetHasWeaponDamageType(bool exclude, params PhysicalDamageForm[] damageTypes)
    {
      DamageTypes.AddRange(damageTypes);
      Exclude = exclude;
    }

    public string GetAbilityTargetRestrictionUIText(UnitEntityData caster, TargetWrapper target)
    {
      var damageTypes =
        DamageTypes.Select(type => LocalizedTexts.Instance.DamageForm.GetText(type));
      return Exclude
        ? string.Format(
            LocalizationTool.GetString("AbilityTargetHasWeaponSubcategory.Restriction.Negated"),
            string.Join(", ", damageTypes))
        : string.Format(
            LocalizationTool.GetString("AbilityCasterHasWeaponSubcategory.Restriction"),
            string.Join(", ", damageTypes));
    }

    public bool IsTargetRestrictionPassed(UnitEntityData caster, TargetWrapper target)
    {
      try
      {
        var unit = target.Unit;
        if (unit is null)
        {
          Logger.Warning("No target");
          return false;
        }

        var weapon = unit.GetThreatHand().MaybeWeapon;
        if (weapon is null)
          return false;

        foreach (var type in DamageTypes)
        {
          // If there's a match either we're excluding it (Exclude is true, so return false), or we're including it
          // (Exclude is false, so return true)
          if (weapon.Blueprint.DamageType.IsPhysical && weapon.Blueprint.DamageType.Physical.Form == type)
            return !Exclude;
        }
        // No match. If Exclude is true then the type is not excluded, return true; otherwise return false.
        return Exclude;
      }
      catch (Exception e)
      {
        Logger.LogException("AbilityTargetHasWeaponDamageType.IsTargetRestrictionPassed", e);
      }
      return false;
    }
  }
}
