using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
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
        .Configure();

      // Trick from TTT: attach base abilities to FightDefensivelyFeature to add them to all characters.
      FeatureConfigurator.For(FeatureRefs.FightDefensivelyFeature)
        .EditComponent<AddFacts>(
          c => c.m_Facts = CommonTool.Append(c.m_Facts, deliverTouch.ToReference<BlueprintUnitFactReference>()))
        .Configure();
    }

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

        return true;
      }
    }
  }
}
