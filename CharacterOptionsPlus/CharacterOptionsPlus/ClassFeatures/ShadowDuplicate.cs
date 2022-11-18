using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class ShadowDuplicate
  {
    private const string FeatureName = "ShadowDuplicate";

    internal const string DisplayName = "ShadowDuplicate.Name";
    private const string Description = "ShadowDuplicate.Description";

    private const string AbilityName = "ShadowDuplicate.Ability";
    private const string BuffName = "ShadowDuplicate.Buff";
    private const string HiddenBuffName = "ShadowDuplicate.Buff.Hidden";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ShadowDuplicateTalent))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ShadowDuplicate.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.ShadowDuplicateBuff).Configure();
      BuffConfigurator.New(HiddenBuffName, Guids.ShadowDuplicateHiddenBuff).Configure();
      ActivatableAbilityConfigurator.New(AbilityName, Guids.ShadowDuplicateAbility).Configure();
      FeatureConfigurator.New(FeatureName, Guids.ShadowDuplicateTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = BuffRefs.MirrorImageBuff.Reference.Get().Icon;
      var buff = BuffConfigurator.New(BuffName, Guids.ShadowDuplicateBuff)
        .CopyFrom(BuffRefs.MirrorImageBuff)
        .SetDisplayName(DisplayName)
        .AddMirrorImage(count: ContextDice.Value(DiceType.One))
        .Configure();

      var hiddenBuff = BuffConfigurator.New(HiddenBuffName, Guids.ShadowDuplicateHiddenBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .AddMirrorImage(count: ContextDice.Value(DiceType.One))
        .AddTargetAttackRollTrigger(
          actionOnSelf:
            ActionsBuilder.New()
              .RemoveSelf()
              .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank()), asChild: false),
          onlyHit: true)
        .AddContextRankConfig(
          ContextRankConfigs.ClassLevel(classes: new[] { CharacterClassRefs.RogueClass.ToString() }))
        .Configure();

      ActivatableAbilityConfigurator.New(AbilityName, Guids.ShadowDuplicateAbility)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetBuff(hiddenBuff)
        .Configure();

      FeatureConfigurator.New(FeatureName, Guids.ShadowDuplicateTalent, FeatureGroup.RogueTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetIsClassFeature()
        .AddFacts(new() { AbilityName })
        .Configure(delayed: true);
    }
  }
}
