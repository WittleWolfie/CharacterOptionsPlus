using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowedOn(typeof(BlueprintAbility))]
  [AllowedOn(typeof(BlueprintActivatableAbility))]
  [TypeId("44b89d49-a223-4b98-bd5d-76c67657ff66")]
  internal class AlwaysAddToActionBar : UnitFactComponentDelegate
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AlwaysAddToActionBar));

    public override void OnTurnOn()
    {
      try
      {
        Owner.UISettings.RemoveFromAlreadyAutomaticallyAdded((BlueprintAbility)Fact.Blueprint);
        base.OnTurnOn();
      }
      catch (Exception e)
      {
        Logger.LogException("AlwaysAddToActionBar.OnTurnOn", e);
      }
    }
  }
}
