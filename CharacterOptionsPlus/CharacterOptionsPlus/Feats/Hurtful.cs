using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.NewEx;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using TabletopTweaks.Core.NewEvents;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  public class Hurtful
  {
    private const string FeatName = "Hurtful";

    internal const string FeatDisplayName = "Hurtful.Name";
    private const string FeatDescription = "Hurtful.Description";

    private const string AbilityName = "Hurtful.Ability";

    private const string BuffName = "Hurtful.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "hurtful.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      if (Settings.IsEnabled(Guids.HurtfulFeat))
        ConfigureEnabled();
      else
        ConfigureDisabled();
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.HurtfulBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .Configure();

      ActivatableAbilityConfigurator.New(AbilityName, Guids.HurtfulAbility)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .Configure();

      FeatureConfigurator.New(FeatName, Guids.HurtfulFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var buff =
        BuffConfigurator.New(BuffName, Guids.HurtfulBuff)
          // No need to clutter the UI, the ability itself is sufficient to indicate it is active.
          .SetFlags(BlueprintBuff.Flags.HiddenInUi)
          .AddComponent(
            new HurtfulComponent(ConditionsBuilder.New().TargetInMeleeRange().HasActionsAvailable(requireSwift: true)))
          .Configure();

      // Toggle ability to enable / disable hurtful trigger
      var ability =
        ActivatableAbilityConfigurator.New(AbilityName, Guids.HurtfulAbility)
          .SetDisplayName(FeatDisplayName)
          .SetDescription(FeatDescription)
          .SetIcon(IconName)
          .SetBuff(buff)
          .Configure();

      FeatureConfigurator.New(FeatName, Guids.HurtfulFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddFeatureTagsComponent(featureTags: FeatureTag.Melee | FeatureTag.Attack | FeatureTag.Skills)
        .AddPrerequisiteStatValue(StatType.Strength, 13)
        .AddPrerequisiteFeature(FeatureRefs.PowerAttackFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.IntimidatingProwess.ToString())
        .AddRecommendationHasFeature(FeatureRefs.CornugonSmash.ToString())
        .AddRecommendationStatMiminum(stat: StatType.SkillPersuasion, minimalValue: 1, goodIfHigher: true)
        .AddFacts(new() { ability })
        .Configure(delayed: true);
    }

    [TypeId("fe3bdf24-c75d-4a6f-b8df-2df18f96ecbd")]
    private class HurtfulComponent : UnitFactComponentDelegate, IInitiatorDemoralizeHandler
    {
      private readonly ConditionsChecker Conditions;

      public HurtfulComponent(ConditionsBuilder conditions)
      {
        Conditions = conditions.Build();
      }

      public void AfterIntimidateSuccess(Demoralize action, RuleSkillCheck intimidateCheck, Buff appliedBuff)
      {
        try
        {
          if (!Conditions.Check())
          {
            Logger.NativeLog($"Conditions not met");
            return;
          }

          var target = ContextData<MechanicsContext.Data>.Current?.CurrentTarget?.Unit;
          if (target is null)
          {
            Logger.Warning($"No target for demoralize.");
            return;
          }

          if (appliedBuff is null)
          {
            Logger.NativeLog($"{target.CharacterName} immune to demoralize");
            return;
          }

          Logger.NativeLog($"{target.CharacterName} demoralized");
          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning($"Caster is missing");
            return;
          }

          var threatHandMelee = caster.GetThreatHandMelee();
          if (threatHandMelee is null)
          {
            Logger.Warning($"Unable to make melee attack");
            return;
          }

          caster.SpendAction(UnitCommand.CommandType.Swift, isFullRound: False, timeSinceCommandStart: 0);
          var attack =
            Context.TriggerRule<RuleAttackWithWeapon>(
              new(caster, target, threatHandMelee.Weapon, attackBonusPenalty: 0));

          if (!attack.AttackRoll.IsHit)
          {
            Logger.NativeLog($"Attack missed, removing demoralize effects: {appliedBuff.Name}");
            target.RemoveFact(appliedBuff);
          }
        }
        catch(Exception e)
        {
          Logger.LogException("Failed to process hurtful.", e);
        }
      }
    }
  }
}