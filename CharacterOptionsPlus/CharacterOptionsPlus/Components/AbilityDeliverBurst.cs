using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [TypeId("bed832dd-3701-4d7b-9f66-552bf89a8cfe")]
  internal class AbilityDeliverBurst : AbilityDeliverEffect
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(AbilityDeliverBurst));

    private readonly Feet Radius;
    private readonly bool IncludeCaster;
    private readonly bool CenterOnCaster;

    internal AbilityDeliverBurst(Feet radius, bool includeCaster = false, bool centerOnCaster = true)
    {
      Radius = radius;
      IncludeCaster = includeCaster;
      CenterOnCaster = centerOnCaster;
    }

    public override IEnumerator<AbilityDeliveryTarget> Deliver(AbilityExecutionContext context, TargetWrapper target)
    {
      var caster = context.MaybeCaster;
      if (caster is null)
      {
        Logger.Warning("No caster");
        yield break;
      }

      var targetPoint = CenterOnCaster ? caster.Position : target.m_Point;
      foreach (var unit in GameHelper.GetTargetsAround(targetPoint, Radius))
      {
        if (unit == context.MaybeCaster && !IncludeCaster)
          continue;

        if (context.HasMetamagic(Metamagic.Selective) && !unit.IsEnemy(context.MaybeCaster))
          continue;

        yield return new(unit);
      }
      yield break;
    }
  }
}
