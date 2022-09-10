using BlueprintCore.Blueprints.Configurators.Classes.Spells;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.UnitParts
{
  /// <summary>
  /// Adds additional spells to the character's spell list. These are not spells known and must still otherwise be
  /// selected as spells known before they can be cast.
  /// </summary>
  public class UnitPartExpandedSpellList : OldStyleUnitPart
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(UnitPartExpandedSpellList));

    [JsonProperty]
    public Dictionary<BlueprintCharacterClassReference, List<SpellLevelList>> ExtraSpells = new();

    public void AddSpells(
      BlueprintCharacterClassReference clazz, int level, params BlueprintAbilityReference[] spells)
    {
      Logger.NativeLog($"Adding to spell list for {Owner.CharacterName} - {clazz}");

      if (!ExtraSpells.ContainsKey(clazz))
        ExtraSpells.Add(clazz, new());

      var spellList = ExtraSpells[clazz];
      var spellLevelList = ExtraSpells[clazz].Where(list => list.SpellLevel == level).FirstOrDefault();
      if (spellLevelList is null)
      {
        spellLevelList = new SpellLevelList(level);
        spellList.Add(spellLevelList);
      }
      spellLevelList.m_Spells.AddRange(spells);

      // Update the cached spell list if it exists
      if (ExpandedSpellLists.ContainsKey(clazz))
        UpdateExpandedSpellList(clazz, spellLevelList);
    }

    public void RemoveSpells(
      BlueprintCharacterClassReference clazz, int level, params BlueprintAbilityReference[] spells)
    {
      Logger.NativeLog($"Removing from spell list for {Owner.CharacterName} - {clazz}");

      if (!ExtraSpells.ContainsKey(clazz))
        return;

      var spellLevelList =
        ExtraSpells[clazz].Where(list => list.SpellLevel == level).FirstOrDefault() ?? new SpellLevelList(level);
      spellLevelList.m_Spells = spellLevelList.m_Spells.Except(spells).ToList();

      // Update the cached spell list if it exists
      if (ExpandedSpellLists.ContainsKey(clazz))
        UpdateExpandedSpellList(clazz, spellLevelList);
    }

    public SpellSelectionData GetSpellSelection(SpellSelectionData spellSelection)
    {
      if (!ExtraSpells.ContainsKey(spellSelection?.Spellbook.m_CharacterClass))
        return spellSelection;
      
      Logger.NativeLog(
        $"Returning spell selection with expanded spells for {Owner.CharacterName} - {spellSelection.Spellbook.m_CharacterClass}");
      return new(
        spellSelection.Spellbook,
        GetExpandedSpellList(spellSelection.Spellbook.m_CharacterClass, spellSelection.SpellList));
    }

    private readonly Dictionary<BlueprintCharacterClassReference, BlueprintSpellList> ExpandedSpellLists =
      new();
    private BlueprintSpellList GetExpandedSpellList(
      BlueprintCharacterClassReference clazz, BlueprintSpellList spellList)
    {
      if (ExpandedSpellLists.ContainsKey(clazz))
        return ExpandedSpellLists[clazz];

      var guid = Guids.ReserveDynamic();
      Logger.NativeLog($"Creating expanded spell list for {Owner.CharacterName} - {clazz}, using dynamic guid {guid}");

      var extraSpells = ExtraSpells[clazz];
      var expandedSpellList =
        SpellListConfigurator.New($"ExpandedSpellList_{Owner.CharacterName}_{clazz}", guid)
          .SetSpellsByLevel(Combine(spellList.SpellsByLevel, extraSpells))
          .Configure();
      ExpandedSpellLists.Add(clazz, expandedSpellList);
      return expandedSpellList;
    }

    private void UpdateExpandedSpellList(BlueprintCharacterClassReference clazz, SpellLevelList newSpellLevelList)
    {
      Logger.NativeLog($"Updating expanded spell list for {Owner.CharacterName} - {clazz}");
      Replace(ExpandedSpellLists[clazz].SpellsByLevel, newSpellLevelList);
    }

    private static void Replace(SpellLevelList[] baseList, SpellLevelList newList)
    {
      for (int i = 0; i < baseList.Length; i++)
      {
        if (baseList[i].SpellLevel == newList.SpellLevel)
        {
          baseList[i].m_Spells = newList.m_Spells;
          return;
        }
      }
      throw new InvalidOperationException(
        $"Cannot add extra spell with level {newList.SpellLevel} which is not in the base list.");
    }

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

    [HarmonyPatch(typeof(CharGenSpellsPhaseVM))]
    static class CharGenSpellsPhaseVM_Patch
    {
      // TODO: Figure out why this doesn't work. Probably need to muck w/ Spellbook?
      [HarmonyPatch(nameof(CharGenSpellsPhaseVM.OnBeginDetailedView)), HarmonyPrefix]
      static void OnBeginDetailedView(CharGenSpellsPhaseVM __instance)
      {
        try
        {
          Logger.NativeLog("Swapping spell selection data");
          __instance.m_SelectionData =
            __instance.m_UnitDescriptor.Value.Ensure<UnitPartExpandedSpellList>()
              .GetSpellSelection(__instance.m_SelectionData);
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to swap spell selection data.", e);
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

    // TODO: This is called on every selection and OnDeactivate is NOT. Need to fix.
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
        Owner.Ensure<UnitPartExpandedSpellList>().AddSpells(
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
        Owner.Ensure<UnitPartExpandedSpellList>().AddSpells(
          Clazz, spellLevel, spell.ToReference<BlueprintAbilityReference>());
      }
      catch (Exception e)
      {
        Logger.LogException("Failed to remove extra spell to spell list.", e);
      }
    }
  }
}
