using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class ShadowTrap
  {
    private const string FeatureName = "ShadowTrap";

    internal const string DisplayName = "ShadowTrap.Name";
    private const string Description = "ShadowTrap.Description";

    private const string BuffName = "ShadowTrap.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ShadowTrapSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ShadowTrap.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.ShadowTrapBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.ShadowTrapSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.ShadowTrapSpell,
          SpellSchool.Illusion,
          canSpecialize: true,
          SpellDescriptor.MovementImpairing)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(
          level: 1,
          SpellList.Bard,
          SpellList.Bloodrager,
          SpellList.Cleric,
          SpellList.Shaman,
          SpellList.Wizard,
          SpellList.Witch)
        .AddToSpellList(level: 1, ModSpellListRefs.AntipaladinSpelllist.ToString())
        // TODO: Add effect! Also you should probably just create a util in BPCore to set duration text
        .Configure();
    }
  }
}
