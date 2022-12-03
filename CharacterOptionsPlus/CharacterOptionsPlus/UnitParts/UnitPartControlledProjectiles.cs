using CharacterOptionsPlus.Util;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.Utility;
using Kingmaker.Visual.Particles;
using Kingmaker.Visual.Particles.FxSpawnSystem;
using Newtonsoft.Json;
using Owlcat.Runtime.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.UnitParts
{
  /// <summary>
  /// Similar to <c>ControllableProjectileContainerPart</c> but allows multiple projectiles per buff.
  /// </summary>
  [JsonObject]
  internal class UnitPartControlledProjectiles : UnitPart
  {
    [JsonProperty]
    private Dictionary<BlueprintGuid, List<ControlledProjectile>> Projectiles;

    internal List<ControlledProjectile> Get(BlueprintBuffReference buff)
    {
      return Projectiles.Get(buff.Guid);
    }

    internal void Register(BlueprintBuffReference buff, List<ControlledProjectile> projectiles)
    {
      ConsumeAll(buff);
      Projectiles[buff.Guid] = projectiles;
    }

    internal ControlledProjectile Consume(BlueprintBuffReference buff)
    {
      if (!Projectiles.ContainsKey(buff.Guid))
        return null;

      var projectiles = Projectiles[buff.Guid];
      var projectile = projectiles.FirstOrDefault();
      if (projectile is null || projectile.Handle is null)
        return null;

      projectiles.Remove(projectile);
      if (!projectiles.Any())
        Projectiles.Remove(buff.Guid);

      return projectile;
    }

    internal void ConsumeAll(BlueprintBuffReference buff)
    {
      if (!Projectiles.ContainsKey(buff.Guid))
        return;

      foreach (var projectile in Projectiles[buff.Guid])
      {
        if (projectile?.Handle is not null)
          FxHelper.Destroy(projectile.Handle);
      }
      Projectiles.Remove(buff.Guid);
    }
  }

  [JsonObject]
  internal class ControlledProjectile
  {
    [JsonIgnore]
    internal IFxHandle Handle;

    internal ControlledProjectile() { }

    internal ControlledProjectile(IFxHandle handle)
    {
      Handle = handle;
    }
  }

  [AllowedOn(typeof(BlueprintBuff))]
  [TypeId("9ac5f05d-3710-42b1-a378-0630b55e6ee7")]
  public abstract class ProjectileController : UnitBuffComponentDelegate
  {
    public abstract void SpawnProjectiles();
  }

  [TypeId("d64f7afb-419b-48a5-a8f0-b343724fe5a4")]
  internal class AbilityDeliverControlledProjectile : AbilityDeliverEffect
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AbilityDeliverControlledProjectile));

    private readonly BlueprintBuffReference Buff;

    internal AbilityDeliverControlledProjectile(BlueprintBuffReference buff)
    {
      Buff = buff;
    }

    public override IEnumerator<AbilityDeliveryTarget> Deliver(AbilityExecutionContext context, TargetWrapper target)
    {
      var caster = context.Caster;
      if (caster is null)
      {
        Logger.Warning("No caster");
        yield break;
      }

      var unitPart = context.Caster.Get<UnitPartControlledProjectiles>();
      if (unitPart is null)
      {
        Logger.Warning("Attempted to deliver a controlled projectile but there is no unit part.");
        yield break;
      }

      var projectile = unitPart.Consume(Buff);
      if (projectile is null || projectile.Handle is null)
      {
        Logger.Warning("Attempted to deliver a controlled projectile but there are none.");
        yield break;
      }

      while (!projectile.Handle.IsSpawned)
        yield return null;

      var startTime = Game.Instance.TimeController.GameTime;
      var direction = (target.Point - projectile.Handle.SpawnedObject.transform.position).ToXZ().normalized;

    }
  }

  [TypeId("03ed7c7d-3859-48a7-a91a-0ec3e09e575e")]
  internal class ControlledProjectilesHolder : BlueprintComponent, IRuntimeEntityFactComponentProvider
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(ControlledProjectilesHolder));

    public EntityFactComponent CreateRuntimeFactComponent()
    {
      return new Runtime((OwnerBlueprint as BlueprintBuff).ToReference<BlueprintBuffReference>());
    }

    [JsonObject]
    public class Runtime : EntityFactComponent<UnitEntityData, ControlledProjectilesHolder>, IAreaHandler
    {
      [JsonProperty]
      internal BlueprintBuffReference Buff;

      internal Runtime(BlueprintBuffReference buff)
      {
        Buff = buff;
      }

      public void OnAreaDidLoad()
      {
        try
        {
          SpawnProjectiles();
        }
        catch (Exception e)
        {
          Logger.LogException("ControlledProjectilesHolder.OnAreaDidLoad", e);
        }
      }

      private void SpawnProjectiles()
      {
        Owner.GetFact(Buff).GetComponent<ProjectileController>().SpawnProjectiles();
      }

      public void OnAreaBeginUnloading()
      {
        try
        {
          ConsumeAll();
        }
        catch (Exception e)
        {
          Logger.LogException("ControlledProjectilesHolder.OnAreaBeginUnloading", e);
        }
      }

      private void ConsumeAll()
      {
        Owner.Get<UnitPartControlledProjectiles>().ConsumeAll(Buff);
      }
    }
  }
}
