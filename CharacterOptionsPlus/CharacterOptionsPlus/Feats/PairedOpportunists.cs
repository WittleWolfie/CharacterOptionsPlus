using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using static UnityModManagerNet.UnityModManager.ModEntry;
using Kingmaker.Blueprints.Classes;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Facts;
using BlueprintCore.Utils;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Blueprints.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using System;

namespace CharacterOptionsPlus.Feats
{
  public class PairedOpportunists
  {
    internal const string FeatName = "PairedOpportunists";
    internal const string FeatDisplayName = "PairedOpportunists.Name";
    private const string FeatDescription = "PairedOpportunists.Description";

    internal const string BuffName = "PairedOpportunists.Buff";
    internal const string AbilityName = "PairedOpportunists.Ability";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "pairedopportunists.png";

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
        Logger.LogException("PairedOpportunists.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      BuffConfigurator.New(BuffName, Guids.PairedOpportunistsBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .Configure();

      ActivatableAbilityConfigurator.New(AbilityName, Guids.PairedOpportunistsAbility)
        .Configure();

      FeatureConfigurator.New(FeatName, Guids.PairedOpportunistsFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var buff = BuffConfigurator.New(BuffName, Guids.PairedOpportunistsBuff)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .Configure();

      var ability = ActivatableAbilityConfigurator.New(AbilityName, Guids.PairedOpportunistsAbility)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .SetBuff(buff)
        .SetIsOnByDefault()
        .SetDeactivateImmediately()
        .Configure();

      FeatureConfigurator.New(
          FeatName, Guids.PairedOpportunistsFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat, FeatureGroup.TeamworkFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddFeatureTagsComponent(FeatureTag.Melee | FeatureTag.Attack | FeatureTag.Teamwork)
        .AddRecommendationHasFeature(FeatureRefs.BattleProwessFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.MonsterTacticsFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.CavalierTacticianFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.VanguardTacticianFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.TacticalLeaderFeatShareFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.HunterTactics.ToString())
        .AddRecommendationHasFeature(FeatureRefs.SacredHuntsmasterTactics.ToString())
        .AddRecommendationHasFeature(FeatureRefs.PackRagerRagingTacticianBaseFeature.ToString())
        .AddRecommendationHasFeature(FeatureRefs.SoloTactics.ToString())
        .AddRecommendationHasFeature(FeatureRefs.InquisitorSoloTactician.ToString())
        .AddAsTeamworkFeat(
          Guids.PairedOpportunistsCavalier,
          Guids.PairedOpportunistsVanguardBuff,
          Guids.PairedOpportunistsVanguardAbility,
          Guids.PairedOpportunistsRagerBuff,
          Guids.PairedOpportunistsRagerArea,
          Guids.PairedOpportunistsRagerAreaBuff,
          Guids.PairedOpportunistsRagerToggleBuff,
          Guids.PairedOpportunistsRagerToggle)
        .AddComponent<PairedOpportunistsComponent>()
        .AddFacts(new() { ability })
        .Configure(delayed: true);
    }

    [TypeId("ce4218b8-27b6-4484-93df-2458aa7ae788")]
    public class PairedOpportunistsComponent
      : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAttackBonus>, IAttackOfOpportunityHandler
    {
      private static readonly Feet Adjacency = new(5);
      // Used to ensure Paired Opportunist can't trigger off itself
      private static bool Provoking = false;

      private static BlueprintUnitFact _pairedOpportunists;
      private static BlueprintUnitFact PairedOpportunists
      {
        get
        {
          _pairedOpportunists ??= BlueprintTool.Get<BlueprintUnitFact>(Guids.PairedOpportunistsFeat);
          return _pairedOpportunists;
        }
      }

      private static BlueprintBuff _opportunistBuff;
      private static BlueprintBuff OpportunistBuff
      {
        get
        {
          _opportunistBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.PairedOpportunistsBuff);
          return _opportunistBuff;
        }
      }

      public void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
      {
        try
        {
          if (evt.Reason.Rule is not RuleAttackWithWeapon attack || !attack.IsAttackOfOpportunity)
          {
            Logger.Verbose(() => "Skipping: Not AOO.");
            return;
          }

          if (Owner.State.Features.SoloTactics)
          {
            AddAttackBonus(evt);
            return;
          }

          foreach (var unit in GameHelper.GetTargetsAround(Owner.Position, Adjacency))
          {
            if (unit != Owner
              && unit.IsAlly(Owner)
              && unit.Descriptor.HasFact(PairedOpportunists)
              && unit.IsEngage(
                evt.Target,
                canEngage: unit.Descriptor.State.CanAct,
                threatRange: (float)unit.GetThreatRange(attack.WeaponSlot)))
            {
              AddAttackBonus(evt);
              return;
            }
          }

          Logger.Verbose(() => "Skipping: No supporting ally.");
        }
        catch (Exception e)
        {
          Logger.LogException("PairedOpportunists.OnEventAboutToTrigger", e);
        }
      }

      private void AddAttackBonus(RuleCalculateAttackBonus evt)
      {
        Logger.Verbose(() => "Adding Paired Opportunists attack bonus.");
        evt.AddModifier(4, Fact, ModifierDescriptor.Circumstance);
      }

      public void HandleAttackOfOpportunity(UnitEntityData attacker, UnitEntityData target)
      {
        try
        {
          if (Provoking)
          {
            Logger.Verbose(() => "Not Provoking: Currently resolving provoke attack.");
            return;
          }

          if (attacker == Owner)
          {
            Logger.Verbose(() => "Not Provoking: Attacker is owner.");
            return;
          }

          if (!Owner.HasFact(OpportunistBuff))
          {
            Logger.Verbose(() => "Not Provoking: Ability turned off.");
            return;
          }

          if (!Owner.State.Features.SoloTactics
            && !(attacker.IsAlly(Owner)
              && attacker.Descriptor.HasFact(PairedOpportunists)
              && attacker.DistanceTo(Owner) < Adjacency.Meters))
          {
            Logger.Verbose(() => "Not Provoking: No supporting ally.");
            return;
          }

          var threatRange = Owner.GetThreatRange(Owner.GetThreatHand());
          if (threatRange is null)
          {
            Logger.Verbose(() => "Not Provoking: Not threat range");
          }

          if (!Owner.IsAttackOfOpportunityReach(target, threatRange.Value))
          {
            Logger.Verbose(() => $"Not Provoking: Not in range of {target.CharacterName}.");
            return;
          }

          Logger.Verbose(() => $"{attacker.CharacterName} provoked an attack against {target.CharacterName}");
          Provoking = true;
          Owner.CombatState.AttackOfOpportunity(target);
          Provoking = false;
        }
        catch (Exception e)
        {
          Logger.LogException("PairedOpportunists.HandleAttackOfOpportunity", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAttackBonus evt) { }
    }
  }
}
