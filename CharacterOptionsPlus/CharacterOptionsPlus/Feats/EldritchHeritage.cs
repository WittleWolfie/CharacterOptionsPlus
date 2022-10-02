using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.UnitLogic.Properties;
using BlueprintCore.Blueprints.CustomConfigurators;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.TargetCheckers;
using Kingmaker.UnitLogic.Mechanics.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  public class EldritchHeritage
  {
    internal const string FeatName = "EldritchHeritage";

    internal const string FeatDisplayName = "EldritchHeritage.Name";
    private const string FeatDescription = "EldritchHeritage.Description";

    private const string ImprovedFeatName = "EldritchHeritageImproved";
    private const string ImprovedFeatDisplayName = "EldritchHeritageImproved.Name";
    private const string ImprovedFeatDescription = "EldritchHeritageImproved.Description";

    private const string GreaterFeatName = "EldritchHeritageGreater";
    private const string GreaterFeatDisplayName = "EldritchHeritageGreater.Name";
    private const string GreaterFeatDescription = "EldritchHeritageGreater.Description";

    private const string EffectiveLevelProperty = "EldritchHeritage.EffectiveLevel";

    private const string IconPrefix = "assets/icons/";
    private const string Icon = IconPrefix + "gloriousheat.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.EldritchHeritageFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("EldritchHeritage.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      #region Abyssal
      FeatureConfigurator.New(AbyssalHeritageName, Guids.AbyssalHeritage).Configure();
      FeatureConfigurator.New(AbyssalHeritageResistance, Guids.AbyssalHeritageResistance).Configure();
      FeatureConfigurator.New(AbyssalHeritageStrength, Guids.AbyssalHeritageStrength).Configure();
      FeatureConfigurator.New(AbyssalHeritageSummons, Guids.AbyssalHeritageSummons).Configure();
      #endregion

      #region Arcane
      FeatureSelectionConfigurator.New(ArcaneHeritageName, Guids.ArcaneHeritage).Configure();
      FeatureSelectionConfigurator.New(ArcaneHeritageAdept, Guids.ArcaneHeritageAdept).Configure();
      FeatureSelectionConfigurator.New(ArcaneHeritageFocus, Guids.ArcaneHeritageFocus).Configure();
      #endregion

      #region Celestial
      AbilityConfigurator.New(CelestialHeritageRay, Guids.CelestialHeritageRay).Configure();
      FeatureConfigurator.New(CelestialHeritageName, Guids.CelestialHeritage).Configure();
      FeatureConfigurator.New(CelestialHeritageResistances, Guids.CelestialHeritageResistances).Configure();
      AbilityResourceConfigurator.New(CelestialHeritageAuraResource, Guids.CelestialHeritageAuraResource).Configure();
      AbilityConfigurator.New(CelestialHeritageAuraAbility, Guids.CelestialHeritageAuraAbility).Configure();
      FeatureConfigurator.New(CelestialHeritageAura, Guids.CelestialHeritageAura).Configure();
      FeatureConfigurator.New(CelestialHeritageConviction, Guids.CelestialHeritageConviction).Configure();
      #endregion

      #region Draconic
      FeatureConfigurator.New(DraconicBlackName, Guids.DraconicBlackHeritage).Configure();
      FeatureConfigurator.New(DraconicBlackResistance, Guids.DraconicBlackHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicBlackBreathAbility, Guids.DraconicBlackHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicBlackBreath, Guids.DraconicBlackHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicBlackWings, Guids.DraconicBlackHeritageWings).Configure();
      #endregion

      #region Base
      UnitPropertyConfigurator.New(EffectiveLevelProperty, Guids.EldritchHeritageEffectiveLevel)
        .AddComponent<SorcererLevelGetter>()
        .Configure();

      FeatureSelectionConfigurator.New(FeatName, Guids.EldritchHeritageFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(Icon)
        .Configure();

      FeatureSelectionConfigurator.New(ImprovedFeatName, Guids.ImprovedEldritchHeritageFeat)
        .SetDisplayName(ImprovedFeatDisplayName)
        .SetDescription(ImprovedFeatDescription)
        .SetIcon(Icon)
        .Configure();

      FeatureSelectionConfigurator.New(GreaterFeatName, Guids.GreaterEldritchHeritageFeat)
        .SetDisplayName(GreaterFeatDisplayName)
        .SetDescription(GreaterFeatDescription)
        .SetIcon(Icon)
        .Configure();
      #endregion
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      UnitPropertyConfigurator.New(EffectiveLevelProperty, Guids.EldritchHeritageEffectiveLevel)
        .AddComponent<SorcererLevelGetter>()
        .Configure();

      FeatureSelectionConfigurator.New(FeatName, Guids.EldritchHeritageFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(Icon)
        .SetIsClassFeature()
        .AddFeatureTagsComponent(featureTags: FeatureTag.Magic)
        .AddPrerequisiteStatValue(StatType.Charisma, 13)
        .AddPrerequisiteCharacterLevel(3)
        .AddToAllFeatures(
          ConfigureAbyssalHeritage1(),
          ConfigureArcaneHeritage1(),
          ConfigureCelestialHeritage1(),
          ConfigureDraconicBlack1())
        .Configure();

      // Since feature selection logic is only in FeatureConfigurator, do this instead of trying to do in parametrized
      // configurator.
      FeatureConfigurator.For(FeatName).AddToGroups(FeatureGroup.Feat).Configure(delayed: true);

      FeatureSelectionConfigurator.New(ImprovedFeatName, Guids.ImprovedEldritchHeritageFeat)
        .SetDisplayName(ImprovedFeatDisplayName)
        .SetDescription(ImprovedFeatDescription)
        .SetIcon(Icon)
        .SetIsClassFeature()
        .AddFeatureTagsComponent(featureTags: FeatureTag.Magic)
        .AddPrerequisiteFeature(FeatName)
        .AddPrerequisiteStatValue(StatType.Charisma, 15)
        .AddPrerequisiteCharacterLevel(11)
        .AddToAllFeatures(
          ConfigureAbyssalHeritage3(),
          ConfigureAbyssalHeritage9(),
          ConfigureArcaneHeritage3(),
          ConfigureCelestialHeritage3(),
          ConfigureCelestialHeritage9(),
          ConfigureDraconicBlack3(),
          ConfigureDraconicBlack9())
        .Configure();

      // Since feature selection logic is only in FeatureConfigurator, do this instead of trying to do in parametrized
      // configurator.
      FeatureConfigurator.For(ImprovedFeatName).AddToGroups(FeatureGroup.Feat).Configure(delayed: true);

      FeatureSelectionConfigurator.New(GreaterFeatName, Guids.GreaterEldritchHeritageFeat)
        .SetDisplayName(GreaterFeatDisplayName)
        .SetDescription(GreaterFeatDescription)
        .SetIcon(Icon)
        .SetIsClassFeature()
        .AddFeatureTagsComponent(featureTags: FeatureTag.Magic)
        .AddPrerequisiteFeature(ImprovedFeatName)
        .AddPrerequisiteStatValue(StatType.Charisma, 17)
        .AddPrerequisiteCharacterLevel(17)
        .AddToAllFeatures(
          ConfigureAbyssalHeritage15(),
          ConfigureArcaneHeritage15(),
          ConfigureCelestialHeritage15(),
          ConfigureDraconicBlack15())
        .Configure();

      // Since feature selection logic is only in FeatureConfigurator, do this instead of trying to do in parametrized
      // configurator.
      FeatureConfigurator.For(GreaterFeatName).AddToGroups(FeatureGroup.Feat).Configure(delayed: true);
    }

    [TypeId("7970fae3-1dba-4f52-9bf7-44fa8b4d4a09")]
    private class SorcererLevelGetter : PropertyValueGetter
    {
      private static BlueprintFeature _greaterEldritchHeritage;
      private static BlueprintFeature GreaterEldritchHeritage
      {
        get
        {
          _greaterEldritchHeritage ??= BlueprintTool.Get<BlueprintFeature>(GreaterFeatName);
          return _greaterEldritchHeritage;
        }
      }

      private static BlueprintFeature _eldritchHeritage;
      private static BlueprintFeature EldritchHeritage
      {
        get
        {
          _eldritchHeritage ??= BlueprintTool.Get<BlueprintFeature>(FeatName);
          return _eldritchHeritage;
        }
      }

      public override int GetBaseValue(UnitEntityData unit)
      {
        try
        {
          if (unit.HasFact(GreaterEldritchHeritage))
            return Math.Min(20, unit.Descriptor.Progression.CharacterLevel);
          else if (unit.HasFact(EldritchHeritage))
            return Math.Min(20, unit.Descriptor.Progression.CharacterLevel - 2);
        }
        catch (Exception e)
        {
          Logger.LogException($"SorcererLevelGetter.GetBaseValue", e);
        }
        return 0;
      }
    }

    #region Abyssal
    private const string AbyssalHeritageName = "EldrichHeritage.Abyssal";

    private const string AbyssalHeritageResistance = "EldritchHeritage.Abyssal.Resistance";
    private const string AbyssalHeritageStrength = "EldritchHeritage.Abyssal.Strength";
    private const string AbyssalHeritageSummons = "EldritchHeritage.Abyssal.Summons";

    private static BlueprintFeature ConfigureAbyssalHeritage1()
    {
      var abyssalBloodline = ProgressionRefs.BloodlineAbyssalProgression.Reference.Get();
      return AddClaws(
        AbyssalHeritageName,
        Guids.AbyssalHeritage,
        abyssalBloodline,
        prereq: FeatureRefs.SkillFocusPhysique.ToString(),
        excludePrereq: FeatureRefs.AbyssalBloodlineRequisiteFeature.ToString(),
        resource: AbilityResourceRefs.BloodlineAbyssalClawsResource.ToString(),
        level3Claw: FeatureRefs.BloodlineAbyssalClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineAbyssalClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineAbyssalClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineAbyssalClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureAbyssalHeritage3()
    {
      var abyssalResistance = FeatureRefs.BloodlineAbyssalResistancesAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        AbyssalHeritageResistance,
        Guids.AbyssalHeritageResistance,
        abyssalResistance,
        new() { AbyssalHeritageName },
        new() { (abyssalResistance.ToReference<BlueprintFeatureReference>(), level: 11) });
    }

    private static BlueprintFeature ConfigureAbyssalHeritage9()
    {
      var abyssalStrength = FeatureRefs.BloodlineAbyssalStrengthAbilityLevel1.Reference.Get();
      return AddFeaturesByLevel(
        AbyssalHeritageStrength,
        Guids.AbyssalHeritageStrength,
        abyssalStrength,
        new() { AbyssalHeritageName },
        new()
        {
          (abyssalStrength.ToReference<BlueprintFeatureReference>(), level: 11),
          (FeatureRefs.BloodlineAbyssalStrengthAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 15),
          (FeatureRefs.BloodlineAbyssalStrengthAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 19)
        },
        BlueprintTool.GetRef<BlueprintFeatureReference>(Guids.AbyssalHeritageSummons),
        new()
        {
          (abyssalStrength.ToReference<BlueprintFeatureReference>(), level: 9),
          (FeatureRefs.BloodlineAbyssalStrengthAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 13),
          (FeatureRefs.BloodlineAbyssalStrengthAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17)
        });
    }

    private static BlueprintFeature ConfigureAbyssalHeritage15()
    {
      var abyssalSummons = FeatureRefs.BloodlineAbyssalAddedSummonings.Reference.Get();
      return AddFeaturesByLevel(
        AbyssalHeritageSummons,
        Guids.AbyssalHeritageSummons,
        abyssalSummons,
        new() { AbyssalHeritageName, ImprovedFeatName },
        new() { (abyssalSummons.ToReference<BlueprintFeatureReference>(), 15) });
    }
    #endregion

    #region Arcane
    private const string ArcaneHeritageName = "EldrichHeritage.Arcane";

    private const string ArcaneHeritageAdept = "EldritchHeritage.Arcane.Adept";
    private const string ArcaneHeritageFocus = "EldritchHeritage.Arcane.Focus";

    private static BlueprintFeature ConfigureArcaneHeritage1()
    {
      var arcaneBloodline = ProgressionRefs.BloodlineArcaneProgression.Reference.Get();
      return FeatureSelectionConfigurator.New(ArcaneHeritageName, Guids.ArcaneHeritage)
        .SetDisplayName(arcaneBloodline.m_DisplayName)
        .SetDescription(arcaneBloodline.m_Description)
        .SetIcon(arcaneBloodline.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() {
            FeatureRefs.SkillFocusKnowledgeArcana.ToString(),
            FeatureRefs.SkillFocusKnowledgeWorld.ToString(),
            FeatureRefs.SkillFocusLoreNature.ToString(),
            FeatureRefs.SkillFocusLoreReligion.ToString(),
          },
          amount: 1)
        .AddPrerequisiteNoFeature(FeatureRefs.ArcaneBloodlineRequisiteFeature.ToString())
        .AddPrerequisiteNoFeature(ArcaneHeritageName)
        .AddToAllFeatures(FeatureSelectionRefs.BloodlineArcaneArcaneBondFeature.ToString())
        .Configure();
    }

    private static BlueprintFeature ConfigureArcaneHeritage3()
    {
      var arcaneAdept = FeatureRefs.BloodlineArcaneCombatCastingAdeptFeatureLevel2.Reference.Get();
      return AddFeaturesByLevel(
        ArcaneHeritageAdept,
        Guids.ArcaneHeritageAdept,
        arcaneAdept,
        new() { ArcaneHeritageName },
        new()
        {
          (arcaneAdept.ToReference<BlueprintFeatureReference>(), level: 11),
          (FeatureRefs.BloodlineArcaneCombatCastingAdeptFeatureLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
        });
    }

    private static BlueprintFeature ConfigureArcaneHeritage15()
    {
      var arcaneFocus = ParametrizedFeatureRefs.BloodlineArcaneSchoolPowerFeature.Reference.Get();
      return FeatureSelectionConfigurator.New(ArcaneHeritageFocus, Guids.ArcaneHeritageFocus)
        .SetDisplayName(arcaneFocus.m_DisplayName)
        .SetDescription(arcaneFocus.m_Description)
        .SetIcon(arcaneFocus.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(ArcaneHeritageName)
        .AddPrerequisiteFeature(ImprovedFeatName)
        .AddPrerequisiteNoFeature(ArcaneHeritageFocus)
        .AddToAllFeatures(arcaneFocus)
        .Configure();
    }
    #endregion

    #region Celestial
    private const string CelestialHeritageName = "EldrichHeritage.Celestial";

    private const string CelestialHeritageRay = "EldritchHeritage.Celestial.Ray.Attack";
    private const string CelestialHeritageResistances = "EldritchHeritage.Celestial.Resitances";
    private const string CelestialHeritageAura = "EldritchHeritage.Celestial.Aura";
    private const string CelestialHeritageAuraResource = "EldritchHeritage.Celestial.Aura.Resources";
    private const string CelestialHeritageAuraAbility = "EldritchHeritage.Celestial.Aura.Ability";
    private const string CelestialHeritageConviction = "EldritchHeritage.Celestial.Aura.Convection";

    private static BlueprintFeature ConfigureCelestialHeritage1()
    {
      var heavenlyFire = AbilityRefs.BloodlineCelestialHeavenlyFireAbility.Reference.Get();
      var ray = AbilityConfigurator.New(CelestialHeritageRay, Guids.CelestialHeritageRay)
        .SetDisplayName(heavenlyFire.m_DisplayName)
        .SetDescription(heavenlyFire.m_Description)
        .SetIcon(heavenlyFire.Icon)
        .SetType(AbilityType.SpellLike)
        .SetCustomRange(30)
        .SetCanTargetEnemies()
        .SetCanTargetFriends()
        .SetCanTargetSelf()
        .SetEffectOnAlly(heavenlyFire.EffectOnAlly)
        .SetEffectOnEnemy(heavenlyFire.EffectOnEnemy)
        .SetAnimation(heavenlyFire.Animation)
        .SetActionType(heavenlyFire.ActionType)
        .SetAvailableMetamagic(heavenlyFire.AvailableMetamagic)
        .AddComponent(heavenlyFire.GetComponent<SpellComponent>())
        .AddComponent(heavenlyFire.GetComponent<AbilityDeliverProjectile>())
        .AddComponent(heavenlyFire.GetComponent<AbilityEffectRunAction>())
        .AddComponent(heavenlyFire.GetComponent<AbilityResourceLogic>())
        .AddComponent(heavenlyFire.GetComponent<SpellDescriptorComponent>())
        .AddComponent(heavenlyFire.GetComponent<AbilityTargetAlignment>())
        .AddContextRankConfig(
          ContextRankConfigs.CustomProperty(
              EffectiveLevelProperty, type: AbilityRankType.DamageBonus, min: 0, max: 20)
            .WithDiv2Progression())
        .Configure();

      var celestialBloodline = ProgressionRefs.BloodlineCelestialProgression.Reference.Get();
      return FeatureConfigurator.New(CelestialHeritageName, Guids.CelestialHeritage)
        .SetDisplayName(celestialBloodline.m_DisplayName)
        .SetDescription(celestialBloodline.m_Description)
        .SetIcon(celestialBloodline.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(FeatureRefs.SkillFocusLoreReligion.ToString())
        .AddPrerequisiteNoFeature(FeatureRefs.CelestialBloodlineRequisiteFeature.ToString())
        .AddFacts(new() { ray })
        .AddAbilityResources(
          resource: AbilityResourceRefs.BloodlineCelestialHeavenlyFireResource.ToString(), restoreAmount: true)
        .Configure();
    }

    private static BlueprintFeature ConfigureCelestialHeritage3()
    {
      var celestialResistances = FeatureRefs.BloodlineCelestialResistancesAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        CelestialHeritageResistances,
        Guids.CelestialHeritageResistances,
        celestialResistances,
        new() { CelestialHeritageName },
        new() { (celestialResistances.ToReference<BlueprintFeatureReference>(), level: 3) });
    }

    private static BlueprintFeature ConfigureCelestialHeritage9()
    {
      var celestialAuraResource = AbilityResourceRefs.BloodlineCelestialAuraOfHeavenResource.Reference.Get();
      var resource =
        AbilityResourceConfigurator.New(CelestialHeritageAuraResource, Guids.CelestialHeritageAuraResource)
          .SetIcon(celestialAuraResource.Icon)
          .Configure();

      var celestialAuraAbility = AbilityRefs.BloodlineCelestialAuraOfHeavenAbility.Reference.Get();
      var aura = AbilityConfigurator.New(CelestialHeritageAuraAbility, Guids.CelestialHeritageAuraAbility)
        .SetDisplayName(celestialAuraAbility.m_DisplayName)
        .SetDescription(celestialAuraAbility.m_Description)
        .SetIcon(celestialAuraAbility.Icon)
        .AddComponent(celestialAuraAbility.GetComponent<SpellComponent>())
        .AddComponent(celestialAuraAbility.GetComponent<AbilityEffectRunAction>())
        .AddComponent(celestialAuraAbility.GetComponent<AbilitySpawnFx>())
        .AddAbilityResourceLogic(amount: 1, isSpendResource: true, requiredResource: resource)
        .Configure();

      var celestialAura = FeatureRefs.BloodlineCelestialAuraOfHeavenFeature.Reference.Get();
      return FeatureConfigurator.New(CelestialHeritageAura, Guids.CelestialHeritageAura)
        .SetDisplayName(celestialAura.m_DisplayName)
        .SetDescription(celestialAura.m_Description)
        .SetIcon(celestialAura.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(CelestialHeritageName)
        .AddFacts(new() { aura })
        .AddAbilityResources(resource: resource, restoreAmount: true)
        .AddComponent(
          new SetResourceMax(ContextValues.Rank(), resource.ToReference<BlueprintAbilityResourceReference>()))
        .AddContextRankConfig(ContextRankConfigs.CustomProperty(EffectiveLevelProperty, max: 10).WithDiv2Progression())
        .Configure();
    }

    private static BlueprintFeature ConfigureCelestialHeritage15()
    {
      var celestialConviction = FeatureRefs.BloodlineCelestialConvictionAbility.Reference.Get();
      return FeatureConfigurator.New(CelestialHeritageConviction, Guids.CelestialHeritageConviction)
        .SetDisplayName(celestialConviction.m_DisplayName)
        .SetDescription(celestialConviction.m_Description)
        .SetIcon(celestialConviction.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(CelestialHeritageName)
        .AddPrerequisiteFeature(ImprovedFeatName)
        .AddSpellResistanceAgainstAlignment(alignment: AlignmentComponent.Evil, value: ContextValues.Rank())
        .AddContextRankConfig(ContextRankConfigs.CharacterLevel(max: 20).WithBonusValueProgression(11))
        .Configure();
    }
    #endregion

    #region Draconic
    private const string DraconicBlackName = "EldrichHeritage.Draconic.Black";
    private const string DraconicBlackResistance = "EldrichHeritage.Draconic.Black.Resistance";
    private const string DraconicBlackBreath = "EldrichHeritage.Draconic.Black.Breath";
    private const string DraconicBlackBreathAbility = "EldrichHeritage.Draconic.Black.Breath.Ability";
    private const string DraconicBlackWings = "EldrichHeritage.Draconic.Black.Wings";

    private static BlueprintFeature ConfigureDraconicBlack1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicBlackProgression.Reference.Get();
      return AddClaws(
        DraconicBlackName,
        Guids.DraconicBlackHeritage,
        draconicBloodline,
        prereq: FeatureRefs.SkillFocusPerception.ToString(),
        excludePrereq: FeatureRefs.BloodlineDraconicClassSkill.ToString(),
        resource: AbilityResourceRefs.BloodlineDraconicClawsResource.ToString(),
        level3Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicBlack3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicBlackResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicBlackResistance,
        Guids.DraconicBlackHeritageResistance,
        draconicResistances,
        new() { DraconicBlackName },
        new()
        {
          (FeatureRefs.BloodlineDraconicBlackResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
          (FeatureRefs.BloodlineDraconicBlackResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
          (FeatureRefs.BloodlineDraconicBlackResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
        });
    }

    private static BlueprintFeature ConfigureDraconicBlack9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicBlackBreathAbility,
        abilityGuid: Guids.DraconicBlackHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicBlackBreathWeaponAbility.Reference.Get(),
        featureName: DraconicBlackBreath,
        featureGuid: Guids.DraconicBlackHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicBlackBreathWeaponFeature.Reference.Get(),
        resource: AbilityResourceRefs.BloodlineDraconicBreathWeaponResource.ToString(),
        extraUse: FeatureRefs.BloodlineDraconicBlackBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicBlack15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicBlack.Reference.Get();
      return FeatureConfigurator.New(DraconicBlackWings, Guids.DraconicBlackHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(DraconicBlackName)
        .AddPrerequisiteFeature(ImprovedFeatName)
        .AddFacts(new() { wings })
        .Configure();
    }

    private static BlueprintFeature ConfigureDraconicBreath(
      string abilityName,
      string abilityGuid,
      BlueprintAbility breath,
      string featureName,
      string featureGuid,
      BlueprintFeature breathFeature,
      string resource,
      BlueprintFeatureReference extraUse)
    {
      var ability = AbilityConfigurator.New(abilityName, abilityGuid)
        .SetDisplayName(breath.m_DisplayName)
        .SetDescription(breath.m_Description)
        .SetIcon(breath.Icon)
        .SetType(breath.Type)
        .SetRange(breath.Range)
        .SetCanTargetEnemies()
        .SetCanTargetFriends()
        .SetCanTargetSelf()
        .SetCanTargetPoint()
        .SetEffectOnEnemy(breath.EffectOnEnemy)
        .SetAnimation(breath.Animation)
        .SetActionType(breath.ActionType)
        .SetAvailableMetamagic(breath.AvailableMetamagic)
        .SetLocalizedSavingThrow(breath.LocalizedSavingThrow)
        .AddComponent(breath.GetComponent<AbilityEffectRunAction>())
        .AddComponent(breath.GetComponent<AbilityDeliverProjectile>())
        .AddComponent(breath.GetComponent<SpellDescriptorComponent>())
        .AddComponent(breath.GetComponent<AbilityResourceLogic>())
        .AddContextRankConfig(ContextRankConfigs.CustomProperty(EffectiveLevelProperty, max: 20))
        .Configure();

      return FeatureConfigurator.New(featureName, featureGuid)
        .SetDisplayName(breathFeature.m_DisplayName)
        .SetDescription(breathFeature.m_Description)
        .SetIcon(breathFeature.Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddFacts(new() { ability })
        .AddAbilityResources(resource: resource, restoreAmount: true)
        .AddComponent(new BindDragonBreath(ability.ToReference<BlueprintAbilityReference>()))
        .AddComponent(
          new ApplyFeatureOnCharacterLevel(
            new() { (extraUse, level: 19) },
            greaterFeature: BlueprintTool.GetRef<BlueprintFeatureReference>(Guids.DraconicBlackHeritageWings),
            greaterFeatureLevels: new() { (extraUse, level: 17) }))
        .Configure();
    }

    [TypeId("cf0a51da-5296-4463-ad8d-ccf5c4a7598d")]
    private class BindDragonBreath :
      UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAbilityParams>
    {
      private static BlueprintUnitProperty _effectiveLevel;
      private static BlueprintUnitProperty EffectiveLevel
      {
        get
        {
          _effectiveLevel ??= BlueprintTool.Get<BlueprintUnitProperty>(EffectiveLevelProperty);
          return _effectiveLevel;
        }
      }

      private readonly BlueprintAbilityReference Ability;

      public BindDragonBreath(BlueprintAbilityReference breathAbility)
      {
        Ability = breathAbility;
      }

      public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
      {
        try
        {
          if (Ability.deserializedGuid == evt.Spell.AssetGuid)
          {
            Logger.NativeLog($"Binding dragon breath for {evt.Spell.Name}");
            evt.ReplaceStat = StatType.Charisma;
            evt.ReplaceCasterLevel = EffectiveLevel.GetInt(Owner);
            evt.ReplaceSpellLevel = EffectiveLevel.GetInt(Owner) / 2;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("BindAbilityToCharacterLevel.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }
    }
    #endregion

    // For bloodline abilities that add a feature which is replaced by a higher level feature later.
    // Greater enables the change from Level - 2 to Level when Greater Eldritch Heritage is acquired.
    private static BlueprintFeature AddFeaturesByLevel(
      string name,
      string guid,
      BlueprintFeature sourceFeature,
      List<string> prerequisites,
      List<(BlueprintFeatureReference feature, int level)> featuresByLevel,
      BlueprintFeatureReference greaterFeature = null,
      List<(BlueprintFeatureReference feature, int level)> greaterFeatureLevels = null)
    {
      var feature = FeatureConfigurator.New(name, guid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp(true)
        .AddComponent(
          new ApplyFeatureOnCharacterLevel(
            featuresByLevel.ToList(),
            greaterFeature: greaterFeature,
            greaterFeatureLevels: greaterFeatureLevels));

      foreach (var prereq in prerequisites)
        feature.AddPrerequisiteFeature(prereq);

      return feature.Configure();
    }

    private static BlueprintFeature AddClaws(
      string name,
      string guid,
      BlueprintFeature sourceFeature,
      string prereq,
      string excludePrereq,
      string resource,
      BlueprintFeatureReference level3Claw,
      BlueprintFeatureReference level7Claw,
      BlueprintFeatureReference level9Claw,
      BlueprintFeatureReference level13Claw)
    {
      return FeatureConfigurator.New(name, guid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.m_Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddPrerequisiteFeature(prereq)
        .AddPrerequisiteNoFeature(excludePrereq)
        .AddComponent(
          new ApplyFeatureOnCharacterLevel(
            new() {
              (level3Claw, level: 3),
              (level7Claw, level: 7),
              (level9Claw, level: 9),
              (level13Claw, level: 13),
            }))
        .AddAbilityResources(resource: resource, restoreAmount: true)
        .Configure();
    }
  }
}
