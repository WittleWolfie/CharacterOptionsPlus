using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using System;
using System.Linq;
using TabletopTweaks.Core.NewRules;
using TabletopTweaks.Core.Utilities;

namespace CharacterOptionsPlus.MechanicsChanges
{
  /// <summary>
  /// Taken from TTT-Base: https://github.com/Vek17/TabletopTweaks-Base/blob/master/TabletopTweaks-Base/Bugfixes/Features/Feats.cs
  /// </summary>
  internal class VitalStrikeEventHandler :
    IInitiatorRulebookHandler<RuleCalculateWeaponStats>,
    IRulebookHandler<RuleCalculateWeaponStats>,
    IInitiatorRulebookHandler<RulePrepareDamage>,
    IRulebookHandler<RulePrepareDamage>,
    IInitiatorRulebookHandler<RuleAttackWithWeapon>,
    IRulebookHandler<RuleAttackWithWeapon>,
    ISubscriber, IInitiatorRulebookSubscriber
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(VitalStrikeEventHandler));

    private static BlueprintFeature _rowdy;
    private static BlueprintFeature Rowdy
    {
      get
      {
        _rowdy ??= FeatureRefs.RowdyVitalDamage.Reference.Get();
        return _rowdy;
      }
    }

    private static BlueprintFeature _mythic;
    private static BlueprintFeature MythicVitalStrike
    {
      get
      {
        _mythic ??= FeatureRefs.VitalStrikeMythicFeat.Reference.Get();
        return _mythic;
      }
    }

    public VitalStrikeEventHandler(UnitEntityData unit, int damageMod, bool mythic, bool rowdy, EntityFact fact)
    {
      Unit = unit;
      DamageMod = damageMod;
      IsMythic = mythic;
      IsRowdy = rowdy;
      Fact = fact;
    }

    public UnitEntityData GetSubscribingUnit()
    {
      return Unit;
    }

    public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
    {
    }

    public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
    {
      DamageDescription damageDescription = evt.DamageDescription.FirstItem();
      if (damageDescription != null && damageDescription.TypeDescription.Type == DamageType.Physical)
      {
        Logger.Verbose(() => "Calculating vital strike damage");
        var vitalDamage = CalculateVitalDamage(evt);
        evt.DamageDescription.Insert(1, vitalDamage);
      }
    }

    private DamageDescription CalculateVitalDamage(RuleCalculateWeaponStats evt)
    {
      var WeaponDice = new ModifiableDiceFormula(evt.WeaponDamageDice.ModifiedValue);
      WeaponDice.Modify(
        new DiceFormula(
          WeaponDice.ModifiedValue.Rolls * Math.Max(1, DamageMod - 1),
          WeaponDice.ModifiedValue.Dice),
        Fact);

      DamageDescription damageDescriptor =
        evt.Weapon.Blueprint.DamageType.GetDamageDescriptor(
          WeaponDice, evt.Initiator.Stats.AdditionalDamage.BaseValue);
      damageDescriptor.TemporaryContext(
        dd =>
        {
          dd.TypeDescription.Physical.Enhancement = evt.Enhancement;
          dd.TypeDescription.Physical.EnhancementTotal = evt.EnhancementTotal + evt.Weapon.EnchantmentValue;
          dd.TypeDescription.Common.Alignment = evt.Alignment;
          dd.SourceFact = Fact;

          if (IsMythic)
          {
            Logger.Verbose(() => "Vital Strike is mythic");
            dd.AddModifier(
              new(
                evt.DamageDescription.FirstItem().Bonus * Math.Max(1, DamageMod - 1),
                evt.Initiator.GetFact(MythicVitalStrike),
                ModifierDescriptor.UntypedStackable));
          }
        });
      return damageDescriptor;
    }

    public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

    //For Ranged - Handling of damage calcs does not occur the same due to projectiles
    public void OnEventDidTrigger(RuleAttackWithWeapon evt)
    {
      if (!IsRowdy){ return; }

      var RowdyFact = evt.Initiator.GetFact(Rowdy);
      RuleAttackRoll ruleAttackRoll = evt.AttackRoll;
      if (ruleAttackRoll == null) { return; }
      if (evt.Initiator.Stats.SneakAttack < 1) { return; }

      if (!ruleAttackRoll.TargetUseFortification)
      {
        var FortificationCheck = Rulebook.Trigger(new RuleFortificationCheck(ruleAttackRoll));
        if (FortificationCheck.UseFortification)
        {
          ruleAttackRoll.FortificationChance = FortificationCheck.FortificationChance;
          ruleAttackRoll.FortificationRoll = FortificationCheck.Roll;
        }
      }

      if (!ruleAttackRoll.TargetUseFortification || ruleAttackRoll.FortificationOvercomed)
      {
        DamageTypeDescription damageTypeDescription = evt.ResolveRules
            .Select(e => e.Damage).First()
            .DamageBundle.First().CreateTypeDescription();
        var rowdyDice = new ModifiableDiceFormula(new DiceFormula(evt.Initiator.Stats.SneakAttack * 2, DiceType.D6));
        var RowdyDamage = damageTypeDescription.GetDamageDescriptor(rowdyDice, 0);
        RowdyDamage.SourceFact = RowdyFact;
        BaseDamage baseDamage = RowdyDamage.CreateDamage();
        baseDamage.Precision = true;
        evt.ResolveRules.Select(e => e.Damage)
            .ForEach(e => e.Add(baseDamage));
      }
    }

    //For Melee
    public void OnEventAboutToTrigger(RulePrepareDamage evt)
    {
      if (!IsRowdy) { return; }

      var RowdyFact = evt.Initiator.GetFact(Rowdy);
      RuleAttackRoll ruleAttackRoll = evt.ParentRule.AttackRoll;
      if (ruleAttackRoll == null) { return; }
      if (evt.Initiator.Stats.SneakAttack < 1) { return; }

      if (!ruleAttackRoll.TargetUseFortification)
      {
        var FortificationCheck = Rulebook.Trigger(new RuleFortificationCheck(ruleAttackRoll));
        if (FortificationCheck.UseFortification)
        {
          ruleAttackRoll.FortificationChance = FortificationCheck.FortificationChance;
          ruleAttackRoll.FortificationRoll = FortificationCheck.Roll;
        }
      }

      if (!ruleAttackRoll.TargetUseFortification || ruleAttackRoll.FortificationOvercomed)
      {
        DamageTypeDescription damageTypeDescription = evt.DamageBundle
            .First()
            .CreateTypeDescription();
        var rowdyDice = new ModifiableDiceFormula(new DiceFormula(evt.Initiator.Stats.SneakAttack * 2, DiceType.D6));
        var RowdyDamage = damageTypeDescription.GetDamageDescriptor(rowdyDice, 0);
        RowdyDamage.SourceFact = RowdyFact;
        BaseDamage baseDamage = RowdyDamage.CreateDamage();
        baseDamage.Precision = true;
        evt.Add(baseDamage);
      }
    }

    public void OnEventDidTrigger(RulePrepareDamage evt) { }

    private readonly UnitEntityData Unit;
    private readonly EntityFact Fact;
    private readonly int DamageMod;
    private readonly bool IsMythic;
    private readonly bool IsRowdy;
  }
}
