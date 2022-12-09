using BlueprintCore.Utils;
using BlueprintCore.Utils.Assets;
using CharacterOptionsPlus.Util;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers.Projectiles;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.Utility;
using Kingmaker.Visual.Particles;
using Kingmaker.Visual.Particles.FxSpawnSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private Dictionary<BlueprintGuid, List<ControlledProjectile>> Projectiles = new();

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
      var projectile = projectiles.LastOrDefault();
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
  public abstract class ProjectileControllerComponent : UnitBuffComponentDelegate
  {
    public abstract RuleAttackRoll RollAttack(AbilityExecutionContext context);
    public abstract void SpawnProjectiles(UnitEntityData caster);
  }

  [TypeId("d64f7afb-419b-48a5-a8f0-b343724fe5a4")]
  internal class AbilityDeliverControlledProjectile : AbilityDeliverEffect
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(AbilityDeliverControlledProjectile));
    private static readonly BlueprintProjectileReference FakeProjectile =
      BlueprintTool.GetRef<BlueprintProjectileReference>(Guids.BlankProjectile);

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

      var unitPart = caster.Get<UnitPartControlledProjectiles>();
      if (unitPart is null)
      {
        Logger.Warning("Attempted to deliver a controlled projectile but there is no unit part.");
        yield break;
      }

      var controlledProjectile = unitPart.Consume(Buff);
      if (controlledProjectile is null || controlledProjectile.Handle is null)
      {
        Logger.Warning("Attempted to deliver a controlled projectile but there are none.");
        yield break;
      }

      while (!controlledProjectile.Handle.IsSpawned)
        yield return null;

      // This setup is a mixture of AbilityDeliverProjectile.Deliver() and ProjectileController.Launch()
      var spawnedProjectile = controlledProjectile.Handle.SpawnedObject;
      var projectile =
        new Projectile
        {
          Launcher = spawnedProjectile.transform.position,
          Target = target,
          Blueprint = FakeProjectile.Get(),
          MaxRange = context.AbilityBlueprint.GetRange(abilityData: context.Ability).Meters,
          IsFirstProjectile = true,
        };
      projectile.LaunchPosition = projectile.Launcher.Point + Vector3.up;
      projectile.BeforeLaunch();

      // Make sure the projectile controller view is setup; same logic as in ProjectileController
      var projectileController = Game.Instance.ProjectileController;
      if (projectileController.m_ViewParent is null)
      {
        projectileController.m_ViewParent = new GameObject("__Projectiles__").transform;
        SceneManager.MoveGameObjectToScene(
          projectileController.m_ViewParent.gameObject, SceneManager.GetSceneByName("BaseMechanics"));
      }

      // We can't call ProjectileController because it uses the Blueprint, so just configure the view on our own
      projectile.View = spawnedProjectile;
      spawnedProjectile.DestroyComponents<SnapToTransformWithRotation>();
      spawnedProjectile.transform.parent = projectileController.m_ViewParent;
      projectileController.AddProjectile(projectile);
      ProjectileController.ApplyLightProbeAnchor(projectile.View);

      // The rest of the logic in ProjectileController.Launch() isn't needed, move on to the attack roll
      var buff = caster.Buffs.GetBuff(Buff);
      var controller = buff.GetComponent<ProjectileControllerComponent>();
      var attackRoll = controller.RollAttack(context);
      projectile.AttackRoll = attackRoll;
      projectile.MissTarget = context.MissTarget;

      // Loop until the projectile hits
      while (!projectile.IsHit && !projectile.Cleared)
        yield return null;

      attackRoll.ConsumeMirrorImageIfNecessary();
      using (ContextData<BuffCollection.RemoveByRank>.Request())
        buff.Remove();
      GameObject.DestroyImmediate(spawnedProjectile);

      if (projectile.Cleared)
        yield break;

      yield return new AbilityDeliveryTarget(projectile.Target)
        {
          AttackRoll = attackRoll,
          Projectile = projectile
        };
      yield break;
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
        Owner.GetFact(Buff).GetComponent<ProjectileControllerComponent>().SpawnProjectiles(Owner);
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
