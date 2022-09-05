using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  internal class SkaldsVigor
  {
    internal const string FeatureName = "SkaldsVigor";
    internal const string FeatureDisplayName = "SkaldsVigor.Name";
    internal const string FeatureDescription = "SkaldsVigor.Description";

    internal const string FeatName = "SkaldsVigor.Feat";

    internal const string GreaterFeatName = "GreaterSkaldsVigor.Feat";
    internal const string GreaterFeatDisplayName = "GreaterSkaldsVigor.Name";
    internal const string GreaterFeatDescription = "GreaterSkaldsVigor.Description";

    internal const string BuffName = "SkaldsVigor.Buff";

    internal const string IconPrefix = "assets/icons/";
    internal const string IconName = IconPrefix + "skaldvigor.png";
    internal const string GreaterIconName = IconPrefix + "greaterskaldvigor.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      Logger.Log($"Configuring {FeatureName}");
      var skaldClass = CharacterClassRefs.SkaldClass.ToString();

      // Buff to apply fast healing
      BuffConfigurator.New(BuffName, Guids.SkaldsVigorBuff)
        .SetDisplayName(FeatureDisplayName)
        .SetDescription(FeatureDescription)
        .SetIcon(IconName)
        .AddEffectContextFastHealing(bonus: ContextValues.Rank())
        .AddContextRankConfig(
          // Wrath uses the unchained version of Inspired Rage, this re-creates the progression of strength bonus:
          // +2 until level 8, then +4 until level 16, then +6.
          ContextRankConfigs.ClassLevel(new string[] { skaldClass }).WithCustomProgression((7, 2), (15, 4), (16, 6)))
        .AddComponent<InspiredRageDeactivationHandler>()
        .Configure();

      // Skald's Vigor feat
      var ragingSong = FeatureRefs.RagingSong.ToString();
      var skaldsVigor =
        FeatureConfigurator.New(FeatName, Guids.SkaldsVigorFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat)
          .SetDisplayName(FeatureDisplayName)
          .SetDescription(FeatureDescription)
          .SetIcon(IconName)
          .AddPrerequisiteFeature(ragingSong)
          .AddRecommendationHasFeature(ragingSong)
          .AddToIsPrerequisiteFor(Guids.SkaldsVigorGreaterFeat) // Reference by Guid since it doesn't exist yet.
          .AddFeatureTagsComponent(FeatureTag.Defense | FeatureTag.ClassSpecific)
          .Configure();
      
      // Greater Skald's Vigor
      var greaterSkaldsVigor =
        FeatureConfigurator.New(
          GreaterFeatName, Guids.SkaldsVigorGreaterFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat)
            .SetDisplayName(GreaterFeatDisplayName)
            .SetDescription(GreaterFeatDescription)
            .SetIcon(GreaterIconName)
            .AddPrerequisiteFeature(FeatName)
            // Since Performance isn't a skill in Wrath and there's not a great equivalent just make level 10 the pre-req.
            .AddPrerequisiteCharacterLevel(10)
            .AddRecommendationHasFeature(skaldsVigor)
            .AddFeatureTagsComponent(FeatureTag.Defense | FeatureTag.ClassSpecific)
            .Configure();

      var applyBuff = ActionsBuilder.New().ApplyBuffPermanent(BuffName, isNotDispelable: true);
      BuffConfigurator.For(BuffRefs.InspiredRageEffectBuff)
        .AddFactContextActions(
          activated:
            // Since it is actually part of the Inspired Rage buff it's not a valid dispel target.
            ActionsBuilder.New()
              .Conditional(
                ConditionsBuilder.New().TargetIsYourself().HasFact(skaldsVigor),
                ifTrue: applyBuff)
              .Conditional(
                ConditionsBuilder.New().CasterHasFact(greaterSkaldsVigor),
                ifTrue: applyBuff))
        // Prevents Inspired Rage from being removed and reapplied each round.
        .SetStacking(StackingType.Ignore)
        .Configure();
    }

    [TypeId("5ae05713-a303-4b4e-8bec-be5f6d17108a")]
    private class InspiredRageDeactivationHandler : UnitFactComponentDelegate, IActivatableAbilityWillStopHandler
    {
      private static readonly BlueprintActivatableAbility InspiredRage =
        ActivatableAbilityRefs.InspiredRageAbility.Reference.Get();
      private static readonly BlueprintBuff InspiredRageBuff = BuffRefs.InspiredRageEffectBuff.Reference.Get();

      private static readonly BlueprintBuff SkaldsVigor = BlueprintTool.Get<BlueprintBuff>(BuffName);

      public void HandleActivatableAbilityWillStop(ActivatableAbility ability)
      {
        try
        {
          if (ability?.Blueprint != InspiredRage)
          {
            return;
          }
          Logger.Log("Inspired Rage deactivated.");

          var inspiredRageBuff = ability.Owner.Buffs.GetBuff(InspiredRageBuff);
          if (inspiredRageBuff is null)
          {
            Logger.Warning($"Inspired Rage buff missing from {ability.Owner.CharacterName}");
            return;
          }

          var areaEntity = EntityService.Instance.GetEntity<AreaEffectEntityData>(inspiredRageBuff.SourceAreaEffectId);
          foreach (var unit in areaEntity.InGameUnitsInside)
          {
            var skaldsVigor = unit.GetFact<Buff>(SkaldsVigor);
            if (skaldsVigor?.Context?.MaybeCaster == ability.Owner.Unit)
            {
              Logger.NativeLog($"Removing Skald's Vigor from {skaldsVigor.Context.MaybeOwner?.CharacterName}");
              skaldsVigor.Remove();
            }
          }
        }
        catch (Exception e)
        {
          Logger.LogException("Error processing Raging Song deactivation.", e);
        }
      }
    }
  }
}
