using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [TypeId("c7869037-c989-45e0-a4e1-d00daf644fbc")]
  internal class SuppressConditions : UnitBuffComponentDelegate<SuppressConditions.ComponentData>
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger("SuppressCondition");

    private readonly UnitCondition[] Conditions;

    public SuppressConditions(params UnitCondition[] conditions)
    {
      Conditions = conditions;
    }

    public override void OnActivate()
    {
      try
      {
        foreach (var condition in Conditions)
        {
          if (Owner.State.HasCondition(condition))
          {
            Logger.Verbose(() => $"Suppressing {string.Join(", ", Conditions)} on {Owner.CharacterName}");
            Owner.State.RemoveCondition(condition);
            Data.SuppressedConditions.Add(condition);
          }
        }
      }
      catch (Exception e)
      {
        Logger.LogException("SuppressCondition.OnActivate", e);
      }
    }

    public override void OnDeactivate()
    {
      try
      {
        foreach (var condition in Data.SuppressedConditions)
        {
          Logger.Verbose(() => $"Removing suppression of {condition} on {Owner.CharacterName}");
          Owner.State.AddCondition(condition);
        }
        Data.SuppressedConditions.Clear();
      }
      catch (Exception e)
      {
        Logger.LogException("SuppressCondition.OnDeactivate", e);
      }
    }

    public class ComponentData
    {
      [JsonProperty]
      public List<UnitCondition> SuppressedConditions = new();
    }
  }
}
