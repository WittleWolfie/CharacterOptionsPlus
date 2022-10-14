using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Conditions
{
  [TypeId("fdbf45eb-cb0b-4875-b1d3-153b57561c31")]
  internal class HasConditions : ContextCondition
  {
    private static readonly ModLogger Logger = Logging.GetLogger("HasCondition");

    private readonly UnitCondition[] Conditions;
    private readonly bool Caster;
    private readonly bool All;

    public HasConditions(List<UnitCondition> conditions, bool caster = false, bool all = false)
    {
      Conditions = conditions.ToArray();
      Caster = caster;
      All = all;
    }

    public override bool CheckCondition()
    {
      try
      {
        var unit = Caster ? Context?.MaybeCaster : Context?.MaybeOwner;
        if (unit is null)
        {
          Logger.Warning("No unit for condition checking");
          return false;
        }

        foreach (var condition in Conditions)
        {
          if (unit.State.HasCondition(condition) && !All)
            return true;

          if (All && !unit.State.HasCondition(condition))
            return false;
        }
        // If !All this is reached if unit does not have any condition.
        // If All this is reached if unit has all conditions.
        return All;
      }
      catch (Exception e)
      {
        Logger.LogException("HasCondition.CheckCondition", e);
      }
      return false;
    }

    public override string GetConditionCaption()
    {
      var unit = Caster ? $"{Context.MaybeCaster?.CharacterName}" : $"{Context.MaybeOwner?.CharacterName}";
      var amount = All ? "all of" : "any of";
      return $"{unit} has {amount} {string.Join(", ", Conditions)}";
    }
  }
}
