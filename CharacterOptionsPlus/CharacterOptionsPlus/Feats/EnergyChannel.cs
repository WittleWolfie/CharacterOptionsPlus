using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using TabletopTweaks.Core.NewActions;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;

namespace CharacterOptionsPlus.Feats
{
  internal class EnergyChannel
  {
    internal const string FeatName = "EnergyChannel";
    internal const string FeatDisplayName = "EnergyChannel.Name";
    private const string FeatDescription = "EnergyChannel.Description";

    private const string ClericVariantDisplayName = "EnergyChannel.Cleric.Name";
    private const string WarpriestVariantDisplayName = "EnergyChannel.Warpriest.Name";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.EnergyChannelFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("EnergyChannel.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      var airDetails = EnergyChannelConfig[DamageEnergyType.Electricity];
      BuffConfigurator.New($"{airDetails.FeatureName}.Cleric.Buff", airDetails.ClericBuffGuid).Configure();
      AbilityConfigurator.New($"{airDetails.FeatureName}.ClericVariant", airDetails.ClericVariantGuid).Configure();
      BuffConfigurator.New($"{airDetails.FeatureName}.Warpriest.Buff", airDetails.WarpriestBuffGuid).Configure();
      AbilityConfigurator.New($"{airDetails.FeatureName}.WarpriestVariant", airDetails.WarpriestVariantGuid).Configure();
      AbilityConfigurator.New($"{airDetails.FeatureName}.Ability", airDetails.AbilityGuid).Configure();
      FeatureConfigurator.New(airDetails.FeatureName, airDetails.FeatureGuid).Configure();

      var earthDetails = EnergyChannelConfig[DamageEnergyType.Acid];
      BuffConfigurator.New($"{earthDetails.FeatureName}.Cleric.Buff", earthDetails.ClericBuffGuid).Configure();
      AbilityConfigurator.New($"{earthDetails.FeatureName}.ClericVariant", earthDetails.ClericVariantGuid).Configure();
      BuffConfigurator.New($"{earthDetails.FeatureName}.Warpriest.Buff", earthDetails.WarpriestBuffGuid).Configure();
      AbilityConfigurator.New($"{earthDetails.FeatureName}.WarpriestVariant", earthDetails.WarpriestVariantGuid).Configure();
      AbilityConfigurator.New($"{earthDetails.FeatureName}.Ability", earthDetails.AbilityGuid).Configure();
      FeatureConfigurator.New(earthDetails.FeatureName, earthDetails.FeatureGuid).Configure();

      var fireDetails = EnergyChannelConfig[DamageEnergyType.Fire];
      BuffConfigurator.New($"{fireDetails.FeatureName}.Cleric.Buff", fireDetails.ClericBuffGuid).Configure();
      AbilityConfigurator.New($"{fireDetails.FeatureName}.ClericVariant", fireDetails.ClericVariantGuid).Configure();
      BuffConfigurator.New($"{fireDetails.FeatureName}.Warpriest.Buff", fireDetails.WarpriestBuffGuid).Configure();
      AbilityConfigurator.New($"{fireDetails.FeatureName}.WarpriestVariant", fireDetails.WarpriestVariantGuid).Configure();
      AbilityConfigurator.New($"{fireDetails.FeatureName}.Ability", fireDetails.AbilityGuid).Configure();
      FeatureConfigurator.New(fireDetails.FeatureName, fireDetails.FeatureGuid).Configure();

      var waterDetails = EnergyChannelConfig[DamageEnergyType.Cold];
      BuffConfigurator.New($"{waterDetails.FeatureName}.Cleric.Buff", waterDetails.ClericBuffGuid).Configure();
      AbilityConfigurator.New($"{waterDetails.FeatureName}.ClericVariant", waterDetails.ClericVariantGuid).Configure();
      BuffConfigurator.New($"{waterDetails.FeatureName}.Warpriest.Buff", waterDetails.WarpriestBuffGuid).Configure();
      AbilityConfigurator.New($"{waterDetails.FeatureName}.WarpriestVariant", waterDetails.WarpriestVariantGuid).Configure();
      AbilityConfigurator.New($"{waterDetails.FeatureName}.Ability", waterDetails.AbilityGuid).Configure();
      FeatureConfigurator.New(waterDetails.FeatureName, waterDetails.FeatureGuid).Configure();

      FeatureSelectionConfigurator.New(FeatName, Guids.EnergyChannelFeat).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      FeatureSelectionConfigurator.New(FeatName, Guids.EnergyChannelFeat, FeatureGroup.Feat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
  //      .SetIcon(IconName)
        .AddFeatureTagsComponent(FeatureTag.ClassSpecific | FeatureTag.Damage)
        .AddPrerequisiteFeaturesFromList(
          new()
          {
            FeatureRefs.ChannelEnergyFeature.ToString(),
            FeatureRefs.WarpriestChannelEnergyFeature.ToString(),
            FeatureRefs.WarpriestShieldbearerChannelEnergyFeature.ToString(),
          },
          amount: 1)
        .AddPrerequisiteFeaturesFromList(
          new()
          {
            FeatureRefs.AirDomainBaseFeature.ToString(),
            FeatureRefs.EarthDomainBaseFeature.ToString(),
            FeatureRefs.FireDomainBaseFeature.ToString(),
            FeatureRefs.WaterDomainBaseFeature.ToString(),
            FeatureRefs.AirBlessingFeature.ToString(),
            FeatureRefs.EarthBlastFeature.ToString(),
            FeatureRefs.FireBlessingFeature.ToString(),
            FeatureRefs.WaterBlessingFeature.ToString(),
          },
          amount: 1)
        .SetAllFeatures(
          // Air
          ConfigureEnergyChannel(DamageEnergyType.Electricity),
          // Earth
          ConfigureEnergyChannel(DamageEnergyType.Acid),
          // Fire
          ConfigureEnergyChannel(DamageEnergyType.Fire),
          // Water
          ConfigureEnergyChannel(DamageEnergyType.Cold))
        .Configure(delayed: true);
    }

    private struct EnergyChannelDetails
    {
      internal string FeatureGuid;
      internal string AbilityGuid;

      internal string ClericBuffGuid;
      internal string ClericVariantGuid;

      internal string WarpriestBuffGuid;
      internal string WarpriestVariantGuid;

      internal string FeatureName;
      internal string DisplayName;
      internal string Description;
      internal string Icon;

      internal string Domain;
      internal string Blessing;
    }

    private static readonly Dictionary<DamageEnergyType, EnergyChannelDetails> EnergyChannelConfig =
      new()
      {
        // Air
        {
          DamageEnergyType.Electricity,
          new()
          {
            FeatureGuid = Guids.EnergyChannelAir,
            AbilityGuid = Guids.EnergyChannelAirAbility,

            ClericBuffGuid = Guids.EnergyChannelAirClericBuff,
            ClericVariantGuid = Guids.EnergyChannelAirClericAbility,

            WarpriestBuffGuid = Guids.EnergyChannelAirWarpriestBuff,
            WarpriestVariantGuid = Guids.EnergyChannelAirWarpriestAbility,

            FeatureName = "EnergyChannel.Air",
            DisplayName = "EnergyChannel.Air.Name",
            Description = "EnergyChannel.Air.Description",
            Icon = "",

            Domain = FeatureRefs.AirDomainBaseFeature.ToString(),
            Blessing = FeatureRefs.AirBlessingFeature.ToString(),
          }
        },

        // Earth
        {
          DamageEnergyType.Acid,
          new()
          {
            FeatureGuid = Guids.EnergyChannelEarth,
            AbilityGuid = Guids.EnergyChannelEarthAbility,

            ClericBuffGuid = Guids.EnergyChannelEarthClericBuff,
            ClericVariantGuid = Guids.EnergyChannelEarthClericAbility,

            WarpriestBuffGuid = Guids.EnergyChannelEarthWarpriestBuff,
            WarpriestVariantGuid = Guids.EnergyChannelEarthWarpriestAbility,

            FeatureName = "EnergyChannel.Earth",
            DisplayName = "EnergyChannel.Earth.Name",
            Description = "EnergyChannel.Earth.Description",
            Icon = "",

            Domain = FeatureRefs.EarthDomainBaseFeature.ToString(),
            Blessing = FeatureRefs.EarthBlessingFeature.ToString(),
          }
        },

        // Fire
        {
          DamageEnergyType.Fire,
          new()
          {
            FeatureGuid = Guids.EnergyChannelFire,
            AbilityGuid = Guids.EnergyChannelFireAbility,

            ClericBuffGuid = Guids.EnergyChannelFireClericBuff,
            ClericVariantGuid = Guids.EnergyChannelFireClericAbility,

            WarpriestBuffGuid = Guids.EnergyChannelFireWarpriestBuff,
            WarpriestVariantGuid = Guids.EnergyChannelFireWarpriestAbility,

            FeatureName = "EnergyChannel.Fire",
            DisplayName = "EnergyChannel.Fire.Name",
            Description = "EnergyChannel.Fire.Description",
            Icon = "",

            Domain = FeatureRefs.FireDomainBaseFeature.ToString(),
            Blessing = FeatureRefs.FireBlessingFeature.ToString(),
          }
        },

        // Water
        {
          DamageEnergyType.Cold,
          new()
          {
            FeatureGuid = Guids.EnergyChannelWater,
            AbilityGuid = Guids.EnergyChannelWaterAbility,

            ClericBuffGuid = Guids.EnergyChannelWaterClericBuff,
            ClericVariantGuid = Guids.EnergyChannelWaterClericAbility,

            WarpriestBuffGuid = Guids.EnergyChannelWaterWarpriestBuff,
            WarpriestVariantGuid = Guids.EnergyChannelWaterWarpriestAbility,

            FeatureName = "EnergyChannel.Water",
            DisplayName = "EnergyChannel.Water.Name",
            Description = "EnergyChannel.Water.Description",
            Icon = "",

            Domain = FeatureRefs.WaterDomainBaseFeature.ToString(),
            Blessing = FeatureRefs.WaterBlessingFeature.ToString(),
          }
        }
      };

    private static BlueprintFeature ConfigureEnergyChannel(DamageEnergyType type)
    {
      var details = EnergyChannelConfig[type];

      var clericBuff = BuffConfigurator.New($"{details.FeatureName}.Cleric.Buff", details.ClericBuffGuid)
        .SetDisplayName(details.DisplayName)
        .SetDescription(details.Description)
        //.SetIcon(details.Icon)
        .AddCombatStateTrigger(combatEndActions: ActionsBuilder.New().RemoveSelf())
        .AddComponent(new BonusDamage(type))
        .AddContextRankConfig(
          ContextRankConfigs.ClassLevel(new[] { CharacterClassRefs.ClericClass.ToString() }, min: 1)
            .WithDiv2Progression())
        .Configure();

      var clericVariant = AbilityConfigurator.New($"{details.FeatureName}.ClericVariant", details.ClericVariantGuid)
        .SetDisplayName(ClericVariantDisplayName)
        .SetDescription(details.Description)
        //.SetIcon(details.Icon)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Swift)
        .SetRange(AbilityRange.Personal)
        .SetParent(details.AbilityGuid)
        .SetType(AbilityType.Supernatural)
        .AddAbilityResourceLogic(
          requiredResource: AbilityResourceRefs.ChannelEnergyResource.ToString(),
          isSpendResource: true,
          amount: 1)
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .Add<ContextActionApplyBuffRanks>(
              a =>
              {
                a.m_Buff = clericBuff.ToReference<BlueprintBuffReference>();
                a.Rank = 3;
                a.Permanent = true;
              }))
        .Configure();

      var warpriestBuff = BuffConfigurator.New($"{details.FeatureName}.Warpriest.Buff", details.WarpriestBuffGuid)
        .SetDisplayName(details.DisplayName)
        .SetDescription(details.Description)
        //.SetIcon(details.Icon)
        .AddCombatStateTrigger(combatEndActions: ActionsBuilder.New().RemoveSelf())
        .AddComponent(new BonusDamage(type))
        .AddContextRankConfig(
          ContextRankConfigs.ClassLevel(new[] { CharacterClassRefs.WarpriestClass.ToString() })
            .WithStartPlusDivStepProgression(divisor: 3, start: 2, delayStart: true))
        .Configure();

      var warpriestVariant = AbilityConfigurator.New($"{details.FeatureName}.WarpriestVariant", details.WarpriestVariantGuid)
        .SetDisplayName(WarpriestVariantDisplayName)
        .SetDescription(details.Description)
        //.SetIcon(details.Icon)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Swift)
        .SetRange(AbilityRange.Personal)
        .SetParent(details.AbilityGuid)
        .SetType(AbilityType.Supernatural)
        .AddAbilityResourceLogic(
          requiredResource: AbilityResourceRefs.WarpriestFervorResource.ToString(),
          isSpendResource: true,
          amount: 2)
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .Add<ContextActionApplyBuffRanks>(
              a =>
              {
                a.m_Buff = warpriestBuff.ToReference<BlueprintBuffReference>();
                a.Rank = 3;
                a.Permanent = true;
              }))
        .Configure();

      var ability = AbilityConfigurator.New($"{details.FeatureName}.Ability", details.AbilityGuid)
        .SetDisplayName(details.DisplayName)
        .SetDescription(details.Description)
        //.SetIcon(details.Icon)
        .AllowTargeting(self: true)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetActionType(CommandType.Swift)
        .SetRange(AbilityRange.Personal)
        .SetType(AbilityType.Supernatural)
        .AddAbilityVariants(new() { clericVariant, warpriestVariant })
        .Configure();

      return FeatureConfigurator.New(details.FeatureName, details.FeatureGuid)
        .SetIsClassFeature()
        .SetDisplayName(details.DisplayName)
        .SetDescription(details.Description)
        //.SetIcon(details.Icon)
        .AddPrerequisiteFeaturesFromList(new() { details.Domain, details.Blessing }, amount: 1)
        .AddFacts(new() { ability })
        .Configure();
    }

    [TypeId("9086afa7-61ea-49b2-bf79-7d98d656d533")]
    private class BonusDamage : UnitBuffComponentDelegate, IInitiatorRulebookHandler<RuleCalculateDamage>
    {
      private readonly ContextValue Bonus = ContextValues.Rank();
      private readonly DamageEnergyType Type;

      internal BonusDamage(DamageEnergyType type)
      {
        Type = type;
      }

      public void OnEventAboutToTrigger(RuleCalculateDamage evt)
      {
        try
        {
          if (evt.DamageBundle.WeaponDamage is null)
            return;

          var bonusDamage = Bonus.Calculate(Context);
          Logger.Verbose(() => $"Adding {bonusDamage} {Type} damage");
          evt.AddUnsafe(DamageTypes.Energy(Type).CreateDamage(DiceFormula.One, bonusDamage));
        } catch (Exception e)
        {
          Logger.LogException("BonusDamage.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateDamage evt)
      {
        try
        {
          if (evt.DamageBundle.WeaponDamage is null)
            return;

          Logger.Verbose(() => "Damage added, removing a charge");
          Buff.RemoveRank();
        } catch (Exception e)
        {
          Logger.LogException("BonusDamage.OnEventDidTrigger", e);
        }
      }
    }
  }
}
