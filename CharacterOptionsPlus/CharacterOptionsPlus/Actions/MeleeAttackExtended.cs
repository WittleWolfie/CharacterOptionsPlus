using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Actions
{
  internal class MeleeAttackExtended : ContextActionMeleeAttack
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(MeleeAttackExtended));

    internal ActionList OnHit = Constants.Empty.Actions;

    public override void RunAction()
    {
      try
      {
        base.RunAction();
        var attack = AbilityContext.RulebookContext?.LastEvent<RuleAttackWithWeapon>();
        if (attack is null)
        {
          Logger.Warning("No attack triggered");
          return;
        }

        Logger.Verbose($"Result: {attack.AttackRoll.IsHit}");
        if (attack.AttackRoll.IsHit)
          OnHit.Run();
      }
      catch (Exception e)
      {
        Logger.LogException("MeleeAttackExtended.RunAction", e);
      }
    }
  }
}
