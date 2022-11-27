using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using Owlcat.Runtime.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class BladedDash
  {
    private const string FeatureName = "BladedDash";
    private const string FeatureNameGreater = "BladedDash.Greater";

    internal const string DisplayName = "BladedDash.Name";
    private const string Description = "BladedDash.Description";
    internal const string DisplayNameGreater = "BladedDash.Greater.Name";
    private const string DescriptionGreater = "BladedDash.Greater.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";
    private const string IconNameGreater = IconPrefix + "gloriousheat.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

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

      AbilityConfigurator.New(FeatureName, Guids.BladedDashSpell).Configure();
      AbilityConfigurator.New(FeatureNameGreater, Guids.BladedDashGreaterSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AbilityConfigurator.NewSpell(FeatureName, Guids.BladedDashSpell, SpellSchool.Transmutation, canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetCustomRange(30.Feet())
        .AllowTargeting(enemies: true, friends: true)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Standard)
        .SetType(AbilityType.Spell)
        .SetNeedEquipWeapons(true)
        .SetAvailableMetamagic(Metamagic.CompletelyNormal, Metamagic.Quicken, Metamagic.Reach)
        .AddToSpellLists(level: 2, SpellList.Bard, SpellList.Magus)
        //.AddAbilityEffectRunAction(
        //  actions: ActionsBuilder.New()
        //    .Conditional(
        //      conditions: ConditionsBuilder.New().Add<WantsToDropWeapon>(),
        //      ifTrue: ActionsBuilder.New()
        //        .SavingThrow(
        //          SavingThrowType.Reflex,
        //          onResult: ActionsBuilder.New()
        //            .ConditionalSaved(
        //              succeed: ActionsBuilder.New().Add<Disarm>(),
        //              failed: dealDamage)),
        //      ifFalse: dealDamage))
        .AddAbilityCasterHasWeaponWithRangeType(WeaponRangeType.Melee)
        .Configure();
    }

    [TypeId("c84fdb32-bc4f-4205-afdb-5b5464aec020")]
    private class AbilityBladedDash : AbilityDeliverProjectile
    {
      private readonly bool Greater = false;

      public AbilityBladedDash(bool greater)
      {
        Greater = greater;
        Type = AbilityProjectileType.Line;
      }

      public AbilityBladedDash()
      {

      }
      // TODO: Eh fuck it. This is possible but a lot of work. Let's just do it like CotW.

      public override bool WouldTargetUnit(AbilityData ability, Vector3 targetPos, UnitEntityData unit)
      {
        try
        {
          var caster = ability.Caster.Unit;
          var normalized = (targetPos - caster.Position).normalized;
          var launchPos = caster.EyePosition + normalized * caster.Corpulence;
          var rangeInMeters = ability.Blueprint.GetRange(ability.HasMetamagic(Metamagic.Reach), ability).Meters;
          var context = ability.CreateExecutionContext(new(targetPos));
          if (Greater)
            return WouldTargetUnitLine(context, caster, launchPos, normalized.To2D(), rangeInMeters);
          else
          {
            return false;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("AbilityBladedDash.WouldTargetUnit", e);
        }
      }
    }
  }
}
