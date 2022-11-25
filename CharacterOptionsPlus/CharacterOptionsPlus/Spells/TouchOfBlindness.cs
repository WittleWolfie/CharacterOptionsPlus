using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.ModReferences;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.UnitParts;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static TabletopTweaks.Core.MechanicsChanges.MetamagicExtention;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Spells
{
  internal class TouchOfBlindness
  {
    private const string FeatureName = "TouchOfBlindness";

    internal const string DisplayName = "TouchOfBlindness.Name";
    private const string Description = "TouchOfBlindness.Description";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatureName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.TouchOfBlindnessSpell))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("TouchOfBlindness.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatureName} (disabled)");

      AbilityConfigurator.New(FeatureName, Guids.TouchOfBlindnessSpell).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatureName}");

      var icon = AbilityRefs.Blindness.Reference.Get().Icon;
      AbilityConfigurator.NewSpell(
          FeatureName, Guids.TouchOfBlindnessSpell, SpellSchool.Necromancy, canSpecialize: true)
        .SetDisplayName(DisplayName)
        .SetDescription(Description)
        .SetIcon(icon)
        .SetRange(AbilityRange.Touch)
        .AllowTargeting(enemies: true)
        .SetSpellResistance()
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Touch)
        .SetActionType(CommandType.Standard)
        .SetAvailableMetamagic(
          Metamagic.CompletelyNormal,
          Metamagic.Heighten,
          Metamagic.Persistent,
          Metamagic.Quicken,
          Metamagic.Reach,
          (Metamagic)CustomMetamagic.Piercing)
        .AddToSpellLists(
          level: 1, SpellList.Bard, SpellList.Cleric, SpellList.Wizard, SpellList.Shaman, SpellList.Witch)
        .AddToSpellList(1, ModSpellListRefs.AntipaladinSpelllist.ToString())
        .AddComponent(new TouchCharges(ContextValues.Rank()))
        .AddAbilityDeliverTouch(touchWeapon: ItemWeaponRefs.TouchItem.ToString())
        .AddAbilityEffectRunAction(
          actions: ActionsBuilder.New()
            .SavingThrow(
              SavingThrowType.Fortitude,
              onResult: ActionsBuilder.New()
                .ConditionalSaved(
                  succeed: ActionsBuilder.New()
                    .ApplyBuff(BuffRefs.BlindnessBuff.ToString(), ContextDuration.Fixed(1)))))
        .AddContextRankConfig(ContextRankConfigs.CasterLevel())
        .Configure(delayed: true);
    }
  }
}
