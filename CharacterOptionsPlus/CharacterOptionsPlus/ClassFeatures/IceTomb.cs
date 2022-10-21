using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.ClassFeatures
{
  internal class IceTomb
  {
    private const string FeatureName = "IceTomb";

    internal const string DisplayName = "IceTomb.Name";
    private const string Description = "IceTomb.Description";

    private const string AbilityName = "IceTomb.Ability";

    private const string BuffName = "IceTomb.Buff";
    private const string CooldownName = "IceTomb.Cooldown";
    private const string CooldownDisplayName = "IceTomb.Cooldown.Name";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.IceTombHex))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("IceTomb.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.IceTombBuff).Configure();
      BuffConfigurator.New(CooldownName, Guids.IceTombCooldown).Configure();
      AbilityConfigurator.New(AbilityName, Guids.IceTombAbility).Configure();
      FeatureConfigurator.New(FeatureName, Guids.IceTombHex).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icyPrison = BuffRefs.IcyPrisonEntangledBuff.Reference.Get();
      var buff = BuffConfigurator.New(BuffName, Guids.IceTombBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icyPrison.Icon)
        .SetFxOnStart(icyPrison.FxOnStart)
        .AddCondition(condition: UnitCondition.Paralyzed)
        .AddCondition(condition: UnitCondition.Unconscious)
        .AddFactContextActions(
          newRound:
            ActionsBuilder.New()
              .SavingThrow(
                SavingThrowType.Fortitude,
                onResult:
                  ActionsBuilder.New()
                    .ConditionalSaved(
                      succeed:
                        ActionsBuilder.New()
                          .RemoveSelf()
                          .ApplyBuff(
                            BuffRefs.Staggered.ToString(),
                            ContextDuration.FixedDice(DiceType.D4),
                            isNotDispelable: true))))
        .Configure();

      var cooldown = BuffConfigurator.New(CooldownName, Guids.IceTombCooldown)
        .SetDisplayName(CooldownDisplayName) // Used for ability target restriction text
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .Configure();

      var agonyHex = AbilityRefs.WitchHexAgonyAbility.Reference.Get();
      var ability = AbilityConfigurator.New(AbilityName, Guids.IceTombAbility)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icyPrison.Icon)
        .SetType(AbilityType.Supernatural)
        .SetRange(AbilityRange.Long)
        .AllowTargeting(enemies: true)
        .SetEffectOnAlly(AbilityEffectOnUnit.Harmful)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Directional)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.Quicken, Metamagic.Extend, Metamagic.Heighten, Metamagic.CompletelyNormal, Metamagic.Persistent)
        .SetLocalizedSavingThrow(agonyHex.LocalizedSavingThrow)
        .AddSpellDescriptorComponent(SpellDescriptor.Hex | SpellDescriptor.Cold | SpellDescriptor.Paralysis)
        .AddAbilityTargetHasFact(checkedFacts: new() { cooldown }, inverted: true) // Prevent targeting twice
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .ApplyBuff(cooldown, ContextDuration.Fixed(1, rate: DurationRate.Days), isNotDispelable: true)
            .DealDamage(
              damageType: new() { Type = DamageType.Energy, Energy = DamageEnergyType.Cold},
              value: new() { DiceType = DiceType.D8, DiceCountValue = 3, BonusValue = 0},
              halfIfSaved: true)
            .ConditionalSaved(failed: ActionsBuilder.New().ApplyBuffPermanent(buff)),
          savingThrowType: SavingThrowType.Fortitude)
        .AddComponent(agonyHex.GetComponent<ContextSetAbilityParams>())
        .Configure();

      FeatureConfigurator.New(FeatureName, Guids.IceTombHex, FeatureGroup.WitchHex)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icyPrison.Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(FeatureRefs.WitchMajorHex.ToString())
        .AddFacts(new() { ability })
        .Configure(delayed: true);
    }
  }
}
