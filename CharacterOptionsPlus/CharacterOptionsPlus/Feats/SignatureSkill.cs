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
using HarmonyLib;
using Kingmaker.Controllers.Rest.State;
using Kingmaker.Blueprints.Facts;
using BlueprintCore.Utils;
using Kingmaker.PubSubSystem;
using Kingmaker.EntitySystem;
using Kingmaker.Inspect;
using Kingmaker.EntitySystem.Entities;
using System.Collections.Generic;
using System.Reflection;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using Kingmaker.Enums;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using Kingmaker.Designers;
using System.Linq;
using Kingmaker.Controllers;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using System.Text;
using Kingmaker.Blueprints.Root;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using static Kingmaker.Blueprints.Classes.Prerequisites.Prerequisite;
using Kingmaker.UnitLogic.FactLogic;

namespace CharacterOptionsPlus.Feats
{
  // TODO: Acrobatics, Escape Artist, Stealth
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
      FeatureConfigurator.New(DemoralizeName, Guids.SignatureSkillDemoralize).Configure();
      FeatureConfigurator.New(PerceptionName, Guids.SignatureSkillPerception).Configure();

      BuffConfigurator.New(KnowledgeBuff, Guids.SignatureSkillKnowledgeBuff).Configure();
      AbilityConfigurator.New(KnowledgeAbility, Guids.SignatureSkillKnowledgeAbility).Configure();

      FeatureConfigurator.New(KnowledgeArcanaName, Guids.SignatureSkillKnowledgeArcana).Configure();
      FeatureConfigurator.New(KnowledgeWorldName, Guids.SignatureSkillKnowledgeArcana).Configure();
      FeatureConfigurator.New(KnowledgeNatureName, Guids.SignatureSkillKnowledgeArcana).Configure();
      FeatureConfigurator.New(KnowledgeReligionName, Guids.SignatureSkillKnowledgeArcana).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      ConfigureKnowledgeCommon();

      var feat = FeatureSelectionConfigurator.New(FeatName, Guids.SignatureSkillFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddFeatureTagsComponent(featureTags: FeatureTag.Skills)
        .AddComponent<RecommendationSignatureSkill>()
        .AddToAllFeatures(
          ConfigureDemoralize(),
          ConfigureKnowledgeArcana(),
          ConfigureKnowledgeWorld(),
          ConfigureKnowledgeNature(),
          ConfigureKnowledgeReligion(),
          ConfigurePerception())
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
          if (unit.Stats.GetStat(Skill.Value).BaseValue >= unit.Progression.CharacterLevel)
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
          if (levelUpState.Unit.Stats.GetStat(stat).BaseValue >= unit.Progression.CharacterLevel)
            return RecommendationPriority.Good;
        }
        return RecommendationPriority.Same;
      }
    }

    #region Acrobatics
    private const string AcrobaticsName = "SignatureSkill.Acrobatics";
    private const string AcrobaticsDisplayName = "SignatureSkill.Acrobatics.Name";
    private const string AcrobaticsDescription = "SignatureSkill.Acrobatics.Description";

    private const string AcrobaticsAbility = "SignatureSkill.Acrobatics.Ability";
    private const string AcrobaticsAbilityBuff = "SignatureSkill.Acrobatics.Ability.Buff";
    private const string AcrobaticsAbilityDescription = "SignatureSkill.Acrobatics.Ability.Description";

    private static BlueprintFeature ConfigureAcrobatics()
    {
      var buff = BuffConfigurator.New(AcrobaticsAbilityBuff, Guids.SignatureSkillAcrobaticsAbilityBuff)
        .SetDisplayName(AcrobaticsDisplayName)
        .SetDescription(AcrobaticsAbilityDescription)
        .AddComponent<AcrobaticMovement>()
        // None is used for Mobility checks to avoid AOO
        .AddCMDBonusAgainstManeuvers(
          value: -5, maneuvers: new[] { CombatManeuver.None }, descriptor: ModifierDescriptor.UntypedStackable)
        .Configure();

      var ability = ActivatableAbilityConfigurator.New(AcrobaticsAbility, Guids.SignatureSkillAcrobaticsAbility)
        .SetDisplayName(AcrobaticsDisplayName)
        .SetDescription(AcrobaticsAbilityDescription)
        .SetDeactivateIfCombatEnded()
        .SetDeactivateImmediately()
        .SetActivationType(AbilityActivationType.WithUnitCommand)
        .SetBuff(buff)
        .Configure();

      return FeatureConfigurator.New(AcrobaticsName, Guids.SignatureSkillAcrobatics)
        .SetDisplayName(AcrobaticsDisplayName)
        .SetDescription(AcrobaticsDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillMobility, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddFacts(new() { ability })
        .Configure();
    }

    [HarmonyPatch(typeof(UnitEntityData))]
    static class UnitEntityData_Patch
    {
      private static BlueprintBuff _acrobatics;
      private static BlueprintBuff Acrobatics
      {
        get
        {
          _acrobatics ??= BlueprintTool.Get<BlueprintBuff>(Guids.SignatureSkillAcrobaticsAbilityBuff);
          return _acrobatics;
        }
      }

      [HarmonyPatch(nameof(UnitEntityData.CalculateSpeedModifier)), HarmonyPostfix]
      static void CalculateSpeedModifier(UnitEntityData __instance, ref float __result)
      {
        try
        {
          if (!__instance.Descriptor.State.HasCondition(UnitCondition.UseMobilityToNegateAttackOfOpportunity))
            return;

          if (!__instance.HasFact(Acrobatics) || __instance.Descriptor.State.Features.TricksterMobilityFastMovement)
            return;

          __result *= 2f;
        }
        catch (Exception e)
        {
          Logger.LogException("UnitEntityData_Patch.CalculateSpeedModifier", e);
        }
      }
    }
    #endregion

    #region Demoralize
    private const string DemoralizeName = "SignatureSkill.Demoralize";
    private const string DemoralizeDisplayName = "SignatureSkill.Demoralize.Name";
    private const string DemoralizeDescription = "SignatureSkill.Demoralize.Description";

    private static BlueprintFeature ConfigureDemoralize()
    {
      return FeatureConfigurator.New(DemoralizeName, Guids.SignatureSkillDemoralize)
        .SetDisplayName(DemoralizeDisplayName)
        .SetDescription(DemoralizeDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillPersuasion, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillPersuasion))
        .AddComponent<SignatureDemoralizeComponent>()
        .Configure();
    }

    [TypeId("c01de10e-c307-450d-8c78-81bc2fdaacb3")]
    private class SignatureDemoralizeComponent : UnitFactComponentDelegate, IInitiatorDemoralizeHandler
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
          Logger.LogException("SignatureDemoralizeComponent.AfterIntimidateSuccess", e);
        }
      }
    }
    #endregion

    #region Knowledge / Lore

    #region Blueprint Links
    private static BlueprintFeature _arcana;
    private static BlueprintFeature Arcana
    {
      get
      {
        _arcana ??= BlueprintTool.Get<BlueprintFeature>(Guids.SignatureSkillKnowledgeArcana);
        return _arcana;
      }
    }

    private static BlueprintFeature _World;
    private static BlueprintFeature World
    {
      get
      {
        _World ??= BlueprintTool.Get<BlueprintFeature>(Guids.SignatureSkillKnowledgeWorld);
        return _World;
      }
    }

    private static BlueprintFeature _Nature;
    private static BlueprintFeature Nature
    {
      get
      {
        _Nature ??= BlueprintTool.Get<BlueprintFeature>(Guids.SignatureSkillLoreNature);
        return _Nature;
      }
    }

    private static BlueprintFeature _Religion;
    private static BlueprintFeature Religion
    {
      get
      {
        _Religion ??= BlueprintTool.Get<BlueprintFeature>(Guids.SignatureSkillLoreReligion);
        return _Religion;
      }
    }

    private static BlueprintBuff _buff;
    private static BlueprintBuff Buff
    {
      get
      {
        _buff ??= BlueprintTool.Get<BlueprintBuff>(Guids.SignatureSkillKnowledgeBuff);
        return _buff;
      }
    }

    private static BlueprintAbility _ability;
    private static BlueprintAbility Ability
    {
      get
      {
        _ability ??= BlueprintTool.Get<BlueprintAbility>(Guids.SignatureSkillKnowledgeAbility);
        return _ability;
      }
    }
    #endregion

    private const string KnowledgeDescription = "SignatureSkill.Knowledge.Description";

    private const string KnowledgeAbility = "SignatureSkill.Knowledge.Ability.Name";
    private const string KnowledgeAbilityDescription = "SignatureSkill.Knowledge.Ability.Description";

    private const string KnowledgeBuff = "SignatureSkill.Knowledge.Arcana.Buff";

    private static void ConfigureKnowledgeCommon()
    {
      BuffConfigurator.New(KnowledgeBuff, Guids.SignatureSkillKnowledgeBuff)
        .SetDisplayName(KnowledgeAbility)
        .SetDescription(KnowledgeAbilityDescription)
        .Configure();

      AbilityConfigurator.New(KnowledgeAbility, Guids.SignatureSkillKnowledgeAbility)
        .SetDisplayName(KnowledgeArcanaDisplayName)
        .SetDescription(KnowledgeAbilityDescription)
        .SetRange(AbilityRange.Long)
        .SetType(AbilityType.Extraordinary)
        .AllowTargeting(enemies: true)
        .SetActionType(CommandType.Move)
        .SetAnimation(CastAnimationStyle.Omni)
        .AddComponent<SignatureSkillAbilityRequirements>()
        .AddAbilityEffectRunAction(
          ActionsBuilder.New().MakeKnowledgeCheck(successActions: ActionsBuilder.New().Add<ApplySignatureSkillBuff>()))
        .Configure();
    }

    #region Arcana
    private const string KnowledgeArcanaName = "SignatureSkill.KnowledgeArcana";
    private const string KnowledgeArcanaDisplayName = "SignatureSkill.KnowledgeArcana.Name";

    private static BlueprintFeature ConfigureKnowledgeArcana()
    {
      return FeatureConfigurator.New(KnowledgeArcanaName, Guids.SignatureSkillKnowledgeArcana)
        .SetDisplayName(KnowledgeArcanaDisplayName)
        .SetDescription(KnowledgeDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillKnowledgeArcana, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillKnowledgeArcana))
        .AddComponent(new SignatureKnowledgeComponent(StatType.SkillKnowledgeArcana))
        .Configure();
    }
    #endregion

    #region World
    private const string KnowledgeWorldName = "SignatureSkill.KnowledgeWorld";
    private const string KnowledgeWorldDisplayName = "SignatureSkill.KnowledgeWorld.Name";

    private static BlueprintFeature ConfigureKnowledgeWorld()
    {
      return FeatureConfigurator.New(KnowledgeWorldName, Guids.SignatureSkillKnowledgeWorld)
        .SetDisplayName(KnowledgeWorldDisplayName)
        .SetDescription(KnowledgeDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillKnowledgeWorld, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillKnowledgeWorld))
        .AddComponent(new SignatureKnowledgeComponent(StatType.SkillKnowledgeWorld))
        .Configure();
    }
    #endregion

    #region Nature
    private const string KnowledgeNatureName = "SignatureSkill.LoreNature";
    private const string KnowledgeNatureDisplayName = "SignatureSkill.LoreNature.Name";

    private static BlueprintFeature ConfigureKnowledgeNature()
    {
      return FeatureConfigurator.New(KnowledgeNatureName, Guids.SignatureSkillLoreNature)
        .SetDisplayName(KnowledgeNatureDisplayName)
        .SetDescription(KnowledgeDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillLoreNature, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillLoreNature))
        .AddComponent(new SignatureKnowledgeComponent(StatType.SkillLoreNature))
        .Configure();
    }
    #endregion

    #region Religion
    private const string KnowledgeReligionName = "SignatureSkill.LoreReligion";
    private const string KnowledgeReligionDisplayName = "SignatureSkill.LoreReligion.Name";

    private static BlueprintFeature ConfigureKnowledgeReligion()
    {
      return FeatureConfigurator.New(KnowledgeReligionName, Guids.SignatureSkillLoreReligion)
        .SetDisplayName(KnowledgeReligionDisplayName)
        .SetDescription(KnowledgeDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillLoreReligion, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillLoreReligion))
        .AddComponent(new SignatureKnowledgeComponent(StatType.SkillLoreReligion))
        .Configure();
    }
    #endregion

    [TypeId("769750b3-8f3b-42c5-8cbd-f6aa9910ab62")]
    private class SignatureKnowledgeComponent :
      UnitFactComponentDelegate,
      IUnitIdentifiedHandler,
      IInitiatorRulebookHandler<RuleRollD20>,
      IInitiatorRulebookHandler<RuleSavingThrow>,
      IInitiatorRulebookHandler<RuleCalculateAttackBonus>,
      IInitiatorRulebookHandler<RuleSpellResistanceCheck>,
      IUnitLevelUpHandler
    {
      private readonly StatType Skill;

      public SignatureKnowledgeComponent(StatType skill)
      {
        Skill = skill;
      }

      #region Reroll
      public void OnEventAboutToTrigger(RuleRollD20 evt)
      {
        try
        {
          if (Rulebook.CurrentContext.PreviousEvent is not RuleSkillCheck skillCheck)
            return;

          if (skillCheck.StatType != Skill)
            return;

          var skillRanks = Owner.Stats.GetStat(Skill).BaseValue;
          if (skillRanks < 20)
            return;

          Logger.Log($"Adding reroll for {Owner.CharacterName}.");
          evt.AddReroll(amount: 1, takeBest: true, Fact);
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureInspectionComponent.OnEventAboutToTrigger(RuleRollD20)", e);
        }
      }

      #endregion

      #region Competence Bonus
      public void OnEventAboutToTrigger(RuleSavingThrow evt)
      {
        try
        {
          var buff = evt.Reason.Caster.GetFact(Buff);
          if (buff is null || buff.MaybeContext?.MaybeCaster != evt.Initiator)
            return;

          var bonus = GetBonus(evt.Initiator);
          Logger.NativeLog($"Adding +{bonus} to saving throw for {evt.Initiator.CharacterName}");
          evt.AddModifier(bonus, Fact, ModifierDescriptor.Competence);
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureInspectionComponent.OnEventAboutToTrigger(RuleSavingThrow)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
      {
        try
        {
          if (evt.Target is null)
          {
            Logger.Warning("No target available for attack.");
            return;
          }

          var buff = evt.Target.GetFact(Buff);
          if (buff is null || buff.MaybeContext?.MaybeCaster != evt.Initiator)
            return;

          var bonus = GetBonus(evt.Initiator);
          Logger.NativeLog($"Adding +{bonus} to attack against {evt.Target.CharacterName}");
          evt.AddModifier(bonus, Fact, ModifierDescriptor.Competence);
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureKnowledgeBuffComponent.OnEventAboutToTrigger(RuleCalculateAttackBonus)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleSpellResistanceCheck evt)
      {
        try
        {
          if (evt.Target is null)
          {
            Logger.Warning("No target available for attack.");
            return;
          }

          var buff = evt.Target.GetFact(Buff);
          if (buff is null || buff.MaybeContext?.MaybeCaster != evt.Initiator)
            return;

          var bonus = GetBonus(evt.Initiator);
          Logger.NativeLog($"Adding +{bonus} to spell resistance check against {evt.Target.CharacterName}");
          evt.AddSpellPenetration(bonus, ModifierDescriptor.Competence);
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureKnowledgeBuffComponent.OnEventAboutToTrigger(RuleSpellResistanceCheck)", e);
        }
      }

      private int GetBonus(UnitEntityData unit)
      {
        return Math.Max(3, (unit.Stats.GetStat(Skill).BaseValue - 5) / 5);
      }
      #endregion

      public void OnUnitIdentified(RuleSkillCheck skillCheck, ref int checkBonus)
      {
        try
        {
          if (skillCheck.StatType != Skill)
            return;

          checkBonus += Owner.Stats.GetStat(Skill).BaseValue;
          Logger.NativeLog($"Adding +{checkBonus} to identify success for {Owner.CharacterName}");
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureInspectionComponent.OnUnitIdentified", e);
        }
      }

      // Grants the ability at rank 10
      public void HandleUnitAfterLevelUp(UnitEntityData unit, LevelUpController controller)
      {
        try
        {
          if (unit != Owner)
            return;

          if (unit.HasFact(Ability))
            return;

          if (unit.Stats.GetStat(Skill).BaseValue >= 10)
          {
            Logger.NativeLog($"Granting {Ability.Name} to {Owner.CharacterName}");
            unit.AddFact(Ability);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureInspectionComponent.HandleUnitAfterLevelUp", e);
        }
      }

      #region Unused
      public void OnEventDidTrigger(RuleCalculateAttackBonus evt) { }

      public void OnEventDidTrigger(RuleSpellResistanceCheck evt) { }

      public void OnEventDidTrigger(RuleSavingThrow evt) { }

      public void OnEventDidTrigger(RuleRollD20 evt) { }

      public void HandleUnitBeforeLevelUp(UnitEntityData unit) { }
      #endregion
    }

    [TypeId("3addba51-c42a-476c-9847-d547b29757cc")]
    private class ApplySignatureSkillBuff : ContextAction
    {
      public override string GetCaption()
      {
        return "Apply Signature Skill buff";
      }

      public override void RunAction()
      {
        try
        {
          List<UnitEntityData> targets = new() { Context.MainTarget.Unit };

          var targetType = Context.MainTarget.Unit.Blueprint.Type;
          targets.AddRange(
            GameHelper.GetTargetsAround(Context.MaybeCaster.Position, 120.Feet())
              .Where(unit => unit.IsEnemy(Context.MaybeCaster) && unit.Blueprint.Type == targetType));

          foreach (var target in targets)
          {
            var buff = target.AddBuff(Buff, Context, duration: 1.Minutes());
            buff.IsNotDispelable = true;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("ApplySignatureSkillBuff.RunAction", e);
        }
      }
    }

    [TypeId("77039b8b-eb62-407d-b959-c301905d3882")]
    private class SignatureSkillAbilityRequirements : BlueprintComponent, IAbilityTargetRestriction
    {
      private const string MissingFeat = "SignatureSkill.Knowledge.Ability.TargetRestriction.Feat";
      private const string MissingRanks = "SignatureSkill.Knowledge.Ability.TargetRestriction.Ranks";

      public string GetAbilityTargetRestrictionUIText(UnitEntityData caster, TargetWrapper target)
      {
        if (target.Unit is null)
          return string.Empty;

        var requiredFeat = GetRequiredFeat(target.Unit);
        if (caster.HasFact(requiredFeat))
        {
          return string.Format(
            LocalizationTool.GetString(MissingRanks), LocalizedTexts.Instance.Stats.GetText(GetStatType(target.Unit)));
        }
        return string.Format(LocalizationTool.GetString(MissingFeat).ToString(), requiredFeat.Name);
      }

      public bool IsTargetRestrictionPassed(UnitEntityData caster, TargetWrapper target)
      {
        try
        {
          if (target.Unit is null)
            return false;

          if (caster.Stats.GetStat(GetStatType(target.Unit)).BaseValue < 10)
            return False;

          return caster.HasFact(GetRequiredFeat(target.Unit));
        }
        catch (Exception e)
        {
          Logger.LogException("SignatureSkillAbilityRequirements.IsTargetRestrictionPassed", e);
        }
        return false;
      }

      private static StatType GetStatType(UnitEntityData target)
      {
        return target.Blueprint.Type ? target.Blueprint.Type.KnowledgeStat : StatType.SkillLoreNature;
      }

      private static BlueprintFeature GetRequiredFeat(UnitEntityData target)
      {
        return
          GetStatType(target) switch
          {
            StatType.SkillLoreNature => Nature,
            StatType.SkillLoreReligion => Religion,
            StatType.SkillKnowledgeWorld => World,
            StatType.SkillKnowledgeArcana => Arcana,
            _ => throw new NotImplementedException()
          };
      }
    }

    public interface IUnitIdentifiedHandler : IUnitSubscriber
    {
      void OnUnitIdentified(RuleSkillCheck skillCheck, ref int checkBonus);
    }

    [HarmonyPatch(typeof(InspectUnitsManager))]
    static class InspectUnitsManager_Patch
    {
      private static int OnUnitIdentified(RuleSkillCheck skillCheck, UnitEntityData identifier)
      {
        int bonus = 0;
        EventBus.RaiseEvent<IUnitIdentifiedHandler>(identifier, h => h.OnUnitIdentified(skillCheck, ref bonus));
        return skillCheck.RollResult + bonus;
      }

      static readonly MethodInfo UnitInfo_SetCheck =
        AccessTools.Method(typeof(InspectUnitsManager.UnitInfo), nameof(InspectUnitsManager.UnitInfo.SetCheck));
      static readonly MethodInfo UnitEntityData_Descriptor =
        AccessTools.PropertyGetter(typeof(UnitEntityData), nameof(UnitEntityData.Descriptor));

      [HarmonyPatch(nameof(InspectUnitsManager.TryMakeKnowledgeCheck), new Type[] { typeof(UnitEntityData) }), HarmonyTranspiler]
      static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
      {
        // Patch should be doing:
        //   ldloc.s ruleSkillCheck   [Existing]
        //   ldloc.s unitEntityData   [New]
        //   call OnUnitIdentified    [New]
        //   callvirt SetCheck(int32) [Existing]
        // Note that this removes the call to GetSuccess which normally follows load for ruleSkillCheck
        try
        {
          var code = new List<CodeInstruction>(instructions);

          // Search backwards for the SetCheck() instruction which is the insertion point.
          var index = code.Count - 1;
          var insertionIndex = 0;
          for (; index >= 0; index--)
          {
            if (code[index].Calls(UnitInfo_SetCheck))
            {
              insertionIndex = index;
              break;
            }
          }
          if (insertionIndex == 0)
          {
            throw new InvalidOperationException("Missing inspect units manager transpiler insertion index.");
          }

          // Keep searching backwards to find the load statement for unitEntityData
          CodeInstruction loadInitiator = null;
          index--;
          for (; index >= 0; index--)
          {
            if (code[index].Calls(UnitEntityData_Descriptor))
            {
              // Statement before Descriptor must load the skill check
              loadInitiator = code[index - 1].Clone();
              break;
            }
          }
          if (loadInitiator is null)
          {
            throw new InvalidOperationException("Missing unit entity data load instruction.");
          }

          var newCode =
            new List<CodeInstruction>()
            {
              loadInitiator,
              CodeInstruction.Call(typeof(InspectUnitsManager_Patch), nameof(InspectUnitsManager_Patch.OnUnitIdentified)),
            };
          code.InsertRange(insertionIndex, newCode);
          code.RemoveAt(insertionIndex - 1); // Remove the call to ruleSkillCheck.Success
          return code;
        }
        catch (Exception e)
        {
          Logger.LogException("InspectUnitsManager_Patch.Transpiler", e);
          return instructions;
        }
      }
    }
    #endregion

    #region Perception
    private const string PerceptionName = "SignatureSkill.Perception";
    private const string PerceptionDisplayName = "SignatureSkill.Perception.Name";
    private const string PerceptionDescription = "SignatureSkill.Perception.Description";

    private static BlueprintFeature ConfigurePerception()
    {
      return FeatureConfigurator.New(PerceptionName, Guids.SignatureSkillPerception)
        .SetDisplayName(PerceptionDisplayName)
        .SetDescription(PerceptionDescription)
        .SetIsClassFeature()
        .AddPrerequisiteStatValue(StatType.SkillPerception, value: 5, group: GroupType.Any)
        .AddPrerequisiteClassLevel(CharacterClassRefs.RogueClass.ToString(), level: 5, group: GroupType.Any)
        .AddComponent(new RecommendationSignatureSkill(StatType.SkillPerception))
        .AddComponent<SignaturePerceptionComponent>()
        .Configure();
    }

    [TypeId("e48b6792-f51c-465a-87d8-5903ac170bfb")]
    private class SignaturePerceptionComponent :
      UnitFactComponentDelegate,
      IInitiatorRulebookHandler<RuleSkillCheck>,
      IInitiatorRulebookHandler<RuleCachedPerceptionCheck>
    {
      public void OnEventAboutToTrigger(RuleSkillCheck evt)
      {
        try
        {
          if (evt.StatType != StatType.SkillPerception)
            return;

          if (evt.Reason.SourceEntity is null || evt.Reason.SourceEntity is not StaticEntityData)
            return;

          var perceptionRanks = Owner.Stats.GetStat(StatType.SkillPerception).BaseValue;
          if (perceptionRanks < 10)
            return;

          var bonus = perceptionRanks >= 20 ? 10 : 5;
          Logger.NativeLog($"Adding (+{bonus}) to {Owner.CharacterName} (hidden object)");
          evt.AddModifier(bonus, Fact);
        }
        catch (Exception e)
        {
          Logger.LogException("SignaturePerceptionComponent.OnEventAboutToTrigger(RuleSkillCheck)", e);
        }
      }

      // RuleCachedPerceptionCheck is only used for hidden units
      public void OnEventAboutToTrigger(RuleCachedPerceptionCheck evt)
      {
        try
        {
          var perceptionRanks = Owner.Stats.GetStat(StatType.SkillPerception).BaseValue;
          if (perceptionRanks < 10)
            return;

          var bonus = perceptionRanks >= 20 ? 10 : 5;
          Logger.NativeLog($"Adding (+{bonus}) to {Owner.CharacterName} (hidden unit)");
          evt.AddModifier(bonus, Fact);
        }
        catch (Exception e)
        {
          Logger.LogException("SignaturePerceptionComponent.OnEventAboutToTrigger(RuleCachedPerceptionCheck)", e);
        }
      }

      public void OnEventDidTrigger(RuleSkillCheck evt) { }

      public void OnEventDidTrigger(RuleCachedPerceptionCheck evt) { }
    }

    [HarmonyPatch(typeof(CampingRole))]
    static class CampingRole_Patch
    {
      private static BlueprintUnitFact _signaturePerception;
      private static BlueprintUnitFact SignaturePerception
      {
        get
        {
          _signaturePerception ??= BlueprintTool.Get<BlueprintUnitFact>(Guids.SignatureSkillPerception);
          return _signaturePerception;
        }
      }

      [HarmonyPatch(nameof(CampingRole.CreateRuleCheck)), HarmonyPostfix]
      static void CreateRuleCheck(CampingRole __instance, RuleSkillCheck __result)
      {
        try
        {
          if (__result == null)
            return;

          if (__instance.m_RoleType != CampingRoleType.GuardFirstWatch && __instance.m_RoleType != CampingRoleType.GuardSecondWatch)
            return;

          var signatureSkill = __result.Initiator.GetFact(SignaturePerception);
          if (signatureSkill is not null)
          {
            var bonus = __result.Initiator.Stats.SkillPerception.BaseValue >= 15 ? 4 : 2;
            Logger.NativeLog($"Adding (+{bonus}) to {__result.Initiator.CharacterName} (guard duty)");
            __result.AddModifier(bonus, signatureSkill);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("CampingRole_Patch.CreateRuleCheck", e);
        }
      }
    }
    #endregion
  }
}
