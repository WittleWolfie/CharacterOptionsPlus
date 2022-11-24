using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  internal class DazingAssault
  {
    internal const string FeatName = "DazingAssault";
    internal const string FeatDisplayName = "DazingAssault.Name";
    private const string FeatDescription = "DazingAssault.Description";

    internal const string BuffName = "DazingAssault.Buff";
    internal const string AbilityName = "DazingAssault.Ability";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "dazingassault.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

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
        .AddComponent<DazingAssaultComponent>()
        .Configure();

      var toggle = ActivatableAbilityConfigurator.New(AbilityName, Guids.DazingAssaultToggle)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .SetBuff(buff)
        .Configure();

      FeatureConfigurator.New(FeatName, Guids.DazingAssaultFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat)
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
    }

    [TypeId("54c53f23-8c38-4f3a-91c5-83e262d3eb25")]
    private class DazingAssaultComponent :
      UnitFactComponentDelegate,
      IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>,
      IInitiatorRulebookHandler<RuleAttackRoll>
    {
      public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
      {
        try
        {
          if (evt.Weapon?.Blueprint?.IsMelee != true)
            return;

          evt.AddModifier(-5, Fact, ModifierDescriptor.UntypedStackable);
        }
        catch (Exception e)
        {
          Logger.LogException("DazingAssaultComponent.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventAboutToTrigger(RuleAttackRoll evt) { }

      public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }

      public void OnEventDidTrigger(RuleAttackRoll evt)
      {
        try
        {
          var target = evt.Target;
          if (target is null)
          {
            Logger.Warning("No target");
            return;
          }

          if (evt.Weapon?.Blueprint?.IsMelee != true || !evt.IsHit)
            return;

          var dc = 10 + Owner.Stats.GetStat(StatType.BaseAttackBonus);
          Logger.NativeLog($"Attempting to daze {target.CharacterName} with DC {dc}");
          var savingThrow = Context.TriggerRule(new RuleSavingThrow(target, SavingThrowType.Fortitude, dc));
          if (savingThrow.IsPassed)
            return;

          var buff = target.AddBuff(BuffRefs.Daze.Reference.Get(), Context, 1.Rounds().Seconds);
          buff.IsNotDispelable = true;
        }
        catch (Exception e)
        {
          Logger.LogException("DazingAssaultComponent.OnEventAboutToTrigger", e);
        }
      }
    }
  }
}
