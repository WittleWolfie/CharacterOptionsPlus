﻿using BlueprintCore.Blueprints.CustomConfigurators.Classes;
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
using System;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  public class FuriousFocus
  {
    private const string FeatName = "FuriousFocus";
    private const string FeatGuid = "de9a75d3-1289-4098-a0b7-fda465a79576";

    private const string FeatDisplayName = "FuriousFocus.Name";
    private const string FeatDescription = "FuriousFocus.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "furiousfocus.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    /// <summary>
    /// Adds the Furious Focus feat.
    /// </summary>
    public static void Configure()
    {
      Logger.Log($"Configuring {FeatName}");

      FeatureConfigurator.New(FeatName, FeatGuid, FeatureGroup.Feat, FeatureGroup.CombatFeat, FeatureGroup.RangerStyle)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddRecommendationThreeQuartersBAB()
        .AddRecommendationWeaponSubcategoryFocus(subcategory: WeaponSubCategory.TwoHanded, hasFocus: true)
        .AddFeatureTagsComponent(featureTags: FeatureTag.Melee | FeatureTag.Damage)
        .AddPrerequisiteStatValue(StatType.Strength, 13)
        .AddPrerequisiteStatValue(StatType.BaseAttackBonus, 1)
        .AddPrerequisiteFeature(FeatureRefs.PowerAttackFeature.ToString())
        .AddComponent<FuriousFocusBonus>()
        // FeatureGroup.RangerStyle isn't associated with a selection, this adds it to the appropriate selections.
        .AddToRangerStyles(RangerStyle.TwoHanded6)
        .Configure();
    }

    [TypeId("d7aa29aa-b4d0-4739-8856-6ee954d84aa8")]
    private class FuriousFocusBonus :
      UnitFactComponentDelegate,
      IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>
    {
      private readonly BlueprintBuff PowerAttackBuff = BuffRefs.PowerAttackBuff.Reference.Get();

      public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }

      public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt)
      {
        try
        {
          if (evt.Weapon is null || !evt.Weapon.Blueprint.IsTwoHanded)
          {
            Logger.NativeLog("Skipped: not a 2H weapon attack.");
            return;
          }

          var rule = evt.Reason.Rule;
          if (rule is RuleAttackWithWeapon attackRule && !attackRule.IsFirstAttack)
          {
            Logger.NativeLog("Skipped: not first attack");
            return;
          }

          var powerAttackModifier =
            evt.m_ModifiableBonus?.Modifiers?
              .Where(m => m.Fact?.Blueprint == PowerAttackBuff)
              .Select(m => (Modifier?)m) // Cast to avoid getting a default struct
              .FirstOrDefault();
          if (powerAttackModifier is null)
          {
            Logger.NativeLog("Skipped: power attack not applied");
            return;
          }

          Logger.NativeLog($"Adding attack bonus to {Owner.CharacterName}'s attack");
          evt.AddModifier(new Modifier(-powerAttackModifier.Value.Value, Fact, ModifierDescriptor.UntypedStackable));
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to handle event.", e);
        }
      }
    }
  }
}