using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Actions;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Craft;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using System;
using TabletopTweaks.Core.NewActions;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class DefensiveShock
  {
    private const string FeatureName = "DefensiveShock";

    internal const string DisplayName = "DefensiveShock.Name";
    private const string Description = "DefensiveShock.Description";

    private const string BuffName = "DefensiveShock.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "defensiveshock.png";

    // common_electrobuff00
    private const string BuffFx = "8304b3b886f756542ae0068668c8ad2f";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.DefensiveShockSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("DefensiveShock.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.DefensiveShockBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.DefensiveShockSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.DefensiveShockBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetStacking(StackingType.Rank)
        .AddSpellDescriptorComponent(SpellDescriptor.Electricity)
        .SetRanks(6)
        .SetFxOnStart(BuffFx)
        .AddContextRankConfig(ContextRankConfigs.BuffRank(Guids.DefensiveShockBuff))
        .AddTargetAttackRollTrigger(
          onlyHit: true,
          onlyMelee: true,
          actionsOnAttacker: ActionsBuilder.New()
            .Add<SpellResistanceCheck>(
              a =>
              {
                a.OnResistFail = ActionsBuilder.New()
                  .DealDamage(
                    DamageTypes.Energy(DamageEnergyType.Electricity),
                    ContextDice.Value(DiceType.D6, diceCount: ContextValues.Rank()),
                    ignoreCritical: true)
                  .Add<RemoveHalfBuffRanks>()
                  .Build();
              }))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.DefensiveShockSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Electricity)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.MinutePerLevel)
        .SetRange(AbilityRange.Personal)
        .SetSpellResistance()
        .SetIgnoreSpellResistanceForAlly()
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Empower,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Maximize,
          Metamagic.Quicken,
          (Metamagic)CustomMetamagic.ElementalAcid,
          (Metamagic)CustomMetamagic.ElementalCold,
          (Metamagic)CustomMetamagic.ElementalFire,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(
          level: 2,
          SpellList.Alchemist,
          SpellList.Bloodrager,
          SpellList.Magus,
          SpellList.Wizard)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddContextRankConfig(
          ContextRankConfigs.CasterLevel(min: 1, max: 6, type: AbilityRankType.DamageDice).WithDiv2Progression())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .Add<ContextActionApplyBuffRanks>(
              a =>
              {
                a.m_Buff = buff.ToReference<BlueprintBuffReference>();
                a.Rank = ContextValues.Rank(type: AbilityRankType.DamageDice);
                a.DurationValue = ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes);
              }))
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.None,
          spellType: CraftSpellType.Buff)
        .Configure();
    }
  }
}
