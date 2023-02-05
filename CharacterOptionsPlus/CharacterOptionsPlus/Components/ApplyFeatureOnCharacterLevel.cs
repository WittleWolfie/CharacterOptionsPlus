using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
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
  [TypeId("611365a4-832a-4a2c-a4b0-ae8bc2ce8b41")]
  internal class ApplyFeatureOnCharacterLevel :
    UnitFactComponentDelegate<ApplyFeatureOnCharacterLevel.ComponentData>, IOwnerGainLevelHandler
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger("ApplyFeatureOnCharacterLevel");

    private readonly List<(BlueprintFeatureReference feature, int level)> FeatureLevels;
    private readonly BlueprintFeatureReference GreaterFeature;
    private readonly List<(BlueprintFeatureReference feature, int level)> GreaterFeatureLevels;

    /// <summary>Applies the specified feature at the specified level, replacing the last applied feature.</summary>
    /// <param name="greaterFeature">If the character has this feature the greaterFeatureLevels list applies</param>
    /// <param name="greaterFeatureLevels"></param>
    public ApplyFeatureOnCharacterLevel(
      List<(BlueprintFeatureReference feature, int level)> featureLevels,
      BlueprintFeatureReference greaterFeature = null,
      List<(BlueprintFeatureReference feature, int level)> greaterFeatureLevels = null)
    {
      FeatureLevels = featureLevels.ToList();
      // Reverse sort so we apply the highest level feature applicable
      FeatureLevels.Sort((a, b) => b.level.CompareTo(a.level));

      GreaterFeature = greaterFeature;
      GreaterFeatureLevels = greaterFeatureLevels;
      // Reverse sort so we apply the highest level feature applicable
      GreaterFeatureLevels?.Sort((a, b) => b.level.CompareTo(a.level));
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
          ApplyFeature(GreaterFeatureLevels);
        else
          ApplyFeature(FeatureLevels);
      }
      catch (Exception e)
      {
        Logger.LogException("ApplyFeatureOnCharacterLevel.Apply", e);
      }
    }

    private void ApplyFeature(List<(BlueprintFeatureReference feature, int level)> featureLevels)
    {
      var characterLevel = Owner.Descriptor.Progression.CharacterLevel;
      // FeatureLevels is ordered from highest to lowest. Find the highest level and apply that feature.
      foreach (var (feature, level) in featureLevels)
      {
        if (characterLevel >= level)
        {
          if (Data.AppliedLevel != level)
          {
            Remove();
            Data.AppliedFact = Owner.AddFact(feature);
            Data.AppliedLevel = level;
            Logger.Verbose(() => $"Applied {Data.AppliedFact.Name} for level {level}.");
          }
          break;
        }
      }
    }

    private void Remove()
    {
      try
      {
        if (Data.AppliedFact is not null)
        {
          Logger.Verbose(() => $"Removing {Data.AppliedFact.Name}");
          Owner.RemoveFact(Data.AppliedFact);
          Data.AppliedFact = null;
          Data.AppliedLevel = -1;
        }
      }
      catch (Exception e)
      {
        Logger.LogException("ApplyFeatureOnCharacterLevel.Remove", e);
      }
    }

    public class ComponentData
    {
      [JsonProperty]
      public EntityFact AppliedFact;

      [JsonProperty]
      public int AppliedLevel = -1;
    }
  }
}
