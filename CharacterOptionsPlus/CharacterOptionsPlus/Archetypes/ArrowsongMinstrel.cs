using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Archetypes
{
  internal class ArrowsongMinstrel
  {
    private const string ArchetypeName = "ArrowsongMinstrel";

    internal const string ArchetypeDisplayName = "ArrowsongMinstrel.Name";
    private const string ArchetypeDescription = "ArrowsongMinstrel.Description";

    private const string SpellListName = "ArrowsongMinstrel.SpellList";

    private static readonly ModLogger Logger = Logging.GetLogger(ArchetypeName);

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "hurtful.png"; // TODO: Replace with new icon

    internal static void Configure()
    {
      if (Settings.IsEnabled(Guids.ArrowsingMinstrelArchetype))
        ConfigureEnabled();
      else
        ConfigureDisabled();
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {ArchetypeName} (disabled)");
      ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsingMinstrelArchetype)
        .SetLocalizedName(ArchetypeDisplayName)
        .SetLocalizedDescription(ArchetypeDescription)
        .Configure(); 
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {ArchetypeName}");

      var archetype =
        ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsingMinstrelArchetype, CharacterClassRefs.BardClass)
          .SetLocalizedName(ArchetypeDisplayName)
          .SetLocalizedDescription(ArchetypeDescription);

      // Remove features
      archetype
        .AddToRemoveFeatures(1, FeatureRefs.BardProficiencies.ToString(), FeatureRefs.BardicKnowledge.ToString())
        // Remove the bard talent which is a stand-in for Versatile Performance
        // Notably BardTalentSelection is the incorrect reference!
        .AddToRemoveFeatures(
          2, FeatureRefs.BardWellVersed.ToString(), FeatureSelectionRefs.BardTalentSelection_0.ToString())
        .AddToRemoveFeatures(5, FeatureRefs.BardLoreMaster.ToString())
        .AddToRemoveFeatures(6, FeatureRefs.FascinateFeature.ToString())
        .AddToRemoveFeatures(8, FeatureRefs.DirgeOfDoomFeature.ToString())
        .AddToRemoveFeatures(12, FeatureRefs.SoothingPerformanceFeature.ToString())
        .AddToRemoveFeatures(3, FeatureRefs.InspireCompetenceFeature.ToString()) // All Inspire Competence
        .AddToRemoveFeatures(7, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(11, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(15, FeatureRefs.InspireCompetenceFeature.ToString())
        .AddToRemoveFeatures(19, FeatureRefs.InspireCompetenceFeature.ToString());

     // TODO: Add features

      archetype.Configure();
    }

    private static readonly BlueprintSpellList WizardEvocationSpells =
      SpellListRefs.WizardEvocationSpellList.Reference.Get();
    private static BlueprintSpellList ConfigureArcaneArcherySpellSelection()
    {
      var firstLevelSpells = new SpellLevelList(1)
      {
        m_Spells =
        WizardEvocationSpells.GetSpells(1)
          .Select(s => s.ToReference<BlueprintAbilityReference>())
          .Append(AbilityRefs.MagicWeapon.Cast<BlueprintAbilityReference>().Reference)
          .Append(AbilityRefs.TrueStrike.Cast<BlueprintAbilityReference>().Reference)
          .ToList()
      };

      var secondLevelSpells = new SpellLevelList(2)
      {
        m_Spells =
        WizardEvocationSpells.GetSpells(2)
          .Select(s => s.ToReference<BlueprintAbilityReference>())
          .Append(AbilityRefs.ProtectionFromArrows.Cast<BlueprintAbilityReference>().Reference)
          .Append(AbilityRefs.AcidArrow.Cast<BlueprintAbilityReference>().Reference)
          .ToList()
      };

      var thirdLevelSpells = new SpellLevelList(3)
      {
        m_Spells =
        WizardEvocationSpells.GetSpells(3)
          .Select(s => s.ToReference<BlueprintAbilityReference>())
          .Append(AbilityRefs.MagicWeaponGreater.Cast<BlueprintAbilityReference>().Reference)
          .Append(AbilityRefs.ProtectionFromArrowsCommunal.Cast<BlueprintAbilityReference>().Reference)
          .ToList()
      };

      var fourthLevelSpells = new SpellLevelList(4)
      {
        m_Spells = WizardEvocationSpells.GetSpells(4).Select(s => s.ToReference<BlueprintAbilityReference>()).ToList()
      };

      var fifthLevelSpells = new SpellLevelList(5)
      {
        m_Spells = WizardEvocationSpells.GetSpells(5).Select(s => s.ToReference<BlueprintAbilityReference>()).ToList()
      };

      var sixthLevelSpells = new SpellLevelList(6)
      {
        m_Spells = WizardEvocationSpells.GetSpells(6).Select(s => s.ToReference<BlueprintAbilityReference>()).ToList()
      };

      return
        SpellListConfigurator.New(SpellListName, Guids.ArrowsingMinstrelSpellList)
          .AddToSpellsByLevel(
            firstLevelSpells,
            secondLevelSpells,
            thirdLevelSpells,
            fourthLevelSpells,
            fifthLevelSpells,
            sixthLevelSpells)
          .Configure();
    }
  }
}
