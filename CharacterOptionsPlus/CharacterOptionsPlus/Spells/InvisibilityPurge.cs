using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Conditions;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class InvisibilityPurge
  {
    private const string FeatureName = "InvisibilityPurge";

    internal const string DisplayName = "InvisibilityPurge.Name";
    private const string Description = "InvisibilityPurge.Description";

    private const string SelfBuff = "InvisibilityPurge.Buff.Self";
    private const string TargetBuff = "InvisibilityPurge.Buff.Target";
    private const string FxBuff = "InvisibilityPurge.Buff.Fx";
    private const string AreaEffect = "InvisibilityPurge.Area";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "invisibilitypurge.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.InvisibilityPurgeSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("InvisibilityPurge.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityAreaEffectConfigurator.New(AreaEffect, Guids.InvisibilityPurgeArea).Configure();
      BuffConfigurator.New(FxBuff, Guids.InvisibilityPurgeFxBuff).Configure();
      BuffConfigurator.New(SelfBuff, Guids.InvisibilityPurgeSelfBuff).Configure();
      BuffConfigurator.New(TargetBuff, Guids.InvisibilityPurgeBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.InvisibilityPurgeSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var fxBuff = BuffConfigurator.New(FxBuff, Guids.InvisibilityPurgeFxBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .SetFxOnStart(BuffRefs.FaerieFireBuff.Reference.Get().FxOnStart)
        .Configure();

      var applyFx = ActionsBuilder.New()
        .Conditional(
          ConditionsBuilder.New().Add<HasCondition>(c => c.Condition = UnitCondition.Invisible),
          ifTrue: ActionsBuilder.New().ApplyBuffPermanent(fxBuff));
      var buff = BuffConfigurator.New(TargetBuff, Guids.InvisibilityPurgeBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddConditionImmunity(condition: UnitCondition.Invisible)
        .AddFactContextActions(activated: applyFx)
        .AddComponent<InvisibilityPurgeComponent>()
        .Configure();

      var area = AbilityAreaEffectConfigurator.New(AreaEffect, Guids.InvisibilityPurgeArea)
        .SetShape(AreaEffectShape.Cylinder)
        .SetSize(50.Feet())
        .SetAffectEnemies()
        .AddContextRankConfig(
          ContextRankConfigs.CasterLevel()
            .WithCustomProgression(
              (Base: 2, Progression: 5),
              (Base: 4, Progression: 10),
              (Base: 6, Progression: 15),
              (Base: 8, Progression: 20),
              (Base: 10, Progression: 25),
              (Base: 12, Progression: 30),
              (Base: 14, Progression: 35),
              (Base: 16, Progression: 40),
              (Base: 18, Progression: 45),
              (Base: 20, Progression: 50)))
        .AddAbilityAreaEffectBuff(
          buff: buff,
          checkConditionEveryRound: true,
          condition: ConditionsBuilder.New()
            .Add<DistanceFromCaster>(c => c.DistanceInFeet = ContextValues.Rank()))
        .Configure();

      var selfBuff = BuffConfigurator.New(SelfBuff, Guids.InvisibilityPurgeSelfBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddAreaEffect(areaEffect: area)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.InvisibilityPurgeSpell, SpellSchool.Evocation, canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetRange(AbilityRange.Personal)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetEffectOnAlly(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Omni)
        .SetActionType(CommandType.Standard)
        .SetLocalizedDuration(Duration.MinutePerLevel)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Quicken,
          Metamagic.Selective)
        .AddToSpellLists(level: 3, SpellList.Cleric, SpellList.Inquisitor)
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .ApplyBuff(selfBuff, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Minutes)))
        .Configure();
    }

    [TypeId("4f3f7044-e3ce-4922-99dc-7cf9d41491a7")]
    private class InvisibilityPurgeComponent : UnitFactComponentDelegate, IUnitConditionChangeAttemptHandler
    {
      private static readonly BlueprintBuffReference FxBuff =
        BlueprintTool.GetRef<BlueprintBuffReference>(Guids.InvisibilityPurgeFxBuff);

      public void HandleUnitConditionAddAttempt(UnitEntityData unit, UnitCondition condition, bool success)
      {
        try
        {
          if (condition == UnitCondition.Invisible)
            unit.AddBuff(FxBuff, Context);
        }
        catch (Exception e)
        {
          Logger.LogException("InvisibilityPurgeComponent.HandleUnitConditionAddAttempt", e);
        }
      }
    }
  }
}
