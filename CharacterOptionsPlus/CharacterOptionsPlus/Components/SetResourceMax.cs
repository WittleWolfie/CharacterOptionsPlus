using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [TypeId("2d933a77-2c29-42ef-aad1-cfd2a4197ac1")]
  public class SetResourceMax : UnitFactComponentDelegate, IResourceAmountBonusHandler
  {
    private static readonly ModLogger Logger = Logging.GetLogger("SetResourceAmountMax");

    private readonly ContextValue Max;
    private readonly BlueprintAbilityResourceReference Resource;

    public SetResourceMax(ContextValue max, BlueprintAbilityResourceReference resource)
    {
      Max = max;
      Resource = resource;
    }

    public void CalculateMaxResourceAmount(BlueprintAbilityResource resource, ref int bonus)
    {
      try
      {
        if (Resource.deserializedGuid == resource.AssetGuid)
          bonus = Max.Calculate(Context);
      }
      catch (Exception e)
      {
        Logger.LogException("SetResourceAmountMax.CalculateMaxResourceAmount", e);
      }
    }
  }
}
