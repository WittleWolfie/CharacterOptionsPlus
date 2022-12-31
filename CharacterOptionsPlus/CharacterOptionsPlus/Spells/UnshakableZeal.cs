using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class UnshakableZeal
  {
    private const string FeatureName = "UnshakableZeal";

    internal const string DisplayName = "UnshakableZeal.Name";
    private const string Description = "UnshakableZeal.Description";

    private const string BuffName = "UnshakableZeal.Buff";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.UnshakableZealSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("UnshakableZeal.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.UnshakableZealBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.UnshakableZealSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.UnshakableZealBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .AddSavingThrowBonusAgainstDescriptor(
          bonus: 2, spellDescriptor: SpellDescriptor.Fear | SpellDescriptor.Emotion)
        .AddComponent<UnshakableZealComponent>()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.UnshakableZealSpell,
          SpellSchool.Conjuration,
          canSpecialize: true,
          SpellDescriptor.Summoning,
          SpellDescriptor.Evil)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.BurstOfGlory.Reference.Get().Icon)
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
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Encouraging)
        .AddToSpellLists(level: 5, SpellList.Bard, SpellList.Inquisitor)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), DurationRate.Hours)))
        .Configure();
    }

    [TypeId("1239ba46-45b3-4c4b-bd98-ae8ace1f3252")]
    private class UnshakableZealComponent :
      UnitBuffComponentDelegate<UnshakableZealComponent.ComponentData>,
      IInitiatorRulebookHandler<RuleAttackRoll>,
      IInitiatorRulebookHandler<RuleSkillCheck>,
      IInitiatorRulebookHandler<RuleSavingThrow>,
      IInitiatorRulebookHandler<RuleCheckConcentration>
    {
      public void OnEventAboutToTrigger(RuleAttackRoll evt)
      {
        try
        {
          if (!Data.MissedAttacks.Contains(evt.Target))
            return;

          Data.MissedAttacks.Remove(evt.Target);
          evt.AddModifier(4, Buff, ModifierDescriptor.Morale);
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventAboutToTrigger(RuleAttackRoll)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleSkillCheck evt)
      {
        try
        {
          if (!Data.FailedSkillChecks.Contains(evt.StatType))
            return;

          Data.FailedSkillChecks.Remove(evt.StatType);
          evt.AddModifier(4, Buff, ModifierDescriptor.Morale);
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventAboutToTrigger(RuleSkillCheck)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleSavingThrow evt)
      {
        try
        {
          var caster = evt.Reason.Context?.MaybeCaster;
          var source = evt.Reason.Context?.SourceAbility;
          var failedSave = Data.FailedSavingThrows.Where(save => save.Caster == caster && save.Ability == source).FirstOrDefault();
          if (failedSave is null)
            return;

          Data.FailedSavingThrows.Remove(failedSave);
          evt.AddModifier(4, Buff, ModifierDescriptor.Morale);
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventAboutToTrigger(RuleSavingThrow)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleCheckConcentration evt)
      {
        try
        {
          if (!Data.FailedConcentration)
            return;

          Data.FailedConcentration = false;
          evt.AddModifier(4, Buff, ModifierDescriptor.Morale);
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventAboutToTrigger(RuleCheckConcentration)", e);
        }
      }

      public void OnEventDidTrigger(RuleAttackRoll evt)
      {
        try
        {
          if (evt.IsHit)
            return;

          Data.MissedAttacks.Add(evt.Target);
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventDidTrigger(RuleAttackRoll)", e);
        }
      }

      public void OnEventDidTrigger(RuleSkillCheck evt)
      {
        try
        {
          if (evt.Success)
            return;

          Data.FailedSkillChecks.Add(evt.StatType);
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventDidTrigger(RuleSkillCheck)", e);
        }
      }

      public void OnEventDidTrigger(RuleSavingThrow evt)
      {
        try
        {
          if (evt.Success)
            return;

          var caster = evt.Reason.Context?.MaybeCaster;
          var source = evt.Reason.Context?.SourceAbility;
          Data.FailedSavingThrows.Add(new(caster, source));
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventDidTrigger(RuleSavingThrow)", e);
        }
      }

      public void OnEventDidTrigger(RuleCheckConcentration evt)
      {
        try
        {
          if (evt.Success)
            return;

          Data.FailedConcentration = true;
        }
        catch (Exception e)
        {
          Logger.LogException("UnshakableZealComponent.OnEventDidTrigger(RuleCheckConcentration)", e);
        }
      }

      public class SavingThrow
      {
        public readonly UnitEntityData Caster;
        public readonly BlueprintAbility Ability;

        public SavingThrow(UnitEntityData caster, BlueprintAbility ability)
        {
          Caster = caster;
          Ability = ability;
        }
      }

      public class ComponentData
      {
        public readonly HashSet<UnitEntityData> MissedAttacks = new();
        public readonly HashSet<StatType> FailedSkillChecks = new();
        public readonly List<SavingThrow> FailedSavingThrows = new();
        public bool FailedConcentration = false;
      }
    }
  }
}
