using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using System;
using static Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbilityResourceLogic;
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
      BuffConfigurator.New(AbyssalHeritageClawsBuff, Guids.AbyssalHeritageClawsBuff).Configure();
      ActivatableAbilityConfigurator.New(AbyssalHeritageClaws, Guids.AbyssalHeritageClawsAbility).Configure();
      FeatureConfigurator.New(AbyssalHeritageName, Guids.AbyssalHeritage).Configure();
      #endregion

      ParametrizedFeatureConfigurator.New(FeatName, Guids.EldritchHeritageFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(Icon)
        .Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      FeatureSelectionConfigurator.New(FeatName, Guids.EldritchHeritageFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(Icon)
        .SetIsClassFeature()
        .AddFeatureTagsComponent(featureTags: FeatureTag.Magic)
        .AddPrerequisiteStatValue(StatType.Charisma, 13)
        .AddPrerequisiteCharacterLevel(3)
        .AddToAllFeatures(ConfigureAbyssalHeritage1())
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
        .AddToAllFeatures(ConfigureAbyssalHeritage3(), ConfigureAbyssalHeritage9())
        .Configure();

      // Since feature selection logic is only in FeatureConfigurator, do this instead of trying to do in parametrized
      // configurator.
      FeatureConfigurator.For(ImprovedFeatName).AddToGroups(FeatureGroup.Feat).Configure(delayed: true);
    }

    #region Abyssal
    private const string AbyssalHeritageName = "EldrichHeritage.Abyssal";

    private const string AbyssalHeritageClaws = "EldritchHeritage.Abyssal.Claws";
    private const string AbyssalHeritageClawsBuff = "EldritchHeritage.Abyssal.Claws.Buff";

    private const string AbyssalHeritageResistance = "EldritchHeritage.Abyssal.Resistance";

    private const string AbyssalHeritageStrength = "EldritchHeritage.Abyssal.Strength";

    private static BlueprintFeature ConfigureAbyssalHeritage1()
    {
      var abyssalClawsBuff = BuffRefs.BloodlineAbyssalClawsBuffLevel1.Reference.Get();
      var buff = BuffConfigurator.New(AbyssalHeritageClawsBuff, Guids.AbyssalHeritageClawsBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddComponent<AbyssalHeritageClawsComponent>()
        .Configure();

      var abyssalClaws = ActivatableAbilityRefs.BloodlineAbyssalClawsAbililyLevel1.Reference.Get();
      var ability = ActivatableAbilityConfigurator.New(AbyssalHeritageClaws, Guids.AbyssalHeritageClawsAbility)
        .SetDisplayName(abyssalClaws.m_DisplayName)
        .SetDescription(abyssalClaws.m_Description)
        .SetIcon(abyssalClaws.m_Icon)
        .SetDeactivateIfCombatEnded()
        .SetDeactivateIfOwnerDisabled()
        .SetDeactivateImmediately()
        .SetActivationType(AbilityActivationType.WithUnitCommand)
        .AddActivatableAbilityResourceLogic(
          spendType: ResourceSpendType.NewRound,
          requiredResource: AbilityResourceRefs.BloodlineAbyssalClawsResource.ToString())
        .SetBuff(buff)
        .Configure();

      var abyssalBloodline = ProgressionRefs.BloodlineAbyssalProgression.Reference.Get();
      return FeatureConfigurator.New(AbyssalHeritageName, Guids.AbyssalHeritage)
        .SetDisplayName(abyssalBloodline.m_DisplayName)
        .SetDescription(abyssalClaws.m_Description)
        .SetIcon(abyssalBloodline.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(FeatureRefs.SkillFocusPhysique.ToString())
        .AddPrerequisiteNoFeature(FeatureRefs.AbyssalBloodlineRequisiteFeature.ToString())
        .AddFacts(new() { ability })
        .AddAbilityResources(
          resource: AbilityResourceRefs.BloodlineAbyssalClawsResource.ToString(), restoreAmount: true)
        .Configure();
    }

    private static BlueprintFeature ConfigureAbyssalHeritage3()
    {
      var abyssalResistance = FeatureRefs.BloodlineAbyssalResistancesAbilityLevel2.Reference.Get();
      return AddFeaturesByLevel(
        AbyssalHeritageResistance,
        Guids.AbyssalHeritageResistance,
        abyssalResistance,
        AbyssalHeritageName,
        (abyssalResistance.ToReference<BlueprintFeatureReference>(), level: 11));
    }

    private static BlueprintFeature ConfigureAbyssalHeritage9()
    {
      var abyssalStrength = FeatureRefs.BloodlineAbyssalStrengthAbilityLevel1.Reference.Get();
      return AddFeaturesByLevel(
        AbyssalHeritageStrength,
        Guids.AbyssalHeritageStrength,
        abyssalStrength,
        AbyssalHeritageName,
        (abyssalStrength.ToReference<BlueprintFeatureReference>(), level: 11),
        (FeatureRefs.BloodlineAbyssalStrengthAbilityLevel2.Cast<BlueprintFeatureReference>().Reference, level: 15),
        (FeatureRefs.BloodlineAbyssalStrengthAbilityLevel3.Cast<BlueprintFeatureReference>().Reference, level: 19));
    }
    #endregion

    // For heritage which just directly adds a feature
    private static BlueprintFeature AddFeaturesByLevel(
      string name,
      string guid,
      BlueprintFeature sourceFeature,
      string prerequisite,
      params (BlueprintFeatureReference feature, int level)[] featuresByLevel)
    {
      return FeatureConfigurator.New(name, guid)
        .SetDisplayName(sourceFeature.m_DisplayName)
        .SetDescription(sourceFeature.m_Description)
        .SetIcon(sourceFeature.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(prerequisite)
        .AddComponent(new ApplyFeatureOnCharacterLevel(featuresByLevel))
        .Configure();
    }

    [TypeId("83224ddf-2f92-48c3-bf2d-9a8d26a5432e")]
    private class AbyssalHeritageClawsComponent : UnitBuffComponentDelegate
    {
      public override void OnActivate()
      {
        var characterLevel = Owner.Descriptor.Progression.CharacterLevel;
        Buff buff;
        if (characterLevel < 7)
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel1.Reference.Get(), Context);
        else if (characterLevel < 9)
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel2.Reference.Get(), Context);
        else if (characterLevel < 13)
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel3.Reference.Get(), Context);
        else
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel4.Reference.Get(), Context);

        // Links the buff to this one so they get removed at the same time
        Buff.StoreFact(buff);
      }
    }
  }
}
