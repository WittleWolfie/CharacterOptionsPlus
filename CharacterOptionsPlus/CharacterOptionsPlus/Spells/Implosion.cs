using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class Implosion
  {
    private const string FeatureName = "Implosion";

    internal const string DisplayName = "Implosion.Name";
    internal const string ConcentrationDisplayName = "Implosion.Concentration.Name";
    private const string Description = "Implosion.Description";

    private const string BuffName = "Implosion.Buff";
    private const string ConcentrationBuffName = "Implosion.Buff.Concentration";
    private const string CooldownBuff = "Implosion.Buff.Cooldown";
    private const string AbilityName = "Implosion.Ability";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ImplosionSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("Implosion.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(CooldownBuff, Guids.ImplosionCooldownBuff).Configure();
      BuffConfigurator.New(ConcentrationBuffName, Guids.ImplosionCooldownBuff).Configure();
      AbilityConfigurator.New(AbilityName, Guids.ImplosionAbility).Configure();
      BuffConfigurator.New(BuffName, Guids.ImplosionBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.ImplosionSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var cooldownBuff = BuffConfigurator.New(CooldownBuff, Guids.ImplosionCooldownBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Stack)
        .Configure();

      var concentrationBuff = BuffConfigurator.New(ConcentrationBuffName, Guids.ImplosionConcentrationBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Prolong)
        .AddFactContextActions(deactivated: ActionsBuilder.New().RemoveBuff(Guids.ImplosionBuff))
        .Configure();

      var ability = AbilityConfigurator.NewSpell(
          AbilityName,
          Guids.ImplosionAbility,
          SpellSchool.Evocation,
          canSpecialize: true)
        .SetDisplayName(ConcentrationDisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.Contagion.Reference.Get().Icon)
        .SetLocalizedDuration(Common.DurationRoundPerTwoLevels)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetAnimation(CastAnimationStyle.Point)
        .SetShouldTurnToTarget()
        .SetActionType(CommandType.Standard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel().WithMultiplyByModifierProgression(1))
        .AddAbilityDeliverProjectile(
          type: AbilityProjectileType.Simple, projectiles: new() { ProjectileRefs.WindProjectile00.ToString() })
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(concentrationBuff, ContextDuration.Fixed(1), isNotDispelable: true, toCaster: true)
            .ConditionalSaved(
              failed: ActionsBuilder.New()
                .DealDamage(DamageTypes.Direct(), ContextDice.Value(DiceType.Zero, bonus: ContextValues.Rank()))),
          savingThrowType: SavingThrowType.Fortitude)
        .AddAbilityTargetHasFact(
          checkedFacts: new() { cooldownBuff, FeatureRefs.Incorporeal.ToString(), },
          inverted: true)
        .Configure();

      var buff = BuffConfigurator.New(BuffName, Guids.ImplosionBuff)
        .SetDisplayName(ConcentrationDisplayName)
        .SetDescription(Description)
        .AddFacts(new() { ability })
        .AddComponent<ImplosionComponent>()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.ImplosionSpell,
          SpellSchool.Evocation,
          canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.Contagion.Reference.Get().Icon)
        .SetLocalizedDuration(Common.DurationRoundPerTwoLevels)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetAnimation(CastAnimationStyle.Point)
        .SetShouldTurnToTarget()
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(level: 9, SpellList.Cleric)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel().WithDiv2Progression())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank()), isNotDispelable: true, toCaster: true)
            .CastSpell(ability))
        .AddAbilityTargetHasFact(
          checkedFacts: new() { FeatureRefs.Incorporeal.ToString(), },
          inverted: true)
        .Configure();
    }

    [TypeId("9365d00f-c4e2-41e8-a3fa-2768f11f9c57")]
    private class ImplosionComponent :
      UnitBuffComponentDelegate<ImplosionComponent.ComponentData>,
      ITargetRulebookHandler<RuleDealDamage>,
      IInitiatorRulebookHandler<RuleCastSpell>
    {
      private static BlueprintAbility _implosionAbility;
      private static BlueprintAbility ImplosionAbility
      {
        get
        {
          _implosionAbility ??= BlueprintTool.Get<BlueprintAbility>(Guids.ImplosionAbility);
          return _implosionAbility;
        }
      }

      private static BlueprintBuff _cooldown;
      private static BlueprintBuff Cooldown
      {
        get
        {
          _cooldown ??= BlueprintTool.Get<BlueprintBuff>(Guids.ImplosionCooldownBuff);
          return _cooldown;
        }
      }

      public void OnEventAboutToTrigger(RuleDealDamage evt) { }

      public void OnEventDidTrigger(RuleDealDamage evt)
      {
        try
        {
          if (!Rulebook.Trigger<RuleCheckConcentration>(new(Owner, Context.SourceAbilityContext.Ability, evt)).Success)
            Buff.Remove();
        }
        catch (Exception e)
        {
          Logger.LogException("ImplosionComponent.OnEventDidTrigger(RuleDealDamage)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleCastSpell evt) { }

      public void OnEventDidTrigger(RuleCastSpell evt)
      {
        try
        {
          if (evt.Spell.Blueprint != ImplosionAbility)
            return;

          var cooldown = evt.SpellTarget.Unit.AddBuff(Cooldown, evt.Context, Buff.TimeLeft);
          cooldown.IsNotDispelable = true;
          Data.AppliedBuffs.Add(cooldown);
        }
        catch (Exception e)
        {
          Logger.LogException("ImplosionComponent.OnEventDidTrigger(RuleCastSpell)", e);
        }
      }

      public override void OnDeactivate()
      {
        try
        {
          foreach (var buff in Data.AppliedBuffs)
          {
            if (buff.IsActive)
              buff.Remove();
          }
          Data.AppliedBuffs.Clear();
        }
        catch (Exception e)
        {
          Logger.LogException("ImplosionComponent.OnEventAboutToTrigger(RuleCastSpell)", e);
        }
      }

      public class ComponentData
      {
        [JsonProperty]
        public List<Buff> AppliedBuffs = new();
      }
    }
  }
}
