using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Parts;
using System;
using TabletopTweaks.Core.NewActions;
using static Kingmaker.RuleSystem.RulebookEvent;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Spells
{
  internal class NineLives
  {
    private const string FeatureName = "NineLives";

    internal const string DisplayName = "NineLives.Name";
    private const string Description = "NineLives.Description";

    private const string BuffName = "NineLives.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "ninelives.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.NineLivesSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("NineLives.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.NineLivesBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.NineLivesSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.NineLivesBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetRanks(9)
        .SetStacking(StackingType.Rank)
        .AddComponent<NineLivesComponent>()
        .AddConditionImmunity(UnitCondition.Prone)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.NineLivesSpell, SpellSchool.Abjuration, canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.HourPerLevel)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(self: true, friends: true)
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Quicken,
          Metamagic.Reach)
        .AddToSpellLists(level: 8, SpellList.Cleric, SpellList.Witch)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .RemoveBuff(buff) // Clear the old buff first
            .Add<ContextActionApplyBuffRanks>(
              a =>
              {
                a.m_Buff = buff.ToReference<BlueprintBuffReference>();
                a.Rank = 9;
                a.DurationValue = ContextDuration.Variable(ContextValues.Rank(), DurationRate.Hours);
              }))
        .Configure(delayed: true);
    }

    [TypeId("5792b1f0-f0ac-46db-846c-34b7158d2747")]
    private class NineLivesComponent :
      UnitBuffComponentDelegate,
      ISavingThrowFailed,
      IDamageHandler,
      IUnitConditionsChanged,
      IUnitConditionChangeAttemptHandler,
      IUnitBuffHandler,
      ITargetRulebookHandler<RuleAttackRoll>,
      ITargetRulebookHandler<RuleCombatManeuver>
    {
      private static readonly CustomDataKey FortificationKey = new("NineLives.Fortification");
      private static readonly CustomDataKey RerollKey = new("NineLives.CatsLuck");

      #region Stay Up
      public void HandleUnitConditionAddAttempt(UnitEntityData unit, UnitCondition condition, bool success)
      {
        try
        {
          if (unit != Owner || condition != UnitCondition.Prone)
            return;

          Logger.Verbose(() => "Removing proine condition");
          Owner.State.RemoveCondition(UnitCondition.Prone);
          RemoveRank();
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.HandleUnitConditionAddAttempt", e);
        }
      }

      public void OnEventAboutToTrigger(RuleCombatManeuver evt) { }

      public void OnEventDidTrigger(RuleCombatManeuver evt)
      {
        try
        {
          if (evt.Type != CombatManeuver.Trip)
            return;

          if (!evt.Success)
            return;

          Logger.Verbose(() => "Trip attempted, removing a charge");
          RemoveRank();
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.OnEventDidTrigger(RuleCombatManeuver)", e);
        }
      }
      #endregion

      #region Cat's Luck
      public void HandleSavingThrowFailed(RuleSavingThrow rule)
      {
        try
        {
          if (rule.Initiator != Owner)
            return;

          if (rule.IsPassed)
            return;

          // Don't let this chain until success!
          if (rule.TryGetCustomData(RerollKey, out bool _))
          {
            Logger.Verbose(() => "Skipping reroll chain");
            return;
          }

          Logger.Verbose(() => "Rerolling saving throw");
          var newSavingThrow = new RuleSavingThrow(Owner, rule);
          newSavingThrow.SetCustomData(RerollKey, true);
          rule.IsAlternativePassed = Rulebook.Trigger(newSavingThrow).Success;
          RemoveRank();
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.HandleSavingThrowFailed", e);
        }
      }
      #endregion

      #region Fortitude
      public void OnEventAboutToTrigger(RuleAttackRoll evt)
      {
        try
        {
          Logger.Verbose(() => "Setting fortification to 100%");
          var fortification = Owner.Ensure<UnitPartFortification>();
          evt.SetCustomData(FortificationKey, fortification.Value);
          fortification.Add(100); 
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.OnEventAboutToTrigger(RuleAttackRoll)", e);
        }
      }

      public void OnEventDidTrigger(RuleAttackRoll evt)
      {
        try
        {
          Logger.Verbose(() => "Removing 100% fortification");
          Owner.Get<UnitPartFortification>()?.Remove(100);
          if (!evt.FortificationNegatesCriticalHit && !evt.FortificationNegatesSneakAttack)
          {
            Logger.Verbose(() => "Fortification had no effect");
            return;
          }

          if (evt.TryGetCustomData(FortificationKey, out int fortificationValue))
          {
            // The fortifiation roll would have overcome existing fortification, so use a charge
            if (evt.FortificationRoll > fortificationValue)
            {
              Logger.Verbose(() => "Fortification effect was used, removing a charge");
              RemoveRank();
            }
          }
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.OnEventDidTrigger(RuleAttackRoll)", e);
        }
      }
      #endregion

      #region Rejuvenate
      public void HandleDamageDealt(RuleDealDamage dealDamage)
      {
        try
        {
          if (dealDamage.Target != Owner)
            return;

          if (Owner.HPLeft > 0)
            return;

          Logger.Verbose(() => $"Healing {Owner}");
          var heal = new RuleHealDamage(Owner, Owner, new DiceFormula(rollsCount: 3, diceType: DiceType.D6), bonus: 0);
          heal.SourceFact = Buff;
          Rulebook.Trigger(heal);
          RemoveRank();
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.HandleDamageDealt", e);
        }
      }
      #endregion

      #region Shake Off
      public void HandleUnitConditionsChanged(UnitEntityData unit, UnitCondition condition)
      {
        try
        {
          if (unit != Owner)
            return;

          if (!unit.State.HasCondition(condition))
            return;

          switch (condition)
          {
            case UnitCondition.Blindness:
            case UnitCondition.Confusion:
            case UnitCondition.Cowering:
            case UnitCondition.Dazed:
            case UnitCondition.Dazzled:
            case UnitCondition.Entangled:
            case UnitCondition.Exhausted:
            case UnitCondition.Fatigued:
            case UnitCondition.Frightened:
            case UnitCondition.Nauseated:
            case UnitCondition.Shaken:
            case UnitCondition.Sickened:
            case UnitCondition.Staggered:
              Logger.Verbose(() => $"Blocking condition: {unit} - {condition}");
              unit.State.RemoveCondition(condition);
              RemoveRank();
              break;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.HandleUnitConditionsChanged", e);
        }
      }
      #endregion

      #region Shimmy Out
      public void HandleBuffDidAdded(Buff buff)
      {
        try
        {
          if (buff.Owner != Owner)
            return;

          var grapple = Owner.Get<UnitPartGrappleTarget>();
          if (grapple is null)
            return;

          if (grapple.m_Buff != buff)
            return;

          Logger.Verbose(() => "Breaking grapple");
          Owner.Remove<UnitPartGrappleTarget>();
          RemoveRank();
        }
        catch (Exception e)
        {
          Logger.LogException("NineLivesComponent.HandleBuffDidAdded", e);
        }
      }

      public void HandleBuffDidRemoved(Buff buff) { }
      #endregion

      private void RemoveRank()
      {
        using (ContextData<BuffCollection.RemoveByRank>.Request())
          Buff.Remove();
      }
    }
  }
}
