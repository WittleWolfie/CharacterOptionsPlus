using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Conditions
{
  [TypeId("1ca328f5-2b39-496c-9241-98577942d946")]
  internal class HasCondition : ContextCondition
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(HasCondition));

    internal UnitCondition Condition;

    public override bool CheckCondition()
    {
      try
      {
        var target = Target.Unit;
        if (target is null)
        {
          Logger.Warning("No target");
          return false;
        }

        return target.State.HasCondition(Condition);
      }
      catch (Exception e)
      {
        Logger.LogException("HasCondition.CheckCondition", e);
      }
      return false;
    }

    public override string GetConditionCaption()
    {
      return "Checks if the target has a given condition";
    }
  }
}
