using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Craft;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using System;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;

namespace CharacterOptionsPlus.Spells
{
  internal class ShadowTrap
  {
    private const string FeatureName = "ShadowTrap";

    internal const string DisplayName = "ShadowTrap.Name";
    private const string Description = "ShadowTrap.Description";

    private const string BuffName = "ShadowTrap.Buff";
    private const string DelayBuffName = "ShadowTrap.Delay.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "shadowtrap.png";

    // Some shadow demon effect
    private const string BuffEffect = "a2d55bc9-793f-4dc3-a948-6b35abb6fe36";
    private const string BuffEffectSource = "cfcad40e39eab5b499c47f2113f86b45";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ShadowTrapSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("ShadowTrap.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(DelayBuffName, Guids.ShadowTrapDelayBuff).Configure();
      BuffConfigurator.New(BuffName, Guids.ShadowTrapBuff).Configure();
      AbilityConfigurator.New(FeatureName, Guids.ShadowTrapSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var delayBuff = BuffConfigurator.New(DelayBuffName, Guids.ShadowTrapDelayBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .Configure();

      // This handles updating the look of the effect
      AssetTool.RegisterDynamicPrefabLink(BuffEffect, BuffEffectSource, ModifyFx);
      var buff = BuffConfigurator.New(BuffName, Guids.ShadowTrapBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetFxOnStart(BuffEffect)
        .AddCondition(UnitCondition.Entangled)
        .AddCondition(UnitCondition.CantMove)
        .AddFactContextActions(
          activated: ActionsBuilder.New().ApplyBuff(delayBuff, ContextDuration.Fixed(1)),
          newRound: ActionsBuilder.New()
            .Conditional(
              ConditionsBuilder.New().HasBuff(buff: delayBuff),
              ifTrue: ActionsBuilder.New().RemoveBuff(delayBuff),
              ifFalse: ActionsBuilder.New()
                .SavingThrow(
                  SavingThrowType.Will,
                  onResult: ActionsBuilder.New().ConditionalSaved(succeed: ActionsBuilder.New().RemoveSelf()))))
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.ShadowTrapSpell,
          SpellSchool.Illusion,
          canSpecialize: true,
          SpellDescriptor.MovementImpairing)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.RoundPerLevel)
        .SetLocalizedSavingThrow(SavingThrow.WillNegates)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(
          level: 1,
          SpellList.Bard,
          SpellList.Bloodrager,
          SpellList.Cleric,
          SpellList.Shaman,
          SpellList.Wizard,
          SpellList.Witch)
        .AddToSpellList(level: 1, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .SavingThrow(
              SavingThrowType.Will,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  failed: ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank())))))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.Will,
          spellType: CraftSpellType.Debuff)
        .Configure(delayed: true);
    }

    private static void ModifyFx(GameObject smoke)
    {
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("Torso").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("EnergyBody").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("EnergyBody (1)").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("Point Light").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("Root/Sparks_Body (1)").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("Root/Skull").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("Root/SmokeOuter").gameObject);
      UnityEngine.Object.DestroyImmediate(smoke.transform.Find("Root/TrailSmoke").gameObject);
    }
  }
}
