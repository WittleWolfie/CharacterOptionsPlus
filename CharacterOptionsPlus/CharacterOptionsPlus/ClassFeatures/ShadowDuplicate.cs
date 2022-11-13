using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
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
  internal class ShadowDuplicate
  {
    private const string FeatureName = "ShadowDuplicate";

    internal const string DisplayName = "ShadowDuplicate.Name";
    private const string Description = "ShadowDuplicate.Description";

    private const string AbilityName = "ShadowDuplicate.Ability";

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

      ActivatableAbilityConfigurator.New(AbilityName, Guids.ShadowDuplicateAbility).Configure();
      FeatureConfigurator.New(FeatureName, Guids.ShadowDuplicateTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = BuffRefs.MirrorImageBuff.Reference.Get().Icon;
      ActivatableAbilityConfigurator.New(AbilityName, Guids.ShadowDuplicateAbility)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .Configure();

      FeatureConfigurator.New(
          FeatureName, Guids.ShadowDuplicateTalent, FeatureGroup.SlayerTalent, FeatureGroup.RogueTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetIsClassFeature()
        .AddFacts(new() { AbilityName })
        .Configure(delayed: true);
    }
  }
}
