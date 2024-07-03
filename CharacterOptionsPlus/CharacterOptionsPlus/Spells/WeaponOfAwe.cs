﻿using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.Items.Ecnchantments;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Craft;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static Kingmaker.UI.GenericSlot.EquipSlotBase;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Spells
{
  internal class WeaponOfAwe
  {
    private const string FeatureName = "WeaponOfAwe";

    internal const string DisplayName = "WeaponOfAwe.Name";
    private const string Description = "WeaponOfAwe.Description";

    private const string MainHandAbility = "WeaponOfAwe.MainHand";
    private const string MainHandName = "WeaponOfAwe.MainHand.Name";
    private const string OffHandAbility = "WeaponOfAwe.OffHand";
    private const string OffHandName = "WeaponOfAwe.OffHand.Name";

    private const string Enchantment = "WeaponOfAwe.Enchantment";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.WeaponOfAweSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("WeaponOfAwe.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      WeaponEnchantmentConfigurator.New(Enchantment, Guids.WeaponOfAweEnchant).Configure();
      AbilityConfigurator.New(MainHandAbility, Guids.WeaponOfAweMainHand).Configure();
      AbilityConfigurator.New(OffHandAbility, Guids.WeaponOfAweOffHand).Configure();
      AbilityConfigurator.New(FeatureName, Guids.WeaponOfAweSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var enchant = WeaponEnchantmentConfigurator.New(Enchantment, Guids.WeaponOfAweEnchant)
        .SetEnchantName(DisplayName)
        .SetDescription(Description)
        .AddComponent(new WeaponDamageBonus(2, descriptor: ModifierDescriptor.Sacred))
        .AddComponent(
          new WeaponCriticalEffect(
            ActionsBuilder.New().ApplyBuff(BuffRefs.Shaken.ToString(), ContextDuration.Fixed(1)).Build()))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.WeaponOfAweSpell,
          SpellSchool.Transmutation,
          canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(BuffRefs.ArcaneStrikeBuff.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.MinutePerLevel)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(friends: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetIgnoreSpellResistanceForAlly(true)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Quicken,
          Metamagic.Reach)
        .AddToSpellLists(level: 2, SpellList.Cleric, SpellList.Inquisitor, SpellList.Paladin, SpellList.Warpriest)
        .AddAbilityVariants(
          new() { ConfigureVariant(mainHand: true, enchant), ConfigureVariant(mainHand: false, enchant) })
        .Configure();
    }

    private static BlueprintAbility ConfigureVariant(bool mainHand, BlueprintWeaponEnchantment enchantment)
    {
      return AbilityConfigurator.NewSpell(
          mainHand ? MainHandAbility : OffHandAbility,
          mainHand ? Guids.WeaponOfAweMainHand : Guids.WeaponOfAweOffHand,
          SpellSchool.Transmutation,
          canSpecialize: true,
          SpellDescriptor.Emotion,
          SpellDescriptor.MindAffecting,
          SpellDescriptor.Fear)
        .SetDisplayName(mainHand ? MainHandName : OffHandName)
        .SetDescription(Description)
        .SetIcon(BuffRefs.ArcaneStrikeBuff.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.MinutePerLevel)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(friends: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetIgnoreSpellResistanceForAlly(true)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Quicken,
          Metamagic.Reach)
        .SetParent(Guids.WeaponOfAweSpell)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddComponent(
          new AbilityTargetHasWeaponSubcategory(exclude: true, mainHand: mainHand, WeaponSubCategory.Natural))
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .EnchantWornItem(
              ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes),
              enchantment: enchantment,
              slot: mainHand ? SlotType.PrimaryHand : SlotType.SecondaryHand))
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.None,
          spellType: CraftSpellType.Buff)
        .Configure();
    }
  }
}
