using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Craft;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Spells
{
  internal class DimensionalBlade
  {
    private const string FeatureName = "DimensionalBlade";

    internal const string DisplayName = "DimensionalBlade.Name";
    private const string Description = "DimensionalBlade.Description";

    private const string BuffName = "DimensionalBlade.Buff";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.DimensionalBladeSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("DimensionalBlade.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.DimensionalBladeBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.DimensionalBladeSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.DimensionalBladeBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .AddComponent<DimensionalBladeComponent>()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.DimensionalBladeSpell,
          SpellSchool.Conjuration,
          canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.ArcaneAccuracyAbility.Reference.Get().Icon)
        .SetLocalizedDuration(Duration.OneRound)
        .SetRange(AbilityRange.Personal)
        .SetIgnoreSpellResistanceForAlly(true)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Swift)
        .SetAvailableMetamagic(Metamagic.CompletelyNormal, Metamagic.Extend)
        .AddToSpellLists(
          level: 4,
          SpellList.Bloodrager,
          SpellList.Paladin)
        .AddToSpellList(4, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddToSpellLists(
          level: 5,
          SpellList.Magus,
          SpellList.Inquisitor)
        .AddToSpellLists(
          level: 6,
          SpellList.Cleric,
          SpellList.Wizard)
        .AddAbilityEffectRunAction(actions: ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(1)))
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.None,
          spellType: CraftSpellType.Buff)
        .Configure();
    }

    [TypeId("53a29a8c-a710-429a-9c1c-6eb9ac4aaa5f")]
    private class DimensionalBladeComponent :
      UnitBuffComponentDelegate,
      IInitiatorRulebookHandler<RuleCalculateWeaponStats>,
      IInitiatorRulebookHandler<RuleAttackWithWeaponResolve>,
      IInitiatorRulebookHandler<RuleAttackRoll>
    {

      public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt) { }

      public void OnEventAboutToTrigger(RuleAttackWithWeaponResolve evt)
      {
        try
        {
          if (!Applies(evt.AttackWithWeapon.WeaponStats))
            return;

          var weapon = evt.AttackWithWeapon.Weapon;
          if (weapon.Blueprint.DamageType.IsPhysical
            && weapon.Blueprint.DamageType.Physical.Form == PhysicalDamageForm.Bludgeoning)
          {
            Logger.Verbose(() => "Halving damage for bludgeoning weapon");
            evt.Damage.Half = true;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("DimensionalBladeComponent.OnEventAboutToTrigger(RuleAttackWithWeaponResolve)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleAttackRoll evt)
      {
        try
        {
          if (!Applies(evt.RuleAttackWithWeapon?.WeaponStats))
            return;

          Logger.Verbose(() => "Changing to touch attack");
          evt.AttackType = AttackType.Touch;
        }
        catch (Exception e)
        {
          Logger.LogException("DimensionalBladeComponent.OnEventAboutToTrigger(RuleAttackRoll)", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
      {
        try
        {
          if (!Applies(evt))
            return;

          var dmg = evt.DamageDescription[0];
          if (dmg.TypeDescription.Type == DamageType.Physical
            && dmg.TypeDescription.Physical.Form == PhysicalDamageForm.Bludgeoning)
          {
            Logger.Verbose(() => "Changing damage to slashing");
            dmg.TypeDescription.Physical.Form = PhysicalDamageForm.Slashing;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("DimensionalBladeComponent.OnEventDidTrigger(RuleCalculateWeaponStats)", e);
        }
      }

      public void OnEventDidTrigger(RuleAttackWithWeaponResolve evt) { }

      public void OnEventDidTrigger(RuleAttackRoll evt) { }

      private bool Applies(RuleCalculateWeaponStats weaponStats)
      {
        if (weaponStats is null)
        {
          Logger.Verbose(() => "No weapon stats");
          return false;
        }

        if (weaponStats.IsSecondary || weaponStats.Weapon.Blueprint.IsNatural)
        {
          Logger.Verbose(() => "Secondary | natural weapon");
          return false;
        }

        if (!weaponStats.Weapon.Blueprint.IsMelee)
        {
          Logger.Verbose(() => "Ranged weapon");
          return false;
        }

        return true;
      }
    }
  }
}
