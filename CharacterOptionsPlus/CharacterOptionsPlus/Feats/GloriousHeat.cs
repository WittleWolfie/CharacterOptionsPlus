using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using System;
using System.Linq;
using TabletopTweaks.Core.NewComponents.Prerequisites;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  public class GloriousHeat
  {
    internal const string FeatName = "GloriousHeat";
    internal const string FeatDisplayName = "GloriousHeat.Name";
    private const string FeatDescription = "GloriousHeat.Description";

    internal const string BuffName = "GloriousHeat.Buff";
    private const string BuffDescription = "GloriousHeat.Buff.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.GloriousHeatFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("GloriousHeat.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.GloriousHeatBuff)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(BuffDescription)
        .Configure();

      FeatureConfigurator.New(FeatName, Guids.GloriousHeatFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var buff = BuffConfigurator.New(BuffName, Guids.GloriousHeatBuff)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(BuffDescription)
        .SetIcon(IconName)
        .AddContextStatBonus(StatType.AdditionalAttackBonus, 1, descriptor: ModifierDescriptor.Morale)
        .Configure();

      FeatureConfigurator.New(FeatName, Guids.GloriousHeatFeat, FeatureGroup.Feat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .SetIsClassFeature()
        .AddRecommendationRequiresSpellbook()
        .AddFeatureTagsComponent(FeatureTag.Magic | FeatureTag.Defense)
        .AddPrerequisiteCasterType(isArcane: false)
        .AddComponent<PrerequisiteCasterLevel>(c => c.RequiredCasterLevel = 5)
        .AddComponent(new GloriousHeatTrigger(buff))
        .Configure(delayed: true);
    }

    [TypeId("51ba2dc7-12a2-4915-919b-3381242ea498")]
    private class GloriousHeatTrigger : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCastSpell>
    {
      private static readonly Feet Range = new(30);

      private readonly BlueprintBuff Buff;

      public GloriousHeatTrigger(BlueprintBuff buff)
      {
        Buff = buff;
      }

      public void OnEventAboutToTrigger(RuleCastSpell evt) { }

      public void OnEventDidTrigger(RuleCastSpell evt)
      {
        try
        {
          if (evt.Spell.Spellbook?.Blueprint.IsArcane == true)
          {
            Logger.Verbose("Skipped: Arcane spell.");
            return;
          }

          if (!evt.Spell.Blueprint.SpellDescriptor.HasFlag(SpellDescriptor.Fire))
          {
            Logger.Verbose("Skipped: Missing fire descriptor.");
            return;
          }

          var targets = GameHelper.GetTargetsAround(Owner.Position, Range).Where(unit => unit.IsAlly(Owner));
          if (!targets.Any())
          {
            Logger.Verbose("Skipped: No valid targets.");
            return;
          }

          // Exclude targets that would be hurt
          targets = targets.Where(unit => !unit.Facts.HasComponent<SufferFromHealing>(fact => true));
          if (!targets.Any())
          {
            Logger.Verbose("Skipped: Targets suffer from healing.");
            return;
          }

          var effectTarget = targets.First();
          foreach (var target in targets)
          {
            if (effectTarget.Damage < evt.Spell.SpellLevel && effectTarget.Damage < target.Damage)
            {
              // Change to the target which receives more healing
              effectTarget = target;
            }
            else if (target.Damage > 0 && target.CurrentHP() < effectTarget.CurrentHP())
            {
              // Change to the target with the lowest health
              effectTarget = target;
            }
          }

          Logger.Verbose($"Applying GLORIOUS HEAT to {effectTarget.CharacterName}");
          effectTarget.AddBuff(Buff, Context, duration: ContextDuration.Fixed(1).Calculate(Context).Seconds);

          int healValue =
            Settings.IsEnabled(Homebrew.OriginalGloriousHeat) && evt.Spell.SpellLevel > 0
              ? Owner.Descriptor.Progression.CharacterLevel
              : evt.Spell.SpellLevel;
          if (healValue > 0)
            Rulebook.Trigger<RuleHealDamage>(new(Owner, effectTarget, healValue));
        }
        catch (Exception e)
        {
          Logger.LogException("OnEventDidTrigger", e);
        }
      }
    }
  }
}
