using BlueprintCore.Actions.Builder;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  internal class AreaEffectSpawnUnitActions : AreaEffectSpawnLogic
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AreaEffectSpawnUnitActions));

    private readonly ActionList OnSpawn;
    private readonly ConditionsChecker OnSpawnConditions;

    public AreaEffectSpawnUnitActions(ActionsBuilder onSpawn, ConditionsBuilder onSpawnConditions = null)
    {
      OnSpawn = onSpawn.Build();
      OnSpawnConditions = onSpawnConditions?.Build() ?? Constants.Empty.Conditions;
    }

    public override void OnAreaEffectSpawn(MechanicsContext context, AreaEffectEntityData areaEffect)
    {
      try
      {
        using (ContextData<AreaEffectContextData>.Request().Setup(areaEffect))
        {
          foreach (var unit in areaEffect.InGameUnitsInside)
          {
            using (areaEffect.Context.GetDataScope(unit))
            {
              if (OnSpawnConditions.Check())
              {
                Logger.Log($"[{areaEffect.Blueprint.name}] Running spawn actions on {unit.CharacterName}");
                OnSpawn.Run();
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        Logger.LogException("AreaEffectHandler.OnAreaEffectSpawn", e);
      }
    }
  }
}
