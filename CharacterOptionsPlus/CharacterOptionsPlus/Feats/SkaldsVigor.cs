using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes;

namespace CharacterOptionsPlus.Feats
{
  internal class SkaldsVigor
  {
    internal const string FeatureName = "SkaldsVigor";
    internal const string FeatName = "SkaldsVigor";
    internal const string FeatGuid = "55dd527b-8721-426b-aaa2-036ccc9a0458";

    private static readonly LogWrapper Logger = LogWrapper.Get(FeatureName);

    internal static void Configure()
    {
      Logger.Verbose($"Configuring {FeatureName}");

      FeatureConfigurator.New(FeatureName, FeatGuid, FeatureGroup.Feat, FeatureGroup.CombatFeat)
        .SetDisplayName("SkaldsVigor.Name")
        .SetDescription("SkaldsVigor.Description")
        .AddPrerequisiteFeature(FeatureRefs.RagingSong.ToString())
        .Configure();
    }
  }
}
