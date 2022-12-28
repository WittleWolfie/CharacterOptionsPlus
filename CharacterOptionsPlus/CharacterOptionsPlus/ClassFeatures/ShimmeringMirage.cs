using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;
using Kingmaker.UnitLogic.FactLogic;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using BlueprintCore.Actions.Builder.ContextEx;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Buffs.Blueprints;

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

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

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

      var effectBuff = BuffConfigurator.New(EffectBuffName, Guids.ShimmeringMirageEffectBuff)
        .CopyFrom(BuffRefs.BlurBuff, typeof(AddConcealment))
        .AddToFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetStacking(StackingType.Ignore)
        .Configure();

      var buff = BuffConfigurator.New(BuffName, Guids.ShimmeringMirageBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .AddFactsChangeTrigger(
          checkedFacts:
            new() { BuffRefs.ShroudOfWaterArmorBuff.ToString(), BuffRefs.ShroudOfWaterShieldBuff.ToString() },
          onFactGainedActions: ActionsBuilder.New().ApplyBuffPermanent(effectBuff, asChild: true),
          onFactLostActions: ActionsBuilder.New().RemoveBuff(effectBuff))
        .Configure();

      var icon = AbilityRefs.PredictionOfFailure.Reference.Get().Icon;
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
        .AddAbilityCasterHasFacts(
          facts: new() { BuffRefs.ShroudOfWaterArmorBuff.ToString(), BuffRefs.ShroudOfWaterShieldBuff.ToString() })
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
