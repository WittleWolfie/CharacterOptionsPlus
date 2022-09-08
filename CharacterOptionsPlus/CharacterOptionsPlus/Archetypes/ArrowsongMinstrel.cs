using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Linq;
using TabletopTweaks.Core.NewComponents;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Archetypes
{
  internal class ArrowsongMinstrel
  {
    private const string ArchetypeName = "ArrowsongMinstrel";

    internal const string ArchetypeDisplayName = "ArrowsongMinstrel.Name";
    private const string ArchetypeDescription = "ArrowsongMinstrel.Description";

    private const string ProficienciesDisplayName = "ArrowsongMinstrel.Proficiencies";
    private const string ProficienciesDescription = "ArrowsongMinstrel.Proficiencies.Description";

    private const string ArcaneArchery = "ArrowsingMinstrel.ArcaneArchery";

    private const string Proficiencies = "ArrowsongMinstrel.Proficiencies";
    private const string Spellbook = "ArrowsongMinstrel.Spellbook";
    private const string SpellList = "ArrowsongMinstrel.SpellList";
    private const string SpellsPerDay = "ArrowsongMinstrel.SpellsPerDay";
    
    private const string SpellSelection = "ArrowsongMinstrel.SpellSelection";
    private const string SpellSelectionName = "ArrowsongMinstrel.ArcaneArchery.BonusSpells";
    private const string SpellSelectionDescription = "ArrowsongMinstrel.ArcaneArchery.BonusSpells.Description";

    private static readonly ModLogger Logger = Logging.GetLogger(ArchetypeName);

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "hurtful.png"; // TODO: Replace with new icon

    internal static void Configure()
    {
      if (Settings.IsEnabled(Guids.ArrowsongMinstrelArchetype))
        ConfigureEnabled();
      else
        ConfigureDisabled();
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {ArchetypeName} (disabled)");
      ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsongMinstrelArchetype)
        .SetLocalizedName(ArchetypeDisplayName)
        .SetLocalizedDescription(ArchetypeDescription)
        .Configure(); 
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {ArchetypeName}");

      var archetype =
        ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsongMinstrelArchetype, CharacterClassRefs.BardClass)
          .SetLocalizedName(ArchetypeDisplayName)
          .SetLocalizedDescription(ArchetypeDescription)
          .SetReplaceSpellbook(CreateSpellbook());

      // Remove features
      archetype
        .AddToRemoveFeatures(1, FeatureRefs.BardProficiencies.ToString(), FeatureRefs.BardicKnowledge.ToString())
        // Remove the bard talent which is a stand-in for Versatile Performance
        // Notably BardTalentSelection is the incorrect reference, gotta have _0!
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

 
      archetype
        .AddToAddFeatures(1, CreateArcaneArchery(), CreateProficiencies()) // TODO: Bonus spell selection
        .AddToAddFeatures(2, FeatureRefs.PreciseShot.ToString());
      // TODO: Arrowsong Strike at 6
      // TODO: Full-attack Arrowsong Strike at 18
      // TODO: Bonus spell selection at 4 / 8 / 12 / 16 / 20

      archetype.Configure();
    }

    // TODO: Icon!
    private static BlueprintFeature CreateProficiencies()
    {
      var bardProficiencies = FeatureRefs.BardProficiencies.Reference.Get();
      return FeatureConfigurator.New(Proficiencies, Guids.ArrowsongMinstrelProficiencies)
        .SetDisplayName(ProficienciesDisplayName)
        .SetDescription(ProficienciesDescription)
        .SetIsClassFeature(true)
        .AddComponent(bardProficiencies.GetComponent<AddFacts>())
        .AddComponent(bardProficiencies.GetComponent<ArcaneArmorProficiency>())
        .AddProficiencies(
          weaponProficiencies:
            new WeaponCategory[]
            {
              WeaponCategory.Longbow,
              WeaponCategory.Shortbow,
              WeaponCategory.Shortsword,
            })
        .Configure();
    }

    private static BlueprintSpellbook CreateSpellbook()
    {
      return SpellbookConfigurator.New(Spellbook, Guids.ArrowsongMinstrelSpellbook)
        .SetName(ArchetypeDisplayName)
        .SetSpellsPerDay(GetSpellSlots())
        .SetSpellsKnown(SpellsTableRefs.BardSpellsKnownTable.ToString())
        .SetSpellList(SpellListRefs.BardSpellList.ToString())
        .SetCharacterClass(CharacterClassRefs.BardClass.ToString())
        .SetCastingAttribute(StatType.Charisma)
        .SetSpontaneous(true)
        .SetIsArcane(true)
        .Configure();
    }

    // Generates the diminished spell slots
    private static BlueprintSpellsTable GetSpellSlots()
    {
      var bardSpellSlots = SpellsTableRefs.BardSpellSlotsTable.Reference.Get();
      var levelEntries =
        bardSpellSlots.Levels.Select(
          l =>
          {
            var count = l.Count.Select(c => Math.Max(0, c - 1)).ToArray();
            return new SpellsLevelEntry { Count = count };
          });
      return SpellsTableConfigurator.New(SpellsPerDay, Guids.ArrowsongMinstrelSpellsPerDay)
        .SetLevels(levelEntries.ToArray())
        .Configure();
    }

    // TODO: ICON
    private static BlueprintFeature CreateArcaneArchery()
    {
      return FeatureConfigurator.New(ArcaneArchery, Guids.ArrowsongMinstrelArcaneArchery)
        .AddReplaceStatForPrerequisites(CharacterClassRefs.BardClass.ToString(), StatType.BaseAttackBonus)
        .Configure();
    }
    
    // TODO: Icon!
    private static BlueprintFeature CreateBonusSpellSelection()
    {
      // TODO Bonus selection feature (at least the one used from level 4+, initial one probably should be in ArcaneArchery
      return FeatureConfigurator.New(SpellSelection, Guids.ArrowsongMinstrelSpellSelection)
        .SetDisplayName(SpellSelectionName)
        .SetDescription(SpellSelectionDescription)
        .SetIsClassFeature(true)
        // Actually this probably doesn't work. This will grant them as spells known. OOPS.
        .AddComponent<AdditionalSpellSelection>(
          c =>
          {
            c.m_SpellCastingClass = CharacterClassRefs.MagusClass.Cast<BlueprintCharacterClassReference>().Reference;
            c.m_SpellList = CreateArcaneArcherySpellList().ToReference<BlueprintSpellListReference>();
          })
        .Configure();
    }

    private static readonly BlueprintSpellList WizardEvocationSpells =
      SpellListRefs.WizardEvocationSpellList.Reference.Get();
    private static BlueprintSpellList CreateArcaneArcherySpellList()
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
        SpellListConfigurator.New(SpellList, Guids.ArrowsongMinstrelSpellList)
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
