using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Conditions;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using static CharacterOptionsPlus.UnitParts.UnitPartTouchCharges;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class Frostbite
  {
    private const string FeatureName = "Frostbite";
    private const string EffectName = "Frostbite.Effect";
    private const string BuffName = "Frostbite.Buff";

    internal const string DisplayName = "Frostbite.Name";
    private const string Description = "Frostbite.Description";
    private const string TouchName = "Frostbite.Touch.Name";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.FrostbiteSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("Frostbite.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.FrostbiteBuff).Configure();
      AbilityConfigurator.New(EffectName, Guids.FrostbiteEffect).Configure();
      AbilityConfigurator.New(FeatureName, Guids.FrostbiteSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = BuffRefs.Fatigued.Reference.Get().Icon;
      var buff = BuffConfigurator.New(BuffName, Guids.FrostbiteBuff)
        .CopyFrom(BuffRefs.Fatigued)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .AddHealTrigger(onHealDamage: true, action: ActionsBuilder.New().RemoveSelf())
        .Configure();

      var effectAbility = AbilityConfigurator.NewSpell(
          EffectName, Guids.FrostbiteEffect, SpellSchool.Transmutation, canSpecialize: false, SpellDescriptor.Cold)
        .SetDisplayName(TouchName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Piercing,
          (Metamagic)CustomMetamagic.Rime)
        .AddComponent(new TouchCharges(ContextValues.Rank()))
        .AddAbilityDeliverTouch(touchWeapon: ItemWeaponRefs.TouchItem.ToString())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .DealDamage(
              DamageTypes.Energy(DamageEnergyType.Cold),
              ContextDice.Value(DiceType.D6, diceCount: ContextValues.Constant(1), bonus: ContextValues.Rank()))
            .Conditional(
              conditions: ConditionsBuilder.New()
                .UseOr()
                .Add<HasCondition>(c => c.Condition = UnitCondition.Fatigued)
                .Add<HasCondition>(c => c.Condition = UnitCondition.Exhausted),
              ifFalse: ActionsBuilder.New().ApplyBuffPermanent(buff)))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.FrostbiteSpell, SpellSchool.Transmutation, canSpecialize: true, SpellDescriptor.Cold)
        .CopyFrom(effectAbility)
        .SetDisplayName(DisplayName)
        .SetShouldTurnToTarget(true)
        .AddToSpellLists(
          level: 1, SpellList.Bloodrager, SpellList.Druid, SpellList.Magus, SpellList.Shaman, SpellList.Witch)
        .AddAbilityEffectStickyTouch(touchDeliveryAbility: effectAbility)
        .Configure(delayed: true);
    }
  }
}
