using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Actions
{
  internal class ConditionalHitDice : ContextAction
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(ConditionalHitDice));

    internal ActionList OnHigh = Constants.Empty.Actions;
    internal ActionList OnInBetween = Constants.Empty.Actions;
    internal ActionList OnLower = Constants.Empty.Actions;

    internal ContextValue HighValue = 0;
    internal ContextValue LowValue = 0;

    public override string GetCaption()
    {
      return "Takes actions based on the target's hit dice";
    }

    public override void RunAction()
    {
      try
      {
        var caster = Context.MaybeCaster;
        if (caster is null)
        {
          Logger.Warning("No caster");
          return;
        }

        var target = Context.MainTarget.Unit;
        if (target is null)
        {
          Logger.Warning("No target");
          return;
        }

        var high = HighValue.Calculate(Context);
        var low = LowValue.Calculate(Context);
        var targetHD = target.Progression.CharacterLevel;

        if (targetHD >= high)
        {
          OnHigh.Run();
        }
        else if (targetHD <= low)
        {
          OnLower.Run();
        }
        else
        {
          OnInBetween.Run();
        }
      }
      catch (Exception e)
      {
        Logger.LogException("ConditionalHitDice.RunAction", e);
      }
    }
  }
}
