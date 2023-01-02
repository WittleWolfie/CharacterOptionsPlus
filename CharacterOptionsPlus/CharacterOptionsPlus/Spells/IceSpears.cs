using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System;
using System.Linq;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class IceSpears
  {
    private const string FeatureName = "IceSpears";

    internal const string DisplayName = "IceSpears.Name";
    private const string Description = "IceSpears.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "icespears.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.IceSpearsSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("IceSpears.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.IceSpearsSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.IceSpearsSpell,
          SpellSchool.Conjuration,
          canSpecialize: true,
          SpellDescriptor.Cold)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedSavingThrow(SavingThrow.ReflexPartial)
        .SetRange(AbilityRange.Personal)
        .AllowTargeting(enemies: true)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.Dazing,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalElectricity,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Piercing,
          (Metamagic)CustomMetamagic.Rime)
        .AddToSpellLists(level: 3, SpellList.Druid, SpellList.Witch, SpellList.Wizard)
        .AddAbilityAoERadius(radius: 30.Feet())
        .AddAbilityEffectRunAction(actions: ActionsBuilder.New().Add<ApplyIceSpears>())
        .Configure();
    }

    [TypeId("929cf077-685a-40a5-8d12-d9a14ffdf857")]
    private class ApplyIceSpears : ContextAction
    {
      private static readonly DamageTypeDescription PiercingDamage =
        DamageTypes.Physical(form: PhysicalDamageForm.Piercing);
      private static readonly DamageTypeDescription ColdDamage = DamageTypes.Energy(DamageEnergyType.Cold);
      private static readonly DiceFormula DamageDice = new(rollsCount: 2, DiceType.D6);

      public override string GetCaption()
      {
        return "Custom action for Ice Spears spell";
      }

      public override void RunAction()
      {
        try
        {
          var caster = Context.MaybeCaster;
          var targets =
            GameHelper.GetTargetsAround(caster.Position, 30.Feet()).Where(unit => unit.IsEnemy(caster)).ToList();
          targets.Sort((a, b) => a.DistanceTo(caster).CompareTo(b.DistanceTo(caster)));

          if (!targets.Any())
          {
            Logger.Warning("No targets");
            return;
          }

          int numTargets = targets.Count;
          int[] spearsPerTarget = new int[numTargets];
          int numSpears = Math.Max(1, Context.Params.CasterLevel / 4);
          for (int i = 0; i < numSpears; i++)
          {
            spearsPerTarget[i % numTargets]++;
          }

          Logger.Verbose($"Distributing {numSpears} across {numTargets} targets");
          for (int i = 0; i < spearsPerTarget.Length; i++)
          {
            if (spearsPerTarget[i] < 1)
              break;
            var target = targets[i];

            #region Affected by Cold
            bool affectedByCold = false;
            foreach (var buff in target.Buffs)
            {
              var spellDescriptor = buff.GetComponent<SpellDescriptorComponent>();
              if (spellDescriptor is not null && spellDescriptor.Descriptor.HasAnyFlag(SpellDescriptor.Cold))
              {
                Logger.Verbose($"{target} is affected by a cold spell");
                affectedByCold = true;
                break;
              }
            }

            foreach (var area in Game.Instance.State.AreaEffects)
            {
              if (!area.InGameUnitsInside.Contains(target))
                continue;

              var spellDescriptor = area.Blueprint.GetComponent<SpellDescriptorComponent>();
              if (spellDescriptor is not null && spellDescriptor.Descriptor.HasAnyFlag(SpellDescriptor.Cold))
              {
                Logger.Verbose($"{target} is in a cold spell area");
                affectedByCold = true;
                break;
              }
            }
            #endregion

            #region Saving Throw
            var dc = affectedByCold ? Context.Params.DC + 2 : Context.Params.DC;
            var savingThrow =
              new RuleSavingThrow(target, SavingThrowType.Reflex, difficultyClass: dc)
              {
                Reason = Context,
                PersistentSpell = Context.HasMetamagic(Metamagic.Persistent)
              };
            var saveResult = Context.TriggerRule(savingThrow);
            #endregion

            #region Damage
            var damage = GetDamage(spearsPerTarget[i]);
            var ruleDealDamage =
              new RuleDealDamage(caster, target, damage)
              {
                Reason = Context,
                HalfBecauseSavingThrow = saveResult.Success,
                SourceAbility = Context.SourceAbility,
              };
            Context.TriggerRule(ruleDealDamage);
            #endregion

            if (saveResult.Success)
            {
              Logger.Verbose($"{target} saved, skipping trip");
              continue;
            }

            #region Trip
            var statBonus =
              Math.Max(
                Math.Max(caster.Stats.Intelligence.Bonus, caster.Stats.Charisma.Bonus),
                caster.Stats.Wisdom.Bonus);
            var multipleSpearsBonus = 10 * (spearsPerTarget[i] - 1);
            var cl = affectedByCold ? Context.Params.CasterLevel + 4 : Context.Params.CasterLevel;
            var combatManeuver =
              new RuleCombatManeuver(caster, target, CombatManeuver.Trip)
              {
                Reason = Context,
                OverrideBonus = cl + statBonus + multipleSpearsBonus,
                IgnoreConcealment = true
              };
            Context.TriggerRule(combatManeuver);
            #endregion
          }
        }
        catch (Exception e)
        {
          Logger.LogException("ApplyIceSpears.RunAction", e);
        }
      }

      private DamageBundle GetDamage(int numSpears)
      {
        var damage = new DamageBundle();
        for (int i = 0; i < numSpears; i++)
        {
          var cold = ColdDamage.CreateDamage(DamageDice, bonus: 0);
          var piercing = PiercingDamage.CreateDamage(DamageDice, bonus: 0);

          if (Context.HasMetamagic(Metamagic.Empower))
          {
            Logger.Verbose("Spell is empowered");
            cold.EmpowerBonus.Set(1.5f, Metamagic.Empower);
            piercing.EmpowerBonus.Set(1.5f, Metamagic.Empower);
          }

          if (Context.HasMetamagic(Metamagic.Maximize))
          {
            Logger.Verbose("Spell is maximized");
            cold.CalculationType.Set(DamageCalculationType.Maximized, Metamagic.Maximize);
            piercing.CalculationType.Set(DamageCalculationType.Maximized, Metamagic.Maximize);
          }

          damage.Add(cold);
          damage.Add(piercing);
        }

        return damage;
      }
    }
  }
}
