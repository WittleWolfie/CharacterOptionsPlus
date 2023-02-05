using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using System;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class ArmoredMarauder
  {
    private const string FeatureName = "ArmoredMarauder";

    internal const string DisplayName = "ArmoredMarauder.Name";
    private const string Description = "ArmoredMarauder.Description";

    private const string SwiftnessFeature = "ArmoredSwiftness";
    private const string SwiftnessDisplayName = "ArmoredSwiftness.Name";
    private const string SwiftnessDescription = "ArmoredSwiftness.Description";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ArmoredMarauderTalent))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ArmoredMarauder.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      FeatureConfigurator.New(FeatureName, Guids.ArmoredMarauderTalent).Configure();
      FeatureConfigurator.New(SwiftnessFeature, Guids.ArmoredSwiftnessTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var proficiency = FeatureRefs.HeavyArmorProficiency.Reference.Get();
      var marauder = FeatureConfigurator.New(FeatureName, Guids.ArmoredMarauderTalent, FeatureGroup.SlayerTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(proficiency.Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddPrerequisiteFeature(FeatureRefs.AdvanceTalents.ToString())
        .AddFacts(new() { proficiency })
        .AddContextRankConfig(
          ContextRankConfigs.ClassLevel(
              classes: new[] { CharacterClassRefs.SlayerClass.ToString() })
            .WithDivStepProgression(6))
        .AddArmorCheckPenaltyIncrease(
          bonus: ContextValues.Rank(), category: ArmorProficiencyGroup.Heavy, checkCategory: true)
        .Configure(delayed: true);

      FeatureConfigurator.New(SwiftnessFeature, Guids.ArmoredSwiftnessTalent, FeatureGroup.SlayerTalent)
        .SetDisplayName(SwiftnessDisplayName)
        .SetDescription(SwiftnessDescription)
        .SetIcon(FeatureRefs.ArmorTraining.Reference.Get().Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddPrerequisiteFeature(marauder)
        .AddFacts(new() { proficiency })
        .AddContextRankConfig(
          ContextRankConfigs.ClassLevel(
              classes: new[] { CharacterClassRefs.SlayerClass.ToString() })
            .WithDivStepProgression(6))
        .AddArmorSpeedPenaltyRemoval()
        .AddComponent<ArmoredSwiftness>()
        .Configure(delayed: true);
    }

    [TypeId("b8ebc7f2-eeaa-488f-bf71-de721348c41c")]
    private class ArmoredSwiftness :
      UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateArmorMaxDexBonusLimit>
    {
      private readonly ContextValue Bonus = ContextValues.Rank();

      public void OnEventAboutToTrigger(RuleCalculateArmorMaxDexBonusLimit evt)
      {
        try
        {
          if (evt.Armor.Blueprint.ProficiencyGroup != ArmorProficiencyGroup.Heavy)
          {
            Logger.Verbose(() => $"Armored swiftness does not apply: {evt.Armor.Blueprint.ProficiencyGroup}");
            return;
          }

          var bonus = Bonus.Calculate(Context);
          Logger.Verbose(() => $"Increasing max dex limit by {bonus}");
          evt.AddBonus(bonus);
        }
        catch (Exception e)
        {
          Logger.LogException("ArmoredSwiftness.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateArmorMaxDexBonusLimit evt) { }
    }
  }
}
