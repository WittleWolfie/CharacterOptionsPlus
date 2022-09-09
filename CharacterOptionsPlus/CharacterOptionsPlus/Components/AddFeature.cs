using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [TypeId("0f51b6f0-17ce-4695-96e9-cc55ec5e8363")]
  public class AddFeature : UnitFactComponentDelegate<AddFeature.AddFeatureData>
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AddFeature));

    public readonly BlueprintFeatureReference Feature;
    public readonly ContextValue Count;

    /// <param name="feature">Feature to add</param>
    /// <param name="count">Number of times to add the feature, defaults to 1</param>
    public AddFeature(BlueprintFeatureReference feature, ContextValue count = null)
    {
      Feature = feature;
      Count = count ?? ContextValues.Constant(1);
    }

    public override void OnActivate()
    {
      if (Data.AppliedFacts.Any())
        return;

      var count = Count.Calculate(Context);
      Logger.NativeLog($"Adding {Feature} to {Owner.CharacterName} {count} time(s)");
      for (int i = 0; i < count; i++)
        Data.AppliedFacts.Add(Owner.AddFact(Feature));
    }

    public override void OnDeactivate()
    {
      Logger.NativeLog($"Removing {Feature} from {Owner.CharacterName} {Data.AppliedFacts.Count} time(s)");
      foreach (var fact in Data.AppliedFacts)
      {
        Owner.RemoveFact(fact);
      }
      Data.AppliedFacts.Clear();
    }

    public class AddFeatureData
    {
      [JsonProperty]
      public List<EntityFact> AppliedFacts = new();
    }
  }
}
