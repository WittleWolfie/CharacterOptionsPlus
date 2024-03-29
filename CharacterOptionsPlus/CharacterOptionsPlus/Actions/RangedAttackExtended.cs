﻿using CharacterOptionsPlus.Util;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Actions
{
  internal class RangedAttackExtended : ContextActionRangedAttack
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(RangedAttackExtended));

    internal ActionList OnHit;

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
