using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class Consecrate
  {
    private const string FeatureName = "Consecrate";

    internal const string DisplayName = "Consecrate.Name";
    private const string Description = "Consecrate.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "consecrate.png";

    private const string AreaEffect = "Consecrate.AoE";
    private const string BuffName = "Consecrate.Buff";

    // A 20-ft holy aura
    private const string AreaEffectFxSource = "bbd6decdae32bce41ae8f06c6c5eb893";
    private const string AreaEffectFx = "c46d9d34-5f6b-4fb5-bcb1-8f75c09b6558";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.ConsecrateSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("Consecrate.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.ConsecrateBuff).Configure();
      AbilityAreaEffectConfigurator.New(AreaEffect, Guids.ConsecrateAoE).Configure();
      AbilityConfigurator.New(FeatureName, Guids.ConsecrateSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      ProhibitSummons(AbilityRefs.AnimateDead.ToString());
      ProhibitSummons(AbilityRefs.DirgeBardAnimateDead.ToString());
      ProhibitSummons(AbilityRefs.OracleBonesAnimateDead.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier2.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier3.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier4.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier5.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier6.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier7.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier8.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier9.ToString());
      ProhibitSummons(AbilityRefs.LichSummonAbilityTier10.ToString());

      var isUndead = ConditionsBuilder.New().HasFact(FeatureRefs.UndeadType.ToString());
      var buff = BuffConfigurator.New(BuffName, Guids.ConsecrateBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddSavingThrowBonusAgainstDescriptor(
          spellDescriptor: SpellDescriptor.ChannelPositiveHarm,
          modifierDescriptor: ModifierDescriptor.Sacred,
          value: -3)
        .AddAttackBonusConditional(
          conditions: isUndead, bonus: -1, descriptor: ModifierDescriptor.Sacred, checkWielder: true)
        .AddDamageBonusConditional(
          conditions: isUndead, bonus: -1, descriptor: ModifierDescriptor.Sacred, checkWielder: true)
        .AddComponent<ConsecrateComponent>()
        .Configure();

      // This handles updating the look of the effect
      AssetTool.RegisterDynamicPrefabLink(AreaEffectFx, AreaEffectFxSource, ModifyFx);
      var area = AbilityAreaEffectConfigurator.New(AreaEffect, Guids.ConsecrateAoE)
        .CopyFrom(AbilityAreaEffectRefs.GreaseArea)
        .SetSize(20.Feet())
        .SetFx(AreaEffectFx)
        .AddSpellDescriptorComponent(SpellDescriptor.Evil)
        .AddAbilityAreaEffectBuff(buff: buff)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName,
          Guids.ConsecrateSpell,
          SpellSchool.Evocation,
          canSpecialize: true,
          SpellDescriptor.Good)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetLocalizedDuration(AbilityRefs.MageArmor.Reference.Get().LocalizedDuration)
        .SetIcon(IconName)
        .SetActionType(CommandType.Standard)
        .SetRange(AbilityRange.Close)
        .AllowTargeting(friends: true, enemies: true, self: true, point: true)
        .SetSpellResistance()
        .SetShouldTurnToTarget()
        .SetEffectOnAlly(AbilityEffectOnUnit.Harmful)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Point)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Extend,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          Metamagic.Selective)
        .AddToSpellLists(
          level: 2,
          SpellList.Cleric,
          SpellList.Inquisitor,
          SpellList.Angel)
        .AddAbilityAoERadius(radius: 20.Feet())
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .SpawnAreaEffect(area, ContextDuration.Variable(ContextValues.Rank(), rate: DurationRate.Hours)))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .SetMaterialComponent(
          new()
          {
            m_Item = ItemRefs.GoldCoins.Cast<BlueprintItemReference>().Reference,
            Count = 25
          })
        .Configure();
    }

    private static void ProhibitSummons(string ability)
    {
      AbilityConfigurator.For(ability)
        .AddAbilityCasterHasNoFacts(new() { Guids.ConsecrateBuff })
        .Configure();
    }

    private static void ModifyFx(GameObject aura)
    {
      // Destroy the aura emanation waves
      UnityEngine.Object.DestroyImmediate(aura.transform.Find("Ground (1)/BorderWaves").gameObject);
      // Make the sparks gold
      var sparks = aura.transform.Find("Ground (1)/sparks (2)").GetComponent<ParticleSystem>();
      sparks.startColor = new Color(1.0f, 0.86f, 0.0f);
    }

    [TypeId("347e2d99-e462-47c1-aadf-677946bd05c3")]
    private class ConsecrateComponent : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleSavingThrow>
    {
      private static BlueprintFeature _undead;
      private static BlueprintFeature Undead
      {
        get
        {
          _undead ??= FeatureRefs.UndeadType.Reference.Get();
          return _undead;
        }
      }

      public void OnEventAboutToTrigger(RuleSavingThrow evt)
      {
        try
        {
          if (!Owner.HasFact(Undead))
            return;

          Logger.Verbose($"Adding -1 penalty to saving throw for {Owner.CharacterName}");
          evt.AddModifier(-1, source: Fact, descriptor: ModifierDescriptor.UntypedStackable);
        }
        catch (Exception e)
        {
          Logger.LogException("ConsecrateComponent.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleSavingThrow evt) { }
    }
  }
}
