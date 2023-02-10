using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Craft;
using Kingmaker.Enums;
using Kingmaker.UI.UnitSettings.Blueprints;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace CharacterOptionsPlus.Spells
{
  internal class CheetahSprint
  {
    private const string FeatureName = "CheetahSprint";

    internal const string DisplayName = "CheetahSprint.Name";
    private const string Description = "CheetahSprint.Description";

    private const string BuffName = "CheetahSprint.Buff";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "cheetahsprint.png";

    private static readonly Logging.Logger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.CheetahSprintSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("CheetahSprint.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.CheetahSprintSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var buff = BuffConfigurator.New(BuffName, Guids.CheetahSprintBuff)
        .CopyFrom(BuffRefs.ExpeditiousRetreatBuff)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .AddBuffMovementSpeed(descriptor: ModifierDescriptor.Enhancement, value: 90)
        .Configure();

      AbilityConfigurator.NewSpell(
          FeatureName, Guids.CheetahSprintSpell, SpellSchool.Transmutation, canSpecialize: false)
        .CopyFrom(AbilityRefs.ExpeditiousRetreat, typeof(AbilitySpawnFx))
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(IconName)
        .SetLocalizedDuration(Duration.OneRound)
        .SetActionType(CommandType.Swift)
        .SetAvailableMetamagic()
        .AddAbilityEffectRunAction(ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(1)))
        .AddToSpellLists(
          level: 1, SpellList.Bloodrager, SpellList.Druid, SpellList.Ranger, SpellList.Shaman, SpellList.Witch)
        .AddCraftInfoComponent(
          aOEType: CraftAOE.None,
          savingThrow: CraftSavingThrow.None,
          spellType: CraftSpellType.Buff)
        .Configure();
    }
  }
}
