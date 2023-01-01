using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Spells;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Spells;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharacterOptionsPlus.UnitParts
{
  /// <summary>
  /// Adds additional spells to the character's spell list. These are not spells known and must still otherwise be
  /// selected as spells known before they can be cast.
  /// </summary>
  public class UnitPartExpandedSpellList : OldStyleUnitPart
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(UnitPartExpandedSpellList));

    // These are needed because you can't tell if a character has an altered spell list when the spell selection is
    // first instantiated. This is essentially a hint that the spell list should be replaced to support a bonus spell
    // selection.
    private static readonly List<UnitRequirements> Requirements =
      new()
      {
        new(
          CharacterClassRefs.BardClass.Cast<BlueprintCharacterClassReference>().Reference,
          BlueprintTool.GetRef<BlueprintArchetypeReference>(Guids.ArrowsongMinstrelArchetype))
      };

    [JsonProperty]
    public Dictionary<BlueprintCharacterClassReference, List<SpellsByLevel>> ExtraSpells = new();

    /// <summary>
    /// Add a spell to the character's spell list.
    /// </summary>
    public void AddSpell(BlueprintCharacterClassReference clazz, int level, BlueprintAbilityReference spell)
    {
      if (!ExtraSpells.ContainsKey(clazz))
        ExtraSpells[clazz] = new();

      var spellList = ExtraSpells[clazz];
      var spellLevelList = spellList.Where(list => list.Level == level).FirstOrDefault();
      if (spellLevelList is null)
      {
        Logger.Verbose("Creating new spell level list");
        spellLevelList = new SpellsByLevel(level);
        spellList.Add(spellLevelList);
      }

      if (spellLevelList.Spells.Contains(spell))
        return;

      Logger.Verbose($"Adding to spell list for {Owner.CharacterName} - {clazz}");
      spellLevelList.Spells.Add(spell);
    }

    /// <summary>
    /// Removes a spell from the character's spell list.
    /// </summary>
    public void RemoveSpell(BlueprintCharacterClassReference clazz, int level, BlueprintAbilityReference spell)
    {
      if (!ExtraSpells.ContainsKey(clazz))
        return;

      var spellList = ExtraSpells[clazz];
      var spellLevelList = spellList.Where(list => list.Level == level).FirstOrDefault();
      if (spellLevelList is null)
        return;

      if (!spellLevelList.Spells.Contains(spell))
        return;

      Logger.Verbose($"Removing from spell list for {Owner.CharacterName} - {clazz}");
      spellLevelList.Spells.Remove(spell);
    }

    /// <summary>
    /// Populates <paramref name="newSelection"/> with  a modified <c>SpellSelectionData</c> which uses the expanded
    /// spell list.
    /// </summary>
    ///
    /// <returns>True if the spell list is modified, false otherwise</returns>
    public bool GetSpellSelection(SpellSelectionData spellSelection, out SpellSelectionData newSelection)
    {
      newSelection = spellSelection;
      if (!Requirements.Exists(reqs => reqs.AreMet(spellSelection?.Spellbook, Owner)))
      {
        Logger.Verbose($"Spell selection unmodified: {spellSelection?.Spellbook}");
        return false;
      }

      var extraSpells =
        ExtraSpells.ContainsKey(spellSelection?.Spellbook.m_CharacterClass)
          ? ExtraSpells[spellSelection.Spellbook.m_CharacterClass]
          : new();

      var spellList =
        GetExpandedSpellList(
          spellSelection.Spellbook.m_CharacterClass,
          spellSelection.SpellList,
          extraSpells);

      Logger.Verbose(
        $"Returning spell selection for {Owner.CharacterName} - {spellSelection.Spellbook.m_CharacterClass}");
      newSelection = new SpellSelectionData(spellSelection.Spellbook, spellList);
      for (int i = 0; i < spellSelection.LevelCount.Length; i++)
      {
        newSelection.LevelCount[i] = spellSelection.LevelCount[i];
      }
      return true;
    }

    /// <summary>
    /// Returns an expanded spell list which replaces <paramref name="spellList"/>.
    /// </summary>
    public bool GetSpellList(
      BlueprintSpellbook spellbook,
      BlueprintSpellList spellList,
      out BlueprintSpellList expandedSpellList)
    {
      expandedSpellList = null;
      if (!Requirements.Exists(reqs => reqs.AreMet(spellbook, Owner)))
      {
        Logger.Verbose($"Spell selection unmodified: {spellbook}");
        return false;
      }

      var extraSpells =
        ExtraSpells.ContainsKey(spellbook.m_CharacterClass) ? ExtraSpells[spellbook.m_CharacterClass] : new();

      Logger.Verbose($"Returning spell list for {Owner.CharacterName} - {spellbook.CharacterClass.Name}");
      expandedSpellList = GetExpandedSpellList(spellbook.m_CharacterClass, spellList, extraSpells);
      return true;
    }

    /// <summary>
    /// Returns the expanded spell list, either by fetching from the cache or creating it.
    /// </summary>
    private BlueprintSpellList GetExpandedSpellList(
      BlueprintCharacterClassReference clazz,
      BlueprintSpellList spellList,
      List<SpellsByLevel> extraSpells)
    {
      var charId =
        Game.Instance?.LevelUpController is null
          ? Owner.Unit.UniqueId
          : Game.Instance.LevelUpController.m_BaseUnit.UniqueId; // This is stable during level up
      var spellListName = $"ExpandedSpellList_{charId}_{clazz}";
      SpellListConfigurator expandedList;
      if (BlueprintTool.TryGet<BlueprintSpellList>(spellListName, out var existingSpellList))
      {
        Logger.Verbose($"Re-using expanded spell list for {Owner.CharacterName} - {clazz}");
        expandedList = SpellListConfigurator.For(existingSpellList);
      }
      else
      {
        var guid = Guids.ReserveDynamic();
        Logger.Verbose(
          $"Creating expanded spell list for {Owner.CharacterName} - {clazz}, using dynamic guid {guid}");
        expandedList = SpellListConfigurator.New(spellListName, guid);
      }

      return expandedList.SetSpellsByLevel(CombineSpellLists(spellList.SpellsByLevel, extraSpells)).Configure();
    }

    /// <summary>
    /// Returns a combined spell level list with the <paramref name="extraSpells"/> added.
    /// </summary>
    private static SpellLevelList[] CombineSpellLists(SpellLevelList[] baseList, List<SpellsByLevel> extraSpells)
    {
      var spellLevelList = new SpellLevelList[baseList.Length];
      for (int i = 0; i < baseList.Length; i++)
      {
        var list = new SpellLevelList(baseList[i].SpellLevel);
        list.m_Spells.AddRange(baseList[i].m_Spells);

        var extraList = extraSpells.Where(l => l.Level == list.SpellLevel).FirstOrDefault();
        if (extraList is not null && extraList.Spells.Any())
        {
          list.m_Spells.AddRange(extraList.Spells);
        }

        list.m_Spells = list.m_Spells.Distinct().ToList();
        spellLevelList[i] = list;
      }
      return spellLevelList;
    }

    /// <summary>
    /// Patch responsible for swapping the selection data with the expanded version before it is bound / viewed in the
    /// level up UI.
    /// </summary>
    /// 
    /// <remarks>
    /// This wouldn't be necessary if there was an easy way to refresh the spells UI when making a selection. The only
    /// other way to do this that I found was to call <c>LevelUpController.UpdatePreview()</c>, but that removes and
    /// reapplies features which re-creates the UnitPart so the stored data is lost. If you statically keep the data
    /// then you have an issue because they remove <c>EntityFact</c> without calling <c>OnDeactivate</c> so now there
    /// are multiple selections.
    /// </remarks>
    [HarmonyPatch(typeof(CharGenSpellsPhaseVM))]
    static class CharGenSpellsPhaseVM_Patch
    {
      [HarmonyPatch(nameof(CharGenSpellsPhaseVM.OnBeginDetailedView)), HarmonyPrefix]
      static void OnBeginDetailedView(CharGenSpellsPhaseVM __instance)
      {
        try
        {
          var unit = __instance.m_UnitDescriptor.Value;
          if (
            unit.Ensure<UnitPartExpandedSpellList>().GetSpellSelection(
              __instance.m_SelectionData, out var selectionData))
          {
            Logger.Verbose("Swapping selection data for CharGenSpellsPhaseVM");
            __instance.LevelUpController.State.SpellSelections.RemoveAll(
              selection =>
                selection.Spellbook == __instance.m_SelectionData.Spellbook
                  && selection.SpellList == __instance.m_SelectionData.SpellList);
            __instance.LevelUpController.State.SpellSelections.Add(selectionData);
            __instance.m_SelectionData = selectionData;

            __instance.SpellList = selectionData.SpellList;
            __instance.m_SpellListIsCreated = false;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("OnBeginDetailedView: Failed to refresh spell selection.", e);
        }
      }
    }

    /// <summary>
    /// Redirects attempts to generate a spell selection from the default spell list to the expanded spell lists.
    /// </summary>
    [HarmonyPatch(typeof(LevelUpState))]
    internal static class LevelUpState_Patch
    {
      [HarmonyPatch(nameof(LevelUpState.DemandSpellSelection)), HarmonyPrefix]
      static void DemandSpellSelection(
        LevelUpState __instance, BlueprintSpellbook spellbook, ref BlueprintSpellList spellList)
      {
        try
        {
          if (
            __instance.Unit.Ensure<UnitPartExpandedSpellList>()
                .GetSpellList(spellbook, spellList, out var expandedSpellList)
              && expandedSpellList != spellList)
          {
            Logger.Verbose("Returning expanded spell list in DemandSpellSelection");
            spellList = expandedSpellList;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("DemandSpellSelection: Failed to replace spell list", e);
        }
      }

      [HarmonyPatch(nameof(LevelUpState.GetSpellSelection)), HarmonyPrefix]
      static void GetSpellSelection(
        LevelUpState __instance, BlueprintSpellbook spellbook, ref BlueprintSpellList spellList)
      {
        try
        {
          if (
            __instance.Unit.Ensure<UnitPartExpandedSpellList>()
                .GetSpellList(spellbook, spellList, out var expandedSpellList)
              && expandedSpellList != spellList)
          {
            Logger.Verbose("Returning expanded spell list in GetSpellSelection");
            spellList = expandedSpellList;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("GetSpellSelection: Failed to replace spell list", e);
        }
      }
    }

    /// <summary>
    /// Checks to make sure the selected spell is on the spell list.
    /// </summary>
    [HarmonyPatch(typeof(SelectSpell))]
    internal static class SelectSpell_Patch
    {
      [HarmonyPatch(nameof(SelectSpell.Check)), HarmonyPrefix]
      static bool Check(SelectSpell __instance, LevelUpState state, ref bool __result)
      {
        try
        {
          var spellSelection = state.GetSpellSelection(__instance.Spellbook, __instance.SpellList);
          if (!spellSelection.SpellList.Contains(__instance.Spell))
          {
            Logger.Verbose("Clearing invalid spell selection");
            // Indicate the spell selection is not valid. This makes sure the user cannot break the leveling state by
            // selecting a bonus spell, then swapping the bonus spell with another. The bonus spell selection is
            // cleared and they are forced to select a different spell.
            __result = false;
            return false;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("Check: Failed to check if spell selection is valid.", e);
        }
        return true;
      }
    }

    /// <summary>
    /// Update the spell list as shown in the spellbook.
    /// </summary>
    [HarmonyPatch(typeof(UIUtilityUnit))]
    internal static class UIUtilityUnit_Patch
    {
      [HarmonyPatch(nameof(UIUtilityUnit.GetAllPossibleSpellsForLevel)), HarmonyPostfix]
      static void GetAllPossibleSpellsForLevel(int level, Spellbook spellbook, ref List<BlueprintAbility> __result)
      {
        try
        {
          if (spellbook.Owner.Ensure<UnitPartExpandedSpellList>()
            .GetSpellList(spellbook.Blueprint, spellbook.Blueprint.SpellList, out var expandedSpellList))
          {
            Logger.Verbose($"Returning expanded spells: {spellbook}");
            __result = expandedSpellList.GetSpells(level);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("GetAllPossibleSpellsForLevel: Failed to return expanded spell list.", e);
        }
      }
    }

    /// <summary>
    /// Simple wrapper to allow serialization of SpellLevelList.
    /// </summary>
    [Serializable]
    public class SpellsByLevel
    {
      [JsonProperty]
      public readonly int Level;

      [JsonProperty]
      public readonly List<BlueprintAbilityReference> Spells = new();

      public SpellsByLevel(int spellLevel)
      {
        Level = spellLevel;
      }

      [JsonConstructor]
      public SpellsByLevel(int spellLevel, List<BlueprintAbilityReference> spells) 
      {
        Level = spellLevel;
        Spells = spells;
      }
    }

    /// <summary>
    /// Identifies requirements for a unit to need a replaced spelllist.
    /// </summary>
    public class UnitRequirements
    {
      public BlueprintCharacterClassReference ClazzRef;
      public BlueprintArchetypeReference ArchetypeRef;

      private BlueprintCharacterClass _clazz;
      private BlueprintCharacterClass Clazz
      {
        get
        {
          _clazz ??= ClazzRef.Get();
          return _clazz;
        }
      }
      private BlueprintArchetype _archetype;
      private BlueprintArchetype Archetype
      {
        get
        {
          _archetype ??= ArchetypeRef.Get();
          return _archetype;
        }
      }

      public UnitRequirements(BlueprintCharacterClassReference clazz, BlueprintArchetypeReference archetype)
      {
        ClazzRef = clazz;
        ArchetypeRef = archetype;
      }

      public bool AreMet(BlueprintSpellbook spellbook, UnitDescriptor unit)
      {
        return spellbook?.CharacterClass == Clazz
          && unit.Progression.GetClassData(Clazz).Archetypes.Contains(Archetype);
      }
    }
  }

  /// <summary>
  /// Adds selected spells to the character's spell list.
  /// </summary>
  /// 
  /// <remarks>
  /// Add to a BlueprintParametrizedFeature with <c>FeatureParameterType.Custom</c> where
  /// <c>BlueprintParameterVariants</c> contains the spells available to select. The <c>sourceSpellList</c> passed to
  /// the constructor should contain all the spells in the blueprint.
  /// </remarks>
  [AllowedOn(typeof(BlueprintParametrizedFeature), true)]
  [TypeId("5b143c58-e784-45de-87a1-b1bbae34db7c")]
  public class AddSpellToSpellList : UnitFactComponentDelegate
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(AddSpellToSpellList));

    private readonly BlueprintCharacterClassReference Clazz;
    private readonly BlueprintSpellListReference SourceSpellList;

    /// <param name="clazz">Class spellbook to which the selected spell is added</param>
    /// <param name="sourceSpellList">Spell list used as the source for determining spell level</param>
    public AddSpellToSpellList(BlueprintCharacterClassReference clazz, BlueprintSpellListReference sourceSpellList)
    {
      Clazz = clazz;
      SourceSpellList = sourceSpellList;
    }

    /// <summary>
    /// Adds the selected spell to <see cref="UnitPartExpandedSpellList"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// This is actually called in the level up UI every time a spell is selected. This actually doesn't matter because
    /// when the user changes the selection, the UnitPart is reset so there won't be any issues with multiple spells
    /// being saved as available.
    /// </remarks>
    public override void OnActivate()
    {
      try
      {
        if (Param?.Blueprint is not BlueprintAbility spell)
        {
          Logger.Warning($"Invalid blueprint type: {Param?.Blueprint?.GetType()}");
          return;
        }

        Logger.Verbose($"Adding {spell.Name} to {Owner.CharacterName} - {Clazz}");
        var spellRef = spell.ToReference<BlueprintAbilityReference>();
        int spellLevel =
          SourceSpellList.Get().SpellsByLevel.Where(list => list.m_Spells.Contains(spellRef)).First().SpellLevel;
        Owner.Ensure<UnitPartExpandedSpellList>().AddSpell(
          Clazz, spellLevel, spell.ToReference<BlueprintAbilityReference>());
      }
      catch (Exception e)
      {
        Logger.LogException("Failed to add extra spell to spell list.", e);
      }
    }

    public override void OnDeactivate()
    {
      try
      {
        if (Param?.Blueprint is not BlueprintAbility spell)
        {
          Logger.Warning($"Invalid blueprint type: {Param?.Blueprint?.GetType()}");
          return;
        }

        Logger.Verbose($"Removing {spell.Name} from {Owner.CharacterName} - {Clazz}");
        var spellRef = spell.ToReference<BlueprintAbilityReference>();
        int spellLevel =
          SourceSpellList.Get().SpellsByLevel.Where(list => list.m_Spells.Contains(spellRef)).First().SpellLevel;
        Owner.Ensure<UnitPartExpandedSpellList>().RemoveSpell(
          Clazz, spellLevel, spell.ToReference<BlueprintAbilityReference>());
      }
      catch (Exception e)
      {
        Logger.LogException("Failed to remove extra spell to spell list.", e);
      }
    }
  }
}
