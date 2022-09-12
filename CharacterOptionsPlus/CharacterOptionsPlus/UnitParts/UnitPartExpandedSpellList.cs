using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Spells;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.UnitParts
{
  // TODO: Show unlocked spells w/ Show all spells in spellbook?
  // TODO: Fix tooltips during selection to show spell tooltip?

  /// <summary>
  /// Adds additional spells to the character's spell list. These are not spells known and must still otherwise be
  /// selected as spells known before they can be cast.
  /// </summary>
  public class UnitPartExpandedSpellList : OldStyleUnitPart
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(UnitPartExpandedSpellList));

    [JsonProperty]
    public Dictionary<BlueprintCharacterClassReference, List<SpellsByLevel>> ExtraSpells = new();

    [JsonIgnore]
    public Dictionary<BlueprintCharacterClassReference, bool> UpToDate = new();

    /// <summary>
    /// Add a spell to the character's spell list.
    /// </summary>
    public void AddSpell(BlueprintCharacterClassReference clazz, int level, BlueprintAbilityReference spell)
    {
      Logger.NativeLog($"Adding to spell list for {Owner.CharacterName} - {clazz}");

      if (!ExtraSpells.ContainsKey(clazz))
        ExtraSpells[clazz] = new();

      var spellList = ExtraSpells[clazz];
      var spellLevelList = spellList.Where(list => list.SpellLevel == level).FirstOrDefault();
      if (spellLevelList is null)
      {
        spellLevelList = new SpellsByLevel(level);
        spellList.Add(spellLevelList);
      }
      spellLevelList.m_Spells.Add(spell);
      UpToDate[clazz] = false;
    }

    /// <summary>
    /// Removes a spell from the character's spell list.
    /// </summary>
    public void RemoveSpell(BlueprintCharacterClassReference clazz, int level, BlueprintAbilityReference spell)
    {
      Logger.NativeLog($"Removing from spell list for {Owner.CharacterName} - {clazz}");

      if (!ExtraSpells.ContainsKey(clazz))
        return;

      var spellLevelList =
        ExtraSpells[clazz].Where(list => list.SpellLevel == level).FirstOrDefault() ?? new SpellLevelList(level);
      spellLevelList.m_Spells = spellLevelList.m_Spells.Where(s => s != spell).ToList();
      UpToDate[clazz] = false;
    }

    /// <summary>
    /// Populates <paramref name="newSelection"/> with  a modified <c>SpellSelectionData</c> which uses the
    /// expanded spell list.
    /// </summary>
    ///
    /// <returns>True if the spell list is modified, false otherwise</returns>
    public bool GetSpellSelection(
      SpellSelectionData spellSelection,
      out SpellSelectionData newSelection,
      BlueprintSpellList expandedSpellList = null)
    {
      newSelection = spellSelection;
      if (!ExtraSpells.ContainsKey(spellSelection?.Spellbook.m_CharacterClass))
        return false;

      var spellList =
        GetExpandedSpellList(spellSelection.Spellbook.m_CharacterClass, spellSelection.SpellList, expandedSpellList);
      // Check if the spell list changed--if not return false so we don't refresh things that don't need refreshing.
      if (spellList.SpellsByLevel.Length == spellSelection.SpellList.SpellsByLevel.Length)
      {
        var spellListChanged = false;
        for (int i = 0; i < spellSelection.SpellList.SpellsByLevel.Length && i < spellList.SpellsByLevel.Length; i++)
        {
          var originalLevel = spellSelection.SpellList.SpellsByLevel[i];
          var expandedLevel = spellList.SpellsByLevel[i];
          if (!originalLevel.m_Spells.SequenceEqual(expandedLevel.m_Spells))
          {
            spellListChanged = true;
            break;
          }
        }

        if (!spellListChanged)
          return false;
      }

      Logger.NativeLog(
        $"Returning spell selection with expanded spells for {Owner.CharacterName} - {spellSelection.Spellbook.m_CharacterClass}");
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
    public bool GetSpellList(BlueprintSpellbook spellbook, BlueprintSpellList spellList, out BlueprintSpellList expandedSpellList)
    {
      expandedSpellList = null;
      if (!ExtraSpells.ContainsKey(spellbook.m_CharacterClass))
        return false;

      Logger.NativeLog($"Fetching expanded spell list for {Owner.CharacterName} - {spellbook.CharacterClass.Name}");
      expandedSpellList = GetExpandedSpellList(spellbook.m_CharacterClass, spellList, null);
      return true;
    }

    /// <summary>
    /// Returns the expanded spell list, either by fetching from the cache or creating it.
    /// </summary>
    private BlueprintSpellList GetExpandedSpellList(
      BlueprintCharacterClassReference clazz, BlueprintSpellList spellList, BlueprintSpellList expandedSpellList) // TODO: Remove expandedSpellList? I don't think we need it.
    {
      var spellListName = $"ExpandedSpellList_{Owner.CharacterName}_{clazz}";
      SpellListConfigurator expandedList;
      if (expandedSpellList is not null)
      {
        expandedList = SpellListConfigurator.For(expandedSpellList);
      }
      else if (BlueprintTool.TryGet<BlueprintSpellList>(spellListName, out var existingSpellList))
      {
        expandedList = SpellListConfigurator.For(existingSpellList);
      }
      else
      {
        var guid = Guids.ReserveDynamic();
        Logger.NativeLog(
          $"Creating expanded spell list for {Owner.CharacterName} - {clazz}, using dynamic guid {guid}");
        expandedList = SpellListConfigurator.New(spellListName, guid);
      }
      return expandedList.SetSpellsByLevel(Combine(spellList.SpellsByLevel, ExtraSpells[clazz])).Configure();
    }

    /// <summary>
    /// Returns a combined spell level list with the <paramref name="extraSpells"/> added.
    /// </summary>
    private static SpellLevelList[] Combine(SpellLevelList[] baseList, List<SpellsByLevel> extraSpells)
    {
      var spellLevelList = new SpellLevelList[baseList.Length];
      for (int i = 0; i < baseList.Length; i++)
      {
        var list = new SpellLevelList(baseList[i].SpellLevel);
        list.m_Spells.AddRange(baseList[i].m_Spells);

        var extraList = extraSpells.Where(l => l.SpellLevel == list.SpellLevel).FirstOrDefault();
        if (extraList is not null && extraList.m_Spells is not null)
        {
          list.m_Spells.AddRange(extraList.m_Spells);
        }

        spellLevelList[i] = list;
      }
      return spellLevelList;
    }

    // TODO: Maybe I don't need this? It should be re-fetching the selection each time one is selected, in which case
    // the new patches should cover it. If you don't need this you also don't need _any_ of the weird spell selection logic!
    /// <summary>
    /// Patch responsible for swapping the selection data with the expanded version before it is bound / viewed in the
    /// level up UI.
    /// </summary>
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
            Logger.NativeLog($"Swapping selection data.");
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
          Logger.LogException("Failed to swap selection data.", e);
        }
      }
    }

    /// <summary>
    /// Redirects attempts to generate a spell selection from the default spell list to the expanded spell lists.
    /// </summary>
    [HarmonyPatch(typeof(LevelUpState))]
    static class LevelUpState_Patch
    {
      [HarmonyPatch(nameof(LevelUpState.DemandSpellSelection)), HarmonyPrefix]
      static void DemandSpellSelection(LevelUpState __instance, BlueprintSpellbook spellbook, ref BlueprintSpellList spellList)
      {
        try
        {
          Logger.NativeLog($"DEMANDING THINGS");
          if (
            __instance.Unit.Ensure<UnitPartExpandedSpellList>()
                .GetSpellList(spellbook, spellList, out var expandedSpellList)
              && expandedSpellList != spellList)
          {
            Logger.NativeLog($"DemandSpellSelection: Replacing spell list");
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
          Logger.NativeLog($"FETCHING THINGS"); // TODO: Check wheter this is necessary at all
          if (__instance.Unit.Ensure<UnitPartExpandedSpellList>().GetSpellList(spellbook, spellList, out var expandedSpellList))
          {
            Logger.NativeLog($"GetSpellSelection: Replacing spell list");
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
    /// Simple wrapper to allow serialization of SpellLevelList.
    /// </summary>
    public class SpellsByLevel : SpellLevelList
    {
      public SpellsByLevel(int spellLevel) : base(spellLevel) { }

      [JsonConstructor]
      public SpellsByLevel(int spellLevel, List<BlueprintAbilityReference> spells) : base(spellLevel)
      {
        m_Spells = spells;
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
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AddSpellToSpellList));

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
          return;

        Logger.Log($"Adding {spell.Name} to {Owner.CharacterName} - {Clazz}");
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
          return;

        Logger.Log($"Removing {spell.Name} from {Owner.CharacterName} - {Clazz}");
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
