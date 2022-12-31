using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Utility;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Conditions
{
  [TypeId("fc411c44-2931-41b9-afde-d142d2d8adc3")]
  internal class DistanceFromCaster : ContextCondition
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(DistanceFromCaster));

    internal ContextValue DistanceInFeet;

    public override bool CheckCondition()
    {
      try
      {
        var caster = Context.MaybeCaster;
        if (caster is null)
        {
          Logger.Warning("No caster");
          return false;
        }

        var target = Target.Unit;
        if (target is null)
        {
          Logger.Warning("No target");
          return false;
        }

        return caster.DistanceTo(target) <= DistanceInFeet.Calculate(Context).Feet().Meters;
      }
      catch (Exception e)
      {
        Logger.LogException("DistanceFromCaster.CheckCondition", e);
      }
      return false;
    }

    public override string GetConditionCaption()
    {
      return "Checks if the target is within a given distance from the caster";
    }
  }
}
