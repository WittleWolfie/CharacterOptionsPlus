using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Conditions
{
  [TypeId("6920e96f-2fa3-4f31-914c-8dcf5eaef476")]
  internal class IsImmuneToPrecisionDamage : ContextCondition
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(IsImmuneToPrecisionDamage));

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

        foreach (var fact in target.Facts.m_Facts)
        {
          var precisionImmunity = fact.Blueprint.GetComponent<AddImmunityToPrecisionDamage>();
          var ghostImmunities = fact.Blueprint.GetComponent<GhostCriticalAndPrecisionImmunity>();
          if (precisionImmunity is not null || ghostImmunities is not null)
            return true;
        }
      }
      catch (Exception e)
      {
        Logger.LogException("IsImmuneToPrecisionDamage.CheckCondition", e);
      }
      return false;
    }

    public override string GetConditionCaption()
    {
      return "Checks if the target is immune to precision damage";
    }
  }
}
