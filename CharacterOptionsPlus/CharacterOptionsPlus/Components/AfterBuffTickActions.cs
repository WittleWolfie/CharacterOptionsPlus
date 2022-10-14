using BlueprintCore.Actions.Builder;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.ContextData;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  internal interface IAfterTickEachRound
  {
    void AfterNewRound();
  }

  /// <summary>
  /// Runs after all <c>ITickEachRound</c> buff mechanics.
  /// </summary>
  [TypeId("493e5b20-7b48-433d-ad81-458e2896c1f3")]
  internal class AfterBuffTickActions : UnitBuffComponentDelegate, IAfterTickEachRound
  {
    private static readonly ModLogger Logger = Logging.GetLogger("AfterBuffTickActions");

    private readonly ActionList Actions;

    public AfterBuffTickActions(ActionsBuilder actions)
    {
      Actions = actions.Build();
    }

    public void AfterNewRound()
    {
      Buff.RunActionInContext(Actions);
    }

    [HarmonyPatch(typeof(BuffCollection))]
    static class BuffCollection_Patch
    {
      [HarmonyPatch(nameof(BuffCollection.Tick)), HarmonyPostfix]
      static void TickMechanics(Buff buff)
      {
        try
        {
          if (buff.Active && buff.IsEnabled)
          {
            using (ContextData<FactData>.Request().Setup(buff))
              buff.CallComponents<IAfterTickEachRound>(c => c.AfterNewRound());
          }
        }
        catch (Exception e)
        {
          Logger.LogException("AfterBuffTickActions.TickMechanics", e);
        }
      }
    }
  }
}
