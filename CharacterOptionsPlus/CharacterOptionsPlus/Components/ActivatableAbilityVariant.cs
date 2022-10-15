using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [TypeId("96a7530c-6473-48aa-bdf4-3d72eea0bd03")]
  internal class ActivatableAbilityVariant : UnitFactComponentDelegate
  {
    private static readonly ModLogger Logger = Logging.GetLogger("ActivatableAbilityVariant");

    private readonly BlueprintActivatableAbilityReference Ability;

    public ActivatableAbilityVariant(BlueprintActivatableAbilityReference ability)
    {
      Ability = ability;
    }

    public override void OnTurnOn()
    {
      try
      {
        var fact = Owner.GetFact<ActivatableAbility>(Ability);
        if (fact is null)
          return;

        fact.IsOn = false;
      }
      catch (Exception e)
      {
        Logger.LogException("ActivatableAbilityVariant.OnTurnOn", e);
      }
    }
  }
}
