using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Designers;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using Kingmaker.Visual.Animation.Kingmaker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.UnitParts
{
  internal class UnitPartEscapeArtist : OldStyleUnitPart
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(UnitPartEscapeArtist));

    private static BlueprintBuff _suppressBuff;
    private static BlueprintBuff SuppressBuff
    {
      get
      {
        _suppressBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.SignatureSkillAthleticsSuppressPassiveBuff);
        return _suppressBuff;
      }
    }

    private static BlueprintBuff _paralyzeBuff;
    private static BlueprintBuff ParalyzeBuff
    {
      get
      {
        _paralyzeBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.SignatureSkillAthleticsSuppressParalyzeBuff);
        return _paralyzeBuff;
      }
    }

    private static BlueprintBuff _slowBuff;
    private static BlueprintBuff SlowBuff
    {
      get
      {
        _slowBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.SignatureSkillAthleticsSuppressSlowBuff);
        return _slowBuff;
      }
    }

    [JsonProperty]
    public HashSet<Buff> BreakFreeBuffs = new();

    [JsonProperty]
    public HashSet<Buff> SuppressBuffs = new();

    [JsonProperty]
    public Buff SuppressTarget;

    [JsonProperty]
    public bool AppliesParalyze;

    [JsonProperty]
    public bool AppliesSlow;

    public void AddSupressBuff(Buff buff, bool appliesParalyze, bool appliesSlow)
    {
      SuppressBuffs.Add(buff);

      if (SuppressTarget is null)
      {
        UpdateSuppressTarget(buff, appliesParalyze, appliesSlow);
        return;
      }

      if (appliesParalyze && appliesSlow && (!AppliesParalyze || !AppliesSlow))
      {
        UpdateSuppressTarget(buff, appliesParalyze, appliesSlow);
        return;
      }

      if (appliesParalyze && !AppliesParalyze)
      {
        UpdateSuppressTarget(buff, appliesParalyze, appliesSlow);
        return;
      }

      var hasLowerDC = buff.Context.Params.DC < SuppressTarget.Context.Params.DC;
      if (hasLowerDC && AppliesParalyze == appliesParalyze && AppliesSlow == appliesSlow)
        UpdateSuppressTarget(buff, appliesParalyze, appliesSlow);
    }

    public void RemoveSuppressBuff(Buff buff)
    {
      SuppressBuffs.Remove(buff);

      if (SuppressTarget == buff)
      {
        SuppressTarget = null;
      }

      UpdateSuppressTarget();
    }

    private void UpdateSuppressTarget()
    {
      foreach (var suppressBuff in SuppressBuffs)
      {
        AddSupressBuff(
          suppressBuff,
          AppliesCondition(suppressBuff, UnitCondition.Paralyzed),
          AppliesCondition(suppressBuff, UnitCondition.Slowed));
      }
    }

    private void UpdateSuppressTarget(Buff buff, bool appliesParalyze, bool appliesSlow)
    {
      SuppressTarget = buff;
      AppliesParalyze = appliesParalyze;
      AppliesSlow = appliesSlow;
    }

    public void TrySuppress(bool spendAction = true)
    {
      if (SuppressTarget is null)
      {
        Logger.Warning("Trying to suppress null buff");
        return;
      }

      var unit = Owner.Unit;
      var dc = SuppressTarget.Context.Params.DC + 10;
      Logger.Log($"Attempting to suppress slow and paralyze on {unit.CharacterName} caused by {SuppressTarget.Name}, DC {dc}");

      var animation = unit.View.AnimationManager.CreateHandle(UnitAnimationType.Dodge);
      unit.View.AnimationManager.Execute(animation);

      if (spendAction)
      {
        var actionCost =
          unit.Stats.GetStat(StatType.SkillAthletics).BaseValue >= 20 ? CommandType.Move : CommandType.Standard;
        unit.SpendAction(actionCost, isFullRound: false, timeSinceCommandStart: 0);
      }

      var result = GameHelper.TriggerSkillCheck(new(unit, StatType.SkillAthletics, dc), SuppressTarget.Context);
      if (result.Success)
      {
        var rounds = (1 + (result.RollResult - dc) / 5).Rounds();
        if (AppliesCondition(SuppressTarget, UnitCondition.Paralyzed))
        {
          Logger.NativeLog($"Suppressing paralyze on {unit.CharacterName} caused by {SuppressTarget.Name} for {rounds} rounds");
          SuppressTarget.StoreFact(unit.AddBuff(ParalyzeBuff, SuppressTarget.Context, duration: rounds.Seconds));
        }
        if (AppliesCondition(SuppressTarget, UnitCondition.Slowed))
        {
          Logger.NativeLog($"Suppressing slow on {unit.CharacterName} caused by {SuppressTarget.Name} for {rounds} rounds");
          SuppressTarget.StoreFact(unit.AddBuff(SlowBuff, SuppressTarget.Context, duration: rounds.Seconds));
        }
      }

      SuppressTarget = null;
      UpdateSuppressTarget();
    }

    internal static bool AppliesCondition(Buff buff, UnitCondition condition)
    {
      return buff.Blueprint.ComponentsArray.Any(
        c =>
        {
          if (c is AddCondition add)
            return add.Condition == condition;
          if (c is BuffStatusCondition status)
            return status.Condition == condition;
          return false;
        });
    }

    [HarmonyPatch(typeof(Buff))]
    static class Buff_Patch
    {
      [HarmonyPatch(nameof(Buff.TickMechanics)), HarmonyPostfix]
      static void TickMechanics(Buff __instance)
      {
        try
        {
          var escapeArtist = __instance.Owner?.Get<UnitPartEscapeArtist>();
          if (escapeArtist?.SuppressTarget != __instance)
            return;

          if (!__instance.Owner.HasFact(SuppressBuff)
            || !AppliesCondition(escapeArtist.SuppressTarget, UnitCondition.Paralyzed))
          {
            return;
          }

          escapeArtist.TrySuppress();
        }
        catch (Exception e)
        {
          Logger.LogException("Buff_Patch.TickMechanics", e);
        }
      }
    }
  }
}
