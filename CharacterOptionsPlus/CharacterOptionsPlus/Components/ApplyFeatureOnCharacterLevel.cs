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
    private static readonly ModLogger Logger = Logging.GetLogger("ApplyFeatureOnCharacterLevel");

    private readonly List<(BlueprintFeatureReference feature, int level)> FeatureLevels;

    public ApplyFeatureOnCharacterLevel(params (BlueprintFeatureReference feature, int level)[] featureLevels)
    {
      FeatureLevels = featureLevels.ToList();
      // Reverse sort so we apply the highest level feature applicable
      FeatureLevels.Sort((a, b) => b.level.CompareTo(a.level));
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
        var characterLevel = Owner.Descriptor.Progression.CharacterLevel;
        // FeatureLevels is ordered from highest to lowest. Find the highest level and apply that feature.
        foreach (var (feature, level) in FeatureLevels)
        {
          if (characterLevel >= level)
          {
            if (Data.AppliedLevel != level)
            {
              Remove();
              Logger.Log($"Applying {feature} for level {level}.");
              Data.AppliedFact = Owner.AddFact(feature);
              Data.AppliedLevel = level;
            }
            break;
          }
        }
      }
      catch (Exception e)
      {
        Logger.LogException("ApplyFeatureOnCharacterLevel.Apply", e);
      }
    }

    private void Remove()
    {
      try
      {
        if (Data.AppliedFact is not null)
        {
          Logger.Log($"Removing {Data.AppliedFact.Name}");
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
      public EntityFact AppliedFact = new();

      [JsonProperty]
      public int AppliedLevel = -1;
    }
  }
}
