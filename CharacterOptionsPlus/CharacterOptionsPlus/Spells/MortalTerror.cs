using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class MortalTerror
  {
    private const string FeatureName = "MortalTerror";

    internal const string DisplayName = "MortalTerror.Name";
    private const string Description = "MortalTerror.Description";

    private const string BuffName = "MortalTerror.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "mortalterror.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.MortalTerrorSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("MortalTerror.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.MortalTerrorBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.MortalTerrorSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var shaken = BuffRefs.Shaken.ToString();
      var frightened = BuffRefs.Frightened.ToString();
      var panicked = Guids.PanickedBuff;
      var cowering = BuffRefs.CowerBuff.ToString();
      var applyBuff = ActionsBuilder.New()
        .Conditional(
          ConditionsBuilder.New().HasBuff(shaken),
          ifTrue: ActionsBuilder.New().RemoveBuff(shaken).ApplyBuffPermanent(frightened),
          ifFalse: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New().HasBuff(frightened),
              ifTrue: ActionsBuilder.New().RemoveBuff(frightened).ApplyBuffPermanent(panicked),
              ifFalse: ActionsBuilder.New()
                .Conditional(
                  ConditionsBuilder.New().HasBuff(panicked),
                  ifTrue: ActionsBuilder.New().RemoveBuff(panicked).ApplyBuffPermanent(cowering))));
      var buff = BuffConfigurator.New(BuffName, Guids.MortalTerrorBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddIncomingDamageTrigger(
          actions:
            ActionsBuilder.New()
              .SavingThrow(SavingThrowType.Will, onResult: ActionsBuilder.New().ConditionalSaved(failed: applyBuff)))
        .AddFactContextActions(
          activated: ActionsBuilder.New().ApplyBuffPermanent(BuffRefs.Shaken.ToString()))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.MortalTerrorSpell,
          SpellSchool.Enchantment,
          canSpecialize: true,
          SpellDescriptor.Fear,
          SpellDescriptor.MindAffecting)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.RoundPerLevel)
        .SetLocalizedSavingThrow(SavingThrow.WillPartial)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(
          level: 2,
          SpellList.Bard,
          SpellList.Cleric,
          SpellList.Inquisitor,
          SpellList.LichInquisitorMinor,
          SpellList.LichBardMinor,
          SpellList.Shaman,
          SpellList.Wizard,
          SpellList.Warpriest,
          SpellList.Witch)
        .AddToSpellList(level: 2, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddContextRankConfig(ContextRankConfigs.CasterLevel(max: 5))
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .SavingThrow(
              SavingThrowType.Will,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  succeed: ActionsBuilder.New().ApplyBuff(BuffRefs.Shaken.ToString(), ContextDuration.Fixed(1)),
                  failed: ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank())))))
        .Configure(delayed: true);
    }
  }
}
