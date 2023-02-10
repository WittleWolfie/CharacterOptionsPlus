using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;

namespace CharacterOptionsPlus.Actions
{
  [TypeId("e15472f7-2357-4cde-b2f2-b6f10d330049")]
  internal class RemoveHalfBuffRanks : ContextAction
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(RemoveHalfBuffRanks));

    public override string GetCaption()
    {
      return "Remove half of the current buff ranks (rounded down)";
    }

    public override void RunAction()
    {
      try
      {
        var data = ContextData<Buff.Data>.Current;
        if (data is null)
        {
          Logger.Warning("No buff!");
          return;
        }

        using (ContextData<BuffCollection.RemoveByRank>.Request())
        {
          int times = (int)Math.Ceiling(data.Buff.GetRank() / 2.0f);
          for (int i = 0; i < times; i++)
            data.Buff.Remove();
        }
      }
      catch (Exception e)
      {
        Logger.LogException("RemoveHalfBuffRanks.RunAction", e);
      }
    }
  }
}
