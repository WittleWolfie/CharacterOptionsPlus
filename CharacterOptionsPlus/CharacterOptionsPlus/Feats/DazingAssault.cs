using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using System;

namespace CharacterOptionsPlus.Feats
{
  // Adopted Envibel's implementation from Martial Excellence: https://github.com/Envibel/MartialExcellence
  internal class DazingAssault
  {
    internal const string FeatName = "DazingAssault";
    internal const string FeatDisplayName = "DazingAssault.Name";
    private const string FeatDescription = "DazingAssault.Description";

    internal const string BuffName = "DazingAssault.Buff";
    internal const string AbilityName = "DazingAssault.Ability";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "dazingassault.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.DazingAssaultFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("DazingAssault.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.DazingAssaultBuff).Configure();
      ActivatableAbilityConfigurator.New(AbilityName, Guids.DazingAssaultToggle).Configure();
      FeatureConfigurator.New(FeatName, Guids.DazingAssaultFeat).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var buff = BuffConfigurator.New(BuffName, Guids.DazingAssaultBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddWeaponParametersAttackBonus(attackBonus: -5, ranged: false)
        .AddContextCalculateAbilityParams(
          statType: StatType.BaseAttackBonus, replaceCasterLevel: true, replaceSpellLevel: true)
        .AddInitiatorAttackWithWeaponTrigger(
          onlyHit: true,
          rangeType: WeaponRangeType.Melee,
          action: ActionsBuilder.New()
            .SavingThrow(
              SavingThrowType.Fortitude,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  failed: ActionsBuilder.New().ApplyBuff(BuffRefs.DazeBuff.ToString(), ContextDuration.Fixed(1)))))
        .Configure();

      var toggle = ActivatableAbilityConfigurator.New(AbilityName, Guids.DazingAssaultToggle)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .SetBuff(buff)
        .Configure();

      var dazingAssault = FeatureConfigurator.New(FeatName, Guids.DazingAssaultFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .SetIsClassFeature()
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Melee)
        .AddPrerequisiteStatValue(StatType.BaseAttackBonus, value: 11)
        .AddPrerequisiteStatValue(StatType.Strength, value: 13)
        .AddPrerequisitePlayerHasFeature(FeatureRefs.PowerAttackFeature.ToString())
        .AddFacts(new() { toggle })
        .Configure(delayed: true);

      Common.AddIsPrequisiteFor(FeatureRefs.PowerAttackFeature, dazingAssault);
    }
  }
}
