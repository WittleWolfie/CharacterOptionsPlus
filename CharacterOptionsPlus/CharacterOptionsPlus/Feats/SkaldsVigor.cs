using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
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

    internal const string BuffName = "SkaldsVigor.Buff";
    internal const string BuffGuid = "9e67121d-0433-4706-a107-7796187df3e1";

    private static readonly LogWrapper Logger = LogWrapper.Get(FeatureName);

    internal static void Configure()
    {
      Logger.Verbose($"Configuring {FeatureName}");

      // Buff to apply fast healing
      BuffConfigurator.New(BuffName, BuffGuid)
        .SetDisplayName(FeatureDisplayName)
        .SetDescription(FeatureDescription)
        .AddEffectFastHealing(heal: 0, bonus: ContextValues.Rank())
        .AddContextRankConfig(ContextRankConfigs.StatBonus(StatType.Strength))
        .Configure();

      // Skald's Vigor feat
      FeatureConfigurator.New(FeatName, FeatGuid, FeatureGroup.Feat, FeatureGroup.CombatFeat)
        .SetDisplayName(FeatureDisplayName)
        .SetDescription(FeatureDescription)
        .AddPrerequisiteFeature(FeatureRefs.RagingSong.ToString())
        .AddFactsChangeTrigger(
          checkedFacts: new() { BuffRefs.InspiredRageBuff.ToString() },
          onFactGainedActions:
            ActionsBuilder.New().ApplyBuffPermanent(BuffName),
          onFactLostActions:
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
