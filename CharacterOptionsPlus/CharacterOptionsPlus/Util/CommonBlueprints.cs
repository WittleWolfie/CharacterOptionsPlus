using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using System.Collections.Generic;

namespace CharacterOptionsPlus.Util
{
  internal class CommonBlueprints
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(CommonBlueprints));

    internal static void Configure()
    {
      Logger.Log($"Configuring {nameof(CommonBlueprints)}");

      ConfigureBlankProjectile();
      ConfigurePanickedDeprecated();
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
    private static void ConfigurePanickedDeprecated()
    {
      BuffConfigurator.New(Panicked, Guids.PanickedBuffDeprecated).Configure();
    }

    #region Channel Blueprints
    internal static readonly List<BlueprintReference<BlueprintAbility>> ChannelPositiveHeal =
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
          
          // Homebrew Archetypes
          BlueprintTool.GetRef<BlueprintReference<BlueprintAbility>>(Guids.EvangelistPositiveHeal),
      };
    #endregion
  }
}
