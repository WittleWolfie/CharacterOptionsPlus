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
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
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
  // TODO: Better implementation might just be to replace UnitDescriptor.DemandSpellbook and use a dynamic spellbook.
  //       This might actually just replace the existing implementation since I think the spell list is generally
  //       pulled from there. It's unclear though how this would work w/ the VM for spell selection, but maybe better?
  // TODO: The spell shows up correctly but cannot be selected if you don't take it the level that you add it.

  /// <summary>
  /// Adds additional spells to the character's spell list. These are not spells known and must still otherwise be
  /// selected as spells known before they can be cast.
  /// </summary>
  public class UnitPartExpandedSpellList : OldStyleUnitPart
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(UnitPartExpandedSpellList));

    [JsonProperty]
    public Dictionary<BlueprintCharacterClassReference, List<SpellLevelList>> ExtraSpells = new();

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
        spellLevelList = new SpellLevelList(level);
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
    /// Returns a spellbook with the expanded spell list.
    /// </summary>
    public bool GetSpellbook(BlueprintSpellbook blueprint, out BlueprintSpellbook spellbook)
    {
      spellbook = blueprint;
      var clazz = blueprint.m_CharacterClass;
      if (!ExtraSpells.ContainsKey(clazz))
        return false;

      var spellbookName = $"ExpandedSpellbook_{Owner.Unit.UniqueId}_{clazz}";
      if (UpToDate.ContainsKey(clazz) && UpToDate[clazz] && BlueprintTool.TryGet(spellbookName, out spellbook))
        return true;

      var spellList = GetExpandedSpellList(clazz, blueprint.SpellList, null);

      SpellbookConfigurator configurator;
      if (BlueprintTool.TryGet<BlueprintSpellbook>(spellbookName, out var existingSpellbook))
      {
        configurator = SpellbookConfigurator.For(existingSpellbook);
      }
      else
      {
        var guid = Guids.ReserveDynamic();
        Logger.NativeLog(
          $"Creating expanded spellbook for {Owner.CharacterName} - {clazz}, using dynamic guid {guid}");
        configurator =
          SpellbookConfigurator.New(spellbookName, guid)
            .SetAllSpellsKnown(blueprint.AllSpellsKnown)
            .SetCanCopyScrolls(blueprint.CanCopyScrolls)
            .SetCantripsType(blueprint.CantripsType)
            .SetCasterLevelModifier(blueprint.CasterLevelModifier)
            .SetCastingAttribute(blueprint.CastingAttribute)
            .SetHasSpecialSpellList(blueprint.HasSpecialSpellList)
            .SetIsArcane(blueprint.IsArcane)
            .SetIsArcanist(blueprint.IsArcanist)
            .SetCharacterClass(blueprint.CharacterClass)
            .SetSpellsKnown(blueprint.SpellsKnown)
            .SetSpellSlots(blueprint.SpellSlots)
            .SetSpellsPerDay(blueprint.SpellsPerDay)
            .SetName(spellbook.Name)
            .SetSpecialSpellListName(blueprint.SpecialSpellListName)
            .SetSpellsPerLevel(blueprint.SpellsPerLevel)
            .SetSpontaneous(blueprint.Spontaneous);
      }
      spellbook = configurator.SetSpellList(spellList).Configure();
      UpToDate[blueprint.m_CharacterClass] = true;
      return true;
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
    /// Returns the expanded spell list, either by fetching from the cache or creating it.
    /// </summary>
    private BlueprintSpellList GetExpandedSpellList(
      BlueprintCharacterClassReference clazz, BlueprintSpellList spellList, BlueprintSpellList expandedSpellList)
    {
      var spellListName = $"ExpandedSpellList_{Owner.Unit.UniqueId}_{clazz}";
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
    private static SpellLevelList[] Combine(SpellLevelList[] baseList, List<SpellLevelList> extraSpells)
    {
      var spellLevelList = new SpellLevelList[baseList.Length];
      for (int i = 0; i < baseList.Length; i++)
      {
        var list = new SpellLevelList(baseList[i].SpellLevel);
        list.m_Spells.AddRange(baseList[i].m_Spells);

        var extraList = extraSpells.Where(l => l.SpellLevel == list.SpellLevel).FirstOrDefault();
        if (extraList is not null)
        {
          list.m_Spells.AddRange(extraList.m_Spells);
        }

        spellLevelList[i] = list;
      }
      return spellLevelList;
    }


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
    /// To make sure the spells always display properly in the spell list, create a dynamic spellbook and spell list
    /// which is inserted into the UnitDescriptor.
    /// </summary>
    [HarmonyPatch(typeof(UnitDescriptor))]
    static class UnitDescriptor_Patch
    {
      [HarmonyPatch(nameof(UnitDescriptor.DemandSpellbook), typeof(BlueprintSpellbook)), HarmonyPostfix]
      static void DemandSpellbook(UnitDescriptor __instance, BlueprintSpellbook blueprint, ref Spellbook __result)
      {
        try
        {
          if (__instance.Ensure<UnitPartExpandedSpellList>().GetSpellbook(blueprint, out var spellbook))
          {
            // Check to see if the replacement was already done
            if (__instance.m_Spellbooks[blueprint].Blueprint == spellbook)
              return;

            Logger.NativeLog($"Swapping spellbook for {__instance.CharacterName}");
            __result = new Spellbook(__instance, spellbook);
            __instance.m_Spellbooks[blueprint] = __result;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to swap spellbook.", e);
        }
      }
    }

    /// <summary>
    /// The LevelUpState gets cleared periodically through level up, so in order to keep the reference to the dynamic
    /// spell list this patch replaces the spell list at the last posssible moment, just before the spell selection is
    /// applied.
    /// </summary>
    [HarmonyPatch(typeof(SelectSpell))]
    static class SelectSpell_Patch
    {
      [HarmonyPatch(nameof(SelectSpell.Check)), HarmonyPrefix]
      static void Check(SelectSpell __instance, LevelUpState state, UnitDescriptor unit)
      {
        try
        {
          // During character generation the character is renamed after the list is created. This allows mapping from
          // the existing list to the new list, because __intance.SpellList will point to the existing list and cannot
          // be changed.
          var expandedSpellList =
            __instance.SpellList.name.StartsWith("ExpandedSpellList") ? __instance.SpellList : null;
          SpellSelectionData toRemove = null, toAdd = null;
          foreach (var selection in state.SpellSelections)
          {
            if (selection.Spellbook.m_CharacterClass == __instance.Spellbook.m_CharacterClass)
            {
              if (
                unit.Ensure<UnitPartExpandedSpellList>()
                  .GetSpellSelection(selection, out var newSelection, expandedSpellList: expandedSpellList))
              {
                Logger.NativeLog($"Replacing spell selection.");
                toRemove = selection;
                toAdd = newSelection;
                break;
              }
            }
          }

          if (toRemove is not null)
            state.SpellSelections.Remove(toRemove);

          if (toAdd is not null)
            state.SpellSelections.Add(toAdd);
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to swap selection data in SelectSpell", e);
        }
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
