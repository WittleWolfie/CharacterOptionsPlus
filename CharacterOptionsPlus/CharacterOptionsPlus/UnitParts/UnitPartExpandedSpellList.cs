using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Mechanics;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.UnitParts
{
  /// <summary>
  /// Adds additional spells to the character's spell list. These are not spells known and must still otherwise be
  /// selected as spells known before they can be cast.
  /// </summary>
  public class UnitPartExpandedSpellList : OldStyleUnitPart
  {
    public readonly List<BlueprintAbility> ExtraSpells = new();
  }

  /// <summary>
  /// Allows the user to select spells and add them to their spell list.
  /// </summary>
  // TODO: Don't forget attributes and typeid and shit
  public class AddSpellsToSpellList : UnitFactComponentDelegate, IUnitCompleteLevelUpHandler
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AddSpellsToSpellList));

    private readonly BlueprintSpellListReference SpellList;
    private readonly ContextValue Count;

    public AddSpellsToSpellList(BlueprintSpellListReference spellList, ContextValue count)
    {
      SpellList = spellList;
      Count = count;
    }

    public override void OnActivate()
    {
      LevelUpController controller = Kingmaker.Game.Instance?.LevelUpController;
      if (controller is null)
        return;

      // TODO: I'm not even sure I need this activate but generally my thought is:
      // * Patch `AddClassLevels.PerformSelections`
      // * When it is called last (line 308 / priority null), if AddSpellsToSpellList is there then inject a selection for spells
      // * Patch `AddClassLevels.PerformSpellSelections` or something in `LevelUpController` to add spells from UnitPart
    }

    public void HandleUnitCompleteLevelup(UnitEntityData unit)
    {
      throw new System.NotImplementedException();
    }
  }
}
