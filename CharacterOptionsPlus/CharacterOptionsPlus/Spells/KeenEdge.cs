using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
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

    private const string MainHandAbility = "KeenEdge.MainHand";
    private const string MainHandName = "KeenEdge.MainHand.Name";
    private const string OffHandAbility = "KeenEdge.OffHand";
    private const string OffHandName = "KeenEdge.OffHand.Name";

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

      AbilityConfigurator.New(MainHandAbility, Guids.WeaponOfAweMainHand).Configure();
      AbilityConfigurator.New(OffHandAbility, Guids.WeaponOfAweOffHand).Configure();
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
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Quicken,
          Metamagic.Reach)
        .AddToSpellLists(level: 3, SpellList.Bloodrager, SpellList.Inquisitor, SpellList.Magus, SpellList.Wizard)
        .AddAbilityVariants(new() { ConfigureVariant(mainHand: true), ConfigureVariant(mainHand: false) })
        .Configure();
    }

    private static BlueprintAbility ConfigureVariant(bool mainHand)
    {
      return AbilityConfigurator.NewSpell(
          mainHand ? MainHandAbility : OffHandAbility,
          mainHand ? Guids.KeenEdgeMainHand : Guids.KeenEdgeOffHand,
          SpellSchool.Transmutation,
          canSpecialize: true)
        .SetDisplayName(mainHand ? MainHandName : OffHandName)
        .SetDescription(Description)
        .SetIcon(BuffRefs.WeaponBondKeenBuff.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.TenMinutesPerLevel)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(self: true, friends: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Quicken,
          Metamagic.Reach)
        .SetParent(Guids.KeenEdgeSpell)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .EnchantWornItem(
              ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.TenMinutes),
              WeaponEnchantmentRefs.Keen.ToString(),
              slot: mainHand ? SlotType.PrimaryHand : SlotType.SecondaryHand))
        .AddComponent(mainHand ? AbilityTargetHasWeaponEquipped.MainHand() : AbilityTargetHasWeaponEquipped.OffHand())
        .AddComponent(
          new AbilityTargetHasWeaponDamageType(
            exclude: false, mainHand: mainHand, PhysicalDamageForm.Piercing, PhysicalDamageForm.Slashing))
        .Configure();
    }
  }
}
