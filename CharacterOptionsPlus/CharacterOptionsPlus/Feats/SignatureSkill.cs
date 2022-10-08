using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic;
using System;
using TabletopTweaks.Core.NewEvents;
using static UnityModManagerNet.UnityModManager.ModEntry;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.RuleSystem;
using Kingmaker.Blueprints;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.UnitLogic.Class.LevelUp;

namespace CharacterOptionsPlus.Feats
{
  // TODO: Perception, Knowledge, Acrobatics, Escape Artist, Stealth
  internal class SignatureSkill
  {
    internal const string FeatName = "SignatureSkill";

    internal const string FeatDisplayName = "SignatureSkill.Name";
    private const string FeatDescription = "SignatureSkill.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "furiousfocus.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.SignatureSkillFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("SignatureSkill.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      FeatureSelectionConfigurator.New(FeatName, Guids.SignatureSkillFeat).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var feat = FeatureSelectionConfigurator.New(FeatName, Guids.SignatureSkillFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddFeatureTagsComponent(featureTags: FeatureTag.Skills)
        .AddComponent<RecommendationSignatureSkill>()
        .AddToAllFeatures(ConfigureDemoralize())
        .Configure();

      // Add to feat selection
      FeatureConfigurator.For(FeatName).AddToGroups(FeatureGroup.Feat).Configure(delayed: true);

      // Grant as a bonus feature for rogues
      ProgressionConfigurator.For(ProgressionRefs.RogueProgression)
        .ModifyLevelEntries(
          entry =>
          {
            if (entry.Level == 5 || entry.Level == 10 || entry.Level == 15 || entry.Level == 20)
              entry.m_Features.Add(feat.ToReference<BlueprintFeatureBaseReference>());
          })
        .Configure();
    }

    private const string DemoralizeName = "SignatureSkill.Demoralize";
    private const string DemoralizeDisplayName = "SignatureSkill.Demoralize.Name";
    private const string DemoralizeDescription = "SignatureSkill.Demoralize.Description";

    private static BlueprintFeature ConfigureDemoralize()
    {
      return FeatureConfigurator.New(DemoralizeName, Guids.SignatureSkillDemoralize)
        .SetDisplayName(DemoralizeDisplayName)
        .SetDescription(DemoralizeDescription)
        .AddRecommendationStatMiminum(16, StatType.Charisma)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillPersuasion))
        .AddComponent<SignatureDemoralizeTrigger>()
        .Configure();
    }

    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintFeature))]
    [TypeId("d97a84e9-bde0-4bad-ab33-89a41841e26a")]
    private class RecommendationSignatureSkill : LevelUpRecommendationComponent
    {
      private readonly StatType? Skill;

      public RecommendationSignatureSkill() { Skill = null; }
      public RecommendationSignatureSkill(StatType skill) { Skill = skill; }

      // Recommend when the character has max ranks in a relevant skill
      public override RecommendationPriority GetPriority(LevelUpState levelUpState)
      {
        if (levelUpState is null)
          return RecommendationPriority.Same;

        var unit = levelUpState.Unit;
        if (Skill is not null)
        {
          if (unit.Stats.GetStat(Skill.Value).BaseValue == unit.Progression.CharacterLevel)
            return RecommendationPriority.Good;
          return RecommendationPriority.Same;
        }

        var skillTypes =
          new[]
          {
            StatType.SkillKnowledgeArcana,
            StatType.SkillKnowledgeWorld,
            StatType.SkillLoreNature,
            StatType.SkillLoreReligion,
            StatType.SkillMobility,
            StatType.SkillPerception,
            StatType.SkillPersuasion,
            StatType.SkillStealth,
          };
        foreach (var stat in skillTypes)
        {
          if (levelUpState.Unit.Stats.GetStat(stat).BaseValue == unit.Progression.CharacterLevel)
            return RecommendationPriority.Good;
        }
        return RecommendationPriority.Bad;
      }
    }

    [TypeId("c01de10e-c307-450d-8c78-81bc2fdaacb3")]
    private class SignatureDemoralizeTrigger : UnitFactComponentDelegate, IInitiatorDemoralizeHandler
    {
      private static BlueprintBuff _frightened;
      private static BlueprintBuff Frightened
      {
        get
        {
          _frightened ??= BuffRefs.Frightened.Reference.Get();
          return _frightened;
        }
      }

      private static BlueprintBuff _panicked;
      private static BlueprintBuff Panicked
      {
        get
        {
          // TODO: Replace w/ equivalent to CowerBuff
          _panicked ??= BuffRefs.EyebitePanickedBuff.Reference.Get();
          return _panicked;
        }
      }

      private static BlueprintBuff _cowering;
      private static BlueprintBuff Cowering
      {
        get
        {
          _cowering ??= BuffRefs.CowerBuff.Reference.Get();
          return _cowering;
        }
      }

      public void AfterIntimidateSuccess(Demoralize action, RuleSkillCheck intimidateCheck, Buff appliedBuff)
      {
        try
        {
          var target = ContextData<MechanicsContext.Data>.Current?.CurrentTarget?.Unit;
          if (target is null)
          {
            Logger.Warning($"No target for demoralize.");
            return;
          }

          if (appliedBuff is null)
          {
            Logger.NativeLog($"{target.CharacterName} is immune to demoralize");
            return;
          }

          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning($"Caster is missing");
            return;
          }

          var succeedBy = intimidateCheck.RollResult - intimidateCheck.DC;
          if (succeedBy < 10)
          {
            Logger.NativeLog($"Failed to exceed DC by 10: {succeedBy}");
            return;
          }

          var intimidateRanks = Owner.Stats.SkillPersuasion.BaseValue;
          var ruleSavingThrow = new RuleSavingThrow(target, SavingThrowType.Will, 10 + intimidateRanks);
          ruleSavingThrow.Reason = Context;

          var result = Context.TriggerRule(ruleSavingThrow);
          if (result.IsPassed)
            return;

          if (succeedBy >= 20 && intimidateRanks >= 20)
          {
            var cowerDuration =
              ContextValueHelper.CalculateDiceValue(DiceType.D4, diceCountValue: 1, bonusValue: 0, Context);
            target.AddBuff(Cowering, Context, duration: cowerDuration.Rounds().Seconds);
            // Link duration of Panicked to the demoralize buff
            appliedBuff.StoreFact(target.AddBuff(Panicked, Context));
          }
          else if (succeedBy >= 20 && intimidateRanks >= 15)
            target.AddBuff(Cowering, Context, duration: 1.Rounds().Seconds);
          else if (intimidateRanks >= 10)
            target.AddBuff(Panicked, Context, duration: 1.Rounds().Seconds);
          else
            target.AddBuff(Frightened, Context, duration: 1.Rounds().Seconds);
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to process Signature Skill: Demoralize.", e);
        }
      }
    }
  }
}
