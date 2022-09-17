using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.UnitParts;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using System;
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

      FeatureConfigurator.New(ArcaneArchery, Guids.ArrowsongMinstrelArcaneArchery).Configure();

      SpellbookConfigurator.New(Spellbook, Guids.ArrowsongMinstrelSpellbook)
        .SetName(ArchetypeDisplayName)
        .Configure();
      SpellListConfigurator.New(SpellList, Guids.ArrowsongMinstrelSpellList).Configure();
      SpellsTableConfigurator.New(SpellsPerDay, Guids.ArrowsongMinstrelSpellsPerDay).Configure();
      ParametrizedFeatureConfigurator.New(SpellSelection, Guids.ArrowsongMinstrelSpellSelection)
        .SetDisplayName(SpellSelectionName)
        .SetDescription(SpellSelectionDescription)
        .Configure();

      FeatureConfigurator.New(Proficiencies, Guids.ArrowsongMinstrelProficiencies)
        .SetDisplayName(ProficienciesDisplayName)
        .SetDescription(ProficienciesDescription)
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

      var bonusSpellSelection = CreateBonusSpellSelection(CreateBonusSpellList());
      archetype
        .AddToAddFeatures(
          1,
          CreateArcaneArchery(),
          bonusSpellSelection,
          bonusSpellSelection,
          bonusSpellSelection,
          bonusSpellSelection,
          CreateProficiencies())
        .AddToAddFeatures(2, FeatureRefs.PreciseShot.ToString())

        .AddToAddFeatures(4, bonusSpellSelection)
        .AddToAddFeatures(8, bonusSpellSelection)
        .AddToAddFeatures(12, bonusSpellSelection)
        .AddToAddFeatures(16, bonusSpellSelection)
        .AddToAddFeatures(20, bonusSpellSelection);
      // TODO: Arrowsong Strike at 6
      // TODO: Full-attack Arrowsong Strike at 18

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
        .SetIsClassFeature(true)
        .Configure();
    }

    private static BlueprintParametrizedFeature CreateBonusSpellSelection(BlueprintSpellList bonusSpells)
    {
      var selection =
        ParametrizedFeatureConfigurator.New(SpellSelection, Guids.ArrowsongMinstrelSpellSelection)
          .SetDisplayName(SpellSelectionName)
          .SetDescription(SpellSelectionDescription)
          .SetParameterType(FeatureParameterType.Custom)
          .SetIsClassFeature(true)
          .AddComponent(
            new AddSpellToSpellList(
              CharacterClassRefs.BardClass.Cast<BlueprintCharacterClassReference>().Reference,
              bonusSpells.ToReference<BlueprintSpellListReference>()));

      // I could not get a cast of list items to work to save my life so this is what you get
      foreach (var spell in bonusSpells.SpellsByLevel.SelectMany(list => list.m_Spells))
      {
        selection.AddToBlueprintParameterVariants(
          BlueprintTool.GetRef<AnyBlueprintReference>(spell.deserializedGuid.ToString()));
      }

      return selection.Configure();
    }

    private static readonly BlueprintSpellList BardSpells = SpellListRefs.BardSpellList.Reference.Get();
    private static readonly BlueprintSpellList WizardEvocationSpells =
      SpellListRefs.WizardEvocationSpellList.Reference.Get();
    private static BlueprintSpellList CreateBonusSpellList()
    {
      var bardSpells = new List<BlueprintAbility>();
      bardSpells.AddRange(BardSpells.GetSpells(1));
      bardSpells.AddRange(BardSpells.GetSpells(2));
      bardSpells.AddRange(BardSpells.GetSpells(3));
      bardSpells.AddRange(BardSpells.GetSpells(4));
      bardSpells.AddRange(BardSpells.GetSpells(5));
      bardSpells.AddRange(BardSpells.GetSpells(6));

      var firstLevelSpells = new SpellLevelList(1)
      {
        m_Spells =
          WizardEvocationSpells.GetSpells(1)
            .Except(bardSpells)
            .Select(s => s.ToReference<BlueprintAbilityReference>())
            .Append(AbilityRefs.MagicWeapon.Cast<BlueprintAbilityReference>().Reference)
            .Append(AbilityRefs.TrueStrike.Cast<BlueprintAbilityReference>().Reference)
            .ToList()
      };

      var secondLevelSpells = new SpellLevelList(2)
      {
        m_Spells =
          WizardEvocationSpells.GetSpells(2)
            .Except(bardSpells)
            .Select(s => s.ToReference<BlueprintAbilityReference>())
            .Append(AbilityRefs.ProtectionFromArrows.Cast<BlueprintAbilityReference>().Reference)
            .Append(AbilityRefs.AcidArrow.Cast<BlueprintAbilityReference>().Reference)
            .ToList()
      };

      var thirdLevelSpells = new SpellLevelList(3)
      {
        m_Spells =
          WizardEvocationSpells.GetSpells(3)
            .Except(bardSpells)
            .Select(s => s.ToReference<BlueprintAbilityReference>())
            .Append(AbilityRefs.MagicWeaponGreater.Cast<BlueprintAbilityReference>().Reference)
            .Append(AbilityRefs.ProtectionFromArrowsCommunal.Cast<BlueprintAbilityReference>().Reference)
            .ToList()
      };

      var fourthLevelSpells = new SpellLevelList(4)
      {
        m_Spells =
          WizardEvocationSpells.GetSpells(4)
            .Except(bardSpells)
            .Select(s => s.ToReference<BlueprintAbilityReference>())
            .ToList()
      };

      var fifthLevelSpells = new SpellLevelList(5)
      {
        m_Spells =
          WizardEvocationSpells.GetSpells(5)
            .Except(bardSpells)
            .Select(s => s.ToReference<BlueprintAbilityReference>())
            .ToList()
      };

      var sixthLevelSpells = new SpellLevelList(6)
      {
        m_Spells =
          WizardEvocationSpells.GetSpells(6)
            .Except(bardSpells)
            .Select(s => s.ToReference<BlueprintAbilityReference>())
            .ToList()
      };

      return
        SpellListConfigurator.New(SpellList, Guids.ArrowsongMinstrelSpellList)
          .AddToSpellsByLevel(
            new(0),
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
