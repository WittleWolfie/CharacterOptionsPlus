using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowedOn(typeof(BlueprintAbility))]
  [TypeId("f1878982-2799-4927-b8df-c742c6b5e56b")]
  internal class AbilityTargetHasWeaponEquipped : BlueprintComponent, IAbilityTargetRestriction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(AbilityTargetHasWeaponEquipped));

    public string GetAbilityTargetRestrictionUIText(UnitEntityData caster, TargetWrapper target)
    {
      return LocalizationTool.GetString("AbilityTargetHasWeaponEquipped.Restriction");
    }

    public bool IsTargetRestrictionPassed(UnitEntityData caster, TargetWrapper target)
    {
      try
      {
        var unit = target?.Unit;
        if (unit is null)
        {
          Logger.Warning("No target");
          return false;
        }

        var mainHandWeapon = unit.Body.PrimaryHand.MaybeWeapon;
        if (mainHandWeapon is not null && !mainHandWeapon.Blueprint.IsNatural && !mainHandWeapon.Blueprint.IsUnarmed)
          return true;

        var offHandWeapon = unit.Body.SecondaryHand.MaybeWeapon;
        if (offHandWeapon is not null && !offHandWeapon.Blueprint.IsNatural && !offHandWeapon.Blueprint.IsUnarmed)
          return true;
      }
      catch(Exception e)
      {
        Logger.LogException("AbilityTargetHasWeaponEquipped.IsTargetRestrictionPassed", e);
      }
      return false;
    }
  }
}
