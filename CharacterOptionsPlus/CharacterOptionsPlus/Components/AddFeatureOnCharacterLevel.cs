using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [TypeId("15c28aa4-c943-4cab-ae6a-3892c92a6c06")]
  internal class AddFeatureOnCharacterLevel :
    UnitFactComponentDelegate<AddFeatureOnCharacterLevel.ComponentData>, IOwnerGainLevelHandler
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger("AddFeatureOnCharacterLevel");

    private readonly List<(BlueprintFeatureReference feature, int level)> FeatureLevels;
    private readonly BlueprintFeatureReference GreaterFeature;
    private readonly List<(BlueprintFeatureReference feature, int level)> GreaterFeatureLevels;

    /// <summary>Adds the specified feature at the specified level. Limit one feature per level, max 20.</summary>
    /// <param name="greaterFeature">If the character has this feature the greaterFeatureLevels list applies</param>
    /// <param name="greaterFeatureLevels"></param>
    public AddFeatureOnCharacterLevel(
      List<(BlueprintFeatureReference feature, int level)> featureLevels,
      BlueprintFeatureReference greaterFeature = null,
      List<(BlueprintFeatureReference feature, int level)> greaterFeatureLevels = null)
    {
      FeatureLevels = featureLevels.ToList();
      GreaterFeature = greaterFeature;
      GreaterFeatureLevels = greaterFeatureLevels;
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
        if (GreaterFeature is not null && Owner.HasFact(GreaterFeature))
          AddFeature(GreaterFeatureLevels);
        else
          AddFeature(FeatureLevels);
      }
      catch (Exception e)
      {
        Logger.LogException("AddFeatureOnCharacterLevel.Apply", e);
      }
    }

    private void AddFeature(List<(BlueprintFeatureReference feature, int level)> featureLevels)
    {
      var characterLevel = Owner.Descriptor.Progression.CharacterLevel;
      // FeatureLevels is ordered from highest to lowest. Find the highest level and apply that feature.
      foreach (var (feature, level) in featureLevels)
      {
        if (characterLevel >= level)
        {
          if (Data.AppliedFacts[level] is null)
          {
            Data.AppliedFacts[level] = Owner.AddFact(feature);
            Logger.Verbose($"Applied {Data.AppliedFacts[level].Name} to {Owner.CharacterName} for level {level}.");
          }
        }
      }
    }

    private void Remove()
    {
      try
      {
        for (int i = 0; i < Data.AppliedFacts.Length; i++)
        {
          var fact = Data.AppliedFacts[i];
          if (fact is null)
            continue;
          Logger.Verbose($"Removing {fact.Name} from {Owner.CharacterName}");
          Owner.RemoveFact(fact);
          Data.AppliedFacts[i] = null;
        }
      }
      catch (Exception e)
      {
        Logger.LogException("AddFeatureOnCharacterLevel.Remove", e);
      }
    }

    public class ComponentData
    {
      [JsonProperty]
      public EntityFact[] AppliedFacts = new EntityFact[20];
    }
  }
}
