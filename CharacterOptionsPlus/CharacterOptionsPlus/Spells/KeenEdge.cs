using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Actions;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Enums.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static Kingmaker.UI.GenericSlot.EquipSlotBase;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Spells
{
  internal class KeenEdge
  {
    private const string FeatureName = "KeenEdge";

    internal const string DisplayName = "KeenEdge.Name";
    private const string Description = "KeenEdge.Description";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.KeenEdgeSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("KeenEdge.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.KeenEdgeSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.KeenEdgeSpell, SpellSchool.Transmutation, canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(BuffRefs.WeaponBondKeenBuff.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.TenMinutesPerLevel)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(self: true, friends: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Quicken,
          Metamagic.Reach)
        .AddToSpellLists(level: 3, SpellList.Bloodrager, SpellList.Inquisitor, SpellList.Magus, SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .EnchantWornItem(
              ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.TenMinutes),
              WeaponEnchantmentRefs.Keen.ToString(),
              slot: SlotType.PrimaryHand))
        .AddComponent<AbilityTargetHasWeaponEquipped>()
        .AddComponent(
          new AbilityTargetHasWeaponDamageType(
            exclude: false, PhysicalDamageForm.Piercing, PhysicalDamageForm.Slashing))
        .Configure();
    }
  }
}
