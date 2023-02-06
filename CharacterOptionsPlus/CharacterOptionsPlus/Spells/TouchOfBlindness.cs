using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;
using static CharacterOptionsPlus.UnitParts.UnitPartTouchCharges;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class TouchOfBlindness
  {
    private const string FeatureName = "TouchOfBlindness";
    private const string EffectName = "TouchOfBlindness.Effect";

    internal const string DisplayName = "TouchOfBlindness.Name";
    private const string Description = "TouchOfBlindness.Description";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.TouchOfBlindnessSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("TouchOfBlindness.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(EffectName, Guids.TouchOfBlindnessEffect).Configure();
      AbilityConfigurator.New(FeatureName, Guids.TouchOfBlindnessSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = AbilityRefs.Blindness.Reference.Get().Icon;
      var effectAbility = AbilityConfigurator.NewSpell(
          EffectName, Guids.TouchOfBlindnessEffect, SpellSchool.Necromancy, canSpecialize: false)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetLocalizedDuration(Duration.OneRound)
        .SetLocalizedSavingThrow(SavingThrow.FortNegates)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Piercing)
        .AddComponent(new TouchCharges(ContextValues.Rank()))
        .AddAbilityDeliverTouch(touchWeapon: ItemWeaponRefs.TouchItem.ToString())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .SavingThrow(
              SavingThrowType.Fortitude,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  failed: ActionsBuilder.New()
                    .ApplyBuff(BuffRefs.BlindnessBuff.ToString(), ContextDuration.Fixed(1)))))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.TouchOfBlindnessSpell, SpellSchool.Necromancy, canSpecialize: true)
        .CopyFrom(effectAbility)
        .SetShouldTurnToTarget(true)
        .AddToSpellLists(
          level: 1, SpellList.Bard, SpellList.Cleric, SpellList.Wizard, SpellList.Shaman, SpellList.Witch)
        .AddToSpellList(1, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddAbilityEffectStickyTouch(touchDeliveryAbility: effectAbility)
        .Configure(delayed: true);
    }
  }
}
