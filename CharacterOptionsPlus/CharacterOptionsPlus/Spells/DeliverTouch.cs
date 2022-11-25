using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.UnitParts;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class DeliverTouch
  {
    private const string FeatureName = "DeliverTouch";

    internal const string DisplayName = "DeliverTouch.Name";
    private const string Description = "DeliverTouch.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "gloriousheat.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.DeliverTouchSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("DeliverTouch.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.DeliverTouchSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var deliverTouch = AbilityConfigurator.New(FeatureName, Guids.DeliverTouchSpell)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(enemies: true, friends: true, self: true)
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .AddComponent<CasterHasTouchCharge>()
        .AddAbilityDeliverTouch(touchWeapon: ItemWeaponRefs.TouchItem.ToString())
        .AddComponent<AbilityEffectDeliverTouch>()
        .Configure();

      // Trick from TTT: attach base abilities to FightDefensivelyFeature to add them to all characters.
      FeatureConfigurator.For(FeatureRefs.FightDefensivelyFeature)
        .EditComponent<AddFacts>(
          c => c.m_Facts = CommonTool.Append(c.m_Facts, deliverTouch.ToReference<BlueprintUnitFactReference>()))
        .Configure();
    }

    [TypeId("accc5915-d7b3-4902-9b04-595a021ff42a")]
    private class AbilityEffectDeliverTouch : AbilityApplyEffect
    {
      public override void Apply(AbilityExecutionContext context, TargetWrapper target)
      {
        try
        {
          var caster = context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var touchPart = caster.Get<UnitPartTouch>();
          if (touchPart is null)
          {
            Logger.Warning($"{caster.CharacterName} is not holding a touch spell");
            return;
          }

          if (caster == target.Unit)
          {
            Rulebook.Trigger<RuleCastSpell>(new(touchPart.Ability.Data, target));
            return;
          }

          UnitCommand unitCommand = UnitUseAbility.CreateCastCommand(touchPart.Ability.Data, target);
          touchPart.AutoCastCommand = unitCommand;
          unitCommand.IgnoreCooldown(new TimeSpan?(touchPart.IgnoreCooldownBeforeTime));
          caster.Commands.AddToQueueFirst(unitCommand);
        }
        catch (Exception e)
        {
          Logger.LogException("AbilityEffectDeliverTouch.Apply", e);
        }
      }
    }

    [TypeId("5f1fe8d6-fa4f-4724-9849-c52d2cb67185")]
    private class CasterHasTouchCharge : UnitFactComponentDelegate, IAbilityCasterRestriction
    {
      public string GetAbilityCasterRestrictionUIText()
      {
        return LocalizationTool.GetString("DeliverTouch.Unavailable.Description");
      }

      public bool IsCasterRestrictionPassed(UnitEntityData caster)
      {
        if (caster.Get<UnitPartTouch>() is null)
          return false;

        var charges = caster.Get<UnitPartTouchCharges>();
        if (charges is not null && !charges.HasCharge())
        return true;
      }
    }
  }
}
