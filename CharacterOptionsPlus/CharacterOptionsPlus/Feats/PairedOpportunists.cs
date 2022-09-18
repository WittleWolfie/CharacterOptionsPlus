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
        .Configure(delayed: true);
    }
  }
}
