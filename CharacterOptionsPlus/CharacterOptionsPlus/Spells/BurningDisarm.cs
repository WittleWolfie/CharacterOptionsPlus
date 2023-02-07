using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Craft;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class BurningDisarm
  {
    private const string FeatureName = "BurningDisarm";

    internal const string DisplayName = "BurningDisarm.Name";
    private const string Description = "BurningDisarm.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "burningdisarm.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.BurningDisarmSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("BurningDisarm.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.BurningDisarmSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var dealDamage = ActionsBuilder.New()
        .DealDamage(
          damageType: DamageTypes.Energy(DamageEnergyType.Fire),
          value: ContextDice.Value(DiceType.D4, diceCount: ContextValues.Rank()));
      AbilityConfigurator.NewSpell(
          FeatureName, Guids.BurningDisarmSpell, SpellSchool.Transmutation, canSpecialize: true, SpellDescriptor.Fire)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.RoundPerLevel)
        .SetLocalizedSavingThrow(SavingThrow.ReflexNegates)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Burning,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalCold,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.Flaring,
          (Metamagic)CustomMetamagic.Intensified,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(level: 1, SpellList.Cleric, SpellList.Druid, SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 5))
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .Conditional(
              conditions: ConditionsBuilder.New().Add<WantsToDropWeapon>(),
              ifTrue: ActionsBuilder.New()
                .SavingThrow(
                  SavingThrowType.Reflex,
                  onResult: ActionsBuilder.New()
                    .ConditionalSaved(
                      succeed: ActionsBuilder.New().Add<Disarm>(),
                      failed: dealDamage)),
              ifFalse: dealDamage))
        .AddComponent(AbilityTargetHasWeaponEquipped.AnyHand())
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.Reflex,
          spellType: CraftSpellType.Debuff)
        .Configure();
    }

    [TypeId("4671134d-cc49-4d75-9e02-b6dd149b4b85")]
    private class Disarm : ContextAction
    {
      public override string GetCaption()
      {
        return "Disarms the target for round per level (max 5)";
      }

      public override void RunAction()
      {
        try
        {
          var target = Target?.Unit;
          if (target is null)
          {
            Logger.Warning("No target");
            return;
          }

          var abilityParams = Context.Params;
          if (abilityParams is null)
          {
            Logger.Warning("No ability parameters");
            return;
          }

          var disarmDuration = Math.Min(abilityParams.CasterLevel, 5).Rounds();

          var mainHand = target.Body.PrimaryHand.MaybeWeapon;
          if (mainHand is not null && !mainHand.Blueprint.IsUnarmed && !mainHand.Blueprint.IsNatural)
          {
            Logger.Verbose(() => $"Disarming {target.CharacterName}'s main hand weapon");
            target.Descriptor.AddBuff(
              BlueprintRoot.Instance.SystemMechanics.DisarmMainHandBuff, Context, disarmDuration.Seconds);
            return;
          }

          var offHand = target.Body.SecondaryHand.MaybeWeapon;
          if (offHand is not null && !offHand.Blueprint.IsUnarmed && !offHand.Blueprint.IsNatural)
          {
            Logger.Verbose(() => $"Disarming {target.CharacterName}'s off hand weapon");
            target.Descriptor.AddBuff(
              BlueprintRoot.Instance.SystemMechanics.DisarmOffHandBuff, Context, disarmDuration.Seconds);
            return;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("Disarm.RunAction", e);
        }
      }
    }

    [TypeId("01c302b4-0642-4947-b0e2-5b309f9899cd")]
    private class WantsToDropWeapon : ContextCondition
    {
      private enum StatLevel
      {
        Low,
        Moderate,
        High
      }

      private enum CasterType
      {
        None,
        Limited,
        Full
      }

      private enum HealthLevel
      {
        Critical,
        Low,
        Moderate
      }

      public override bool CheckCondition()
      {
        try
        {
          var target = Target?.Unit;
          if (target is null)
          {
            Logger.Warning("No target");
            return false;
          }

          var abilityParams = Context.Params;
          if (abilityParams is null)
          {
            Logger.Warning("No ability parameters");
            return false;
          }

          var intLevel = GetStatLevel(target, StatType.Intelligence);
          var wisLevel = GetStatLevel(target, StatType.Wisdom);

          if (
            (intLevel == StatLevel.Low && wisLevel != StatLevel.High)
            || (wisLevel == StatLevel.Low && intLevel != StatLevel.High))
          {
            Logger.Verbose(() => 
              $"{target.CharacterName} is not too bright ({intLevel} Int, {wisLevel} Wis), they want to drop their weapon");
            return true;
          }

          var casterType = GetCasterType(target);
          if (intLevel == StatLevel.Moderate && wisLevel == StatLevel.Moderate)
          {
            switch (casterType)
            {
              case CasterType.None:
              case CasterType.Limited:
                Logger.Verbose(() => $"{target.CharacterName} needs their weapon");
                return false;
              case CasterType.Full:
                Logger.Verbose(() => $"{target.CharacterName} relies on spells, they want to drop their weapon");
                return true;
            }
          }

          // At this point either Int or Wis must be high
          var healthLevel = GetHealthLevel(target, abilityParams.CasterLevel);
          switch (casterType)
          {
            case CasterType.None:
              switch (healthLevel)
              {
                case HealthLevel.Critical:
                  Logger.Verbose(() => $"{target.CharacterName}'s health is {healthLevel}, they want to drop their weapon");
                  return true;
                default:
                  Logger.Verbose(() => $"{target.CharacterName} can take the hit and needs their weapon");
                  return false;
              }
            case CasterType.Limited:
              switch (healthLevel)
              {
                case HealthLevel.Critical:
                case HealthLevel.Low:
                  Logger.Verbose(() => $"{target.CharacterName}'s health is {healthLevel}, they want to drop their weapon");
                  return true;
                default:
                  Logger.Verbose(() => $"{target.CharacterName} can take the hit and needs their weapon");
                  return false;
              }
            case CasterType.Full:
              Logger.Verbose(() => $"{target.CharacterName} relies on spells, they want to drop their weapon");
              return true;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("WantsToDropWeapon.CheckCondition", e);
        }
        return false;
      }

      public override string GetConditionCaption()
      {
        return "If true, the target wants to attempt a reflex save to drop their weapon";
      }

      private static StatLevel GetStatLevel(UnitEntityData unit, StatType stat)
      {
        var statValue = unit.Stats.GetStat(stat);
        if (statValue < 9)
          return StatLevel.Low;
        if (statValue < 15)
          return StatLevel.Moderate;
        return StatLevel.High;
      }

      private static CasterType GetCasterType(UnitEntityData unit)
      {
        var characterLevel = unit.Progression.CharacterLevel;
        var maxSpellLevel = 0;
        foreach (var spellbook in unit.Spellbooks)
          maxSpellLevel = Math.Max(maxSpellLevel, spellbook.GetMaxSpellLevel());

        if (maxSpellLevel == 0)
          return CasterType.None;

        var spellLevelRatio = (float) maxSpellLevel / characterLevel;
        if (spellLevelRatio < 0.5)
          return CasterType.Limited;

        return CasterType.Full;
      }

      private static HealthLevel GetHealthLevel(UnitEntityData unit, int casterLevel)
      {
        var hp = unit.Stats.HitPoints.ModifiedValue - unit.Damage;
        if (hp < casterLevel * 2)
          return HealthLevel.Critical;
        if (hp < casterLevel * 4)
          return HealthLevel.Low;
        return HealthLevel.Moderate;
      }
    }
  }
}
