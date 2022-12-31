using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class SlowingStrike
  {
    private const string FeatureName = "SlowingStrike";

    internal const string DisplayName = "SlowingStrike.Name";
    private const string Description = "SlowingStrike.Description";

    private const string BuffName = "SlowingStrike.Buff";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.SlowingStrikeTalent))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("SlowingStrike.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.SlowingStrikeBuff).Configure();
      FeatureConfigurator.New(FeatureName, Guids.SlowingStrikeTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = FeatureRefs.TiringCriticalFeature.Reference.Get().Icon;
      var buff = BuffConfigurator.New(BuffName, Guids.SlowingStrikeBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .AddCondition(condition: UnitCondition.Slowed)
        .Configure();

      FeatureConfigurator.New(FeatureName, Guids.SlowingStrikeTalent, FeatureGroup.SlayerTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddRecalculateOnStatChange(stat: StatType.Intelligence)
        .AddPrerequisiteClassLevel(CharacterClassRefs.SlayerClass.ToString(), level: 4)
        .AddContextCalculateAbilityParamsBasedOnClass(
          CharacterClassRefs.SlayerClass.ToString(), statType: StatType.Intelligence)
        .AddInitiatorAttackRollTrigger(
          onlyHit: true,
          sneakAttack: true,
          action: ActionsBuilder.New()
            .SavingThrow(
              SavingThrowType.Fortitude,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  failed: ActionsBuilder.New()
                    .Conditional(
                      conditions: ConditionsBuilder.New().HasFact(buff),
                      ifTrue: ActionsBuilder.New().IncreaseBuffDuration(ContextDuration.FixedDice(DiceType.D4), buff),
                      ifFalse: ActionsBuilder.New().ApplyBuff(buff, ContextDuration.FixedDice(DiceType.D4))))))
        .Configure(delayed: true);
    }
  }
}
