using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
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
  [TypeId("198f1661-1db2-48d4-b336-d8fa5e289a07")]
  internal class AbilityTargetHasWeaponSubcategory : BlueprintComponent, IAbilityTargetRestriction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(AbilityCasterHasWeaponSubcategory));

    private readonly List<WeaponSubCategory> SubCategories = new();
    private readonly bool Exclude;

    internal AbilityTargetHasWeaponSubcategory(bool exclude, params WeaponSubCategory[] subCategories)
    {
      SubCategories.AddRange(subCategories);
      Exclude = exclude;
    }

    public string GetAbilityTargetRestrictionUIText(UnitEntityData caster, TargetWrapper target)
    {
      var subCategories =
        SubCategories.Select(subCategory => LocalizedTexts.Instance.WeaponSubCategories.GetText(subCategory));
      return Exclude
        ? string.Format(
            LocalizationTool.GetString("AbilityTargetHasWeaponSubcategory.Restriction.Negated"),
            string.Join(", ", subCategories))
        : string.Format(
            LocalizationTool.GetString("AbilityCasterHasWeaponSubcategory.Restriction"),
            string.Join(", ", subCategories));
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

        var weapon = unit.GetFirstWeapon();
        if (weapon is null)
          return false;

        foreach (var subCategory in SubCategories)
        {
          // If there's a match either we're excluding it (Exclude is true, so return false), or we're including it
          // (Exclude is false, so return true)
          if (weapon.Blueprint.Category.HasSubCategory(subCategory))
            return !Exclude;
        }
        // No match. If Exclude is true then the type is not excluded, return true; otherwise return false.
        return Exclude;
      }
      catch (Exception e)
      {
        Logger.LogException("AbilityTargetHasWeaponSubcategory.IsTargetRestrictionPassed", e);
      }
      return false;
    }
  }
}
