using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class ShimmeringMirage
  {
    private const string FeatureName = "ShimmeringMirage";

    internal const string DisplayName = "ShimmeringMirage.Name";
    private const string Description = "ShimmeringMirage.Description";

    private const string AbilityName = "ShimmeringMirage.Ability";

    private const string BuffName = "ShimmeringMirage.Buff";
    private const string EffectBuffName = "ShimmeringMirage.Effect.Buff";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ShimmeringMirageTalent))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ShimmeringMirage.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.ShimmeringMirageBuff).Configure();
      AbilityConfigurator.New(AbilityName, Guids.ShimmeringMirageAbility).Configure();
      FeatureConfigurator.New(FeatureName, Guids.ShimmeringMirageTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var shroudOfWater =
        new List<Blueprint<BlueprintUnitFactReference>>()
        {
          BuffRefs.ShroudOfWaterArmorBuff.ToString(),
          BuffRefs.ShroudOfWaterShieldBuff.ToString()
        };
      var effectBuff = BuffConfigurator.New(EffectBuffName, Guids.ShimmeringMirageEffectBuff)
        .CopyFrom(BuffRefs.BlurBuff, typeof(AddConcealment))
        .AddToFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Ignore)
        .Configure();

      var applyEffect = ActionsBuilder.New().ApplyBuffPermanent(effectBuff).Build();
      var removeEffect = ActionsBuilder.New().RemoveBuff(effectBuff).Build();
      var buff = BuffConfigurator.New(BuffName, Guids.ShimmeringMirageBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .AddFactContextActions(
          activated: applyEffect,
          newRound: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New()
                .HasFact(BuffRefs.ShroudOfWaterArmorBuff.ToString(), negate: true)
                .HasFact(BuffRefs.ShroudOfWaterShieldBuff.ToString(), negate: true),
              ifTrue: removeEffect),
          deactivated: removeEffect)
        .AddFactsChangeTrigger(checkedFacts: shroudOfWater, onFactGainedActions: applyEffect)
        .AddRestTrigger(action: ActionsBuilder.New().RemoveSelf())
        .Configure();

      var icon = BuffRefs.BlurBuff.Reference.Get().Icon;
      var ability = AbilityConfigurator.New(AbilityName, Guids.ShimmeringMirageAbility)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetType(AbilityType.SpellLike)
        .SetRange(AbilityRange.Personal)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(Metamagic.Quicken)
        .AddAbilityKineticist(wildTalentBurnCost: 1)
        .AddAbilityEffectRunAction(ActionsBuilder.New().ApplyBuffPermanent(buff))
        .AddAbilityCasterHasFacts(facts: shroudOfWater)
        .AddAbilityCasterHasNoFacts(facts: new() { buff })
        .Configure();

      FeatureConfigurator.New(FeatureName, Guids.ShimmeringMirageTalent, FeatureGroup.KineticWildTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(FeatureRefs.ShroudOfWater.ToString())
        .AddPrerequisiteClassLevel(CharacterClassRefs.KineticistClass.ToString(), level: 10)
        .AddFacts(new() { ability })
        .Configure(delayed: true);
    }
  }
}
