using BlueprintCore.Actions.Builder;
using CharacterOptionsPlus.Util;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Actions
{
  internal class RangedAttackExtended : ContextActionRangedAttack
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(RangedAttackExtended));

    private readonly ActionList OnHit;

    public RangedAttackExtended(ActionsBuilder onHit)
    {
      OnHit = onHit.Build();
    }

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

        if (attack.AttackRoll.IsHit)
          OnHit.Run();
      }
      catch (Exception e)
      {
        Logger.LogException("RangedAttackExtended.RunAction", e);
      }
    }
  }
}
