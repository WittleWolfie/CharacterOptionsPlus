using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharacterOptionsPlus.Feats
{
  internal class PurifyingChannel
  {
    internal const string FeatName = "PurifyingChannel";
    internal const string FeatDisplayName = "PurifyingChannel.Name";
    private const string FeatDescription = "PurifyingChannel.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.PurifyingChannelFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("PurifyingChannel.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      FeatureConfigurator.New(FeatName, Guids.PurifyingChannelFeat).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var selectiveChannel = FeatureRefs.SelectiveChannel.Reference.Get();
      FeatureConfigurator.New(FeatName, Guids.PurifyingChannelFeat, FeatureGroup.Feat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddFeatureTagsComponent(FeatureTag.ClassSpecific | FeatureTag.Damage)
        .AddPrerequisiteFeature(selectiveChannel)
        .AddPrerequisiteStatValue(StatType.Charisma, 15)
        .AddRecommendationHasFeature(selectiveChannel)
        .AddComponent<PurifyingChannelTrigger>()
        .Configure(delayed: true);
    }

    [TypeId("6a3a02e8-7aff-4249-ab31-fc5570e9678e")]
    private class PurifyingChannelTrigger : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCastSpell>
    {
      private static readonly Feet Range = new(30);

      private static readonly BlueprintFeature NegativeEnergyAffinity =
        FeatureRefs.NegativeEnergyAffinity.Reference.Get();

      // TODO: Evangelist from Homebrew Archetypes
      private static readonly List<BlueprintReference<BlueprintAbility>> PositiveHeal =
        new()
        {
          AbilityRefs.ChannelEnergy.Reference,
          AbilityRefs.ChannelEnergyHospitalerHeal.Reference,
          AbilityRefs.ChannelEnergyEmpyrealHeal.Reference,
          AbilityRefs.ChannelEnergyPaladinHeal.Reference,
          AbilityRefs.ShamanLifeSpiritChannelEnergy.Reference,
          AbilityRefs.OracleRevelationChannelAbility.Reference,
          AbilityRefs.WarpriestChannelEnergy.Reference,
          AbilityRefs.HexChannelerChannelEnergy.Reference,
        };

      public void OnEventAboutToTrigger(RuleCastSpell evt) { }

      public void OnEventDidTrigger(RuleCastSpell evt)
      {
        try
        {
          UnitEntityData target = null;
          foreach (var positiveChannel in PositiveHeal)
          {
            if (evt.Spell.Blueprint == positiveChannel.Get())
            {
              Logger.Verbose($"Purifying channel triggered for {evt.Spell.Name}");
              target = SelectTarget();
              return;
            }
          }

          if (target is null)
            return;

          int dmg = evt.Context[AbilitySharedValue.Damage];
          if (Rulebook.Trigger<RuleSavingThrow>(new(target, SavingThrowType.Will, evt.Spell.CalculateParams().DC)).IsPassed)
          {
            dmg /= 2;
            Logger.Verbose($"Dealing {dmg} damage to {target} (halved)");
          }
          else
          {
            Logger.Verbose($"Dealing {dmg} damage to {target} and applying dazzled");
            target.AddBuff(
              BuffRefs.DazzledBuff.Reference.Get(),
              Context,
              duration: ContextDuration.Fixed(1).Calculate(Context).Seconds);
          }
          Rulebook.Trigger<RuleDealDamage>(
            new(Owner, target, DamageTypes.Direct().GetDamageDescriptor(DiceFormula.Zero, dmg).CreateDamage()));
        }
        catch (Exception e)
        {
          Logger.LogException("PurifyingChannelTrigger.OnEventDidTrigger", e);
        }
      }

      private UnitEntityData SelectTarget()
      {
        var targets = GameHelper.GetTargetsAround(Owner.Position, Range).Where(unit => !unit.IsAlly(Owner));
        if (!targets.Any())
        {
          Logger.Verbose("Skipped: No valid targets.");
          return null;
        }

        targets = targets.Where(unit => !unit.HasFact(NegativeEnergyAffinity));
        if (!targets.Any())
        {
          Logger.Verbose("Skipped: No affected targets.");
          return null;
        }

        return targets.First();
      }
    }
  }
}
