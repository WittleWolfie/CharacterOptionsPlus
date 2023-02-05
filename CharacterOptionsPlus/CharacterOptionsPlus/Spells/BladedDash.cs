using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Spells
{
  internal class BladedDash
  {
    private const string FeatureName = "BladedDash";
    private const string FeatureNameGreater = "BladedDash.Greater";

    internal const string DisplayName = "BladedDash.Name";
    private const string Description = "BladedDash.Description";
    private const string DisplayNameGreater = "BladedDash.Greater.Name";
    private const string DescriptionGreater = "BladedDash.Greater.Description";

    private const string BuffName = "BladedDash.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "bladeddash.png";
    private const string IconNameGreater = IconPrefix + "bladeddashgreater.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.BladedDashSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("BladedDash.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.BladedDashBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.BladedDashSpell).Configure();
      AbilityConfigurator.New(FeatureNameGreater, Guids.BladedDashGreaterSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.BladedDashBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .AddComponent<BladedDashBuff>()
        .Configure();

      var bladedDash = AbilityConfigurator.NewSpell(
          FeatureName, Guids.BladedDashSpell, SpellSchool.Transmutation, canSpecialize: false)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetCustomRange(30.Feet())
        .AllowTargeting(enemies: true, friends: true)
        .SetAnimation(CastAnimationStyle.Special)
        .SetActionType(CommandType.Standard)
        .SetType(AbilityType.Spell)
        .SetNeedEquipWeapons(true)
        .SetAvailableMetamagic(Metamagic.CompletelyNormal, Metamagic.Quicken)
        .AddToSpellLists(level: 2, SpellList.Bard, SpellList.Magus)
        .AddComponent(new AbilityCasterHasWeaponSubcategory(WeaponSubCategory.Melee))
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .CastSpell(AbilityRefs.DimensionDoorCasterOnly.ToString())
            .ApplyBuff(buff, ContextDuration.Fixed(1), toCaster: true)
            .MeleeAttack()
            .RemoveBuff(buff, toCaster: true))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureNameGreater, Guids.BladedDashGreaterSpell, SpellSchool.Transmutation, canSpecialize: false)
        .CopyFrom(bladedDash)
        .SetDisplayName(DisplayNameGreater)
        .SetDescription(DescriptionGreater)
        .SetIcon(IconNameGreater)
        .SetRange(AbilityRange.Projectile)
        .AddToSpellLists(level: 5, SpellList.Bard, SpellList.Magus)
        .AddComponent(new AbilityCasterHasWeaponSubcategory(WeaponSubCategory.Melee))
        .AddAbilityDeliverProjectile(
          projectiles: new() { ProjectileRefs.Kinetic_AirBlastLine00.ToString() },
          length: 30.Feet(),
          type: AbilityProjectileType.Line,
          needAttackRoll: true)
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New().IsMainTarget(),
              ifTrue: ActionsBuilder.New().CastSpell(AbilityRefs.DimensionDoorCasterOnly.ToString()))
            .Conditional(
              ConditionsBuilder.New().IsEnemy(),
              ifTrue: ActionsBuilder.New()
                .ApplyBuff(buff, ContextDuration.Fixed(1), toCaster: true)
                .MeleeAttack()
                .RemoveBuff(buff, toCaster: true)))
        .Configure();
    }

    [TypeId("4784aa37-5464-41b1-923f-8eff1a932a6d")]
    private class BladedDashBuff :
      UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>
    {
      public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
      {
        try
        {
          var chaMod = (Owner.Stats.GetStat(StatType.Charisma) as ModifiableValueAttributeStat).Bonus;
          var intMod = (Owner.Stats.GetStat(StatType.Intelligence) as ModifiableValueAttributeStat).Bonus;
          Logger.Verbose(() => $"Adding {chaMod} | {intMod} to attack");
          evt.AddModifier(chaMod > intMod ? chaMod : intMod, Fact, ModifierDescriptor.Circumstance);
        }
        catch (Exception e)
        {
          Logger.LogException("BladedDashBuff.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }
    }
  }
}
