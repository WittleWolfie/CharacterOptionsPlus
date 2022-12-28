using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Actions
{
  [TypeId("1ae0c78e-df94-4fc6-bbc9-81f6922b2ab3")]
  internal class SetHitPoints : ContextAction
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(SetHitPoints));

    public ContextValue Value;

    public override string GetCaption()
    {
      return "Set's the target's current hitpoints";
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

        var value = Value.Calculate(Context);
        var dmg = new DirectDamage(DiceFormula.Zero, bonus: target.HPLeft - value);
        dmg.IgnoreReduction = true;

        var dealDmg = new RuleDealDamage(Context.MaybeCaster, target, dmg);
        dealDmg.SourceAbility = Context?.SourceAbility;
        Rulebook.Trigger<RuleDealDamage>(dealDmg);
      }
      catch (Exception e)
      {
        Logger.LogException("SetHitPoints.RunAction", e);
      }
    }
  }
}
