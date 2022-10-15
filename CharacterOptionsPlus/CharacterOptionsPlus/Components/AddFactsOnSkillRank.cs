using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [TypeId("1d9c63dd-c285-42e5-93ca-2978d5404d93")]
  internal class AddFactsOnSkillRank :
    UnitFactComponentDelegate<AddFactsOnSkillRank.ComponentData>, IOwnerGainLevelHandler
  {
    private static readonly ModLogger Logger = Logging.GetLogger("AddFactsOnSkillRank");

    private readonly StatType Skill;
    private readonly (BlueprintUnitFactReference fact, int rank)[] Facts;

    public AddFactsOnSkillRank(StatType skill, params (BlueprintUnitFactReference fact, int rank)[] facts)
    {
      Skill = skill;
      Facts = facts;
    }

    public void HandleUnitGainLevel()
    {
      Apply();
    }

    public override void OnActivate()
    {
      Apply();
    }

    public override void OnDeactivate()
    {
      Remove();
    }

    private void Apply()
    {
      try
      {
        var ranks = Owner.Stats.GetStat(Skill).BaseValue;
        foreach (var (fact, rank) in Facts)
        {
          if (ranks >= rank && !Owner.HasFact(fact))
          {
            Data.AppliedFacts.Add(Owner.AddFact(fact));
            Logger.Log($"Applied {fact.Get().Name} to {Owner.CharacterName} for {ranks} in {Skill}");
          }
        }
      }
      catch (Exception e)
      {
        Logger.LogException("AddFactsOnSkillRank.Apply", e);
      }
    }

    private void Remove()
    {
      try
      {
        foreach (var fact in Data.AppliedFacts)
        {
          if (fact is null)
            continue;
          Logger.Log($"Removing {fact.Name} from {Owner.CharacterName}");
          Owner.RemoveFact(fact);
        }
        Data.AppliedFacts.Clear();
      }
      catch (Exception e)
      {
        Logger.LogException("AddFactsOnSkillRank.Remove", e);
      }
    }

    public class ComponentData
    {
      [JsonProperty]
      public List<EntityFact> AppliedFacts = new();
    }
  }
}
