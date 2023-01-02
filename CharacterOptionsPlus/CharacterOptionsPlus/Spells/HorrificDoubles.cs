using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers.Units;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using System.Linq;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class HorrificDoubles
  {
    private const string FeatureName = "HorrificDoubles";

    internal const string DisplayName = "HorrificDoubles.Name";
    private const string Description = "HorrificDoubles.Description";

    private const string BuffName = "HorrificDoubles.Buff";
    private const string ShakenName = "HorrificDoubles.Shaken";
    private const string ShakenImmunityName = "HorrificDoubles.Shaken.Immunity";
    private const string ShakenDescription = "HorrificDoubles.Shaken.Description";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.HorrificDoublesSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("HorrificDoubles.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(ShakenImmunityName, Guids.HorrificDoublesShakenImmunity).Configure();
      BuffConfigurator.New(ShakenName, Guids.HorrificDoublesShaken).Configure();
      BuffConfigurator.New(BuffName, Guids.HorrificDoublesBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.HorrificDoublesSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = AbilityRefs.Lich1FalseGraceAbility.Reference.Get().Icon;
      var shaken = BuffConfigurator.New(ShakenName, Guids.HorrificDoublesShaken)
        .CopyFrom(BuffRefs.Shaken)
        .SetDisplayName(DisplayName)
        .SetDescription(ShakenDescription)
        .SetIcon(icon)
        .AddSpellDescriptorComponent(SpellDescriptor.MindAffecting)
        .AddComponent<HorrifiedComponent>()
        .Configure();

      var shakenImmunity = BuffConfigurator.New(ShakenImmunityName, Guids.HorrificDoublesShakenImmunity)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .Configure();

      var buff = BuffConfigurator.New(BuffName, Guids.HorrificDoublesBuff)
        .CopyFrom(BuffRefs.MirrorImageBuff, c => true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .AddComponent<HorrificDoublesComponent>()
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.HorrificDoublesSpell,
          SpellSchool.Illusion,
          canSpecialize: true,
          SpellDescriptor.MindAffecting)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetLocalizedDuration(Duration.MinutePerLevel)
        .SetLocalizedSavingThrow(SavingThrow.WillNegates)
        .SetRange(AbilityRange.Personal)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.SelfTouch)
        .SetActionType(CommandType.Standard)
        .AllowTargeting(self: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Helpful)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(level: 4, SpellList.Bard, SpellList.Bloodrager, SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes)))
        .Configure();
    }

    private static BlueprintBuff _shaken;
    private static BlueprintBuff Shaken
    {
      get
      {
        _shaken ??= BuffRefs.Shaken.Reference.Get();
        return _shaken;
      }
    }
    private static BlueprintBuff _horrified;
    private static BlueprintBuff Horrified
    {
      get
      {
        _horrified ??= BlueprintTool.Get<BlueprintBuff>(Guids.HorrificDoublesShaken);
        return _horrified;
      }
    }
    private static BlueprintBuff _horrifiedImmunity;
    private static BlueprintBuff HorrifiedImmunity
    {
      get
      {
        _horrifiedImmunity ??= BlueprintTool.Get<BlueprintBuff>(Guids.HorrificDoublesShakenImmunity);
        return _horrifiedImmunity;
      }
    }
    private static BlueprintBuff _horrificDoublesBuff;
    private static BlueprintBuff HorrificDoublesBuff
    {
      get
      {
        _horrificDoublesBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.HorrificDoublesBuff);
        return _horrificDoublesBuff;
      }
    }

    [TypeId("caddd0ff-8ad9-40c1-9f70-a847e9658962")]
    private class HorrificDoublesComponent : UnitBuffComponentDelegate, ITickEachRound
    {
      public override void OnActivate()
      {
        try
        {
          Logger.Verbose("Activating (HorrificDoublesComponent)");
          Apply();
        }
        catch (Exception e)
        {
          Logger.LogException("HorrificDoublesComponent.OnActivate", e);
        }
      }

      public void OnNewRound()
      {
        try
        {
          Logger.Verbose("New Round (HorrificDoublesComponent)");
          Apply();
        }
        catch (Exception e)
        {
          Logger.LogException("HorrificDoublesComponent.OnNewRound", e);
        }
      }

      private void Apply()
      {
        var enemies = GameHelper.GetTargetsAround(Owner.Position, 120.Feet())
          .Where(unit => unit.IsEnemy(Owner))
          .Where(unit => !unit.HasFact(Horrified) && !unit.HasFact(HorrifiedImmunity));
        if (!enemies.Any())
        {
          Logger.Verbose("No affected enemies");
          return;
        }

        foreach (var enemy in enemies)
        {
          if (Rulebook.Trigger<RuleSpellResistanceCheck>(new(Buff.Context, enemy)).IsSpellResisted)
          {
            Logger.Verbose($"{enemy} resisted");
            return;
          }

          if (Rulebook.Trigger(CreateSavingThrow(enemy)).IsPassed
              && (!Buff.Context.HasMetamagic(Metamagic.Persistent)
                || Rulebook.Trigger(CreateSavingThrow(enemy, persistent: true)).IsPassed))
          {
            Logger.Verbose($"{enemy} saved, granting immunity");
            enemy.AddBuff(HorrifiedImmunity, Buff.Context);
          }
          else
          {
            Logger.Verbose($"{enemy} failed their save");
            enemy.AddBuff(Horrified, Buff.Context);
          }
        }
      }

      private RuleSavingThrow CreateSavingThrow(UnitEntityData target, bool persistent = false)
      {
        return
          new(target, SavingThrowType.Will, Buff.Context.Params.DC)
          {
            Buff = Buff.Blueprint,
            PersistentSpell = persistent
          };
      }
    }

    [TypeId("8f520a35-dbc0-4d3d-9650-dba44f14a40f")]
    private class HorrifiedComponent : UnitBuffComponentDelegate, ITickEachRound, IInitiatorRulebookHandler<RuleAttackRoll>
    {
      public override void OnActivate()
      {
        try
        {
          Logger.Verbose("Activating (HorrifiedComponent)");
          Buff.StoreFact(Owner.AddBuff(Shaken, Context));
        }
        catch (Exception e)
        {
          Logger.LogException("HorrifiedComponent.OnActivate", e);
        }
      }

      public void OnEventAboutToTrigger(RuleAttackRoll evt) { }

      public void OnEventDidTrigger(RuleAttackRoll evt)
      {
        try
        {
          if (evt.Target != Context.MaybeCaster)
          {
            Logger.Verbose("Target is not caster");
            return;
          }

          if (evt.HitMirrorImageIndex < 1)
          {
            Logger.Verbose("Missed images");
            return;
          }

          if (Owner.HasFact(HorrifiedImmunity))
          {
            Logger.Verbose($"{Owner} is immune to the effects");
            return;
          }

          // Hit an image, make a will save (but only the first time)
          Buff.StoreFact(Owner.AddBuff(HorrifiedImmunity, Buff.Context));

          if (Rulebook.Trigger<RuleSpellResistanceCheck>(new(Buff.Context, Owner)).IsSpellResisted)
          {
            Logger.Verbose($"{Owner} resisted");
            return;
          }

          if (Rulebook.Trigger(CreateSavingThrow(Owner)).IsPassed
              && (!Buff.Context.HasMetamagic(Metamagic.Persistent)
                || Rulebook.Trigger(CreateSavingThrow(Owner, persistent: true)).IsPassed))
          {
            Logger.Verbose($"{Owner} passed their save");
            return;
          }

          Buff.StoreFact(Owner.AddBuff(BuffRefs.Frightened.Reference.Get(), Buff.Context, 1.Rounds().Seconds));
          Context.TriggerRule<RuleDealStatDamage>(
            new(Context.MaybeCaster, Owner, StatType.Wisdom, new DiceFormula(1, DiceType.D3), bonus: 0));
        }
        catch (Exception e)
        {
          Logger.LogException("HorrifiedComponent.OnEventDidTrigger", e);
        }
      }

      public void OnNewRound()
      {
        try
        {
          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          if (!caster.HasFact(HorrificDoublesBuff))
          {
            Logger.Verbose($"Removing {Buff.Name} from {Owner.CharacterName} because {caster.CharacterName} has no images");
            Buff.Remove();
            return;
          }

          if (GameHelper.CheckLOS(Owner, caster))
          {
            if (!Owner.HasFact(Shaken))
            {
              Logger.Verbose($"Applying shaken to {Owner.CharacterName} because {caster.CharacterName} is in LOS");
              Buff.StoreFact(Owner.AddBuff(Shaken, Context));
            }
            return;
          }

          Buff shakenBuff = null;
          foreach (var fact in Buff.m_StoredFacts)
          {
            var buff = fact as Buff;
            if (buff is not null && buff.Blueprint == Shaken)
            {
              Logger.Verbose($"Removing {buff.Name} from {Owner.CharacterName} because {caster.CharacterName} is not in LOS");
              shakenBuff = buff;
              break;
            }
          }
          if (shakenBuff is not null)
          {
            Logger.Verbose("Removing shaken");
            shakenBuff.Remove();
            Buff.m_StoredFacts.Remove(shakenBuff);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("HorrifiedComponent.OnNewRound", e);
        }
      }

      private RuleSavingThrow CreateSavingThrow(UnitEntityData target, bool persistent = false)
      {
        return
          new(target, SavingThrowType.Will, Buff.Context.Params.DC)
          {
            Buff = Buff.Blueprint,
            PersistentSpell = persistent
          };
      }
    }
  }
}
