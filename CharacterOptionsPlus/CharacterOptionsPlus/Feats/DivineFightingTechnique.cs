using BlueprintCore.Actions.Builder;
using BlueprintCore.Actions.Builder.ContextEx;
using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Assets;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.MechanicsChanges;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.ContextData;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using UnityEngine;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  internal class DivineFightingTechnique
  {
    internal const string FeatName = "DivineFightingTechnique";
    internal const string FeatDisplayName = "DivineFightingTechnique.Name";
    private const string FeatDescription = "DivineFightingTechnique.Description";

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "consecrate.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.DivineFightingTechniqueFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("DivineFightingTechnique.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      FeatureSelectionConfigurator.New(FeatName, Guids.DivineFightingTechniqueFeat).Configure();

      FeatureConfigurator.New(AsmodeusName, Guids.AsmodeusTechnique).Configure();
      BuffConfigurator.New(AsmodeusBlindBuff, Guids.AsmodeusBlindBuff).Configure();
      ActivatableAbilityConfigurator.New(AsmodeusBlind, Guids.AsmodeusBlind).Configure();
      BuffConfigurator.New(AsmodeusEntangleBuff, Guids.AsmodeusEntangleBuff).Configure();
      ActivatableAbilityConfigurator.New(AsmodeusEntangle, Guids.AsmodeusEntangle).Configure();
      BuffConfigurator.New(AsmodeusSickenBuff, Guids.AsmodeusSickenBuff).Configure();
      ActivatableAbilityConfigurator.New(AsmodeusSicken, Guids.AsmodeusSicken).Configure();

      BuffConfigurator.New(ErastilDistracted, Guids.ErastilDistracted).Configure();
      BuffConfigurator.New(ErastilFocused, Guids.ErastilFocused).Configure();
      BuffConfigurator.New(ErastilFocusedAdvanced, Guids.ErastilFocusedAdvanced).Configure();
      FeatureConfigurator.New(ErastilAdvanced, Guids.ErastilAdvancedTechnique).Configure();
      AbilityConfigurator.New(ErastilAbility, Guids.ErastilAbility).Configure();
      FeatureConfigurator.New(ErastilName, Guids.ErastilTechnique).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var selection = FeatureSelectionConfigurator.New(FeatName, Guids.DivineFightingTechniqueFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(IconName)
        .AddToAllFeatures(
          ConfigureAsmodeus(),
          ConfigureErastil())
        .Configure();

      // Add to the appropriate selections
      FeatureConfigurator.For(selection)
        .SetGroups(FeatureGroup.Feat, FeatureGroup.CombatFeat)
        .Configure(delayed: true);
    }

    [TypeId("270e0386-855e-42d4-8284-1977e61267ba")]
    private class AdvancedTechniqueGrant :
      UnitFactComponentDelegate<AdvancedTechniqueGrant.ComponentData>, IOwnerGainLevelHandler
    {
      private readonly BlueprintFeatureReference AdvancedTechnique;

      private readonly ConditionsChecker Conditions;

      public AdvancedTechniqueGrant(string techniqueId, ConditionsBuilder conditions)
      {
        AdvancedTechnique = BlueprintTool.GetRef<BlueprintFeatureReference>(techniqueId);
        Conditions = conditions.Build();
      }

      public void HandleUnitGainLevel()
      {
        Apply();
      }

      public override void OnActivate()
      {
        Apply();
      }

      public override void OnDeactivate()
      {
        Remove();
      }

      private void Apply()
      {
        try
        {
          if (Data.AppliedFact is not null)
            return;

          if (!Conditions.Check())
            return;

          var technique = AdvancedTechnique.Get();
          Logger.Log($"Applying Advanced Technique: {technique.name}");
          Data.AppliedFact = Owner.AddFact(technique);
        }
        catch (Exception e)
        {
          Logger.LogException("AdvancedTechniqueGrant.Apply", e);
        }
      }

      private void Remove()
      {
        try
        {
          if (Data.AppliedFact is not null)
          {
            Logger.Log($"Removing {Data.AppliedFact.Name}");
            Owner.RemoveFact(Data.AppliedFact);
            Data.AppliedFact = null;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("AdvancedTechniqueGrant.Remove", e);
        }
      }

      public class ComponentData
      {
        [JsonProperty]
        public EntityFact AppliedFact;
      }
    }

    #region Asmodeus
    private const string AsmodeusName = "DFT.Asmodeus";
    private const string AsmodeusDisplayName = "DFT.Asmodeus.Name";
    private const string AsmodeusDescription = "DFT.Asmodeus.Description";

    private const string AsmodeusAdvanced = "DFT.Asmodeus.Advanced";
    private const string AsmodeusAdvancedDescription = "DFT.Asmodeus.Advanced.Description";
    private const string AsmodeusAdvancedToggle = "DFT.Asmodeus.Advanced.Toggle";
    private const string AsmodeusBlind = "DFT.Asmodeus.Blind";
    private const string AsmodeusBlindBuff = "DFT.Asmodeus.Blind.Buff";
    private const string AsmodeusEntangle = "DFT.Asmodeus.Entangle";
    private const string AsmodeusEntangleBuff = "DFT.Asmodeus.Entangle.Buff";
    private const string AsmodeusSicken = "DFT.Asmodeus.Sicken";
    private const string AsmodeusSickenBuff = "DFT.Asmodeus.Sicken.Buff";

    private const string AsmodeusIcon = IconPrefix + "asmodeustechnique.png";
    private const string AsmodeusAdvancedIcon = IconPrefix + "asmodeusadvancedtechnique.png";
    private const string BlindIcon = IconPrefix + "asmodeusblind.png";
    private const string EntangleIcon = IconPrefix + "asmodeusentangle.png";
    private const string SickenIcon = IconPrefix + "asmodeussicken.png";

    private static BlueprintFeature ConfigureAsmodeus()
    {
      var blind =
        CreateAdvancedToggle(
          BlindIcon,
          AsmodeusBlind,
          Guids.AsmodeusBlind,
          AsmodeusBlindBuff,
          Guids.AsmodeusBlindBuff,
          CombatManeuver.DirtyTrickBlind);
      var entangle =
        CreateAdvancedToggle(
          EntangleIcon,
          AsmodeusEntangle,
          Guids.AsmodeusEntangle,
          AsmodeusEntangleBuff,
          Guids.AsmodeusEntangleBuff,
          CombatManeuver.DirtyTrickEntangle);
      var sicken =
        CreateAdvancedToggle(
          SickenIcon,
          AsmodeusSicken,
          Guids.AsmodeusSicken,
          AsmodeusSickenBuff,
          Guids.AsmodeusSickenBuff,
          CombatManeuver.DirtyTrickSickened);

      var toggle = ActivatableAbilityConfigurator.New(AsmodeusAdvancedToggle, Guids.AsmodeusAdvancedToggle)
        .SetDisplayName(AsmodeusDisplayName)
        .SetDescription(AsmodeusAdvancedDescription)
        .SetIcon(AsmodeusAdvancedIcon)
        .SetDeactivateImmediately()
        .SetActivationType(AbilityActivationType.Immediately)
        .SetActivateWithUnitCommand(CommandType.Free)
        .AddActivatableAbilityVariants(variants: new() { blind, entangle, sicken })
        .AddActivationDisable()
        .Configure();

      FeatureConfigurator.New(AsmodeusAdvanced, Guids.AsmodeusAdvancedTechnique)
        .SetDisplayName(AsmodeusDisplayName)
        .SetDescription(AsmodeusDescription)
        .SetIcon(AsmodeusAdvancedIcon)
        .SetIsClassFeature()
        .AddFacts(new() { toggle, blind, entangle, sicken })
        .Configure();

      return FeatureConfigurator.New(AsmodeusName, Guids.AsmodeusTechnique)
        .SetDisplayName(AsmodeusDisplayName)
        .SetDescription(AsmodeusDescription)
        .SetIcon(AsmodeusIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Critical)
        .AddComponent<AsmodeusCritical>()
        .AddComponent<AsmodeusAdvancedTechnique>()
        .AddPrerequisiteAlignment(AlignmentMaskType.LawfulEvil)
        .Configure();
    }

    private static BlueprintActivatableAbility CreateAdvancedToggle(
      Asset<Sprite> icon,
      string abilityName,
      string abilityGuid,
      string buffName,
      string buffGuid,
      CombatManeuver type)
    {
      var buff = BuffConfigurator.New(buffName, buffGuid)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddComponent(new AsmodeusTrick(ActionsBuilder.New().CombatManeuver(onSuccess: ActionsBuilder.New(), type)))
        .Configure();

      return ActivatableAbilityConfigurator.New(abilityName, abilityGuid)
        .SetDisplayName(abilityName)
        .SetDescription(AsmodeusAdvancedDescription)
        .SetIcon(icon)
        .SetBuff(buff)
        .SetDeactivateImmediately()
        .SetActivationType(AbilityActivationType.Immediately)
        .SetActivateWithUnitCommand(CommandType.Free)
        .SetGroup(ExpandedActivatableAbilityGroup.AsmodeusTechnique)
        .SetHiddenInUI()
        .Configure();
    }

    [TypeId("54c920fb-711e-4b5f-8ce2-f86fa8ecb5b9")]
    private class AsmodeusAdvancedTechnique :
      UnitFactComponentDelegate<AsmodeusAdvancedTechnique.ComponentData>, IOwnerGainLevelHandler
    {
      private static BlueprintFeature _advancedTechnique;
      private static BlueprintFeature AdvancedTechnique
      {
        get
        {
          _advancedTechnique ??= BlueprintTool.Get<BlueprintFeature>(Guids.AsmodeusAdvancedTechnique);
          return _advancedTechnique;
        }
      }

      public void HandleUnitGainLevel()
      {
        Apply();
      }

      public override void OnActivate()
      {
        Apply();
      }

      public override void OnDeactivate()
      {
        Remove();
      }

      private void Apply()
      {
        try
        {
          if (Data.AppliedFact is not null)
            return;

          if (Owner.Stats.GetStat(StatType.BaseAttackBonus) < 10
              || Owner.Stats.GetStat(StatType.Intelligence) < 13
              || !Owner.HasFact(FeatureRefs.CombatExpertiseFeature.Reference)
              || !Owner.HasFact(FeatureRefs.ImprovedDirtyTrick.Reference))
            return;

          Logger.Log($"Applying Asmodeus Advanced Technique");
          Data.AppliedFact = Owner.AddFact(AdvancedTechnique);
        }
        catch (Exception e)
        {
          Logger.LogException("AsmodeusAdvancedTechnique.Apply", e);
        }
      }

      private void Remove()
      {
        try
        {
          if (Data.AppliedFact is not null)
          {
            Logger.Log($"Removing {Data.AppliedFact.Name}");
            Owner.RemoveFact(Data.AppliedFact);
            Data.AppliedFact = null;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("AsmodeusAdvancedTechnique.Remove", e);
        }
      }

      public class ComponentData
      {
        [JsonProperty]
        public EntityFact AppliedFact;
      }
    }

    [TypeId("db0de5c9-48dc-4b6e-818c-fb676e14c1b6")]
    private class AsmodeusCritical : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleAttackWithWeapon>
    {
      private static BlueprintBuff _sickened;
      private static BlueprintBuff Sickened
      {
        get
        {
          _sickened ??= BuffRefs.Sickened.Reference.Get();
          return _sickened;
        }
      }

      public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

      public void OnEventDidTrigger(RuleAttackWithWeapon evt)
      {
        try
        {
          if (evt.Target is null)
          {
            Logger.Warning("No target for attack!");
            return;
          }

          if (!evt.AttackRoll.IsCriticalRoll)
            return;

          if (evt.Weapon.Blueprint.Category != WeaponCategory.HeavyMace
            && evt.Weapon.Blueprint.Category != WeaponCategory.LightMace)
          {
            Logger.NativeLog($"Not using a mace: {evt.Weapon.Blueprint.Category}");
            return;
          }

          var rounds = evt.AttackRoll.IsCriticalConfirmed ? 2 : 1;
          Logger.NativeLog($"{evt.Target.CharacterName} is sickened for {rounds} rounds");
          evt.Target.AddBuff(Sickened, Context, rounds.Rounds().Seconds);
        }
        catch (Exception e)
        {
          Logger.LogException("AsmodeusCritical.OnEventDidTrigger", e);
        }
      }
    }

    [TypeId("70347e3f-30b2-43a1-9e73-2b8e332f2daa")]
    private class AsmodeusTrick : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleAttackWithWeapon>
    {
      private readonly ActionList Actions;

      public AsmodeusTrick(ActionsBuilder actions)
      {
        Actions = actions.Build();
      }

      public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

      public void OnEventDidTrigger(RuleAttackWithWeapon evt)
      {
        try
        {
          if (evt.Target is null)
          {
            Logger.Warning("No target for attack!");
            return;
          }

          if (!evt.AttackRoll.IsHit)
            return;

          if (!evt.AttackRoll.IsTargetFlatFooted)
            return;

          if (evt.Weapon.Blueprint.Category != WeaponCategory.HeavyMace
            && evt.Weapon.Blueprint.Category != WeaponCategory.LightMace)
          {
            Logger.NativeLog($"Not using a mace: {evt.Weapon.Blueprint.Category}");
            return;
          }

          if (!Owner.HasSwiftAction())
          {
            Logger.NativeLog($"No swift action available");
            return;
          }

          using (ContextData<ContextAttackData>.Request().Setup(evt.AttackRoll))
          {
            Owner.SpendAction(CommandType.Swift, isFullRound: false, timeSinceCommandStart: 0);
            Fact.RunActionInContext(Actions, evt.Target);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("AsmodeusCritical.OnEventDidTrigger", e);
        }
      }
    }
    #endregion

    #region Erastil
    private const string ErastilName = "DFT.Erastil";
    private const string ErastilDisplayName = "DFT.Erastil.Name";
    private const string ErastilDescription = "DFT.Erastil.Description";

    private const string ErastilAdvanced = "DFT.Erastil.Advanced";
    private const string ErastilAdvancedDescription = "DFT.Erastil.Advanced.Description";
    private const string ErastilAbility = "DFT.Erastil.Ability";
    private const string ErastilDistracted = "DFT.Erastil.Distracted";
    private const string ErastilFocused = "DFT.Erastil.Focused";
    private const string ErastilFocusedAdvanced = "DFT.Erastil.Focused.Advanced";

    private const string ErastilIcon = IconPrefix + "gloriousheat.png";
    private const string ErastilAdvancedIcon = IconPrefix + "gloriousheat.png";

    private static BlueprintFeature ConfigureErastil()
    {
      var distracted = BuffConfigurator.New(ErastilDistracted, Guids.ErastilDistracted)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .Configure();

      var buff = BuffConfigurator.New(ErastilFocused, Guids.ErastilFocused)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilIcon)
        .AddNotDispelable()
        .AddComponent<FocusedAC>()
        .Configure();

      var advancedBuff = BuffConfigurator.New(ErastilFocusedAdvanced, Guids.ErastilFocusedAdvanced)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilAdvancedIcon)
        .AddNotDispelable()
        .AddComponent(new FocusedAC(4))
        .Configure();

      var advancedTechnique = FeatureConfigurator.New(ErastilAdvanced, Guids.ErastilAdvancedTechnique)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilAdvancedDescription)
        .SetIcon(ErastilAdvancedIcon)
        .SetIsClassFeature()
        .Configure();

      var duration = ContextDuration.Fixed(1);
      var ability = AbilityConfigurator.New(ErastilAbility, Guids.ErastilAbility)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilIcon)
        .SetRange(AbilityRange.Weapon)
        .AllowTargeting(enemies: true)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .AddAbilityCasterHasWeaponWithRangeType(WeaponRangeType.Ranged)
        .AddAbilityCasterMainWeaponCheck(WeaponCategory.Longbow, WeaponCategory.Shortbow)
        .AddAbilityDeliverAttackWithWeapon()
        .AddAbilityEffectRunAction(
          ActionsBuilder.New()
            .ApplyBuff(distracted, duration, isNotDispelable: true)
            .Conditional(
              ConditionsBuilder.New().CasterHasFact(advancedTechnique),
              ifTrue:
                ActionsBuilder.New()
                  .OnRandomTargetsAround(
                    ActionsBuilder.New().ApplyBuff(advancedBuff, duration, isNotDispelable: true),
                    numberOfTargets: 1,
                    onEnemies: false,
                    radius: 5.Feet())
                  .OnRandomTargetsAround(
                    ActionsBuilder.New()
                      .Conditional(
                        ConditionsBuilder.New().HasFact(advancedBuff),
                        ifFalse: ActionsBuilder.New().ApplyBuff(buff, duration, isNotDispelable: true)),
                    numberOfTargets: 9999,
                    onEnemies: false,
                    radius: 30.Feet()),
              ifFalse:
                ActionsBuilder.New()
                  .OnRandomTargetsAround(
                    ActionsBuilder.New().ApplyBuff(buff, duration, isNotDispelable: true),
                    numberOfTargets: 1,
                    onEnemies: false,
                    radius: 5.Feet())))
        .Configure();

      return FeatureConfigurator.New(ErastilName, Guids.ErastilTechnique)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Ranged)
        .AddPrerequisiteAlignment(AlignmentMaskType.LawfulGood)
        .AddComponent(
          new AdvancedTechniqueGrant(
            Guids.ErastilAdvancedTechnique,
            ConditionsBuilder.New()
              .StatValue(n: 10, stat: StatType.BaseAttackBonus)
              .StatValue(n: 17, stat: StatType.Dexterity)
              .HasFact(FeatureRefs.PointBlankShot.ToString())
              .HasFact(FeatureRefs.PreciseShot.ToString())))
        .AddFacts(new() { ability })
        .Configure();
    }

    [TypeId("9c2dbfee-2aa0-4a84-8e65-eea8fbbc536b")]
    private class FocusedAC : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAC>
    {
      private static BlueprintBuff _distracted;
      private static BlueprintBuff Distracted
      {
        get
        {
          _distracted ??= BlueprintTool.Get<BlueprintBuff>(Guids.ErastilDistracted);
          return _distracted;
        }
      }

      private readonly int Bonus;

      public FocusedAC()
      {
        Bonus = 2;
      }

      public FocusedAC(int bonus)
      {
        Bonus = bonus;
      }

      public void OnEventAboutToTrigger(RuleCalculateAC evt)
      {
        try
        {
          var distracted = evt.Initiator.GetFact(Distracted);
          if (distracted == null
              || distracted.MaybeContext?.MaybeCaster != Fact.MaybeContext?.MaybeCaster
              || distracted.MaybeContext?.MaybeCaster is null)
            return;

          Logger.NativeLog($"Granting {Owner.CharacterName} +{Bonus} AC against {evt.Initiator.CharacterName}");
          evt.AddModifier(Bonus, Fact, ModifierDescriptor.UntypedStackable);
        }
        catch (Exception e)
        {
          Logger.LogException("FocusedAC.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAC evt) { }
    }
    #endregion
  }
}
