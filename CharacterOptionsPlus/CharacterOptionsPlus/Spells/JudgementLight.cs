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
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using System;
using static Kingmaker.RuleSystem.Rules.RuleDispelMagic;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.UnitLogic.Mechanics.Actions.ContextActionDispelMagic;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class JudgementLight
  {
    private const string FeatureName = "JudgementLight";

    internal const string DisplayName = "JudgementLight.Name";
    private const string Description = "JudgementLight.Description";

    private const string DisplayNameJustice = "JudgementLight.Justice.Name";
    private const string DescriptionJustice = "JudgementLight.Justice.Description";

    private const string DisplayNamePiercing = "JudgementLight.Piercing.Name";
    private const string DescriptionPiercing = "JudgementLight.Piercing.Description";

    private const string DisplayNameProtection = "JudgementLight.Protection.Name";
    private const string DescriptionProtection = "JudgementLight.Protection.Description";

    private const string DisplayNameResiliency = "JudgementLight.Resiliency.Name";
    private const string DescriptionResiliency = "JudgementLight.Resiliency.Description";

    private const string DisplayNameResistance = "JudgementLight.Resistance.Name";
    private const string DescriptionResistance = "JudgementLight.Resistance.Description";

    private const string DisplayNameSmiting = "JudgementLight.Smiting.Name";
    private const string DescriptionSmiting = "JudgementLight.Smiting.Description";

    private const string JusticeBuff = "JudgementLight.Justice";
    private const string PiercingBuff = "JudgementLight.Piercing";
    private const string ProtectionBuff = "JudgementLight.Protection";
    private const string ResistanceBuff = "JudgementLight.Resistance";

    private const string ResiliencyBuffChaos = "JudgementLight.Resliency.Chaos";
    private const string ResiliencyBuffEvil = "JudgementLight.Resliency.Evil";
    private const string ResiliencyBuffGood = "JudgementLight.Resliency.Good";
    private const string ResiliencyBuffLaw = "JudgementLight.Resliency.Law";
    private const string ResiliencyBuffMagic = "JudgementLight.Resliency.Magic";

    private const string SmitingBuffMagic = "JudgementLight.Smiting.Magic";
    private const string SmitingBuffAlignment = "JudgementLight.Smiting.Alignment";
    private const string SmitingBuffAdamantite = "JudgementLight.Smiting.Adamantite";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "judgementlight.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.JudgementLightSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("JudgementLight.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(JusticeBuff, Guids.JudgementLightJusticeBuff).Configure();
      BuffConfigurator.New(PiercingBuff, Guids.JudgementLightPiercingBuff).Configure();
      BuffConfigurator.New(ProtectionBuff, Guids.JudgementLightProtectionBuff).Configure();
      BuffConfigurator.New(ResistanceBuff, Guids.JudgementLightResistanceBuff).Configure();

      BuffConfigurator.New(ResiliencyBuffChaos, Guids.JudgementLightResiliencyChaosBuff).Configure();
      BuffConfigurator.New(ResiliencyBuffEvil, Guids.JudgementLightResiliencyEvilBuff).Configure();
      BuffConfigurator.New(ResiliencyBuffGood, Guids.JudgementLightResiliencyGoodBuff).Configure();
      BuffConfigurator.New(ResiliencyBuffLaw, Guids.JudgementLightResiliencyLawBuff).Configure();
      BuffConfigurator.New(ResiliencyBuffMagic, Guids.JudgementLightResiliencyMagicBuff).Configure();

      BuffConfigurator.New(SmitingBuffMagic, Guids.JudgementLightSmitingMagicBuff).Configure();
      BuffConfigurator.New(SmitingBuffAlignment, Guids.JudgementLightSmitingAlignmentBuff).Configure();
      BuffConfigurator.New(SmitingBuffAdamantite, Guids.JudgementLightSmitingAdamantiteBuff).Configure();

      AbilityConfigurator.New(FeatureName, Guids.JudgementLightSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var justice = BuffConfigurator.New(JusticeBuff, Guids.JudgementLightJusticeBuff)
        .CopyFrom(BuffRefs.FaerieFireBuff, c => c is not DoNotBenefitFromConcealment)
        .SetDisplayName(DisplayNameJustice)
        .SetDescription(DescriptionJustice)
        .Configure();

      var piercing = BuffConfigurator.New(PiercingBuff, Guids.JudgementLightPiercingBuff)
        .SetDisplayName(DisplayNamePiercing)
        .SetDescription(DescriptionPiercing)
        .AddSpellResistance(value: -5)
        .Configure();

      var protection = BuffConfigurator.New(ProtectionBuff, Guids.JudgementLightProtectionBuff)
        .SetDisplayName(DisplayNameProtection)
        .SetDescription(DescriptionProtection)
        .AddStatBonus(stat: StatType.AC, value: 2, descriptor: ModifierDescriptor.Sacred)
        .AddCriticalConfirmationACBonus(4)
        .Configure();

      var resistance = BuffConfigurator.New(ResistanceBuff, Guids.JudgementLightResistanceBuff)
        .SetDisplayName(DisplayNameResistance)
        .SetDescription(DescriptionResistance)
        .AddResistEnergy(value: 5, type: DamageEnergyType.Fire)
        .AddResistEnergy(value: 5, type: DamageEnergyType.Electricity)
        .AddResistEnergy(value: 5, type: DamageEnergyType.Cold)
        .AddResistEnergy(value: 5, type: DamageEnergyType.Acid)
        .AddResistEnergy(value: 5, type: DamageEnergyType.Sonic)
        .Configure();

      var resliencyChaos =
        ConfigureResiliencyBuff(ResiliencyBuffChaos, Guids.JudgementLightResiliencyChaosBuff, DamageAlignment.Chaotic);
      var resliencyEvil =
        ConfigureResiliencyBuff(ResiliencyBuffEvil, Guids.JudgementLightResiliencyEvilBuff, DamageAlignment.Evil);
      var resliencyGood =
        ConfigureResiliencyBuff(ResiliencyBuffGood, Guids.JudgementLightResiliencyGoodBuff, DamageAlignment.Good);
      var resliencyLaw =
        ConfigureResiliencyBuff(ResiliencyBuffLaw, Guids.JudgementLightResiliencyLawBuff, DamageAlignment.Lawful);
      var resliencyMagic = BuffConfigurator.New(ResiliencyBuffMagic, Guids.JudgementLightResiliencyMagicBuff)
        .SetDisplayName(DisplayNameResiliency)
        .SetDescription(DescriptionResiliency)
        .AddDamageResistancePhysical(bypassedByMagic: true, value: 3)
        .Configure();

      var smitingMagic = BuffConfigurator.New(SmitingBuffMagic, Guids.JudgementLightSmitingMagicBuff)
        .CopyFrom(BuffRefs.SmitingJudgmentBuffMagic, typeof(AddOutgoingPhysicalDamageProperty))
        .SetDisplayName(DisplayNameSmiting)
        .SetDescription(DescriptionSmiting)
        .Configure();

      var smitingAlignment = BuffConfigurator.New(SmitingBuffAlignment, Guids.JudgementLightSmitingAlignmentBuff)
        .SetDisplayName(DisplayNameSmiting)
        .SetDescription(DescriptionSmiting)
        .AddComponent<JudgementLightAlignment>()
        .Configure();

      var smitingAdamantite = BuffConfigurator.New(SmitingBuffAdamantite, Guids.JudgementLightSmitingAdamantiteBuff)
        .CopyFrom(BuffRefs.SmitingJudgmentBuffAdamantite, typeof(AddOutgoingPhysicalDamageProperty))
        .SetDisplayName(DisplayNameSmiting)
        .SetDescription(DescriptionSmiting)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.JudgementLightSpell, SpellSchool.Transmutation, canSpecialize: true, SpellDescriptor.Fire)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedSavingThrow(Common.SavingThrowVaries)
        .SetRange(AbilityRange.Personal)
        .AllowTargeting(self: true, enemies: true, friends: true)
        .SetSpellResistance()
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.Bolstered,
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Persistent,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.Dazing)
        .AddToSpellLists(level: 4, SpellList.Inquisitor)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityAoERadius(radius: 30.Feet())
        .AddComponent(new AbilityDeliverBurst(30.Feet()))
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .Conditional(
              conditions: ConditionsBuilder.New().IsEnemy(),
      #region Ally Effect
              ifFalse: ActionsBuilder.New()
                // Healing
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.HealingJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .HealTarget(
                      ContextDice.Value(diceType: DiceType.D8, diceCount: 1, bonus: ContextValues.CasterStatBonus())))
                // Protection
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ProtectionJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New().ApplyBuff(protection, ContextDuration.Variable(ContextValues.Rank())))
                // Purity
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.PurityJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New().DispelMagic(
                    buffType: BuffType.All,
                    checkType: CheckType.DC,
                    maxSpellLevel: 0, // This will actually allow any spell level
                    checkBonus: 2,
                    onlyTargetEnemyBuffs: true,
                    countToRemove: 1))
                // Resiliency
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ResiliencyJudgmentBuffChaos.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(resliencyChaos, ContextDuration.Variable(ContextValues.Rank())))
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ResiliencyJudgmentBuffEvil.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(resliencyEvil, ContextDuration.Variable(ContextValues.Rank())))
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ResiliencyJudgmentBuffGood.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(resliencyGood, ContextDuration.Variable(ContextValues.Rank())))
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ResiliencyJudgmentBuffLaw.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(resliencyLaw, ContextDuration.Variable(ContextValues.Rank())))
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ResiliencyJudgmentBuffMagic.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(resliencyMagic, ContextDuration.Variable(ContextValues.Rank())))
                // Resistance
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.ResistanceJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New().ApplyBuff(resistance, ContextDuration.Variable(ContextValues.Rank())))
                // Smiting
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.SmitingJudgmentBuffMagic.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(smitingMagic, ContextDuration.Variable(ContextValues.Rank())))
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.SmitingJudgmentBuffAlignment.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(smitingAlignment, ContextDuration.Variable(ContextValues.Rank())))
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.SmitingJudgmentBuffAdamantite.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .ApplyBuff(smitingAdamantite, ContextDuration.Variable(ContextValues.Rank()))),
      #endregion
      #region Enemy Effect
              ifTrue: ActionsBuilder.New()
                // Destruction
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.DestructionJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New()
                    .SavingThrow(
                      SavingThrowType.Will,
                      onResult: ActionsBuilder.New()
                        .ConditionalSaved(
                          failed: ActionsBuilder.New()
                            .ApplyBuff(BuffRefs.Shaken.ToString(), ContextDuration.FixedDice(DiceType.D4)))
                        .DealDamage(
                          DamageTypes.Untyped(), ContextDice.Value(DiceType.D8, diceCount: 4), halfIfSaved: true)))
                // Justice
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.JusticeJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New().ApplyBuff(justice, ContextDuration.Variable(ContextValues.Rank())))
                // Piercing
                .Conditional(
                  conditions: ConditionsBuilder.New().CasterHasFact(BuffRefs.PiercingJudgmentBuff.ToString()),
                  ifTrue: ActionsBuilder.New().ApplyBuff(piercing, ContextDuration.Variable(ContextValues.Rank())))))
      #endregion
        .Configure();
    }

    private static BlueprintBuff ConfigureResiliencyBuff(string name, string guid, DamageAlignment alignment)
    {
      return BuffConfigurator.New(name, guid)
        .SetDisplayName(DisplayNameResiliency)
        .SetDescription(DescriptionResiliency)
        .AddDamageResistancePhysical(alignment: alignment, bypassedByAlignment: true, value: 3)
        .Configure();
    }

    [TypeId("d9cd0fc0-f7fe-4c04-a978-58fc6dc547c1")]
    private class JudgementLightAlignment : UnitBuffComponentDelegate, IInitiatorRulebookHandler<RulePrepareDamage>
    {
      public void OnEventAboutToTrigger(RulePrepareDamage evt) { }

      public void OnEventDidTrigger(RulePrepareDamage evt)
      {
        try
        {
          var caster = Buff.MaybeContext?.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var casterAlignment = caster.Alignment.ValueRaw;
          var damage = evt.DamageBundle.WeaponDamage as PhysicalDamage;
          if (casterAlignment.HasComponent(AlignmentComponent.Lawful))
            damage.AddAlignment(DamageAlignment.Lawful);
          else if (casterAlignment.HasComponent(AlignmentComponent.Chaotic))
            damage.AddAlignment(DamageAlignment.Chaotic);
          else if (casterAlignment.HasComponent(AlignmentComponent.Good))
            damage.AddAlignment(DamageAlignment.Good);
          else if (casterAlignment.HasComponent(AlignmentComponent.Evil))
            damage.AddAlignment(DamageAlignment.Evil);
        }
        catch (Exception e)
        {
          Logger.LogException("JudgementLightAlignment.OnEventDidTrigger", e);
        }
      }
    }
  }
}
