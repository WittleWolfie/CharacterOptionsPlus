using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;

namespace CharacterOptionsPlus.Actions
{
  /// <summary>
  /// Runs an action a limited number of times by counting in a shared value.
  /// </summary>
  [TypeId("54c90b97-d6d7-4550-b9d9-068965776cd1")]
  internal class CountRunAction : ContextAction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(CountRunAction));

    internal ActionList Actions;
    internal AbilitySharedValue Counter;
    internal int Count;

    public override string GetCaption()
    {
      return "Runs an action a limited number of times";
    }

    public override void RunAction()
    {
      try
      {
        if (Context[Counter] >= Count)
        {
          Logger.Verbose($"Skipping action, already run {Context[Counter]} times");
          return;
        }

        Context[Counter]++;
        using (Context.GetDataScope(Target))
          Actions.Run();
      }
      catch (Exception e)
      {
        Logger.LogException("CountRunAction.RunAction", e);
      }
    }
  }
}
