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
using CharacterOptionsPlus.Actions;
using CharacterOptionsPlus.Components;
using CharacterOptionsPlus.Conditions;
using CharacterOptionsPlus.MechanicsChanges;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers;
using Kingmaker.Designers;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.ContextData;
using Kingmaker.Utility;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;
using static Kingmaker.RuleSystem.RulebookEvent;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  internal class DivineFightingTechnique
  {
    internal const string FeatName = "DivineFightingTechnique";
    internal const string FeatDisplayName = "DivineFightingTechnique.Name";
    private const string FeatDescription = "DivineFightingTechnique.Description";

    private const string IconPrefix = "assets/icons/";

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

      BuffConfigurator.New(GorumBuff, Guids.GorumTechniqueBuff).Configure();
      BuffConfigurator.New(GorumAdvancedBuff, Guids.GorumAdvancedTechniqueBuff).Configure();
      FeatureConfigurator.New(GorumAdvanced, Guids.GorumAdvancedTechnique).Configure();
      FeatureConfigurator.New(GorumName, Guids.GorumTechnique).Configure();

      BuffConfigurator.New(IomedaeBuff, Guids.IomedaeTechniqueBuff).Configure();
      AbilityConfigurator.New(IomedaeInspire, Guids.IomedaeInspireAbility).Configure();
      AbilityConfigurator.New(IomedaeQuickInspire, Guids.IomedaeQuickInspireAbility).Configure();
      FeatureConfigurator.New(IomedaeAdvanced, Guids.IomedaeAdvancedTechnique).Configure();
      FeatureConfigurator.New(IomedaeName, Guids.IomedaeTechnique).Configure();

      BuffConfigurator.New(IroriBuff, Guids.IroriTechniqueBuff).Configure();
      AbilityConfigurator.New(IroriToggle, Guids.IroriTechniqueToggle).Configure();
      FeatureConfigurator.New(IroriAdvanced, Guids.IroriAdvancedTechnique).Configure();
      FeatureConfigurator.New(IroriName, Guids.IroriTechnique).Configure();

      BuffConfigurator.New(LamashtuBuff, Guids.LamashtuTechniqueBuff).Configure();
      BuffConfigurator.New(LamashtuImmunityBuff, Guids.LamashtuTechniqueImmunityBuff).Configure();
      AbilityConfigurator.New(LamashtuAbility, Guids.LamashtuTechniqueAbility).Configure();
      FeatureConfigurator.New(LamashtuAdvanced, Guids.LamashtuAdvancedTechnique).Configure();
      FeatureConfigurator.New(LamashtuName, Guids.LamashtuTechnique).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      var selection = FeatureSelectionConfigurator.New(FeatName, Guids.DivineFightingTechniqueFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(FeatureRefs.SimpleWeaponProficiency.Reference.Get().Icon)
        .AddToAllFeatures(
          ConfigureAsmodeus(),
          ConfigureErastil(),
          ConfigureGorum(),
          ConfigureIomedae(),
          ConfigureIrori(),
          ConfigureLamashtu())
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

          using (Context.GetDataScope(Owner))
          {
            if (!Conditions.Check())
            {
              return;
            }
          }

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
        .AddComponent(new RecommendationWeaponFocus(WeaponCategory.HeavyMace, WeaponCategory.LightMace))
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

    private const string ErastilIcon = IconPrefix + "erastiltechnique.png";
    private const string ErastilAdvancedIcon = IconPrefix + "erastiladvancedtechnique.png";
    private const string ErastilAbilityIcon = IconPrefix + "distractingshot.png";

    private static BlueprintFeature ConfigureErastil()
    {
      var distracted = BuffConfigurator.New(ErastilDistracted, Guids.ErastilDistracted)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .Configure();

      var buff = BuffConfigurator.New(ErastilFocused, Guids.ErastilFocused)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilAbilityIcon)
        .AddNotDispelable()
        .AddComponent<FocusedAC>()
        .Configure();

      var advancedBuff = BuffConfigurator.New(ErastilFocusedAdvanced, Guids.ErastilFocusedAdvanced)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilAbilityIcon)
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
        .SetIcon(ErastilAbilityIcon)
        .SetRange(AbilityRange.Weapon)
        .SetType(AbilityType.Physical)
        .SetNeedEquipWeapons()
        .SetActionType(CommandType.Standard)
        .AllowTargeting(enemies: true)
        .SetEffectOnEnemy(AbilityEffectOnUnit.Harmful)
        .SetAnimation(CastAnimationStyle.Special)
        .AddAbilityCasterHasWeaponWithRangeType(WeaponRangeType.Ranged)
        .AddAbilityCasterMainWeaponCheck(WeaponCategory.Longbow, WeaponCategory.Shortbow)
        .AddAbilityEffectRunActionOnClickedTarget(
          ActionsBuilder.New().Add<RangedAttackExtended>(a => a.OnHit = ActionsBuilder.New().Add<Distract>().Build()))
        .Configure();

      return FeatureConfigurator.New(ErastilName, Guids.ErastilTechnique)
        .SetDisplayName(ErastilDisplayName)
        .SetDescription(ErastilDescription)
        .SetIcon(ErastilIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddComponent(new RecommendationWeaponFocus(WeaponCategory.Longbow, WeaponCategory.Shortbow))
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

    private static BlueprintBuff _distracted;
    private static BlueprintBuff Distracted
    {
      get
      {
        _distracted ??= BlueprintTool.Get<BlueprintBuff>(Guids.ErastilDistracted);
        return _distracted;
      }
    }

    private static BlueprintBuff _focused;
    private static BlueprintBuff Focused
    {
      get
      {
        _focused ??= BlueprintTool.Get<BlueprintBuff>(Guids.ErastilFocused);
        return _focused;
      }
    }

    private static BlueprintBuff _focusedAdvanced;
    private static BlueprintBuff FocusedAdvanced
    {
      get
      {
        _focusedAdvanced ??= BlueprintTool.Get<BlueprintBuff>(Guids.ErastilFocusedAdvanced);
        return _focusedAdvanced;
      }
    }

    private static BlueprintFeature _erastilAdvancedTechnique;
    private static BlueprintFeature ErastilAdvancedTechnique
    {
      get
      {
        _erastilAdvancedTechnique ??= BlueprintTool.Get<BlueprintFeature>(Guids.ErastilAdvancedTechnique);
        return _erastilAdvancedTechnique;
      }
    }

    [TypeId("6405060b-3cad-4a3e-99ff-00180fd5180e")]
    private class Distract : ContextAction
    {
      public override string GetCaption()
      {
        return "Custom action for Erastil's Distracting Shot";
      }

      public override void RunAction()
      {
        try
        {
          var target = Context.MainTarget?.Unit;
          if (target is null)
          {
            Logger.Warning("No target");
            return;
          }

          target.AddBuff(Distracted, Context, 1.Rounds().Seconds);

          var adjacentAllies =
            GameHelper.GetTargetsAround(target.Position, 5.Feet()).Where(unit => unit.IsAlly(Context.MaybeCaster));
          var nearbyAllies =
            GameHelper.GetTargetsAround(target.Position, 30.Feet()).Where(unit => unit.IsAlly(Context.MaybeCaster));
          var hasAdvancedTechnique = Context.MaybeCaster.HasFact(ErastilAdvancedTechnique);

          if (!adjacentAllies.Any() && (!hasAdvancedTechnique || !nearbyAllies.Any()))
          {
            Logger.NativeLog("No allies nearby");
            return;
          }

          var buffTarget = adjacentAllies.FirstOrDefault();
          if (buffTarget is not null)
            buffTarget.AddBuff(Focused, Context, 1.Rounds().Seconds);

          nearbyAllies = nearbyAllies.Except(unit => unit.Equals(buffTarget));
          if (hasAdvancedTechnique && nearbyAllies.Any())
          {
            foreach (var ally in nearbyAllies)
              ally.AddBuff(FocusedAdvanced, Context, 1.Rounds().Seconds);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("Distract.RunAction", e);
        }
      }
    }

    [TypeId("9c2dbfee-2aa0-4a84-8e65-eea8fbbc536b")]
    private class FocusedAC : UnitFactComponentDelegate, ITargetRulebookHandler<RuleCalculateAC>
    {
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

          Logger.NativeLog($"Granting {evt.Target.CharacterName} +{Bonus} AC against {evt.Initiator.CharacterName}");
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

    #region Gorum
    private const string GorumName = "DFT.Gorum";
    private const string GorumDisplayName = "DFT.Gorum.Name";
    private const string GorumDescription = "DFT.Gorum.Description";

    private const string GorumAdvanced = "DFT.Gorum.Advanced";
    private const string GorumAdvancedDescription = "DFT.Gorum.Advanced.Description";
    private const string GorumBuff = "DFT.Gorum.Distracted";
    private const string GorumAdvancedBuff = "DFT.Gorum.Focused.Advanced";

    private const string GorumIcon = IconPrefix + "gorumtechnique.png";
    private const string GorumAdvancedIcon = IconPrefix + "gorumadvancedtechnique.png";

    private static BlueprintBuff _vitalStrikeBuff;
    private static BlueprintBuff VitalStrikeBuff
    {
      get
      {
        _vitalStrikeBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.GorumTechniqueBuff);
        return _vitalStrikeBuff;
      }
    }

    private static BlueprintFeature ConfigureGorum()
    {
      var buff = BuffConfigurator.New(GorumBuff, Guids.GorumTechniqueBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .Configure();

      var advancedBuff = BuffConfigurator.New(GorumAdvancedBuff, Guids.GorumAdvancedTechniqueBuff)
        .SetDisplayName(GorumDisplayName)
        .SetDescription(GorumDescription)
        .SetIcon(GorumAdvancedIcon)
        .AddNotDispelable()
        .AddCondition(UnitCondition.SpellCastingIsVeryDifficult)
        .Configure();

      var applyAdvancedBuff = ActionsBuilder.New().ApplyBuff(advancedBuff, ContextDuration.Fixed(1));
      var advancedTechnique = FeatureConfigurator.New(GorumAdvanced, Guids.GorumAdvancedTechnique)
        .SetDisplayName(GorumDisplayName)
        .SetDescription(GorumAdvancedDescription)
        .SetIcon(GorumAdvancedIcon)
        .SetIsClassFeature()
        .AddAbilityUseTrigger(
          ability: AbilityRefs.VitalStrikeAbility.ToString(), action: applyAdvancedBuff, actionsOnTarget: true)
        .AddAbilityUseTrigger(
          ability: AbilityRefs.VitalStrikeAbilityImproved.ToString(), action: applyAdvancedBuff, actionsOnTarget: true)
        .AddAbilityUseTrigger(
          ability: AbilityRefs.VitalStrikeAbilityGreater.ToString(), action: applyAdvancedBuff, actionsOnTarget: true)
        .Configure();

      var applyBuff = ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Fixed(1));
      return FeatureConfigurator.New(GorumName, Guids.GorumTechnique)
        .SetDisplayName(GorumDisplayName)
        .SetDescription(GorumDescription)
        .SetIcon(GorumIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddComponent(new RecommendationWeaponFocus(WeaponCategory.Greatsword))
        .AddRecommendationHasFeature(FeatureRefs.VitalStrikeFeature.ToString(), mandatory: true)
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Melee)
        .AddPrerequisiteAlignment(AlignmentMaskType.ChaoticNeutral)
        .AddComponent(
          new AdvancedTechniqueGrant(
            Guids.GorumAdvancedTechnique,
            ConditionsBuilder.New()
              .StatValue(n: 10, stat: StatType.BaseAttackBonus)
              .StatValue(n: 13, stat: StatType.Strength)
              .HasFact(FeatureRefs.CleaveFeature.ToString())
              .HasFact(FeatureRefs.VitalStrikeFeature.ToString())
              .HasFact(FeatureRefs.PowerAttackFeature.ToString())))
        .AddComponent<GorumsStrike>()
        .AddAbilityUseTrigger(ability: AbilityRefs.VitalStrikeAbility.ToString(), action: applyBuff)
        .AddAbilityUseTrigger(ability: AbilityRefs.VitalStrikeAbilityImproved.ToString(), action: applyBuff)
        .AddAbilityUseTrigger(ability: AbilityRefs.VitalStrikeAbilityGreater.ToString(), action: applyBuff)
        .Configure();
    }

    [TypeId("9a1c3f73-4805-4a67-93d1-6ae52cd69862")]
    private class GorumsStrike : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleAttackWithWeapon>
    {
      private static BlueprintFeature _vitalStrike;
      private static BlueprintFeature VitalStrike
      {
        get
        {
          _vitalStrike ??= FeatureRefs.VitalStrikeFeature.Reference.Get();
          return _vitalStrike;
        }
      }

      private static BlueprintFeature _vitalStrikeImproved;
      private static BlueprintFeature VitalStrikeImproved
      {
        get
        {
          _vitalStrikeImproved ??= FeatureRefs.VitalStrikeFeatureImproved.Reference.Get();
          return _vitalStrikeImproved;
        }
      }

      private static BlueprintFeature _vitalStrikeGreater;
      private static BlueprintFeature VitalStrikeGreater
      {
        get
        {
          _vitalStrikeGreater ??= FeatureRefs.VitalStrikeFeatureGreater.Reference.Get();
          return _vitalStrikeGreater;
        }
      }

      private static BlueprintFeature _vitalStrikeMythic;
      private static BlueprintFeature VitalStrikeMythic
      {
        get
        {
          _vitalStrikeMythic ??= FeatureRefs.VitalStrikeMythicFeat.Reference.Get();
          return _vitalStrikeMythic;
        }
      }

      private static BlueprintFeature _rowdy;
      private static BlueprintFeature Rowdy
      {
        get
        {
          _rowdy ??= FeatureRefs.RowdyVitalDamage.Reference.Get();
          return _rowdy;
        }
      }

      private static readonly CustomDataKey HandlerKey = new("GorumsStrike.Handler");

      public void OnEventAboutToTrigger(RuleAttackWithWeapon evt)
      {
        try
        {
          if (evt.Weapon.Blueprint.Category != WeaponCategory.Greatsword)
            return;

          if (evt.IsCharge && evt.IsFirstAttack)
            HandleCharge(evt);
          else if (evt.IsAttackOfOpportunity)
            HandleAoO(evt);
        }
        catch (Exception e)
        {
          Logger.LogException("GorumsStrike.OnEventAboutToTrigger", e);
        }
      }

      public void OnEventDidTrigger(RuleAttackWithWeapon evt)
      {
        try
        {
          if (evt.TryGetCustomData(HandlerKey, out ISubscriber handler))
          {
            Logger.NativeLog("Removing Vital Strike handler");
            EventBus.Unsubscribe(handler);
          }
        }
        catch (Exception e)
        {
          Logger.LogException("GorumsStrike.OnEventDidTrigger", e);
        }
      }

      private void HandleCharge(RuleAttackWithWeapon evt)
      {
        if (Owner.HasFact(VitalStrike))
          RegisterVitalStrike(evt);
        else
          evt.MeleeDamage?.DamageBundle?.First?.AddModifier(1, Fact);
      }

      private void HandleAoO(RuleAttackWithWeapon evt)
      {
        if (Owner.Buffs.GetBuff(VitalStrikeBuff) is not null)
        {
          RegisterVitalStrike(evt);
          Owner.RemoveFact(VitalStrikeBuff);
        }
      }

      private void RegisterVitalStrike(RuleAttackWithWeapon evt)
      {
        Logger.NativeLog("Adding Vital Strike handler");
        var vitalStrikeMod = Owner.HasFact(VitalStrikeGreater) ? 4 : Owner.HasFact(VitalStrikeImproved) ? 3 : 2;
        var handler =
          new AbilityCustomVitalStrike.VitalStrike(
            Owner, vitalStrikeMod, Owner.HasFact(VitalStrikeMythic), Owner.HasFact(Rowdy), Fact);
        EventBus.Subscribe(handler);
        evt.SetCustomData(HandlerKey, handler);
      }
    }
    #endregion

    #region Iomedae
    private const string IomedaeName = "DFT.Iomedae";
    private const string IomedaeDisplayName = "DFT.Iomedae.Name";
    private const string IomedaeDescription = "DFT.Iomedae.Description";

    private const string IomedaeAdvanced = "DFT.Iomedae.Advanced";
    private const string IomedaeAdvancedDescription = "DFT.Iomedae.Advanced.Description";
    private const string IomedaeBuff = "DFT.Iomedae.Distracted";
    private const string IomedaeInspire = "DFT.Iomedae.Inspire";
    private const string IomedaeQuickInspire = "DFT.Iomedae.Inspire.Quick";

    private const string IomedaeIcon = IconPrefix + "iomedaetechnique.png";
    private const string IomedaeAdvancedIcon = IconPrefix + "iomedaeadvancedtechnique.png";

    private static BlueprintFeature ConfigureIomedae()
    {
      var buff = BuffConfigurator.New(IomedaeBuff, Guids.IomedaeTechniqueBuff)
        .SetDisplayName(IomedaeDisplayName)
        .SetDescription(IomedaeDescription)
        .SetIcon(IomedaeIcon)
        .AddNotDispelable()
        .AddStatBonus(stat: StatType.AdditionalAttackBonus, descriptor: ModifierDescriptor.Sacred, value: 2)
        .AddStatBonus(stat: StatType.SaveFortitude, descriptor: ModifierDescriptor.Sacred, value: 2)
        .AddStatBonus(stat: StatType.SaveReflex, descriptor: ModifierDescriptor.Sacred, value: 2)
        .AddStatBonus(stat: StatType.SaveWill, descriptor: ModifierDescriptor.Sacred, value: 2)
        .AddBuffAllSkillsBonus(descriptor: ModifierDescriptor.Sacred, value: 2)
        .Configure();

      var inspire = AbilityConfigurator.New(IomedaeInspire, Guids.IomedaeInspireAbility)
        .SetDisplayName(IomedaeDisplayName)
        .SetDescription(IomedaeDescription)
        .SetIcon(IomedaeIcon)
        .SetType(AbilityType.Physical)
        .SetRange(AbilityRange.Weapon)
        .SetNeedEquipWeapons()
        .AddAbilityCasterMainWeaponCheck(WeaponCategory.Longsword)
        .SetAnimation(CastAnimationStyle.EnchantWeapon)
        .SetIsFullRoundAction()
        .AddAbilityTargetsAround(targetType: TargetType.Ally, radius: 30.Feet())
        .AddAbilityEffectRunAction(
          ActionsBuilder.New().ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank())))
        .AddContextRankConfig(ContextRankConfigs.BaseAttack().WithStartPlusDivStepProgression(5))
        .Configure();

      var quickInspire = AbilityConfigurator.New(IomedaeQuickInspire, Guids.IomedaeQuickInspireAbility)
        .SetDisplayName(IomedaeDisplayName)
        .SetDescription(IomedaeAdvancedDescription)
        .SetIcon(IomedaeAdvancedIcon)
        .SetType(AbilityType.Physical)
        .SetRange(AbilityRange.Weapon)
        .SetNeedEquipWeapons()
        .AddAbilityCasterMainWeaponCheck(WeaponCategory.Longsword)
        .AllowTargeting(enemies: true)
        .SetAnimation(CastAnimationStyle.Special)
        .SetActionType(CommandType.Standard)
        .AddAbilityEffectRunActionOnClickedTarget(
          action: ActionsBuilder.New()
            .Add<MeleeAttackExtended>(attack => attack.OnHit = ActionsBuilder.New().Add<Inspire>().Build()))
        .Configure();

      FeatureConfigurator.New(IomedaeAdvanced, Guids.IomedaeAdvancedTechnique)
        .SetDisplayName(IomedaeDisplayName)
        .SetDescription(IomedaeAdvancedDescription)
        .SetIcon(IomedaeAdvancedIcon)
        .SetIsClassFeature()
        .AddComponent<InspiringCharge>()
        .AddFacts(new() { quickInspire })
        .Configure();

      return FeatureConfigurator.New(IomedaeName, Guids.IomedaeTechnique)
        .SetDisplayName(IomedaeDisplayName)
        .SetDescription(IomedaeDescription)
        .SetIcon(IomedaeIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddComponent(new RecommendationWeaponFocus(WeaponCategory.Longsword))
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Melee)
        .AddPrerequisiteAlignment(AlignmentMaskType.LawfulGood)
        .AddComponent(
          new AdvancedTechniqueGrant(
            Guids.IomedaeAdvancedTechnique,
            ConditionsBuilder.New()
              .StatValue(n: 10, stat: StatType.BaseAttackBonus)
              .HasFact(FeatureRefs.DazzlingDisplayFeature.ToString())
              .Add<HasWeaponFocus>(c => c.Category = WeaponCategory.Longsword)))
        .AddFacts(new() { inspire })
        .Configure();
    }

    [TypeId("075ea01f-a0d5-4951-8fd2-2a3276a8951e")]
    private class InspiringCharge : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleAttackWithWeapon>
    {
      private static BlueprintBuff _blessing;
      private static BlueprintBuff Blessing
      {
        get
        {
          _blessing ??= BlueprintTool.Get<BlueprintBuff>(Guids.IomedaeTechniqueBuff);
          return _blessing;
        }
      }

      public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

      public void OnEventDidTrigger(RuleAttackWithWeapon evt)
      {
        try
        {
          if (evt.Weapon.Blueprint.Category != WeaponCategory.Longsword || !evt.IsCharge)
            return;

          if (!evt.AttackRoll.IsHit)
            return;

          var targets = GameHelper.GetTargetsAround(Owner.Position, 120.Feet()).Where(unit => unit.IsAlly(Owner));
          foreach (var target in targets)
          {
            Logger.NativeLog($"Applying {Blessing.name} to {target.CharacterName}");
            target.AddBuff(Blessing, Context, 1.Minutes());
          }
        }
        catch (Exception e)
        {
          Logger.LogException("InspiringCharge.OnEventAboutToTrigger", e);
        }
      }
    }

    [TypeId("3fc7e68a-f73d-4f3f-8d5b-be53a1ae9db9")]
    private class Inspire : ContextAction
    {
      private static BlueprintBuff _inspireBuff;
      private static BlueprintBuff InspireBuff
      {
        get
        {
          _inspireBuff ??= BlueprintTool.Get<BlueprintBuff>(Guids.IomedaeTechniqueBuff);
          return _inspireBuff;
        }
      }

      public override string GetCaption()
      {
        return "Custom action for Iomedae's Inspiring Sword";
      }

      public override void RunAction()
      {
        try
        {
          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          var targets = GameHelper.GetTargetsAround(caster.Position, 120.Feet()).Where(unit => unit.IsAlly(caster));

          foreach (var target in targets)
            target.AddBuff(InspireBuff, Context, 1.Minutes());
        }
        catch (Exception e)
        {
          Logger.LogException("Inspire.RunAction", e);
        }
      }
    }
    #endregion

    #region Irori
    private const string IroriName = "DFT.Irori";
    private const string IroriDisplayName = "DFT.Irori.Name";
    private const string IroriDescription = "DFT.Irori.Description";

    private const string IroriAdvanced = "DFT.Irori.Advanced";
    private const string IroriAdvancedDescription = "DFT.Irori.Advanced.Description";
    private const string IroriBuff = "DFT.Irori.Buff";
    private const string IroriToggle = "DFT.Irori.Toggle";

    private const string IroriIcon = IconPrefix + "iroritechnique.png";
    private const string IroriAdvancedIcon = IconPrefix + "iroriadvancedtechnique.png";

    private const DamageCalculationType AverageDamage = (DamageCalculationType)192;

    private static BlueprintFeature ConfigureIrori()
    {
      var buff = BuffConfigurator.New(IroriBuff, Guids.IroriTechniqueBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .AddComponent<IrorisFist>()
        .Configure();

      var toggle = ActivatableAbilityConfigurator.New(IroriToggle, Guids.IroriTechniqueToggle)
        .SetDisplayName(IroriDisplayName)
        .SetDescription(IroriDescription)
        .SetIcon(IroriIcon)
        .SetBuff(buff)
        .Configure();

      FeatureConfigurator.New(IroriAdvanced, Guids.IroriAdvancedTechnique)
        .SetDisplayName(IroriDisplayName)
        .SetDescription(IroriAdvancedDescription)
        .SetIcon(IroriAdvancedIcon)
        .SetIsClassFeature()
        .Configure();

      return FeatureConfigurator.New(IroriName, Guids.IroriTechnique)
        .SetDisplayName(IroriDisplayName)
        .SetDescription(IroriDescription)
        .SetIcon(IroriIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddComponent(new RecommendationWeaponFocus(WeaponCategory.UnarmedStrike))
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Melee)
        .AddPrerequisiteAlignment(AlignmentMaskType.LawfulNeutral)
        .AddComponent(
          new AdvancedTechniqueGrant(
            Guids.IroriAdvancedTechnique,
            ConditionsBuilder.New()
              .StatValue(n: 10, stat: StatType.BaseAttackBonus)
              .HasFact(FeatureRefs.CriticalFocus.ToString())
              .Add<HasWeaponFocus>(c => c.Category = WeaponCategory.UnarmedStrike)))
        .AddFacts(new() { toggle })
        .Configure();
    }

    [TypeId("54d0ac2f-b470-4a04-a414-2fd87d31280f")]
    private class IrorisFist :
      UnitFactComponentDelegate,
      IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>,
      IInitiatorRulebookHandler<RuleCalculateDamage>
    {
      private static BlueprintFeature _advancedTechnique;
      private static BlueprintFeature AdvancedTechnique
      {
        get
        {
          _advancedTechnique ??= BlueprintTool.Get<BlueprintFeature>(Guids.IroriAdvancedTechnique);
          return _advancedTechnique;
        }
      }

      public void OnEventAboutToTrigger(RuleCalculateDamage evt)
      {
        try
        {
          var damage = evt.DamageBundle.WeaponDamage;
          if (damage is null)
            return;

          if (evt.DamageBundle.Weapon.Blueprint.Category != WeaponCategory.UnarmedStrike)
            return;

          if (evt.ParentRule.AttackRoll.IsCriticalConfirmed && Owner.HasFact(AdvancedTechnique))
            damage.CalculationType.Set(DamageCalculationType.Maximized, Fact);
          else
            damage.CalculationType.Set(AverageDamage, Fact);
        }
        catch (Exception e)
        {
          Logger.LogException("IrorisFist.OnEventAboutToTrigger(Damage)", e);
        }
      }

      public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
      {
        try
        {
          if (Owner.HasFact(AdvancedTechnique))
            return;

          evt.AddModifier(-2, Fact, ModifierDescriptor.UntypedStackable);
        }
        catch (Exception e)
        {
          Logger.LogException("IrorisFist.OnEventAboutToTrigger(Attack)", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateDamage evt) { }

      public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt) { }
    }

    [HarmonyPatch(typeof(RuleCalculateDamage))]
    static class RuleCalculateDamage_Patch
    {
      [HarmonyPatch(nameof(RuleCalculateDamage.Roll)), HarmonyPrefix]
      static bool Roll(
        DiceFormula damageFormula, int unitsCount, DamageCalculationType calculationType, ref int __result)
      {
        try
        {
          if (damageFormula == DiceFormula.Zero)
          {
            __result = 0;
            return false;
          }

          if (calculationType == AverageDamage)
          {
            int perDie = (int)Math.Floor((damageFormula.Dice.Sides() + 1) / 2f);
            __result = damageFormula.Rolls * perDie * unitsCount;
            return false;
          }
        }
        catch (Exception e)
        {
          Logger.LogException("RuleCalculateDamage_Patch.Roll", e);
        }
        return true;
      }
    }
    #endregion

    #region Lamashtu
    private const string LamashtuName = "DFT.Lamashtu";
    private const string LamashtuDisplayName = "DFT.Lamashtu.Name";
    private const string LamashtuDescription = "DFT.Lamashtu.Description";

    private const string LamashtuAdvanced = "DFT.Lamashtu.Advanced";
    private const string LamashtuAdvancedDescription = "DFT.Lamashtu.Advanced.Description";
    private const string LamashtuBuff = "DFT.Lamashtu.Buff";
    private const string LamashtuImmunityBuff = "DFT.Lamashtu.Immunity.Buff";
    private const string LamashtuAbility = "DFT.Lamashtu.Ability";

    private const string LamashtuIcon = IconPrefix + "lamashtutechnique.png";
    private const string LamashtuAdvancedIcon = IconPrefix + "lamashtuadvancedtechnique.png";

    private static BlueprintFeature ConfigureLamashtu()
    {
      var buff = BuffConfigurator.New(LamashtuBuff, Guids.LamashtuTechniqueBuff)
        .CopyFrom(BuffRefs.Bleed1d4Buff, c => c is not AddFactContextActions)
        .SetDisplayName(LamashtuDisplayName)
        .SetDescription(LamashtuDescription)
        .SetIcon(LamashtuIcon)
        .SetStacking(StackingType.Ignore)
        .AddFactContextActions(
          newRound: ActionsBuilder.New()
            .DealDamagePreRolled(DamageTypes.Physical(form: PhysicalDamageForm.Slashing), AbilitySharedValue.Damage))
        .Configure();

      var immunityBuff = BuffConfigurator.New(LamashtuImmunityBuff, Guids.LamashtuTechniqueImmunityBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddNotDispelable()
        .Configure();

      var ability = AbilityConfigurator.New(LamashtuAbility, Guids.LamashtuTechniqueAbility)
        .SetDisplayName(LamashtuDisplayName)
        .SetDescription(LamashtuDescription)
        .SetIcon(LamashtuIcon)
        .AllowTargeting(enemies: true)
        .SetType(AbilityType.Physical)
        .SetNeedEquipWeapons()
        .SetRange(AbilityRange.Weapon)
        .SetActionType(CommandType.Standard)
        .SetAnimation(CastAnimationStyle.Special)
        .AddAbilityCasterMainWeaponCheck(WeaponCategory.Falchion, WeaponCategory.Kukri)
        .AddAbilityEffectRunAction(ActionsBuilder.New().Add<LamashtusCarving>())
        .Configure();

      FeatureConfigurator.New(LamashtuAdvanced, Guids.LamashtuAdvancedTechnique)
        .SetDisplayName(LamashtuDisplayName)
        .SetDescription(LamashtuAdvancedDescription)
        .SetIcon(LamashtuAdvancedIcon)
        .SetIsClassFeature()
        .AddComponent<LamashtusStaggeringSlice>()
        .Configure();

      return FeatureConfigurator.New(LamashtuName, Guids.LamashtuTechnique)
        .SetDisplayName(LamashtuDisplayName)
        .SetDescription(LamashtuDescription)
        .SetIcon(LamashtuIcon)
        .SetIsClassFeature()
        .SetReapplyOnLevelUp()
        .AddComponent(new RecommendationWeaponFocus(WeaponCategory.Falchion, WeaponCategory.Kukri))
        .AddFeatureTagsComponent(FeatureTag.Attack | FeatureTag.Melee)
        .AddPrerequisiteAlignment(AlignmentMaskType.ChaoticEvil)
        .AddComponent(
          new AdvancedTechniqueGrant(
            Guids.LamashtuAdvancedTechnique,
            ConditionsBuilder.New()
              .StatValue(n: 10, stat: StatType.BaseAttackBonus)
              .StatValue(n: 13, stat: StatType.Strength)
              .HasFact(Guids.DazingAssaultFeat)
              .HasFact(FeatureRefs.PowerAttackFeature.ToString())))
        .AddFacts(new() { ability })
        .Configure();
    }

    [TypeId("2f11e903-f246-4db2-8a65-a41412d03273")]
    private class LamashtusCarving : ContextActionMeleeAttack
    {
      private static BlueprintBuff _bleed;
      private static BlueprintBuff Bleed
      {
        get
        {
          _bleed ??= BlueprintTool.Get<BlueprintBuff>(Guids.LamashtuTechniqueBuff);
          return _bleed;
        }
      }

      public override string GetCaption()
      {
        return "Custom action for Lamashtu's Carving bleed effect";
      }

      public override void RunAction()
      {
        IRulebookHandler<RuleCalculateDamage> bleedHandler = null;
        try
        {
          var target = Context.MainTarget.Unit;
          if (target is null)
          {
            Logger.Warning("No valid target");
            return;
          }

          var caster = Context.MaybeCaster;
          if (caster is null)
          {
            Logger.Warning("No caster");
            return;
          }

          bleedHandler = new LamashtusBleed(caster, Context);
          EventBus.Subscribe(bleedHandler);

          var attack =
            Rulebook.Trigger<RuleAttackWithWeapon>(
              new(caster, target, caster.GetFirstWeapon(), attackBonusPenalty: 0));

          if (attack.AttackRoll.IsHit)
            target.AddBuff(Bleed, Context);
        }
        catch (Exception e)
        {
          Logger.LogException("LamashtusCarving.RunAction", e);
        }
        finally
        {
          if (bleedHandler is not null)
            EventBus.Unsubscribe(bleedHandler);
        }
      }

      private class LamashtusBleed : IInitiatorRulebookHandler<RuleCalculateDamage>
      {
        private readonly UnitEntityData Unit;
        private readonly MechanicsContext Context;

        public LamashtusBleed(UnitEntityData unit, MechanicsContext context)
        {
          Unit = unit;
          Context = context;
        }

        public UnitEntityData GetSubscribingUnit()
        {
          return Unit;
        }

        public void OnEventAboutToTrigger(RuleCalculateDamage evt) { }

        public void OnEventDidTrigger(RuleCalculateDamage evt)
        {
          try
          {
            int bleed = 0;
            foreach (var damage in evt.DamageBundle)
            {
              bleed += damage.TotalBonus;
              damage.Bonus = 0;
              damage.BonusTargetRelated = 0;
            }

            Logger.NativeLog($"Setting bleed damage to {bleed}");
            Context[AbilitySharedValue.Damage] = bleed;
          }
          catch (Exception e)
          {
            Logger.LogException("LamashtusBleed.OnEventDidTrigger", e);
          }
        }
      }
    }

    [TypeId("c6db8fd4-3374-47c7-8ac8-dbdd4bf3e3a3")]
    private class LamashtusStaggeringSlice : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleAttackWithWeapon>
    {
      private static BlueprintBuff _immunity;
      private static BlueprintBuff Immunity
      {
        get
        {
          _immunity ??= BlueprintTool.Get<BlueprintBuff>(Guids.LamashtuTechniqueImmunityBuff);
          return _immunity;
        }
      }

      public void OnEventAboutToTrigger(RuleAttackWithWeapon evt) { }

      public void OnEventDidTrigger(RuleAttackWithWeapon evt)
      {
        try
        {
          if (!evt.AttackRoll.IsHit)
            return;

          if (evt.Weapon.Blueprint.Category != WeaponCategory.Falchion
              && evt.Weapon.Blueprint.Category != WeaponCategory.Kukri)
            return;

          var target = evt.Target;
          if (target.HasFact(Immunity))
          {
            Logger.NativeLog($"{target.CharacterName} is immune");
            return;
          }

          foreach (var buff in target.Buffs)
          {
            if (buff.Blueprint.SpellDescriptor.HasFlag(SpellDescriptor.Bleed))
            {
              var dc = 10 + Owner.Stats.GetStat(StatType.BaseAttackBonus);
              Logger.NativeLog($"Triggering saving throw for {target.CharacterName} with DC {dc}");
              var savingThrow = Rulebook.Trigger<RuleSavingThrow>(new(target, SavingThrowType.Fortitude, dc));
              if (savingThrow.IsPassed)
                target.AddBuff(Immunity, Context, 1.Rounds().Seconds);
              else
                target.AddBuff(BuffRefs.Staggered.Reference.Get(), Context, 1.Rounds().Seconds);
              break;
            }
          }
        }
        catch (Exception e)
        {
          Logger.LogException("LamashtusDaze.OnEventDidTrigger", e);
        }
      }
    }
    #endregion
  }
}
