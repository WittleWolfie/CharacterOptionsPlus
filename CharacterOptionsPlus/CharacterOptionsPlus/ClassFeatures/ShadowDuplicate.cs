using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Parts;
using System;
using static Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class ShadowDuplicate
  {
    private const string FeatureName = "ShadowDuplicate";

    internal const string DisplayName = "ShadowDuplicate.Name";
    private const string Description = "ShadowDuplicate.Description";
    private const string SlayerDescription = "ShadowDuplicate.Description.Slayer";

    private const string AbilityName = "ShadowDuplicate.Ability";
    private const string ResourceName = "ShadowDuplicate.Resource";
    private const string BuffName = "ShadowDuplicate.Buff";
    private const string HiddenBuffName = "ShadowDuplicate.Buff.Hidden";

    private const string SlayerFeatureName = "ShadowDuplicate.Slayer";
    private const string SlayerResourceName = "ShadowDuplicate.Resource.Slayer";
    private const string SlayerAbilityName = "ShadowDuplicate.Ability.Slayer";
    private const string SlayerHiddenBuffName = "ShadowDuplicate.Buff.Hidden.Slayer";

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

      BuffConfigurator.New(SlayerHiddenBuffName, Guids.ShadowDuplicateHiddenBuffSlayer).Configure();
      ActivatableAbilityConfigurator.New(SlayerAbilityName, Guids.ShadowDuplicateAbilitySlayer).Configure();
      FeatureConfigurator.New(SlayerFeatureName, Guids.ShadowDuplicateTalentSlayer).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var rogue = CharacterClassRefs.RogueClass.ToString();
      var slayer = CharacterClassRefs.SlayerClass.ToString();
      var icon = BuffRefs.MirrorImageBuff.Reference.Get().Icon;

      var buff = BuffConfigurator.New(BuffName, Guids.ShadowDuplicateBuff)
        .CopyFrom(BuffRefs.MirrorImageBuff)
        .SetDisplayName(DisplayName)
        .AddMirrorImage(count: ContextDice.Value(DiceType.One))
        .Configure();

      var resource = AbilityResourceConfigurator.New(ResourceName, Guids.ShadowDuplicateResource)
        .SetIcon(icon)
        .SetMaxAmount(
          ResourceAmountBuilder.New(1)
            .IncreaseByLevelStartPlusDivStep(classes: new[] { rogue }, levelsPerStep: 5, bonusPerStep: 1))
        .Configure();

      var hiddenBuff = BuffConfigurator.New(HiddenBuffName, Guids.ShadowDuplicateHiddenBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .AddMirrorImage(count: ContextDice.Value(DiceType.One))
        .AddComponent(new ResourceLogic(resource))
        .AddTargetAttackRollTrigger(
          actionOnSelf:
            ActionsBuilder.New()
              .RemoveSelf()
              .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank()), asChild: false))
        .AddContextCalculateAbilityParams(replaceCasterLevel: true, casterLevel: ContextValues.Rank())
        .AddContextRankConfig(ContextRankConfigs.ClassLevel(classes: new[] { rogue }))
        .Configure();

      var ability = ActivatableAbilityConfigurator.New(AbilityName, Guids.ShadowDuplicateAbility)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetBuff(hiddenBuff)
        .AddActivatableAbilityResourceLogic(requiredResource: resource, spendType: ResourceSpendType.Never) // Spend logic is custom
        .Configure();

      var feature = FeatureConfigurator.New(FeatureName, Guids.ShadowDuplicateTalent, FeatureGroup.RogueTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetIsClassFeature()
        .AddFacts(new() { ability })
        .AddAbilityResources(resource: resource, restoreAmount: true)
        .Configure(delayed: true);

      // Slayer needs a separate copy for the context rank config
      var slayerResource = AbilityResourceConfigurator.New(SlayerResourceName, Guids.ShadowDuplicateResourceSlayer)
        .SetIcon(icon)
        .SetMaxAmount(
          ResourceAmountBuilder.New(1)
            .IncreaseByLevelStartPlusDivStep(classes: new[] { slayer }, levelsPerStep: 5, bonusPerStep: 1))
        .Configure();

      var slayerBuff = BuffConfigurator.New(SlayerHiddenBuffName, Guids.ShadowDuplicateHiddenBuffSlayer)
        .CopyFrom(hiddenBuff, c => c is not ContextRankConfig && c is not ResourceLogic)
        .AddComponent(new ResourceLogic(slayerResource))
        .AddContextRankConfig(ContextRankConfigs.ClassLevel(classes: new[] { slayer }))
        .Configure();

      var slayerAbility = ActivatableAbilityConfigurator.New(SlayerAbilityName, Guids.ShadowDuplicateAbilitySlayer)
        .CopyFrom(ability)
        .SetBuff(slayerBuff)
        .AddActivatableAbilityResourceLogic(requiredResource: slayerResource, spendType: ResourceSpendType.Never) // Spend logic is custom
        .Configure();

      FeatureConfigurator.New(SlayerFeatureName, Guids.ShadowDuplicateTalentSlayer, FeatureGroup.SlayerTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(SlayerDescription)
        .SetIcon(icon)
        .SetIsClassFeature()
        .AddFacts(new() { slayerAbility })
        .AddAbilityResources(resource: slayerResource, restoreAmount: true)
        .Configure(delayed: true);
    }

    [TypeId("37b4ba63-b7b4-45f0-8856-564aabae312a")]
    private class ResourceLogic : EntityFactComponentDelegate, ITargetRulebookHandler<RuleAttackRoll>
    {
      private readonly BlueprintAbilityResourceReference Resource;

      public ResourceLogic(BlueprintAbilityResource resource)
      {
        Resource = resource.ToReference<BlueprintAbilityResourceReference>();
      }

      public void OnEventAboutToTrigger(RuleAttackRoll evt) { }

      public void OnEventDidTrigger(RuleAttackRoll evt)
      {
        try
        {
          if (evt.Result == AttackResult.MirrorImage || evt.IsHit)
          {
            Logger.Log($"Spending a resource.");
            evt.Target.Resources.Spend(Resource, 1);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("ResourceLogic.OnEventDidTrigger", e);
        }
      }
    }

    [HarmonyPatch(typeof(UnitPartMirrorImage))]
    static class UnitPartMirrorImage_Patch
    {
      private static BlueprintBuff _rogueShadow;
      private static BlueprintBuff RogueShadow
      {
        get
        {
          _rogueShadow ??= BlueprintTool.Get<BlueprintBuff>(Guids.ShadowDuplicateHiddenBuff);
          return _rogueShadow;
        }
      }

      // The implementation relies on having a single mirror image secretly "on" at all times. This makes sure that the
      // always on image is not removed when attack misses by 5 or less.
      [HarmonyPatch(nameof(UnitPartMirrorImage.TryAbsorbHit)), HarmonyPrefix]
      static bool TryAbsorbHit(UnitPartMirrorImage __instance, ref int __result, bool force)
      {
        var sourceBlueprint = __instance.Source?.Blueprint;
        if (force && sourceBlueprint == RogueShadow)
        {
          Logger.NativeLog($"Skipping forced image spend. Attack missed by 5 or less.");
          __result = 0;
          return false;
        }
        return true;
      }
    }
  }
}
