using BlueprintCore.Actions.Builder;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;
using BlueprintCore.Utils;

namespace CharacterOptionsPlus.Components
{
  [TypeId("d2e40f9a-36f9-42dc-aa15-5c80f934a3b2")]
  internal class AbilityAreaEffectRunActionExtended : AbilityAreaEffectRunAction
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AbilityAreaEffectRunActionExtended));

    private readonly ActionList OnSpawn;

    public AbilityAreaEffectRunActionExtended(
      ActionsBuilder onSpawn = null,
      ActionsBuilder unitEnter = null,
      ActionsBuilder newRound = null,
      ActionsBuilder unitExit = null)
    {
      OnSpawn = onSpawn?.Build() ?? Constants.Empty.Actions;
      UnitEnter = unitEnter?.Build() ?? Constants.Empty.Actions;
      Round = newRound?.Build() ?? Constants.Empty.Actions;
      UnitExit = unitExit?.Build() ?? Constants.Empty.Actions;
    }

    public override void OnUnitEnter(MechanicsContext context, AreaEffectEntityData areaEffect, UnitEntityData unit)
    {
      try
      {
        if (context.MaybeCaster is null)
        {
          Logger.Warning("No caster!");
          return;
        }

        // Assume this is right when the effect spawned
        if (Game.Instance.TimeController.GameTime - areaEffect.m_CreationTime < 0.25f.Seconds())
        {
          Logger.Log($"Running spawn actions on {unit.CharacterName}");
          using (ContextData<AreaEffectContextData>.Request().Setup(areaEffect))
          {
            using (context.GetDataScope(unit))
              OnSpawn.Run();
          }
        }
        else
        {
          base.OnUnitEnter(context, areaEffect, unit);
        }
      }
      catch (Exception e)
      {
        Logger.LogException("AbilityAreaEffectRunActionExtended.OnUnitEnter", e);
      }
    }
  }
}
