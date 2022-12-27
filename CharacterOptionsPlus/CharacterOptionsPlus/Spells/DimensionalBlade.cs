using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class DimensionalBlade
  {
    private const string FeatureName = "DimensionalBlade";

    internal const string DisplayName = "DimensionalBlade.Name";
    private const string Description = "DimensionalBlade.Description";

    private const string BuffName = "DimensionalBlade.Buff";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

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
          canSpecialize: true,
          SpellDescriptor.Summoning,
          SpellDescriptor.Evil)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(AbilityRefs.ArcaneAccuracyAbility.Reference.Get().Icon)
        .SetRange(AbilityRange.Personal)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Swift)
        .SetAvailableMetamagic(Metamagic.CompletelyNormal, Metamagic.Extend)
        .AddToSpellLists(
          level: 4,
          SpellList.Bloodrager,
          SpellList.Cleric,
          SpellList.Inquisitor,
          SpellList.Paladin,
          SpellList.Magus,
          SpellList.Wizard)
        .AddToSpellList(4, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddAbilityEffectRunAction(actions: ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(1)))
        .Configure();
    }

    [TypeId("53a29a8c-a710-429a-9c1c-6eb9ac4aaa5f")]
    private class DimensionalBladeComponent :
      UnitBuffComponentDelegate,
      IInitiatorRulebookHandler<RuleCalculateWeaponStats>,
      IInitiatorRulebookHandler<RuleCalculateAC>
    {
      public void OnEventAboutToTrigger(RuleCalculateAC evt) { }

      public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt) { }

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
            dmg.TypeDescription.Physical.Form = PhysicalDamageForm.Slashing;
            dmg.ModifyDice(new DiceFormula(dmg.Dice.Rolls / 2, dmg.Dice.Dice), Buff);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("DimensionalBladeComponent.OnEventDidTrigger(RuleCalculateWeaponStats)", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAC evt)
      {
        try
        {
          var attack = evt.Reason?.Rule as RuleAttackRoll;
          if (attack is null)
          {
            Logger.Warning("No attack");
            return;
          }

          var weaponStats = attack.WeaponStats;
          if (!Applies(weaponStats))
            return;

          int penalty = 0;
          foreach (var bonus in evt.AllBonuses)
          {
            if (bonus.Descriptor == ModifierDescriptor.Armor
                || bonus.Descriptor == ModifierDescriptor.ArmorEnhancement
                || bonus.Descriptor == ModifierDescriptor.ArmorFocus
                || bonus.Descriptor == ModifierDescriptor.NaturalArmor
                || bonus.Descriptor == ModifierDescriptor.NaturalArmorEnhancement
                || bonus.Descriptor == ModifierDescriptor.NaturalArmorForm
                || bonus.Descriptor == ModifierDescriptor.Shield
                || bonus.Descriptor == ModifierDescriptor.ShieldEnhancement
                || bonus.Descriptor == ModifierDescriptor.ShieldFocus)
            {
              var spellDescriptor = bonus.Fact.GetComponent<SpellDescriptorComponent>();
              if (spellDescriptor is not null && spellDescriptor.Descriptor.HasAnyFlag(SpellDescriptor.Force))
                continue;

              spellDescriptor = bonus.Fact.SourceAbility?.GetComponent<SpellDescriptorComponent>();
              if (spellDescriptor is not null && spellDescriptor.Descriptor.HasAnyFlag(SpellDescriptor.Force))
                continue;

              penalty += bonus.Value;
            }
          }
          evt.AddModifier(-penalty, Buff, ModifierDescriptor.UntypedStackable);
        }
        catch (Exception e)
        {
          Logger.LogException("DimensionalBladeComponent.OnEventDidTrigger(RuleCalculateAC)", e);
        }
      }

      private bool Applies(RuleCalculateWeaponStats weaponStats)
      {
        if (weaponStats is null)
          return false;

        if (weaponStats.IsSecondary || weaponStats.Weapon.Blueprint.IsNatural)
          return false;

        if (!weaponStats.Weapon.Blueprint.IsMelee)
          return false;

        return true;
      }
    }
  }
}
