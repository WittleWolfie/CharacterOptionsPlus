using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using System;

namespace CharacterOptionsPlus.Feats
{
  internal class SkaldsVigor
  {
    internal const string FeatureName = "SkaldsVigor";
    internal const string FeatureDisplayName = "SkaldsVigor.Name";
    internal const string FeatureDescription = "SkaldsVigor.Description";

    internal const string FeatName = "SkaldsVigor.Feat";
    internal const string FeatGuid = "55dd527b-8721-426b-aaa2-036ccc9a0458";

    internal const string GreaterFeatName = "GreaterSkaldsVigor.Feat";
    internal const string GreaterFeatGuid = "ee4756c6-797f-4848-a814-4a27a159641d";
    internal const string GreaterFeatDisplayName = "GreaterSkaldsVigor.Name";
    internal const string GreaterFeatDescription = "GreaterSkaldsVigor.Description";

    internal const string BuffName = "SkaldsVigor.Buff";
    internal const string BuffGuid = "9e67121d-0433-4706-a107-7796187df3e1";

    internal const string IconPrefix = "assets/icons/";
    internal const string IconName = IconPrefix + "skaldvigor.png";
    internal const string GreaterIconName = IconPrefix + "greaterskaldvigor.png";

    private static readonly LogWrapper Logger = LogWrapper.Get(FeatureName);

    internal static void Configure()
    {
      Logger.Verbose($"Configuring {FeatureName}");

      // Buff to apply fast healing
      BuffConfigurator.New(BuffName, BuffGuid)
        .SetDisplayName(FeatureDisplayName)
        .SetDescription(FeatureDescription)
        .SetIcon(IconName)
        .AddEffectContextFastHealing(bonus: ContextValues.Rank())
        .AddContextRankConfig(
          // Wrath uses the unchained version of Inspired Rage, this re-creates the progression of strength bonus:
          // +2 until level 8, then +4 until level 16, then +6.
          ContextRankConfigs.ClassLevel(new string[] { CharacterClassRefs.SkaldClass.ToString() })
            .WithCustomProgression((7, 2), (15, 4), (16, 6)))
        .Configure();

      // Skald's Vigor feat
      FeatureConfigurator.New(FeatName, FeatGuid, FeatureGroup.Feat, FeatureGroup.CombatFeat)
        .SetDisplayName(FeatureDisplayName)
        .SetDescription(FeatureDescription)
        .SetIcon(IconName)
        .AddPrerequisiteFeature(FeatureRefs.RagingSong.ToString())
        .Configure();
      
      // Greater Skald's Vigor
      FeatureConfigurator.New(GreaterFeatName, GreaterFeatGuid, FeatureGroup.Feat, FeatureGroup.CombatFeat)
         .SetDisplayName(GreaterFeatDisplayName)
         .SetDescription(GreaterFeatDescription)
         .SetIcon(GreaterIconName)
         .AddPrerequisiteFeature(FeatName)
         .Configure();

      var applyBuff =
        ActionsBuilder.New().ApplyBuffPermanent(BuffName, asChild: true, sameDuration: true);
      BuffConfigurator.For(BuffRefs.InspiredRageEffectBuff)
        .AddFactContextActions(
          activated:
            // Since it is actually part of the Inspired Rage buff it's not a valid dispel target.
            ActionsBuilder.New()
              .Conditional(
                ConditionsBuilder.New().TargetIsYourself().HasFact(FeatName),
                ifTrue: applyBuff)
              .Conditional(
                ConditionsBuilder.New().CasterHasFact(GreaterFeatName),
                ifTrue: applyBuff),
          deactivated:
            ActionsBuilder.New().RemoveBuff(BuffName))
        .Configure();

      EventBus.Subscribe(new InspiredRageDeactivationHandler());
    }

    private class InspiredRageDeactivationHandler : IActivatableAbilityWillStopHandler
    {
      public void HandleActivatableAbilityWillStop(ActivatableAbility ability)
      {
        try
        {
          if (ability?.Blueprint != ActivatableAbilityRefs.InspiredRageAbility.Reference.Get())
          {
            return;
          }
          Logger.Verbose("Inspired Rage deactivated.");

          Buff skaldsVigor = ability.Owner.Buffs.GetBuff(BlueprintTool.Get<BlueprintBuff>(BuffGuid));
          skaldsVigor?.Remove();
        }
        catch (Exception e)
        {
          Logger.Error("Error processing Raging Song deactivation.", e);
        }
      }
    }
  }
}
