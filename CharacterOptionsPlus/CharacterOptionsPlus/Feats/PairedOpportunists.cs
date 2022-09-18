using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TabletopTweaks.Core.NewComponents.Prerequisites;
using static UnityModManagerNet.UnityModManager.ModEntry;
using Kingmaker.Blueprints.Classes;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Blueprints.Facts;
using BlueprintCore.Utils;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using Kingmaker;
using Kingmaker.Blueprints.Classes.Selection;
using BlueprintCore.Blueprints.References;

namespace CharacterOptionsPlus.Feats
{
  public class PairedOpportunists
  {
    internal const string FeatName = "PairedOpportunists";
    internal const string FeatDisplayName = "PairedOpportunists.Name";
    private const string FeatDescription = "PairedOpportunists.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "pairedopportunists.png"; // TODO: Create it!

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      if (Settings.IsEnabled(Guids.GloriousHeatFeat))
        ConfigureEnabled();
      else
        ConfigureDisabled();
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      FeatureConfigurator.New(FeatName, Guids.PairedOpportunistsFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      FeatureConfigurator.New(
          FeatName, Guids.PairedOpportunistsFeat, FeatureGroup.Feat, FeatureGroup.CombatFeat, FeatureGroup.TeamworkFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        //.SetIcon(IconName)
        .AddFeatureTagsComponent(FeatureTag.Melee | FeatureTag.Attack | FeatureTag.Teamwork)
        .AddRecommendationWeaponSubcategoryFocus(WeaponSubCategory.Melee)
        .AddRecommendationThreeQuartersBAB()
        .AddRecommendationHasFeature(FeatureRefs.SoloTactics.ToString())
        .AddRecommendationHasFeature(FeatureRefs.InquisitorSoloTactician.ToString())
        .AddComponent<PairedOpportunistsComponent>()
        .Configure(delayed: true);
    }

    [TypeId("ce4218b8-27b6-4484-93df-2458aa7ae788")]
    public class PairedOpportunistsComponent
      : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAttackBonus>, IAttackOfOpportunityHandler
    {
      private static readonly Feet Adjacency = new(5);

      private BlueprintUnitFact _pairedOpportunists;
      private BlueprintUnitFact PairedOpportunists
      {
        get
        {
          _pairedOpportunists ??= BlueprintTool.Get<BlueprintUnitFact>(Guids.PairedOpportunistsFeat);
          return _pairedOpportunists;
        }
      }

      public void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
      {
        if (evt.Reason.Rule is not RuleAttackWithWeapon attack || !attack.IsAttackOfOpportunity)
        {
          Logger.NativeLog("Skipping: Not AOO.");
          return;
        }

        if (Owner.State.Features.SoloTactics)
        {
          AddAttackBonus(evt);
          return;
        }

        foreach (var unit in GameHelper.GetTargetsAround(Owner.Position, Adjacency))
        {
          if (unit.IsAlly(Owner) && unit.Descriptor.HasFact(PairedOpportunists) && unit.IsEngage(evt.Target))
          {
            AddAttackBonus(evt);
            return;
          }
        }

        Logger.NativeLog("Skipping: No supporting ally.");
      }

      private static void AddAttackBonus(RuleCalculateAttackBonus evt)
      {
        Logger.NativeLog("Adding Paired Opportunists attack bonus.");
        evt.AddModifier(new(4, ModifierDescriptor.Circumstance));
      }

      public void HandleAttackOfOpportunity(UnitEntityData attacker, UnitEntityData target)
      {
        if (attacker == Owner)
        {
#if DEBUG
          Logger.NativeLog("Not Provoking: Attacker is owner.");
#endif
          return;
        }

        if (!attacker.IsAlly(Owner) || attacker.Descriptor.HasFact(PairedOpportunists))
        {
#if DEBUG
          Logger.NativeLog("Not Provoking: No supporting ally.");
#endif
          return;
        }

        if (!Owner.IsEngage(target))
        {
#if DEBUG
          Logger.NativeLog("Not Provoking: Not engaged with target.");
#endif
          return;
        }

        Logger.NativeLog($"{attacker.CharacterName} provoked an attack against {target.CharacterName}");
        Game.Instance?.CombatEngagementController.ForceAttackOfOpportunity(Owner, target);
      }

      public void OnEventDidTrigger(RuleCalculateAttackBonus evt) { }
    }
  }
}
