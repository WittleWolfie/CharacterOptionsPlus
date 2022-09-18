using Kingmaker.EntitySystem.Entities;

namespace CharacterOptionsPlus.Util
{
  internal static class UnitExtensions
  {
    internal static int CurrentHP(this UnitEntityData unit)
    {
      return unit.Stats.HitPoints.ModifiedValue - unit.Damage;
    }
  }
}
