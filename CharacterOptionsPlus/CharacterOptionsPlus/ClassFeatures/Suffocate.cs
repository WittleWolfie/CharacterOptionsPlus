using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;
using BlueprintCore.Actions.Builder.ContextEx;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using CharacterOptionsPlus.Actions;
using Kingmaker.Blueprints.Classes;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class Suffocate
  {
    private const string FeatureName = "Suffocate.WildTalent";

    internal const string DisplayName = "Suffocate.WildTalent.Name";
    private const string Description = "Suffocate.WildTalent.Description";

    private const string AbilityName = "Suffocate.WildTalent.Ability";

    private const string BuffName = "Suffocate.WildTalent.Buff";

    // commonnecromancybuff00
    private const string BuffFx = "cbfe312cb8e63e240a859efaad8e467c";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.SuffocateTalent))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("Suffocate.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.SuffocateBuff).Configure();
      AbilityConfigurator.New(AbilityName, Guids.SuffocateAbility).Configure();
      FeatureConfigurator.New(FeatureName, Guids.SuffocateTalent).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.SuffocateBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetFxOnStart(BuffFx)
        .AddFactContextActions(
          activated: ActionsBuilder.New(),
          newRound:
            ActionsBuilder.New()
              .SavingThrow(
                SavingThrowType.Fortitude,
                onResult:
                  ActionsBuilder.New()
                    .ConditionalSaved(
                      succeed: ActionsBuilder.New().RemoveSelf(),
                      failed: ActionsBuilder.New().Add<SetHitPoints>(a => a.Value = -1).RemoveSelf())))
        .Configure();

      var icon = AbilityRefs.PredictionOfFailure.Reference.Get().Icon;
      var ability = AbilityConfigurator.New(AbilityName, Guids.SuffocateAbility)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetType(AbilityType.SpellLike)
        .SetRange(AbilityRange.Long)
        .AllowTargeting(enemies: true)
        .SetAnimation(CastAnimationStyle.Directional)
        .SetActionType(CommandType.Standard)
        .SetSpellResistance()
        .SetAvailableMetamagic(Metamagic.Quicken, Metamagic.Heighten, Metamagic.Reach)
        .AddSpellDescriptorComponent(SpellDescriptor.Death)
        .AddAbilityTargetHasFact(
          checkedFacts:
            new()
            {
              FeatureRefs.UndeadType.ToString(),
              FeatureRefs.ConstructType.ToString(),
              FeatureRefs.SubtypeElemental.ToString() 
            },
          inverted: true)
        .AddAbilityKineticist(wildTalentBurnCost: 1)
        .AddContextCalculateAbilityParamsBasedOnClass(
          characterClass: CharacterClassRefs.KineticistClass.ToString(),
          useKineticistMainStat: true)
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .ConditionalSaved(
              failed: ActionsBuilder.New().Add<SetHitPoints>(a => a.Value = 0).ApplyBuffPermanent(buff)),
          savingThrowType: SavingThrowType.Fortitude)
        .Configure();

      FeatureConfigurator.New(FeatureName, Guids.SuffocateTalent, FeatureGroup.KineticWildTalent)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new()
          {
            ProgressionRefs.ElementalFocusWater.ToString(),
            ProgressionRefs.SecondaryElementWater.ToString(),
            ProgressionRefs.ThirdElementWater.ToString(),
            ProgressionRefs.KineticKnightElementalFocusWater.ToString(),
            ProgressionRefs.ElementalFocusAir.ToString(),
            ProgressionRefs.SecondaryElementAir.ToString(),
            ProgressionRefs.ThirdElementAir.ToString(),
            ProgressionRefs.KineticKnightElementalFocusAir.ToString(),
          })
        .AddPrerequisiteClassLevel(CharacterClassRefs.KineticistClass.ToString(), level: 12)
        .AddFacts(new() { ability })
        .Configure(delayed: true);
    }
  }
}
