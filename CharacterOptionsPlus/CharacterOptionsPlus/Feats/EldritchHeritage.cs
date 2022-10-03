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
      FeatureConfigurator.New(DraconicBlackHeritage, Guids.DraconicBlackHeritage).Configure();
      FeatureConfigurator.New(DraconicBlackHeritageResistance, Guids.DraconicBlackHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicBlackHeritageBreathAbility, Guids.DraconicBlackHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicBlackHeritageBreath, Guids.DraconicBlackHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicBlackHeritageWings, Guids.DraconicBlackHeritageWings).Configure();

      FeatureConfigurator.New(DraconicBlueHeritage, Guids.DraconicBlueHeritage).Configure();
      FeatureConfigurator.New(DraconicBlueHeritageResistance, Guids.DraconicBlueHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicBlueHeritageBreathAbility, Guids.DraconicBlueHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicBlueHeritageBreath, Guids.DraconicBlueHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicBlueHeritageWings, Guids.DraconicBlueHeritageWings).Configure();

      FeatureConfigurator.New(DraconicBrassHeritage, Guids.DraconicBrassHeritage).Configure();
      FeatureConfigurator.New(DraconicBrassHeritageResistance, Guids.DraconicBrassHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicBrassHeritageBreathAbility, Guids.DraconicBrassHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicBrassHeritageBreath, Guids.DraconicBrassHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicBrassHeritageWings, Guids.DraconicBrassHeritageWings).Configure();

      FeatureConfigurator.New(DraconicBronzeHeritage, Guids.DraconicBronzeHeritage).Configure();
      FeatureConfigurator.New(DraconicBronzeHeritageResistance, Guids.DraconicBronzeHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicBronzeHeritageBreathAbility, Guids.DraconicBronzeHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicBronzeHeritageBreath, Guids.DraconicBronzeHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicBronzeHeritageWings, Guids.DraconicBronzeHeritageWings).Configure();

      FeatureConfigurator.New(DraconicCopperHeritage, Guids.DraconicCopperHeritage).Configure();
      FeatureConfigurator.New(DraconicCopperHeritageResistance, Guids.DraconicCopperHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicCopperHeritageBreathAbility, Guids.DraconicCopperHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicCopperHeritageBreath, Guids.DraconicCopperHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicCopperHeritageWings, Guids.DraconicCopperHeritageWings).Configure();

      FeatureConfigurator.New(DraconicGoldHeritage, Guids.DraconicGoldHeritage).Configure();
      FeatureConfigurator.New(DraconicGoldHeritageResistance, Guids.DraconicGoldHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicGoldHeritageBreathAbility, Guids.DraconicGoldHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicGoldHeritageBreath, Guids.DraconicGoldHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicGoldHeritageWings, Guids.DraconicGoldHeritageWings).Configure();

      FeatureConfigurator.New(DraconicGreenHeritage, Guids.DraconicGreenHeritage).Configure();
      FeatureConfigurator.New(DraconicGreenHeritageResistance, Guids.DraconicGreenHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicGreenHeritageBreathAbility, Guids.DraconicGreenHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicGreenHeritageBreath, Guids.DraconicGreenHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicGreenHeritageWings, Guids.DraconicGreenHeritageWings).Configure();

      FeatureConfigurator.New(DraconicRedHeritage, Guids.DraconicRedHeritage).Configure();
      FeatureConfigurator.New(DraconicRedHeritageResistance, Guids.DraconicRedHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicRedHeritageBreathAbility, Guids.DraconicRedHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicRedHeritageBreath, Guids.DraconicRedHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicRedHeritageWings, Guids.DraconicRedHeritageWings).Configure();

      FeatureConfigurator.New(DraconicSilverHeritage, Guids.DraconicSilverHeritage).Configure();
      FeatureConfigurator.New(DraconicSilverHeritageResistance, Guids.DraconicSilverHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicSilverHeritageBreathAbility, Guids.DraconicSilverHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicSilverHeritageBreath, Guids.DraconicSilverHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicSilverHeritageWings, Guids.DraconicSilverHeritageWings).Configure();

      FeatureConfigurator.New(DraconicWhiteHeritage, Guids.DraconicWhiteHeritage).Configure();
      FeatureConfigurator.New(DraconicWhiteHeritageResistance, Guids.DraconicWhiteHeritageResistance).Configure();
      AbilityConfigurator.New(DraconicWhiteHeritageBreathAbility, Guids.DraconicWhiteHeritageBreathAbility).Configure();
      FeatureConfigurator.New(DraconicWhiteHeritageBreath, Guids.DraconicWhiteHeritageBreath).Configure();
      FeatureConfigurator.New(DraconicWhiteHeritageWings, Guids.DraconicWhiteHeritageWings).Configure();
      #endregion

      #region Elemental
      AbilityConfigurator.New(ElementalAirHeritageRay, Guids.ElementalAirHeritageRay).Configure();
      FeatureConfigurator.New(ElementalAirHeritage, Guids.ElementalAirHeritage).Configure();
      FeatureConfigurator.New(ElementalAirHeritageResistance, Guids.ElementalAirHeritageResistance).Configure();
      AbilityConfigurator.New(ElementalAirHeritageBlastAbility, Guids.ElementalAirHeritageBlastAbility).Configure();
      FeatureConfigurator.New(ElementalAirHeritageBlast, Guids.ElementalAirHeritageBlast).Configure();
      FeatureConfigurator.New(ElementalAirHeritageMovement, Guids.ElementalAirHeritageMovement).Configure();

      AbilityConfigurator.New(ElementalEarthHeritageRay, Guids.ElementalEarthHeritageRay).Configure();
      FeatureConfigurator.New(ElementalEarthHeritage, Guids.ElementalEarthHeritage).Configure();
      FeatureConfigurator.New(ElementalEarthHeritageResistance, Guids.ElementalEarthHeritageResistance).Configure();
      AbilityConfigurator.New(ElementalEarthHeritageBlastAbility, Guids.ElementalEarthHeritageBlastAbility).Configure();
      FeatureConfigurator.New(ElementalEarthHeritageBlast, Guids.ElementalEarthHeritageBlast).Configure();
      FeatureConfigurator.New(ElementalEarthHeritageMovement, Guids.ElementalEarthHeritageMovement).Configure();

      AbilityConfigurator.New(ElementalFireHeritageRay, Guids.ElementalFireHeritageRay).Configure();
      FeatureConfigurator.New(ElementalFireHeritage, Guids.ElementalFireHeritage).Configure();
      FeatureConfigurator.New(ElementalFireHeritageResistance, Guids.ElementalFireHeritageResistance).Configure();
      AbilityConfigurator.New(ElementalFireHeritageBlastAbility, Guids.ElementalFireHeritageBlastAbility).Configure();
      FeatureConfigurator.New(ElementalFireHeritageBlast, Guids.ElementalFireHeritageBlast).Configure();
      FeatureConfigurator.New(ElementalFireHeritageMovement, Guids.ElementalFireHeritageMovement).Configure();

      AbilityConfigurator.New(ElementalWaterHeritageRay, Guids.ElementalWaterHeritageRay).Configure();
      FeatureConfigurator.New(ElementalWaterHeritage, Guids.ElementalWaterHeritage).Configure();
      FeatureConfigurator.New(ElementalWaterHeritageResistance, Guids.ElementalWaterHeritageResistance).Configure();
      AbilityConfigurator.New(ElementalWaterHeritageBlastAbility, Guids.ElementalWaterHeritageBlastAbility).Configure();
      FeatureConfigurator.New(ElementalWaterHeritageBlast, Guids.ElementalWaterHeritageBlast).Configure();
      FeatureConfigurator.New(ElementalWaterHeritageMovement, Guids.ElementalWaterHeritageMovement).Configure();
      #endregion

      #region Base
      FeatureConfigurator.New(DraconicHeritage, Guids.DraconicHeritage).Configure();
      FeatureConfigurator.New(ElementalHeritage, Guids.ElementalHeritage).Configure();

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

      // Used as an exclude feature to prevent selecting multiple draconic bloodlines
      var draconicBloodline = ProgressionRefs.BloodlineDraconicBlackProgression.Reference.Get();
      FeatureConfigurator.New(DraconicHeritage, Guids.DraconicHeritage)
        .SetDisplayName(DraconicHeritageDisplayName)
        .SetDescription(DraconicHeritageDescription)
        .SetIcon(draconicBloodline.Icon)
        .Configure();

      // Used as an exclude feature to prevent selecting multiple draconic bloodlines
      var elementalBloodline = ProgressionRefs.BloodlineElementalAirProgression.Reference.Get();
      FeatureConfigurator.New(ElementalHeritage, Guids.ElementalHeritage)
        .SetDisplayName(ElementalHeritageDisplayName)
        .SetDescription(ElementalHeritageDescription)
        .SetIcon(elementalBloodline.Icon)
        .Configure();

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

          ConfigureDraconicBlack1(),

          ConfigureDraconicBlue1(),

          ConfigureDraconicBrass1(),

          ConfigureDraconicBronze1(),

          ConfigureDraconicCopper1(),

          ConfigureDraconicGold1(),

          ConfigureDraconicGreen1(),
          
          ConfigureDraconicRed1(),
          
          ConfigureDraconicSilver1(),
          
          ConfigureDraconicWhite1(),
          
          ConfigureElementalAir1(),
          
          ConfigureElementalEarth1(),

          ConfigureElementalFire1(),

          ConfigureElementalWater1())
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
          ConfigureDraconicBlack9(),

          ConfigureDraconicBlue3(),
          ConfigureDraconicBlue9(),

          ConfigureDraconicBrass3(),
          ConfigureDraconicBrass9(),

          ConfigureDraconicBronze3(),
          ConfigureDraconicBronze9(),

          ConfigureDraconicCopper3(),
          ConfigureDraconicCopper9(),

          ConfigureDraconicGold3(),
          ConfigureDraconicGold9(),

          ConfigureDraconicGreen3(),
          ConfigureDraconicGreen9(),
          
          ConfigureDraconicRed3(),
          ConfigureDraconicRed9(),
          
          ConfigureDraconicSilver3(),
          ConfigureDraconicSilver9(),
          
          ConfigureDraconicWhite3(),
          ConfigureDraconicWhite9(),

          ConfigureElementalAir3(),
          ConfigureElementalAir9(),

          ConfigureElementalEarth3(),
          ConfigureElementalEarth9(),

          ConfigureElementalFire3(),
          ConfigureElementalFire9(),

          ConfigureElementalWater3(),
          ConfigureElementalWater9())
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

          ConfigureDraconicBlack15(),

          ConfigureDraconicBlue15(),

          ConfigureDraconicBrass15(),

          ConfigureDraconicBronze15(),

          ConfigureDraconicCopper15(),

          ConfigureDraconicGold15(),

          ConfigureDraconicGreen15(),
          
          ConfigureDraconicRed15(),
          
          ConfigureDraconicSilver15(),

          ConfigureDraconicWhite15(),
          
          ConfigureElementalAir15(),

          ConfigureElementalEarth15(),

          ConfigureElementalFire15(),

          ConfigureElementalWater15())
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

    [TypeId("cf0a51da-5296-4463-ad8d-ccf5c4a7598d")]
    private class BindToCharacterLevel :
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

      public BindToCharacterLevel(BlueprintAbilityReference breathAbility)
      {
        Ability = breathAbility;
      }

      public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
      {
        try
        {
          if (Ability.deserializedGuid == evt.Spell.AssetGuid)
          {
            Logger.NativeLog($"Binding {evt.Spell.Name} to effective level");
            evt.ReplaceStat = StatType.Charisma;
            evt.ReplaceCasterLevel = EffectiveLevel.GetInt(Owner);
            evt.ReplaceSpellLevel = EffectiveLevel.GetInt(Owner) / 2;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("BindToCharacterLevel.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }
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
        excludePrereqs: new() { FeatureRefs.AbyssalBloodlineRequisiteFeature.ToString() },
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
        prerequisites: new() { Guids.AbyssalHeritageStrength, Guids.AbyssalHeritageResistance },
        featuresByLevel: new() { (abyssalSummons.ToReference<BlueprintFeatureReference>(), 15) });
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
        prerequisites: new() { ArcaneHeritageName },
        featuresByLevel:
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
        .AddPrerequisiteFeature(Guids.ArcaneHeritageAdept)
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
      return AddRay(
        abilityName: CelestialHeritageRay,
        abilityGuid: Guids.CelestialHeritageRay,
        sourceAbility: AbilityRefs.BloodlineCelestialHeavenlyFireAbility.Reference.Get(),
        featureName: CelestialHeritageName,
        featureGuid: Guids.CelestialHeritage,
        sourceFeature: ProgressionRefs.BloodlineCelestialProgression.Reference.Get(),
        prereq: FeatureRefs.SkillFocusLoreReligion.ToString(),
        excludePrereqs: new() { FeatureRefs.CelestialBloodlineRequisiteFeature.ToString() },
        resource: AbilityResourceRefs.BloodlineCelestialHeavenlyFireResource.ToString(),
        extraComponents: new Type[] { typeof(AbilityTargetAlignment) });
    }

    private static BlueprintFeature ConfigureCelestialHeritage3()
    {
      var celestialResistances = FeatureRefs.BloodlineCelestialResistancesAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        CelestialHeritageResistances,
        Guids.CelestialHeritageResistances,
        celestialResistances,
        prerequisites: new() { CelestialHeritageName },
        featuresByLevel: new() { (celestialResistances.ToReference<BlueprintFeatureReference>(), level: 3) });
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
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.CelestialHeritageResistances, Guids.CelestialHeritageAura }, amount: 1 )
        .AddSpellResistanceAgainstAlignment(alignment: AlignmentComponent.Evil, value: ContextValues.Rank())
        .AddContextRankConfig(ContextRankConfigs.CharacterLevel(max: 20).WithBonusValueProgression(11))
        .Configure();
    }
    #endregion

    #region Draconic
    private const string DraconicHeritage = "EldritchHeritage.Draconic";
    private const string DraconicHeritageDisplayName = "DraconicHeritage.Name";
    private const string DraconicHeritageDescription = "DraconicHeritage.Description";

    #region Black Dragon
    private const string DraconicBlackHeritage = "EldrichHeritage.Draconic.Black";
    private const string DraconicBlackHeritageResistance = "EldrichHeritage.Draconic.Black.Resistance";
    private const string DraconicBlackHeritageBreath = "EldrichHeritage.Draconic.Black.Breath";
    private const string DraconicBlackHeritageBreathAbility = "EldrichHeritage.Draconic.Black.Breath.Ability";
    private const string DraconicBlackHeritageWings = "EldrichHeritage.Draconic.Black.Wings";

    private static BlueprintFeature ConfigureDraconicBlack1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicBlackProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicBlackHeritage,
        Guids.DraconicBlackHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicBlackClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicBlack3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicBlackResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicBlackHeritageResistance,
        Guids.DraconicBlackHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicBlackHeritage },
        featuresByLevel:
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
        abilityName: DraconicBlackHeritageBreathAbility,
        abilityGuid: Guids.DraconicBlackHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicBlackBreathWeaponAbility.Reference.Get(),
        featureName: DraconicBlackHeritageBreath,
        featureGuid: Guids.DraconicBlackHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicBlackBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicBlackHeritage,
        extraUse: FeatureRefs.BloodlineDraconicBlackBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicBlackHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicBlack15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicBlack.Reference.Get();
      return FeatureConfigurator.New(DraconicBlackHeritageWings, Guids.DraconicBlackHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicBlackHeritageBreath, Guids.DraconicBlackHeritageResistance }, amount: 1 )
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Blue Dragon
    private const string DraconicBlueHeritage = "EldrichHeritage.Draconic.Blue";
    private const string DraconicBlueHeritageResistance = "EldrichHeritage.Draconic.Blue.Resistance";
    private const string DraconicBlueHeritageBreath = "EldrichHeritage.Draconic.Blue.Breath";
    private const string DraconicBlueHeritageBreathAbility = "EldrichHeritage.Draconic.Blue.Breath.Ability";
    private const string DraconicBlueHeritageWings = "EldrichHeritage.Draconic.Blue.Wings";

    private static BlueprintFeature ConfigureDraconicBlue1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicBlueProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicBlueHeritage,
        Guids.DraconicBlueHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicBlueClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicBlueClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicBlueClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicBlueClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicBlue3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicBlueResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicBlueHeritageResistance,
        Guids.DraconicBlueHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicBlueHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicBlueResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicBlueResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicBlueResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicBlue9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicBlueHeritageBreathAbility,
        abilityGuid: Guids.DraconicBlueHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicBlueBreathWeaponAbility.Reference.Get(),
        featureName: DraconicBlueHeritageBreath,
        featureGuid: Guids.DraconicBlueHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicBlueBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicBlueHeritage,
        extraUse: FeatureRefs.BloodlineDraconicBlueBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicBlueHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicBlue15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicBlue.Reference.Get();
      return FeatureConfigurator.New(DraconicBlueHeritageWings, Guids.DraconicBlueHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicBlueHeritageBreath, Guids.DraconicBlueHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Brass Dragon
    private const string DraconicBrassHeritage = "EldrichHeritage.Draconic.Brass";
    private const string DraconicBrassHeritageResistance = "EldrichHeritage.Draconic.Brass.Resistance";
    private const string DraconicBrassHeritageBreath = "EldrichHeritage.Draconic.Brass.Breath";
    private const string DraconicBrassHeritageBreathAbility = "EldrichHeritage.Draconic.Brass.Breath.Ability";
    private const string DraconicBrassHeritageWings = "EldrichHeritage.Draconic.Brass.Wings";

    private static BlueprintFeature ConfigureDraconicBrass1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicBrassProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicBrassHeritage,
        Guids.DraconicBrassHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicBrassClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicBrassClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicBrassClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicBrassClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicBrass3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicBrassResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicBrassHeritageResistance,
        Guids.DraconicBrassHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicBrassHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicBrassResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicBrassResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicBrassResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicBrass9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicBrassHeritageBreathAbility,
        abilityGuid: Guids.DraconicBrassHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicBrassBreathWeaponAbility.Reference.Get(),
        featureName: DraconicBrassHeritageBreath,
        featureGuid: Guids.DraconicBrassHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicBrassBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicBrassHeritage,
        extraUse: FeatureRefs.BloodlineDraconicBrassBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicBrassHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicBrass15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicBrass.Reference.Get();
      return FeatureConfigurator.New(DraconicBrassHeritageWings, Guids.DraconicBrassHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicBrassHeritageBreath, Guids.DraconicBrassHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Bronze Dragon
    private const string DraconicBronzeHeritage = "EldrichHeritage.Draconic.Bronze";
    private const string DraconicBronzeHeritageResistance = "EldrichHeritage.Draconic.Bronze.Resistance";
    private const string DraconicBronzeHeritageBreath = "EldrichHeritage.Draconic.Bronze.Breath";
    private const string DraconicBronzeHeritageBreathAbility = "EldrichHeritage.Draconic.Bronze.Breath.Ability";
    private const string DraconicBronzeHeritageWings = "EldrichHeritage.Draconic.Bronze.Wings";

    private static BlueprintFeature ConfigureDraconicBronze1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicBronzeProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicBronzeHeritage,
        Guids.DraconicBronzeHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicBronzeClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicBronzeClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicBronzeClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicBronzeClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicBronze3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicBronzeResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicBronzeHeritageResistance,
        Guids.DraconicBronzeHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicBronzeHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicBronzeResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicBronzeResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicBronzeResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicBronze9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicBronzeHeritageBreathAbility,
        abilityGuid: Guids.DraconicBronzeHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicBronzeBreathWeaponAbility.Reference.Get(),
        featureName: DraconicBronzeHeritageBreath,
        featureGuid: Guids.DraconicBronzeHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicBronzeBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicBronzeHeritage,
        extraUse: FeatureRefs.BloodlineDraconicBronzeBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicBronzeHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicBronze15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicBronze.Reference.Get();
      return FeatureConfigurator.New(DraconicBronzeHeritageWings, Guids.DraconicBronzeHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicBronzeHeritageBreath, Guids.DraconicBronzeHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Copper Dragon
    private const string DraconicCopperHeritage = "EldrichHeritage.Draconic.Copper";
    private const string DraconicCopperHeritageResistance = "EldrichHeritage.Draconic.Copper.Resistance";
    private const string DraconicCopperHeritageBreath = "EldrichHeritage.Draconic.Copper.Breath";
    private const string DraconicCopperHeritageBreathAbility = "EldrichHeritage.Draconic.Copper.Breath.Ability";
    private const string DraconicCopperHeritageWings = "EldrichHeritage.Draconic.Copper.Wings";

    private static BlueprintFeature ConfigureDraconicCopper1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicCopperProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicCopperHeritage,
        Guids.DraconicCopperHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicCopperClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicCopperClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicCopperClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicCopperClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicCopper3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicCopperResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicCopperHeritageResistance,
        Guids.DraconicCopperHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicCopperHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicCopperResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicCopperResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicCopperResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicCopper9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicCopperHeritageBreathAbility,
        abilityGuid: Guids.DraconicCopperHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicCopperBreathWeaponAbility.Reference.Get(),
        featureName: DraconicCopperHeritageBreath,
        featureGuid: Guids.DraconicCopperHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicCopperBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicCopperHeritage,
        extraUse: FeatureRefs.BloodlineDraconicCopperBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicCopperHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicCopper15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicCopper.Reference.Get();
      return FeatureConfigurator.New(DraconicCopperHeritageWings, Guids.DraconicCopperHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicCopperHeritageBreath, Guids.DraconicCopperHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Gold Dragon
    private const string DraconicGoldHeritage = "EldrichHeritage.Draconic.Gold";
    private const string DraconicGoldHeritageResistance = "EldrichHeritage.Draconic.Gold.Resistance";
    private const string DraconicGoldHeritageBreath = "EldrichHeritage.Draconic.Gold.Breath";
    private const string DraconicGoldHeritageBreathAbility = "EldrichHeritage.Draconic.Gold.Breath.Ability";
    private const string DraconicGoldHeritageWings = "EldrichHeritage.Draconic.Gold.Wings";

    private static BlueprintFeature ConfigureDraconicGold1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicGoldProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicGoldHeritage,
        Guids.DraconicGoldHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicGoldClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicGoldClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicGoldClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicGoldClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicGold3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicGoldResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicGoldHeritageResistance,
        Guids.DraconicGoldHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicGoldHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicGoldResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicGoldResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicGoldResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicGold9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicGoldHeritageBreathAbility,
        abilityGuid: Guids.DraconicGoldHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicGoldBreathWeaponAbility.Reference.Get(),
        featureName: DraconicGoldHeritageBreath,
        featureGuid: Guids.DraconicGoldHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicGoldBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicGoldHeritage,
        extraUse: FeatureRefs.BloodlineDraconicGoldBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicGoldHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicGold15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicGold.Reference.Get();
      return FeatureConfigurator.New(DraconicGoldHeritageWings, Guids.DraconicGoldHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicGoldHeritageBreath, Guids.DraconicGoldHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Green Dragon
    private const string DraconicGreenHeritage = "EldrichHeritage.Draconic.Green";
    private const string DraconicGreenHeritageResistance = "EldrichHeritage.Draconic.Green.Resistance";
    private const string DraconicGreenHeritageBreath = "EldrichHeritage.Draconic.Green.Breath";
    private const string DraconicGreenHeritageBreathAbility = "EldrichHeritage.Draconic.Green.Breath.Ability";
    private const string DraconicGreenHeritageWings = "EldrichHeritage.Draconic.Green.Wings";

    private static BlueprintFeature ConfigureDraconicGreen1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicGreenProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicGreenHeritage,
        Guids.DraconicGreenHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicGreenClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicGreenClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicGreenClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicGreenClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicGreen3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicGreenResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicGreenHeritageResistance,
        Guids.DraconicGreenHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicGreenHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicGreenResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicGreenResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicGreenResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicGreen9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicGreenHeritageBreathAbility,
        abilityGuid: Guids.DraconicGreenHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicGreenBreathWeaponAbility.Reference.Get(),
        featureName: DraconicGreenHeritageBreath,
        featureGuid: Guids.DraconicGreenHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicGreenBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicGreenHeritage,
        extraUse: FeatureRefs.BloodlineDraconicGreenBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicGreenHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicGreen15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicGreen.Reference.Get();
      return FeatureConfigurator.New(DraconicGreenHeritageWings, Guids.DraconicGreenHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicGreenHeritageBreath, Guids.DraconicGreenHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Red Dragon
    private const string DraconicRedHeritage = "EldrichHeritage.Draconic.Red";
    private const string DraconicRedHeritageResistance = "EldrichHeritage.Draconic.Red.Resistance";
    private const string DraconicRedHeritageBreath = "EldrichHeritage.Draconic.Red.Breath";
    private const string DraconicRedHeritageBreathAbility = "EldrichHeritage.Draconic.Red.Breath.Ability";
    private const string DraconicRedHeritageWings = "EldrichHeritage.Draconic.Red.Wings";

    private static BlueprintFeature ConfigureDraconicRed1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicRedProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicRedHeritage,
        Guids.DraconicRedHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicRedClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicRedClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicRedClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicRedClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicRed3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicRedResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicRedHeritageResistance,
        Guids.DraconicRedHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicRedHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicRedResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicRedResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicRedResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicRed9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicRedHeritageBreathAbility,
        abilityGuid: Guids.DraconicRedHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicRedBreathWeaponAbility.Reference.Get(),
        featureName: DraconicRedHeritageBreath,
        featureGuid: Guids.DraconicRedHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicRedBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicRedHeritage,
        extraUse: FeatureRefs.BloodlineDraconicRedBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicRedHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicRed15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicRed.Reference.Get();
      return FeatureConfigurator.New(DraconicRedHeritageWings, Guids.DraconicRedHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicRedHeritageBreath, Guids.DraconicRedHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region Silver Dragon
    private const string DraconicSilverHeritage = "EldrichHeritage.Draconic.Silver";
    private const string DraconicSilverHeritageResistance = "EldrichHeritage.Draconic.Silver.Resistance";
    private const string DraconicSilverHeritageBreath = "EldrichHeritage.Draconic.Silver.Breath";
    private const string DraconicSilverHeritageBreathAbility = "EldrichHeritage.Draconic.Silver.Breath.Ability";
    private const string DraconicSilverHeritageWings = "EldrichHeritage.Draconic.Silver.Wings";

    private static BlueprintFeature ConfigureDraconicSilver1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicSilverProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicSilverHeritage,
        Guids.DraconicSilverHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicSilverClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicSilverClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicSilverClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicSilverClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicSilver3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicSilverResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicSilverHeritageResistance,
        Guids.DraconicSilverHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicSilverHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicSilverResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicSilverResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicSilverResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicSilver9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicSilverHeritageBreathAbility,
        abilityGuid: Guids.DraconicSilverHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicSilverBreathWeaponAbility.Reference.Get(),
        featureName: DraconicSilverHeritageBreath,
        featureGuid: Guids.DraconicSilverHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicSilverBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicSilverHeritage,
        extraUse: FeatureRefs.BloodlineDraconicSilverBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicSilverHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicSilver15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicSilver.Reference.Get();
      return FeatureConfigurator.New(DraconicSilverHeritageWings, Guids.DraconicSilverHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicSilverHeritageBreath, Guids.DraconicSilverHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    #region White Dragon
    private const string DraconicWhiteHeritage = "EldrichHeritage.Draconic.White";
    private const string DraconicWhiteHeritageResistance = "EldrichHeritage.Draconic.White.Resistance";
    private const string DraconicWhiteHeritageBreath = "EldrichHeritage.Draconic.White.Breath";
    private const string DraconicWhiteHeritageBreathAbility = "EldrichHeritage.Draconic.White.Breath.Ability";
    private const string DraconicWhiteHeritageWings = "EldrichHeritage.Draconic.White.Wings";

    private static BlueprintFeature ConfigureDraconicWhite1()
    {
      var draconicBloodline = ProgressionRefs.BloodlineDraconicWhiteProgression.Reference.Get();
      return AddDraconicClaws(
        DraconicWhiteHeritage,
        Guids.DraconicWhiteHeritage,
        draconicBloodline,
        level3Claw: FeatureRefs.BloodlineDraconicWhiteClawsFeatureLevel1.Cast<BlueprintFeatureReference>().Reference,
        level7Claw: FeatureRefs.BloodlineDraconicWhiteClawsFeatureLevel2.Cast<BlueprintFeatureReference>().Reference,
        level9Claw: FeatureRefs.BloodlineDraconicWhiteClawsFeatureLevel3.Cast<BlueprintFeatureReference>().Reference,
        level13Claw: FeatureRefs.BloodlineDraconicWhiteClawsFeatureLevel4.Cast<BlueprintFeatureReference>().Reference);
    }

    private static BlueprintFeature ConfigureDraconicWhite3()
    {
      var draconicResistances = FeatureRefs.BloodlineDraconicWhiteResistancesAbilityAddLevel1.Reference.Get();
      return AddFeaturesByLevel(
        DraconicWhiteHeritageResistance,
        Guids.DraconicWhiteHeritageResistance,
        draconicResistances,
        prerequisites: new() { DraconicWhiteHeritage },
        featuresByLevel:
          new()
          {
            (FeatureRefs.BloodlineDraconicWhiteResistancesAbilityLevel1.Cast<BlueprintFeatureReference>().Reference, level: 5),
            (FeatureRefs.BloodlineDraconicWhiteResistancesAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 11),
            (FeatureRefs.BloodlineDraconicWhiteResistancesAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17),
          });
    }

    private static BlueprintFeature ConfigureDraconicWhite9()
    {
      return ConfigureDraconicBreath(
        abilityName: DraconicWhiteHeritageBreathAbility,
        abilityGuid: Guids.DraconicWhiteHeritageBreathAbility,
        breath: AbilityRefs.BloodlineDraconicWhiteBreathWeaponAbility.Reference.Get(),
        featureName: DraconicWhiteHeritageBreath,
        featureGuid: Guids.DraconicWhiteHeritageBreath,
        breathFeature: FeatureRefs.BloodlineDraconicWhiteBreathWeaponFeature.Reference.Get(),
        prerequisite: Guids.DraconicWhiteHeritage,
        extraUse: FeatureRefs.BloodlineDraconicWhiteBreathWeaponExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.DraconicWhiteHeritageWings);
    }

    private static BlueprintFeature ConfigureDraconicWhite15()
    {
      var wings = FeatureRefs.FeatureWingsDraconicWhite.Reference.Get();
      return FeatureConfigurator.New(DraconicWhiteHeritageWings, Guids.DraconicWhiteHeritageWings)
        .SetDisplayName(wings.m_DisplayName)
        .SetDescription(wings.m_Description)
        .SetIcon(wings.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeaturesFromList(
          new() { Guids.DraconicWhiteHeritageBreath, Guids.DraconicWhiteHeritageResistance }, amount: 1)
        .AddFacts(new() { wings })
        .Configure();
    }
    #endregion

    private static BlueprintFeature AddDraconicClaws(
      string name,
      string guid,
      BlueprintFeature sourceFeature,
      BlueprintFeatureReference level3Claw,
      BlueprintFeatureReference level7Claw,
      BlueprintFeatureReference level9Claw,
      BlueprintFeatureReference level13Claw)
    {
      return AddClaws(
        name,
        guid,
        sourceFeature,
        prereq: FeatureRefs.SkillFocusPerception.ToString(),
        excludePrereqs: new() { FeatureRefs.BloodlineDraconicClassSkill.ToString(), Guids.DraconicHeritage },
        resource: AbilityResourceRefs.BloodlineDraconicClawsResource.ToString(),
        level3Claw,
        level7Claw,
        level9Claw,
        level13Claw,
        extraFact: Guids.DraconicHeritage);
    }

    private static BlueprintFeature ConfigureDraconicBreath(
      string abilityName,
      string abilityGuid,
      BlueprintAbility breath,
      string featureName,
      string featureGuid,
      BlueprintFeature breathFeature,
      string prerequisite,
      BlueprintFeatureReference extraUse,
      string greaterFeatureGuid)
    {
      return AddBlast(
        abilityName,
        abilityGuid,
        sourceAbility: breath,
        featureName,
        featureGuid,
        sourceFeature: breathFeature,
        prerequisite,
        resource: AbilityResourceRefs.BloodlineDraconicBreathWeaponResource.ToString(),
        extraUse,
        greaterFeatureGuid);
    }
    #endregion

    #region Elemental
    private const string ElementalHeritage = "EldritchHeritage.Elemental";
    private const string ElementalHeritageDisplayName = "ElementalHeritage.Name";
    private const string ElementalHeritageDescription = "ElementalHeritage.Description";

    #region Air
    private const string ElementalAirHeritage = "EldritchHeritage.Elemental.Air";
    private const string ElementalAirHeritageRay = "EldritchHeritage.Elemental.Air.Ray";
    private const string ElementalAirHeritageResistance = "EldritchHeritage.Elemental.Air.Resistance";
    private const string ElementalAirHeritageBlast = "EldritchHeritage.Elemental.Air.Blast";
    private const string ElementalAirHeritageBlastAbility = "EldritchHeritage.Elemental.Air.Blast.Ability";
    private const string ElementalAirHeritageMovement = "EldritchHeritage.Elemental.Air.Movement";

    private static BlueprintFeature ConfigureElementalAir1()
    {
      return AddElementalRay(
        abilityName: ElementalAirHeritageRay,
        abilityGuid: Guids.ElementalAirHeritageRay,
        sourceAbility: AbilityRefs.BloodlineElementalAirElementalRayAbility.Reference.Get(),
        featureName: ElementalAirHeritage,
        featureGuid: Guids.ElementalAirHeritage,
        sourceFeature: ProgressionRefs.BloodlineElementalAirProgression.Reference.Get());
    }

    private static BlueprintFeature ConfigureElementalAir3()
    {
      var elementalResistance = FeatureRefs.BloodlineElementalAirResistanceAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        ElementalAirHeritageResistance,
        Guids.ElementalAirHeritageResistance,
        elementalResistance,
        prerequisites: new() { ElementalAirHeritage },
        featuresByLevel: new() { (elementalResistance.ToReference<BlueprintFeatureReference>(), 11) });
    }

    private static BlueprintFeature ConfigureElementalAir9()
    {
      return ConfigureElementalBlast(
        abilityName: ElementalAirHeritageBlastAbility,
        abilityGuid: Guids.ElementalAirHeritageBlastAbility,
        blast: AbilityRefs.BloodlineElementalAirElementalBlastAbility.Reference.Get(),
        featureName: ElementalAirHeritageBlast,
        featureGuid: Guids.ElementalAirHeritageBlast,
        blastFeature: FeatureRefs.BloodlineElementalAirElementalBlastFeature.Reference.Get(),
        prerequisite: Guids.ElementalAirHeritage,
        extraUse: FeatureRefs.BloodlineElementalAirElementalBlastExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.ElementalAirHeritageMovement);
    }

    private static BlueprintFeature ConfigureElementalAir15()
    {
      var elementalMovement = FeatureRefs.BloodlineElementalAirElementalMovementFeature.Reference.Get();
      return AddFeaturesByLevel(
        ElementalAirHeritageMovement,
        Guids.ElementalAirHeritageMovement,
        elementalMovement,
        prerequisites: new() { Guids.ElementalAirHeritageBlast, Guids.ElementalAirHeritageResistance },
        featuresByLevel: new() { (elementalMovement.ToReference<BlueprintFeatureReference>(), 11) });
    }
    #endregion

    #region Earth
    private const string ElementalEarthHeritage = "EldritchHeritage.Elemental.Earth";
    private const string ElementalEarthHeritageRay = "EldritchHeritage.Elemental.Earth.Ray";
    private const string ElementalEarthHeritageResistance = "EldritchHeritage.Elemental.Earth.Resistance";
    private const string ElementalEarthHeritageBlast = "EldritchHeritage.Elemental.Earth.Blast";
    private const string ElementalEarthHeritageBlastAbility = "EldritchHeritage.Elemental.Earth.Blast.Ability";
    private const string ElementalEarthHeritageMovement = "EldritchHeritage.Elemental.Earth.Movement";

    private static BlueprintFeature ConfigureElementalEarth1()
    {
      return AddElementalRay(
        abilityName: ElementalEarthHeritageRay,
        abilityGuid: Guids.ElementalEarthHeritageRay,
        sourceAbility: AbilityRefs.BloodlineElementalEarthElementalRayAbility.Reference.Get(),
        featureName: ElementalEarthHeritage,
        featureGuid: Guids.ElementalEarthHeritage,
        sourceFeature: ProgressionRefs.BloodlineElementalEarthProgression.Reference.Get());
    }

    private static BlueprintFeature ConfigureElementalEarth3()
    {
      var elementalResistance = FeatureRefs.BloodlineElementalEarthResistanceAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        ElementalEarthHeritageResistance,
        Guids.ElementalEarthHeritageResistance,
        elementalResistance,
        prerequisites: new() { ElementalEarthHeritage },
        featuresByLevel: new() { (elementalResistance.ToReference<BlueprintFeatureReference>(), 11) });
    }

    private static BlueprintFeature ConfigureElementalEarth9()
    {
      return ConfigureElementalBlast(
        abilityName: ElementalEarthHeritageBlastAbility,
        abilityGuid: Guids.ElementalEarthHeritageBlastAbility,
        blast: AbilityRefs.BloodlineElementalEarthElementalBlastAbility.Reference.Get(),
        featureName: ElementalEarthHeritageBlast,
        featureGuid: Guids.ElementalEarthHeritageBlast,
        blastFeature: FeatureRefs.BloodlineElementalEarthElementalBlastFeature.Reference.Get(),
        prerequisite: Guids.ElementalEarthHeritage,
        extraUse: FeatureRefs.BloodlineElementalEarthElementalBlastExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.ElementalEarthHeritageMovement);
    }

    private static BlueprintFeature ConfigureElementalEarth15()
    {
      var elementalMovement = FeatureRefs.BloodlineElementalEarthElementalMovementFeature.Reference.Get();
      return AddFeaturesByLevel(
        ElementalEarthHeritageMovement,
        Guids.ElementalEarthHeritageMovement,
        elementalMovement,
        prerequisites: new() { Guids.ElementalEarthHeritageBlast, Guids.ElementalEarthHeritageResistance },
        featuresByLevel: new() { (elementalMovement.ToReference<BlueprintFeatureReference>(), 11) });
    }
    #endregion

    #region Fire
    private const string ElementalFireHeritage = "EldritchHeritage.Elemental.Fire";
    private const string ElementalFireHeritageRay = "EldritchHeritage.Elemental.Fire.Ray";
    private const string ElementalFireHeritageResistance = "EldritchHeritage.Elemental.Fire.Resistance";
    private const string ElementalFireHeritageBlast = "EldritchHeritage.Elemental.Fire.Blast";
    private const string ElementalFireHeritageBlastAbility = "EldritchHeritage.Elemental.Fire.Blast.Ability";
    private const string ElementalFireHeritageMovement = "EldritchHeritage.Elemental.Fire.Movement";

    private static BlueprintFeature ConfigureElementalFire1()
    {
      return AddElementalRay(
        abilityName: ElementalFireHeritageRay,
        abilityGuid: Guids.ElementalFireHeritageRay,
        sourceAbility: AbilityRefs.BloodlineElementalFireElementalRayAbility.Reference.Get(),
        featureName: ElementalFireHeritage,
        featureGuid: Guids.ElementalFireHeritage,
        sourceFeature: ProgressionRefs.BloodlineElementalFireProgression.Reference.Get());
    }

    private static BlueprintFeature ConfigureElementalFire3()
    {
      var elementalResistance = FeatureRefs.BloodlineElementalFireResistanceAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        ElementalFireHeritageResistance,
        Guids.ElementalFireHeritageResistance,
        elementalResistance,
        prerequisites: new() { ElementalFireHeritage },
        featuresByLevel: new() { (elementalResistance.ToReference<BlueprintFeatureReference>(), 11) });
    }

    private static BlueprintFeature ConfigureElementalFire9()
    {
      return ConfigureElementalBlast(
        abilityName: ElementalFireHeritageBlastAbility,
        abilityGuid: Guids.ElementalFireHeritageBlastAbility,
        blast: AbilityRefs.BloodlineElementalFireElementalBlastAbility.Reference.Get(),
        featureName: ElementalFireHeritageBlast,
        featureGuid: Guids.ElementalFireHeritageBlast,
        blastFeature: FeatureRefs.BloodlineElementalFireElementalBlastFeature.Reference.Get(),
        prerequisite: Guids.ElementalFireHeritage,
        extraUse: FeatureRefs.BloodlineElementalFireElementalBlastExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.ElementalFireHeritageMovement);
    }

    private static BlueprintFeature ConfigureElementalFire15()
    {
      var elementalMovement = FeatureRefs.BloodlineElementalFireElementalMovementFeature.Reference.Get();
      return AddFeaturesByLevel(
        ElementalFireHeritageMovement,
        Guids.ElementalFireHeritageMovement,
        elementalMovement,
        prerequisites: new() { Guids.ElementalFireHeritageBlast, Guids.ElementalFireHeritageResistance },
        featuresByLevel: new() { (elementalMovement.ToReference<BlueprintFeatureReference>(), 11) });
    }
    #endregion

    #region Water
    private const string ElementalWaterHeritage = "EldritchHeritage.Elemental.Water";
    private const string ElementalWaterHeritageRay = "EldritchHeritage.Elemental.Water.Ray";
    private const string ElementalWaterHeritageResistance = "EldritchHeritage.Elemental.Water.Resistance";
    private const string ElementalWaterHeritageBlast = "EldritchHeritage.Elemental.Water.Blast";
    private const string ElementalWaterHeritageBlastAbility = "EldritchHeritage.Elemental.Water.Blast.Ability";
    private const string ElementalWaterHeritageMovement = "EldritchHeritage.Elemental.Water.Movement";

    private static BlueprintFeature ConfigureElementalWater1()
    {
      return AddElementalRay(
        abilityName: ElementalWaterHeritageRay,
        abilityGuid: Guids.ElementalWaterHeritageRay,
        sourceAbility: AbilityRefs.BloodlineElementalWaterElementalRayAbility.Reference.Get(),
        featureName: ElementalWaterHeritage,
        featureGuid: Guids.ElementalWaterHeritage,
        sourceFeature: ProgressionRefs.BloodlineElementalWaterProgression.Reference.Get());
    }

    private static BlueprintFeature ConfigureElementalWater3()
    {
      var elementalResistance = FeatureRefs.BloodlineElementalWaterResistanceAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        ElementalWaterHeritageResistance,
        Guids.ElementalWaterHeritageResistance,
        elementalResistance,
        prerequisites: new() { ElementalWaterHeritage },
        featuresByLevel: new() { (elementalResistance.ToReference<BlueprintFeatureReference>(), 11) });
    }

    private static BlueprintFeature ConfigureElementalWater9()
    {
      return ConfigureElementalBlast(
        abilityName: ElementalWaterHeritageBlastAbility,
        abilityGuid: Guids.ElementalWaterHeritageBlastAbility,
        blast: AbilityRefs.BloodlineElementalWaterElementalBlastAbility.Reference.Get(),
        featureName: ElementalWaterHeritageBlast,
        featureGuid: Guids.ElementalWaterHeritageBlast,
        blastFeature: FeatureRefs.BloodlineElementalWaterElementalBlastFeature.Reference.Get(),
        prerequisite: Guids.ElementalWaterHeritage,
        extraUse: FeatureRefs.BloodlineElementalWaterElementalBlastExtraUse.Cast<BlueprintFeatureReference>().Reference,
        greaterFeatureGuid: Guids.ElementalWaterHeritageMovement);
    }

    private static BlueprintFeature ConfigureElementalWater15()
    {
      var elementalMovement = FeatureRefs.BloodlineElementalWaterElementalMovementFeature.Reference.Get();
      return AddFeaturesByLevel(
        ElementalWaterHeritageMovement,
        Guids.ElementalWaterHeritageMovement,
        elementalMovement,
        prerequisites: new() { Guids.ElementalWaterHeritageBlast, Guids.ElementalWaterHeritageResistance },
        featuresByLevel: new() { (elementalMovement.ToReference<BlueprintFeatureReference>(), 11) });
    }
    #endregion

    private static BlueprintFeature AddElementalRay(
      string abilityName,
      string abilityGuid,
      BlueprintAbility sourceAbility,
      string featureName,
      string featureGuid,
      BlueprintFeature sourceFeature)
    {
      return AddRay(
        abilityName,
        abilityGuid,
        sourceAbility,
        featureName,
        featureGuid,
        sourceFeature,
        FeatureRefs.SkillFocusAcrobatics.ToString(),
        new() { FeatureRefs.BloodlineElementalClassSkill.ToString(), ElementalHeritage },
        AbilityResourceRefs.BloodlineElementalElementalRayResource.ToString());
    }

    private static BlueprintFeature ConfigureElementalBlast(
      string abilityName,
      string abilityGuid,
      BlueprintAbility blast,
      string featureName,
      string featureGuid,
      BlueprintFeature blastFeature,
      string prerequisite,
      BlueprintFeatureReference extraUse,
      string greaterFeatureGuid)
    {
      return AddBlast(
        abilityName,
        abilityGuid,
        sourceAbility: blast,
        featureName,
        featureGuid,
        sourceFeature: blastFeature,
        prerequisite,
        resource: AbilityResourceRefs.BloodlineElementalElementalBlastResource.ToString(),
        extraUse,
        greaterFeatureGuid,
        AbilityRankType.DamageDice);
    }
    #endregion

    #region Fey
    private const string FeyHeritageName = "EldrichHeritage.Fey";

    private const string FeyHeritageResistance = "EldritchHeritage.Fey.Resistance";
    private const string FeyHeritageStrength = "EldritchHeritage.Fey.Strength";
    private const string FeyHeritageSummons = "EldritchHeritage.Fey.Summons";

    private static BlueprintFeature ConfigureFeyHeritage1()
    {
      var feyBloodline = ProgressionRefs.BloodlineFeyProgression.Reference.Get();
      return FeatureConfigurator.New(FeyHeritageName, Guids.FeyHeritage)
        .SetDisplayName(feyBloodline.m_DisplayName)
        .SetDescription(feyBloodline.m_Description)
        .SetIcon(feyBloodline.Icon)
        .AddPrerequisiteFeature(FeatureRefs.SkillFocusLoreNature.ToString())
        .AddPrerequisiteNoFeature(FeatureRefs.FeyBloodlineRequisiteFeature.ToString())
        .AddFacts(new() { FeatureRefs.BloodlineFeyLaughingTouchFeature.ToString() })
        .Configure();
    }

    private static BlueprintFeature ConfigureFeyHeritage3()
    {
      var FeyResistance = FeatureRefs.BloodlineFeyResistancesAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        FeyHeritageResistance,
        Guids.FeyHeritageResistance,
        FeyResistance,
        new() { FeyHeritageName },
        new() { (FeyResistance.ToReference<BlueprintFeatureReference>(), level: 11) });
    }

    private static BlueprintFeature ConfigureFeyHeritage9()
    {
      var FeyStrength = FeatureRefs.BloodlineFeyStrengthAbilityLevel1.Reference.Get();
      return AddFeaturesByLevel(
        FeyHeritageStrength,
        Guids.FeyHeritageStrength,
        FeyStrength,
        new() { FeyHeritageName },
        new()
        {
          (FeyStrength.ToReference<BlueprintFeatureReference>(), level: 11),
          (FeatureRefs.BloodlineFeyStrengthAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 15),
          (FeatureRefs.BloodlineFeyStrengthAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 19)
        },
        BlueprintTool.GetRef<BlueprintFeatureReference>(Guids.FeyHeritageSummons),
        new()
        {
          (FeyStrength.ToReference<BlueprintFeatureReference>(), level: 9),
          (FeatureRefs.BloodlineFeyStrengthAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 13),
          (FeatureRefs.BloodlineFeyStrengthAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 17)
        });
    }

    private static BlueprintFeature ConfigureFeyHeritage15()
    {
      var FeySummons = FeatureRefs.BloodlineFeyAddedSummonings.Reference.Get();
      return AddFeaturesByLevel(
        FeyHeritageSummons,
        Guids.FeyHeritageSummons,
        FeySummons,
        prerequisites: new() { Guids.FeyHeritageStrength, Guids.FeyHeritageResistance },
        featuresByLevel: new() { (FeySummons.ToReference<BlueprintFeatureReference>(), 15) });
    }
    #endregion

    // For bloodline abilities that add a feature which is replaced by a higher level feature later.
    // Greater enables the change from Level - 2 to Level when Greater Eldritch Heritage is acquired.
    private static BlueprintFeature AddFeaturesByLevel(
      string name,
      string guid,
      BlueprintFeature sourceFeature,
      List<Blueprint<BlueprintFeatureReference>> prerequisites,
      List<(BlueprintFeatureReference feature, int level)> featuresByLevel,
      BlueprintFeatureReference greaterFeature = null,
      List<(BlueprintFeatureReference feature, int level)> greaterFeatureLevels = null)
    {
      return FeatureConfigurator.New(name, guid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp(true)
        .AddComponent(
          new ApplyFeatureOnCharacterLevel(
            featuresByLevel.ToList(),
            greaterFeature: greaterFeature,
            greaterFeatureLevels: greaterFeatureLevels))
        .AddPrerequisiteFeaturesFromList(prerequisites, amount: 1)
        .Configure();
    }

    private static BlueprintFeature AddClaws(
      string name,
      string guid,
      BlueprintFeature sourceFeature,
      string prereq,
      List<string> excludePrereqs,
      string resource,
      BlueprintFeatureReference level3Claw,
      BlueprintFeatureReference level7Claw,
      BlueprintFeatureReference level9Claw,
      BlueprintFeatureReference level13Claw,
      string extraFact = "")
    {
      var claws = FeatureConfigurator.New(name, guid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.m_Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddPrerequisiteFeature(prereq)
        .AddComponent(
          new ApplyFeatureOnCharacterLevel(
            new() {
              (level3Claw, level: 3),
              (level7Claw, level: 7),
              (level9Claw, level: 9),
              (level13Claw, level: 13),
            }))
        .AddAbilityResources(resource: resource, restoreAmount: true);

      foreach (var exclude in excludePrereqs)
        claws.AddPrerequisiteNoFeature(exclude);

      if (!string.IsNullOrEmpty(extraFact))
        claws.AddFacts(new() { extraFact });

      return claws.Configure();
    }

    private static BlueprintFeature AddRay(
      string abilityName,
      string abilityGuid,
      BlueprintAbility sourceAbility,
      string featureName,
      string featureGuid,
      BlueprintFeature sourceFeature,
      string prereq,
      List<string> excludePrereqs,
      string resource,
      params Type[] extraComponents)
    {
      var ray = AbilityConfigurator.New(abilityName, abilityGuid)
        .SetDisplayName(sourceAbility.m_DisplayName)
        .SetDescription(sourceAbility.m_Description)
        .SetIcon(sourceAbility.Icon)
        .SetType(AbilityType.SpellLike)
        .SetCustomRange(30)
        .SetCanTargetEnemies(sourceAbility.CanTargetEnemies)
        .SetCanTargetFriends(sourceAbility.CanTargetFriends)
        .SetCanTargetSelf(sourceAbility.CanTargetSelf)
        .SetCanTargetPoint(sourceAbility.CanTargetPoint)
        .SetEffectOnAlly(sourceAbility.EffectOnAlly)
        .SetEffectOnEnemy(sourceAbility.EffectOnEnemy)
        .SetAnimation(sourceAbility.Animation)
        .SetActionType(sourceAbility.ActionType)
        .SetAvailableMetamagic(sourceAbility.AvailableMetamagic)
        .AddComponent(sourceAbility.GetComponent<SpellComponent>())
        .AddComponent(sourceAbility.GetComponent<AbilityDeliverProjectile>())
        .AddComponent(sourceAbility.GetComponent<AbilityEffectRunAction>())
        .AddComponent(sourceAbility.GetComponent<AbilityResourceLogic>())
        .AddComponent(sourceAbility.GetComponent<SpellDescriptorComponent>())
        .AddContextRankConfig(
          ContextRankConfigs.CustomProperty(
              EffectiveLevelProperty, type: AbilityRankType.DamageBonus, min: 0, max: 20)
            .WithDiv2Progression());

      foreach (var type in extraComponents)
        ray.AddComponent(sourceAbility.ComponentsArray.Where(c => c.GetType() == type).FirstOrDefault());

      var feature = FeatureConfigurator.New(featureName, featureGuid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(prereq)
        .AddFacts(new() { ray.Configure(delayed: true) })
        .AddAbilityResources(resource: resource, restoreAmount: true);

      foreach (var exclude in excludePrereqs)
        feature.AddPrerequisiteNoFeature(exclude);

      return feature.Configure();
    }

    private static BlueprintFeature AddBlast(
      string abilityName,
      string abilityGuid,
      BlueprintAbility sourceAbility,
      string featureName,
      string featureGuid,
      BlueprintFeature sourceFeature,
      string prerequisite,
      string resource,
      BlueprintFeatureReference extraUse,
      string greaterFeatureGuid,
      AbilityRankType rankType = AbilityRankType.Default)
    {
      var abilityBuilder = AbilityConfigurator.New(abilityName, abilityGuid)
        .SetDisplayName(sourceAbility.m_DisplayName)
        .SetDescription(sourceAbility.m_Description)
        .SetIcon(sourceAbility.Icon)
        .SetType(sourceAbility.Type)
        .SetCanTargetEnemies(sourceAbility.CanTargetEnemies)
        .SetCanTargetFriends(sourceAbility.CanTargetFriends)
        .SetCanTargetSelf(sourceAbility.CanTargetSelf)
        .SetCanTargetPoint(sourceAbility.CanTargetPoint)
        .SetEffectOnEnemy(sourceAbility.EffectOnEnemy)
        .SetAnimation(sourceAbility.Animation)
        .SetActionType(sourceAbility.ActionType)
        .SetAvailableMetamagic(sourceAbility.AvailableMetamagic)
        .SetLocalizedSavingThrow(sourceAbility.LocalizedSavingThrow)
        .AddComponent(sourceAbility.GetComponent<AbilityEffectRunAction>())
        .AddComponent(sourceAbility.GetComponent<SpellDescriptorComponent>())
        .AddComponent(sourceAbility.GetComponent<AbilityResourceLogic>())
        .AddContextRankConfig(ContextRankConfigs.CustomProperty(EffectiveLevelProperty, max: 20, type: rankType));

      // Draconic & Elemental have slightly different sets of components
      if (sourceAbility.GetComponent<AbilityDeliverProjectile>() is not null)
        abilityBuilder.AddComponent(sourceAbility.GetComponent<AbilityDeliverProjectile>());
      if (sourceAbility.GetComponent<SpellComponent>() is not null)
        abilityBuilder.AddComponent(sourceAbility.GetComponent<SpellComponent>());
      if (sourceAbility.GetComponent<AbilityTargetsAround>() is not null)
        abilityBuilder.AddComponent(sourceAbility.GetComponent<AbilityTargetsAround>());
      if (sourceAbility.GetComponent<AbilitySpawnFx>() is not null)
        abilityBuilder.AddComponent(sourceAbility.GetComponent<AbilitySpawnFx>());

      // Draconic & elemental have slightly different range logic
      if (sourceAbility.Range == AbilityRange.Custom)
        abilityBuilder.SetCustomRange(sourceAbility.CustomRange);
      else
        abilityBuilder.SetRange(sourceAbility.Range);

      var ability = abilityBuilder.Configure();
      return FeatureConfigurator.New(featureName, featureGuid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.Icon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddPrerequisiteFeature(prerequisite)
        .AddFacts(new() { ability })
        .AddAbilityResources(resource: resource, restoreAmount: true)
        .AddComponent(new BindToCharacterLevel(ability.ToReference<BlueprintAbilityReference>()))
        .AddComponent(
          new ApplyFeatureOnCharacterLevel(
            new() { (extraUse, level: 19) },
            greaterFeature: BlueprintTool.GetRef<BlueprintFeatureReference>(greaterFeatureGuid),
            greaterFeatureLevels: new() { (extraUse, level: 17), (extraUse, level: 20) }))
        .Configure();
    }
  }
}
