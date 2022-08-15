using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
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
      Logger.Info($"Configuring {FeatureName}");
      var skaldClass = CharacterClassRefs.SkaldClass.ToString();

      // Buff to apply fast healing
      BuffConfigurator.New(BuffName, BuffGuid)
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
         // Since Performance isn't a skill in Wrath and there's not a great equivalent just make level 10 the pre-req.
         .AddPrerequisiteCharacterLevel(10)
         .Configure();

      var applyBuff = ActionsBuilder.New().ApplyBuffPermanent(BuffName, isNotDispelable: true);
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
                ifTrue: applyBuff))
        // Prevents Inspired Rage from being removed and reapplied each round.
        .SetStacking(StackingType.Ignore)
        .Configure();
    }

    private class InspiredRageDeactivationHandler : UnitFactComponentDelegate, IActivatableAbilityWillStopHandler
    {
      private readonly BlueprintActivatableAbility InspiredRage =
        ActivatableAbilityRefs.InspiredRageAbility.Reference.Get();
      private readonly BlueprintBuff InspiredRageBuff = BuffRefs.InspiredRageEffectBuff.Reference.Get();

      private readonly BlueprintBuff SkaldsVigor = BlueprintTool.Get<BlueprintBuff>(BuffName);

      public void HandleActivatableAbilityWillStop(ActivatableAbility ability)
      {
        try
        {
          if (ability?.Blueprint != InspiredRage)
          {
            return;
          }
          Logger.Info("Inspired Rage deactivated.");

          var inspiredRageBuff = ability.Owner.Buffs.GetBuff(InspiredRageBuff);
          if (inspiredRageBuff is null)
          {
            Logger.Warn($"Inspired Rage buff missing from {ability.Owner.CharacterName}");
            return;
          }

          var areaEntity = EntityService.Instance.GetEntity<AreaEffectEntityData>(inspiredRageBuff.SourceAreaEffectId);
          foreach (var unit in areaEntity.InGameUnitsInside)
          {
            var skaldsVigor = unit.GetFact<Buff>(SkaldsVigor);
            if (skaldsVigor?.Context?.MaybeCaster == ability.Owner.Unit)
            {
              Logger.Verbose($"Removing Skald's Vigor from {skaldsVigor.Context.MaybeOwner?.CharacterName}");
              skaldsVigor.Remove();
            }
          }
        }
        catch (Exception e)
        {
          Logger.Error("Error processing Raging Song deactivation.", e);
        }
      }
    }
  }
}
