using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Linq;

namespace CharacterOptionsPlus.Components
{
  [TypeId("bed832dd-3701-4d7b-9f66-552bf89a8cfe")]
  internal class AbilityDeliverBurst : AbilityDeliverEffect
  {
    private readonly Feet Radius;

    internal AbilityDeliverBurst(Feet radius)
    {
      Radius = radius;
    }

    public override IEnumerator<AbilityDeliveryTarget> Deliver(AbilityExecutionContext context, TargetWrapper target)
    {
      return (IEnumerator<AbilityDeliveryTarget>)GameHelper.GetTargetsAround(target.m_Point, Radius)
        .Select(unit => new AbilityDeliveryTarget(unit));
    }
  }
}
