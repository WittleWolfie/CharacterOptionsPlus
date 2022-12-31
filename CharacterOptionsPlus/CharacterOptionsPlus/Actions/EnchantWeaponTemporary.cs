using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Actions
{
  [TypeId("a2012093-2b65-4019-8696-127c4e05a08b")]
  internal class EnchantWeaponTemporary : ContextAction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(EnchantWeaponTemporary));

    internal ContextDurationValue Duration;
    internal BlueprintWeaponEnchantmentReference Enchantment;

    public override string GetCaption()
    {
      return "Enchants the target's weapon temporarily";
    }

    public override void RunAction()
    {
      try
      {
        var target = Target.Unit;
        if (target is null)
        {
          Logger.Warning("No target");
          return;
        }

        var weapon = target.GetThreatHand().MaybeWeapon;
        if (weapon is null)
        {
          Logger.Warning("No weapon");
          return;
        }

        weapon.AddEnchantment(Enchantment, Context, Duration.Calculate(Context));
      }
      catch (Exception e)
      {
        Logger.LogException("EnchantWeaponTemporary.RunAction", e);
      }
    }
  }
}
