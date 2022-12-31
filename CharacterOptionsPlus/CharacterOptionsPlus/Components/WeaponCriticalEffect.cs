using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [AllowedOn(typeof(BlueprintWeaponEnchantment))]
  [TypeId("a6e5e854-5ae6-48dd-950f-ee364768e171")]
  internal class WeaponCriticalEffect : WeaponEnchantmentLogic, IInitiatorRulebookHandler<RuleAttackWithWeapon>
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(WeaponCriticalEffect));

    private readonly ActionList Effect;

    internal WeaponCriticalEffect(ActionList effect)
    {
      Effect = effect;
    }

    public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

    public void OnEventDidTrigger(RuleAttackWithWeapon evt)
    {
      try
      {
        if (evt.Weapon != Owner)
          return;

        if (!evt.AttackRoll.IsCriticalConfirmed)
          return;

        var context = Fact as IFactContextOwner;
        context.RunActionInContext(Effect, evt.Target);
      }
      catch (Exception e)
      {
        Logger.LogException("WeaponCriticalEffect.OnEventDidTrigger", e);
      }
    }
  }
}
