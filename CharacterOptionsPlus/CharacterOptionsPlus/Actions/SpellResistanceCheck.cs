using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;

namespace CharacterOptionsPlus.Actions
{
  internal class SpellResistanceCheck : ContextAction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(SpellResistanceCheck));

    internal ActionList OnResistSucceed = Constants.Empty.Actions;
    internal ActionList OnResistFail = Constants.Empty.Actions;

    public override string GetCaption()
    {
      return "Make a spell resistance check";
    }

    public override void RunAction()
    {
      try
      {
        if (Context.MaybeCaster is null)
        {
          Logger.Warning("No caster!");
          return;
        }

        if (Target?.Unit is null)
        {
          Logger.Warning("No target!");
          return;
        }

        if (Rulebook.Trigger<RuleSpellResistanceCheck>(new(Context, Target.Unit)).IsSpellResisted)
          OnResistSucceed.Run();
        else
          OnResistFail.Run();
      }
      catch (Exception e)
      {
        Logger.LogException("SpellResistanceCheck.RunAction", e);
      }
    }
  }
}
