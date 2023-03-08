using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Spells;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.UnitParts;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharacterOptionsPlus.Archetypes
{
  internal class ArrowsongMinstrel
  {
    private const string ArchetypeName = "ArrowsongMinstrel";

    internal const string ArchetypeDisplayName = "ArrowsongMinstrel.Name";
    private const string ArchetypeDescription = "ArrowsongMinstrel.Description";

    private const string ProficienciesDisplayName = "ArrowsongMinstrel.Proficiencies";
    private const string ProficienciesDescription = "ArrowsongMinstrel.Proficiencies.Description";

    private const string ArcaneArchery = "ArrowsongMinstrel.ArcaneArchery";
    private const string ArcaneArcheryName = "ArrowsongMinstrel.ArcaneArchery.Name";
    private const string ArcaneArcheryDescription = "ArrowsongMinstrel.ArcaneArchery.Description";

    private const string SpellStrike = "ArrowsongMinstrel.SpellStrike";
    private const string SpellStrikeAbility = "ArrowsongMinstrel.SpellStrike.Ability";
    private const string SpellCombat = "ArrowsongMinstrel.SpellCombat";
    private const string SpellCombatAbility = "ArrowsongMinstrel.SpellCombat.Ability";
    private const string SpellStrikeName = "ArrowsongMinstrel.SpellStrike.Name";
    private const string SpellStrikeDescription = "ArrowsongMinstrel.SpellStrike.Description";

    private const string Proficiencies = "ArrowsongMinstrel.Proficiencies";
    private const string Spellbook = "ArrowsongMinstrel.Spellbook";
    private const string SpellList = "ArrowsongMinstrel.SpellList";
    private const string SpellsPerDay = "ArrowsongMinstrel.SpellsPerDay";

    private const string SpellSelection = "ArrowsongMinstrel.SpellSelection";
    private const string SpellSelectionName = "ArrowsongMinstrel.ArcaneArchery.BonusSpells";
    private const string SpellSelectionDescription = "ArrowsongMinstrel.ArcaneArchery.BonusSpells.Description";

    private const string ArrowsongMinstrelArcaneTrickster = "ArrowsongMinstrel.ArcaneTrickster";
    private const string ArrowsongMinstrelDragonDisciple = "ArrowsongMinstrel.DragonDisciple";
    private const string ArrowsongMinstrelEldritchKnight = "ArrowsongMinstrel.EldritchKnight";
    private const string ArrowsongMinstrelHellknightSignifier = "ArrowsongMinstrel.HellknightSignifier";
    private const string ArrowsongMinstrelLoremaster = "ArrowsongMinstrel.Loremaster";
    private const string ArrowsongMinstrelMysticTheurge = "ArrowsongMinstrel.MysticTheurge";

    private static readonly Logging.Logger Logger = Logging.GetLogger(ArchetypeName);

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "arcanearchery.png";

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ArrowsongMinstrelArchetype))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ArrowsongMinstrel.Configure", e);
      }
    }

    internal static void ConfigureDelayed()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ArrowsongMinstrelArchetype))
          ConfigureEnabledDelayed();
      }
      catch (Exception e)
      {
        Logger.LogException("ArrowsongMinstrel.ConfigureDelayed", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {ArchetypeName} (disabled)");
      ArchetypeConfigurator.New(ArchetypeName, Guids.ArrowsongMinstrelArchetype)
        .SetLocalizedName(ArchetypeDisplayName)
        .SetLocalizedDescription(ArchetypeDescription)
        .Configure();

      FeatureConfigurator.New(Proficiencies, Guids.ArrowsongMinstrelProficiencies)
        .SetDisplayName(ProficienciesDisplayName)
        .SetDescription(ProficienciesDescription)
        .Configure();

      ActivatableAbilityConfigurator.New(SpellStrikeAbility, Guids.ArrowsongMinstrelSpellStrikeAbility)
        .SetDisplayName(SpellStrikeName)
        .SetDescription(SpellStrikeDescription)
        .Configure();
      FeatureConfigurator.New(SpellStrike, Guids.ArrowsongMinstrelSpellStrike)
        .SetDisplayName(SpellStrikeName)
        .SetDescription(SpellStrikeDescription)
        .Configure();

      ActivatableAbilityConfigurator.New(SpellCombatAbility, Guids.ArrowsongMinstrelSpellCombatAbility)
        .SetDisplayName(SpellStrikeName)
        .SetDescription(SpellStrikeDescription)
        .Configure();
      FeatureConfigurator.New(SpellCombat, Guids.ArrowsongMinstrelSpellCombat)
        .SetDisplayName(SpellStrikeName)
        .SetDescription(SpellStrikeDescription)
        .Configure();

      SpellbookConfigurator.New(Spellbook, Guids.ArrowsongMinstrelSpellbook)
        .SetName(ArchetypeDisplayName)
        .Configure();

      SpellsTableConfigurator.New(SpellsPerDay, Guids.ArrowsongMinstrelSpellsPerDay).Configure();

      FeatureConfigurator.New(ArcaneArchery, Guids.ArrowsongMinstrelArcaneArchery).Configure();

      SpellListConfigurator.New(SpellList, Guids.ArrowsongMinstrelSpellList).Configure();
      ParametrizedFeatureConfigurator.New(SpellSelection, Guids.ArrowsongMinstrelSpellSelection)
        .SetDisplayName(SpellSelectionName)
        .SetDescription(SpellSelectionDescription)
        .Configure();

      FeatureReplaceSpellbookConfigurator.New(ArrowsongMinstrelArcaneTrickster, Guids.ArrowsongMinstrelArcaneTrickster).Configure();
      FeatureReplaceSpellbookConfigurator.New(ArrowsongMinstrelDragonDisciple, Guids.ArrowsongMinstrelDragonDisciple).Configure();
      FeatureReplaceSpellbookConfigurator.New(ArrowsongMinstrelEldritchKnight, Guids.ArrowsongMinstrelEldritchKnight).Configure();
      FeatureReplaceSpellbookConfigurator.New(ArrowsongMinstrelHellknightSignifier, Guids.ArrowsongMinstrelHellknightSignifier).Configure();
      FeatureReplaceSpellbookConfigurator.New(ArrowsongMinstrelLoremaster, Guids.ArrowsongMinstrelLoremaster).Configure();
      FeatureReplaceSpellbookConfigurator.New(ArrowsongMinstrelMysticTheurge, Guids.ArrowsongMinstrelMysticTheurge).Configure();
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

      var bonusSpellSelection = CreateBonusSpellSelection();
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
        .AddToAddFeatures(6, CreateRangedSpellStrike())
        .AddToAddFeatures(18, CreateRangedSpellCombat())

        .AddToAddFeatures(4, bonusSpellSelection)
        .AddToAddFeatures(8, bonusSpellSelection)
        .AddToAddFeatures(12, bonusSpellSelection)
        .AddToAddFeatures(16, bonusSpellSelection)
        .AddToAddFeatures(20, bonusSpellSelection);
      archetype.Configure();
    }

    private static void ConfigureEnabledDelayed()
    {
      if (!Settings.IsTTTBaseEnabled())
        return;

      Logger.Log("Patching TTT Loremaster compatbility for Arrowsong Minstrel");
      FeatureSelectionConfigurator.For(Guids.TTTLoremasterSpellbookSelection)
        .AddToAllFeatures(Guids.ArrowsongMinstrelLoremaster)
        .SkipAddToSelections()
        .Configure();
      Common.AddToLoremasterSecrets(
        Guids.ArrowsongMinstrelLoremaster,
        Guids.TTTLoremasterClericSecretBard,
        Guids.TTTLoremasterDruidSecretBard,
        Guids.TTTLoremasterWizardSecretBard);
    }

    private static BlueprintFeature CreateProficiencies()
    {
      var icon = AbilityRefs.HurricaneBow.Reference.Get().Icon;
      var bardProficiencies = FeatureRefs.BardProficiencies.Reference.Get();
      return FeatureConfigurator.New(Proficiencies, Guids.ArrowsongMinstrelProficiencies)
        .SetDisplayName(ProficienciesDisplayName)
        .SetDescription(ProficienciesDescription)
        .SetIcon(icon)
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

    private static BlueprintFeature CreateRangedSpellStrike()
    {
      var icon = FeatureRefs.EldritchArcherRangedSpellStrike.Reference.Get().Icon;
      var spellStrike =
        ActivatableAbilityConfigurator.New(SpellStrikeAbility, Guids.ArrowsongMinstrelSpellStrikeAbility)
          .SetDisplayName(SpellStrikeName)
          .SetDescription(SpellStrikeDescription)
          .SetIcon(icon)
          .SetIsOnByDefault()
          .SetDeactivateImmediately()
          .SetBuff(BuffRefs.SpellStrikeBuff.ToString())
          .Configure();

      return FeatureConfigurator.New(SpellStrike, Guids.ArrowsongMinstrelSpellStrike)
        .SetDisplayName(SpellStrikeName)
        .SetDescription(SpellStrikeDescription)
        .SetIcon(icon)
        .SetIsClassFeature(true)
        .AddMagusMechanicPart(AddMagusMechanicPart.Feature.EldritchArcher)
        .AddMagusMechanicPart(AddMagusMechanicPart.Feature.Spellstrike)
        .AddFacts(new() { spellStrike })
        .Configure();
    }

    private static BlueprintFeature CreateRangedSpellCombat()
    {
      var icon = FeatureRefs.EldritchArcherRangedSpellCombat.Reference.Get().Icon;
      var spellCombat =
        ActivatableAbilityConfigurator.New(SpellCombatAbility, Guids.ArrowsongMinstrelSpellCombatAbility)
          .SetDisplayName(SpellStrikeName)
          .SetDescription(SpellStrikeDescription)
          .SetIcon(icon)
          .SetIsOnByDefault()
          .SetDeactivateImmediately()
          .SetBuff(BuffRefs.SpellCombatBuff.ToString())
          .Configure();

      return FeatureConfigurator.New(SpellCombat, Guids.ArrowsongMinstrelSpellCombat)
        .SetDisplayName(SpellStrikeName)
        .SetDescription(SpellStrikeDescription)
        .SetIcon(icon)
        .SetIsClassFeature(true)
        .AddMagusMechanicPart(AddMagusMechanicPart.Feature.SpellCombat)
        .AddFacts(new() { spellCombat })
        .Configure();
    }

    private static BlueprintSpellbook CreateSpellbook()
    {
      var spellbook = SpellbookConfigurator.New(Spellbook, Guids.ArrowsongMinstrelSpellbook)
        .SetName(ArchetypeDisplayName)
        .SetSpellsPerDay(GetSpellSlots())
        .SetSpellsKnown(SpellsTableRefs.BardSpellsKnownTable.ToString())
        .SetSpellList(SpellListRefs.BardSpellList.ToString())
        .SetCharacterClass(CharacterClassRefs.BardClass.ToString())
        .SetCastingAttribute(StatType.Charisma)
        .SetSpontaneous(true)
        .SetIsArcane(true)
        .Configure();

      var bard = CharacterClassRefs.BardClass.ToString();

      // Update references to the bard spellbook to include the new spellbook
      FeatureConfigurator.For(FeatureRefs.DLC3_ArcaneModFeature)
        .EditComponent<AddAbilityUseTrigger>(
          c =>
            c.m_Spellbooks = CommonTool.Append(c.m_Spellbooks, spellbook.ToReference<BlueprintSpellbookReference>()))
        .Configure();

      // Arcane Trickster
      Common.ConfigureArchetypeSpellbookReplacement(
        characterClass: bard,
        archetype: Guids.ArrowsongMinstrelArchetype,
        baseReplacement: FeatureReplaceSpellbookRefs.ArcaneTricksterBard.ToString(),
        sourceReplacement: FeatureReplaceSpellbookRefs.ArcaneTricksterBard.ToString(),
        replacementName: ArrowsongMinstrelArcaneTrickster,
        replacementGuid: Guids.ArrowsongMinstrelArcaneTrickster,
        spellbook,
        replacementSelection: FeatureSelectionRefs.ArcaneTricksterSpellbookSelection.ToString());

      // Dragon Disciple
      Common.ConfigureArchetypeSpellbookReplacement(
        characterClass: bard,
        archetype: Guids.ArrowsongMinstrelArchetype,
        baseReplacement: FeatureReplaceSpellbookRefs.DragonDiscipleBard.ToString(),
        sourceReplacement: FeatureReplaceSpellbookRefs.DragonDiscipleBard.ToString(),
        replacementName: ArrowsongMinstrelDragonDisciple,
        replacementGuid: Guids.ArrowsongMinstrelDragonDisciple,
        spellbook,
        replacementSelection: FeatureSelectionRefs.DragonDiscipleSpellbookSelection.ToString());

      // Eldritch Knight
      Common.ConfigureArchetypeSpellbookReplacement(
        characterClass: bard,
        archetype: Guids.ArrowsongMinstrelArchetype,
        baseReplacement: FeatureReplaceSpellbookRefs.EldritchKnightBard.ToString(),
        sourceReplacement: FeatureReplaceSpellbookRefs.EldritchKnightBard.ToString(),
        replacementName: ArrowsongMinstrelEldritchKnight,
        replacementGuid: Guids.ArrowsongMinstrelEldritchKnight,
        spellbook,
        replacementSelection: FeatureSelectionRefs.EldritchKnightSpellbookSelection.ToString());

      // Hellknight Signifier
      Common.ConfigureArchetypeSpellbookReplacement(
        characterClass: bard,
        archetype: Guids.ArrowsongMinstrelArchetype,
        baseReplacement: FeatureReplaceSpellbookRefs.HellknightSigniferBard.ToString(),
        sourceReplacement: FeatureReplaceSpellbookRefs.HellknightSigniferBard.ToString(),
        replacementName: ArrowsongMinstrelHellknightSignifier,
        replacementGuid: Guids.ArrowsongMinstrelHellknightSignifier,
        spellbook,
        replacementSelection: FeatureSelectionRefs.HellknightSigniferSpellbook.ToString());

      // Loremaster
      Common.ConfigureArchetypeSpellbookReplacement(
        characterClass: bard,
        archetype: Guids.ArrowsongMinstrelArchetype,
        baseReplacement: FeatureReplaceSpellbookRefs.LoremasterBard.ToString(),
        sourceReplacement: FeatureReplaceSpellbookRefs.LoremasterBard.ToString(),
        replacementName: ArrowsongMinstrelLoremaster,
        replacementGuid: Guids.ArrowsongMinstrelLoremaster,
        spellbook,
        replacementSelection: FeatureSelectionRefs.LoremasterSpellbookSelection.ToString());
      Common.AddToLoremasterSecrets(
        Guids.ArrowsongMinstrelLoremaster,
        ParametrizedFeatureRefs.LoremasterClericSecretBard.ToString(),
        ParametrizedFeatureRefs.LoremasterDruidSecretBard.ToString(),
        ParametrizedFeatureRefs.LoremasterWizardSecretBard.ToString());

      // MysticTheurge
      Common.ConfigureArchetypeSpellbookReplacement(
        characterClass: bard,
        archetype: Guids.ArrowsongMinstrelArchetype,
        baseReplacement: FeatureReplaceSpellbookRefs.MysticTheurgeBard.ToString(),
        sourceReplacement: FeatureReplaceSpellbookRefs.MysticTheurgeBard.ToString(),
        replacementName: ArrowsongMinstrelMysticTheurge,
        replacementGuid: Guids.ArrowsongMinstrelMysticTheurge,
        spellbook,
        replacementSelection: FeatureSelectionRefs.MysticTheurgeArcaneSpellbookSelection.ToString());

      return spellbook;
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

    private static BlueprintFeature CreateArcaneArchery()
    {
      return FeatureConfigurator.New(ArcaneArchery, Guids.ArrowsongMinstrelArcaneArchery)
        .SetDisplayName(ArcaneArcheryName)
        .SetDescription(ArcaneArcheryDescription)
        .SetIcon(IconName)
        .AddReplaceStatForPrerequisites(CharacterClassRefs.BardClass.ToString(), StatType.BaseAttackBonus)
        .SetIsClassFeature(true)
        .Configure();
    }

    private static BlueprintParametrizedFeature CreateBonusSpellSelection()
    {
      var bonusSpells = SpellListConfigurator.New(SpellList, Guids.ArrowsongMinstrelSpellList).Configure();
      var selection =
        ParametrizedFeatureConfigurator.New(SpellSelection, Guids.ArrowsongMinstrelSpellSelection)
          .SetDisplayName(SpellSelectionName)
          .SetDescription(SpellSelectionDescription)
          .SetIcon(IconName)
          .SetParameterType(FeatureParameterType.Custom)
          .SetIsClassFeature(true)
          .AddComponent(
            new AddSpellToSpellList(
              CharacterClassRefs.BardClass.Cast<BlueprintCharacterClassReference>().Reference,
              bonusSpells.ToReference<BlueprintSpellListReference>()))
          .OnConfigure(
            feature =>
            {
              ConfigureBonusSpells();

              var parameterVariants = new List<AnyBlueprintReference>();
              // I could not get a cast of list items to work to save my life so this is what you get
              foreach (var spell in bonusSpells.SpellsByLevel.SelectMany(list => list.m_Spells))
              {
                parameterVariants.Add(BlueprintTool.GetRef<AnyBlueprintReference>(spell.deserializedGuid.ToString()));
              }
              feature.BlueprintParameterVariants = parameterVariants.ToArray();
            });

      return selection.Configure(delayed: true);
    }

    private static readonly BlueprintSpellList BardSpells = SpellListRefs.BardSpellList.Reference.Get();
    private static readonly BlueprintSpellList WizardEvocationSpells =
      SpellListRefs.WizardEvocationSpellList.Reference.Get();
    // Call this in a delayed configure event so that any mods modifying spell lists are taken into account.
    private static void ConfigureBonusSpells()
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
            .Append(AbilityRefs.HurricaneBow.Cast<BlueprintAbilityReference>().Reference)
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

      SpellListConfigurator.For(SpellList)
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

    /// <summary>
    /// Flags songs from the Arrowsong Minstrel spellbook as valid "magus" spells so the magus features work normally.
    /// </summary>
    [HarmonyPatch(typeof(UnitPartMagus))]
    static class UnitPartMagus_Patch
    {
      static BlueprintSpellbook _spellbook;
      static BlueprintSpellbook ArrowsongSpellbook
      {
        get
        {
          _spellbook ??= BlueprintTool.Get<BlueprintSpellbook>(Guids.ArrowsongMinstrelSpellbook);
          return _spellbook;
        }
      }

      [HarmonyPatch(nameof(UnitPartMagus.IsSpellFromMagusSpellList)), HarmonyPostfix]
      static void IsSpellFromMagusSpellList(AbilityData spell, ref bool __result)
      {
        if (spell.SpellbookBlueprint == ArrowsongSpellbook)
          __result = true;
      }
    }
  }
}
