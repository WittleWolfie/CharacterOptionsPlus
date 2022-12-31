using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [AllowedOn(typeof(BlueprintWeaponEnchantment))]
  [TypeId("e4f9b2e9-02c9-4479-a740-09d539efa04f")]
  internal class WeaponDamageBonus : WeaponEnchantmentLogic, IInitiatorRulebookHandler<RuleCalculateWeaponStats>
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(WeaponDamageBonus));

    private readonly ContextValue Bonus;
    private readonly ModifierDescriptor Descriptor;

    internal WeaponDamageBonus(ContextValue bonus, ModifierDescriptor descriptor = ModifierDescriptor.UntypedStackable)
    {
      Bonus = bonus;
      Descriptor = descriptor;
    }

    public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
    {
      try
      {
        if (evt.Weapon == Owner)
          evt.AddDamageModifier(Bonus.Calculate(Context), Enchantment, Descriptor);
      }
      catch (Exception e)
      {
        Logger.LogException("WeaponDamageBonus.OnEventAboutToTrigger", e);
      }
    }

    public void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }
  }
}
