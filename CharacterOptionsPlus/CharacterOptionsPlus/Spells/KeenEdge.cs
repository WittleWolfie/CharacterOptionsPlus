using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class KeenEdge
  {
    private const string FeatureName = "KeenEdge";

    internal const string DisplayName = "KeenEdge.Name";
    private const string Description = "KeenEdge.Description";

    private const string BuffName = "KeenEdge.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "burningdisarm.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

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

      BuffConfigurator.New(BuffName, Guids.KeenEdgeBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.KeenEdgeSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.KeenEdgeBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddComponent<KeenEdgeComponent>()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.KeenEdgeSpell, SpellSchool.Transmutation, canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.TenMinutesPerLevel)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(self: true, friends: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Quicken,
          Metamagic.Reach)
        .AddToSpellLists(level: 3, SpellList.Bloodrager, SpellList.Inquisitor, SpellList.Magus, SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.TenMinutes)))
        .AddComponent<AbilityTargetHasWeaponEquipped>()
        .AddComponent(
          new AbilityTargetHasWeaponDamageType(
            exclude: false, PhysicalDamageForm.Piercing, PhysicalDamageForm.Slashing))
        .Configure();
    }

    [TypeId("05c02182-ab7e-4383-bf83-d9af6b1a6627")]
    private class KeenEdgeComponent : UnitBuffComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>
    {
      public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
      {
        try
        {
          if (evt.Weapon is null)
            return;

          var damageType = evt.Weapon.Blueprint.DamageType;
          if (!damageType.IsPhysical)
            return;

          if (damageType.Physical.Form != PhysicalDamageForm.Piercing
              && damageType.Physical.Form != PhysicalDamageForm.Slashing)
            return;

          evt.DoubleCriticalEdge = true;
        }
        catch (Exception e)
        {
          Logger.LogException("KeenEdgeComponent.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateWeaponStats evt) { }
    }
  }
}
