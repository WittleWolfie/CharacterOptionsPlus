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
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowedOn(typeof(BlueprintAbility))]
  [TypeId("446b614c-fbd4-4c25-98f6-94c3cc0f5eae")]
  internal class AbilityCasterHasWeaponSubcategory : BlueprintComponent, IAbilityCasterRestriction
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AbilityCasterHasWeaponSubcategory));

    private readonly List<WeaponSubCategory> SubCategories = new();

    internal AbilityCasterHasWeaponSubcategory(params WeaponSubCategory[] subCategories)
    {
      SubCategories.AddRange(subCategories);
    }

    public string GetAbilityCasterRestrictionUIText()
    {
      var subCategories =
        SubCategories.Select(subCategory => LocalizedTexts.Instance.WeaponSubCategories.GetText(subCategory));
      return string.Format(
        LocalizationTool.GetString("AbilityCasterHasWeaponSubcategory.Restriction"),
        string.Join(", ", subCategories));
    }

    public bool IsCasterRestrictionPassed(UnitEntityData caster)
    {
      try
      {
        if (caster is null)
        {
          Logger.Warning("No target");
          return false;
        }

        var weapon = caster.GetThreatHand().MaybeWeapon;
        if (weapon is null)
          return false;

        foreach (var subCategory in SubCategories)
        {
          if (weapon.Blueprint.Category.HasSubCategory(subCategory))
            return true;
        }
      }
      catch (Exception e)
      {
        Logger.LogException("AbilityCasterHasWeaponSubcategory.IsCasterRestrictionPassed", e);
      }
      return false;
    }
  }
}
