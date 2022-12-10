using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using Kingmaker.UnitLogic.Mechanics.Components;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Util
{
  internal class CommonBlueprints
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(CommonBlueprints));

    internal static void Configure()
    {
      Logger.Log($"Configuring {nameof(CommonBlueprints)}");

      ConfigureBlankProjectile();
      ConfigurePanicked();
    }

    private const string BlankProjectile = "BlankProjectile";
    private static void ConfigureBlankProjectile()
    {
      ProjectileConfigurator.New(BlankProjectile, Guids.BlankProjectile)
        .CopyFrom(ProjectileRefs.Arrow)
        .SetProjectileHit(new() { FollowTarget = true })
        .SetStuckArrowPrefab(null)
        .SetDeflectedArrowPrefab(null)
        .Configure();
    }

    private const string Panicked = "Panicked";
    private static void ConfigurePanicked()
    {
      BuffConfigurator.New(Panicked, Guids.PanickedBuff)
        .CopyFrom(BuffRefs.EyebitePanickedBuff, c => c is not AddFactContextActions)
        .Configure();
    }
  }
}
