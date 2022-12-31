using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums.Damage;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Parts;
using System;
using System.Linq;
using TabletopTweaks.Core.Utilities;
using static Kingmaker.RuleSystem.RulebookEvent;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Archetypes
{
  internal class WinterWitch
  {
    private const string ArchetypeName = "WinterWitch";

    private const string FamiliarSelection = "WinterWitch.Familiar";
    private const string PatronSelection = "WinterWitch.Patron";
    private const string WinterWitchCantrips = "WinterWitch.Cantrips";
    private const string Spellbook = "WinterWitch.Spellbook";
    private const string SpellList = "WinterWitch.SpellList";

    private const string IceMagic = "WinterWitch.IceMagic";
    private const string ColdFlesh4 = "WinterWitch.ColdFlesh.4";
    private const string ColdFlesh9 = "WinterWitch.ColdFlesh.9";
    private const string ColdFlesh14 = "WinterWitch.ColdFlesh.14";
    private const string FrozenCaress = "WinterWitch.FrozenCaress";
    private const string FrozenCaressAbility = "WinterWitch.FrozenCaress.Ability";
    private const string FrozenCaressBuff = "WinterWitch.FrozenCaress.Buff";

    internal const string ArchetypeDisplayName = "WinterWitch.Name";
    private const string ArchetypeDescription = "WinterWitch.Description";

    private const string IceMagicDisplayName = "WinterWitch.IceMagic.Name";
    private const string IceMagicDescription = "WinterWitch.IceMagic.Description";

    private const string ColdFleshDisplayName = "WinterWitch.ColdFlesh.Name";
    private const string ColdFleshDescription = "WinterWitch.ColdFlesh.Description";

    private const string FrozenCaressDisplayName = "WinterWitch.FrozenCaress.Name";
    private const string FrozenCaressDescription = "WinterWitch.FrozenCaress.Description";

    private static readonly ModLogger Logger = Logging.GetLogger(ArchetypeName);

    private const string IconPrefix = "assets/icons/";
    private const string IconName = IconPrefix + "burningdisarm.png";

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.WinterWitchArchetype))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("WinterWitch.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {ArchetypeName} (disabled)");

      ArchetypeConfigurator.New(ArchetypeName, Guids.WinterWitchArchetype).Configure();
      FeatureSelectionConfigurator.New(FamiliarSelection, Guids.WinterWitchFamiliarSelection).Configure();
      FeatureSelectionConfigurator.New(PatronSelection, Guids.WinterWitchPatronSelection).Configure();
      FeatureConfigurator.New(WinterWitchCantrips, Guids.WinterWitchCantrips).Configure();
      SpellbookConfigurator.New(Spellbook, Guids.WinterWitchSpellbook).Configure();
      SpellListConfigurator.New(SpellList, Guids.WinterWitchSpellList).Configure();

      FeatureConfigurator.New(IceMagic, Guids.WinterWitchIceMagic).Configure();
      FeatureConfigurator.New(ColdFlesh4, Guids.WinterWitchColdFlesh4).Configure();
      FeatureConfigurator.New(ColdFlesh9, Guids.WinterWitchColdFlesh9).Configure();
      FeatureConfigurator.New(ColdFlesh14, Guids.WinterWitchColdFlesh14).Configure();
      FeatureConfigurator.New(FrozenCaress, Guids.WinterWitchFrozenCaress).Configure();
      ActivatableAbilityConfigurator.New(FrozenCaressAbility, Guids.WinterWitchFrozenCaressAbility).Configure();
      BuffConfigurator.New(FrozenCaressBuff, Guids.WinterWitchFrozenCaressBuff).Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {ArchetypeName}");

      var archetype = ArchetypeConfigurator.New(
          ArchetypeName, Guids.WinterWitchArchetype, clazz: CharacterClassRefs.WitchClass.ToString())
        .SetLocalizedName(ArchetypeDisplayName)
        .SetLocalizedDescription(ArchetypeDescription)
        .SetReplaceSpellbook(CreateSpellbook());

      archetype
        .AddToRemoveFeatures(
          level: 1,
          FeatureSelectionRefs.WitchFamiliarSelection.ToString(),
          FeatureSelectionRefs.WitchPatronSelection.ToString())
        .AddToRemoveFeatures(level: 4, FeatureSelectionRefs.WitchHexSelection.ToString());

      archetype
        .AddToAddFeatures(
          level: 1, CreateFamiliarSelection(), CreatePatronSelection(), CreateCantrips(), CreateIceMagic())
        .AddToAddFeatures(level: 4, CreateColdFlesh4())
        .AddToAddFeatures(level: 9, CreateColdFlesh9())
        .AddToAddFeatures(level: 14, CreateColdFlesh14());
      archetype.Configure();

      var buff = BuffConfigurator.New(FrozenCaressBuff, Guids.WinterWitchFrozenCaressBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddComponent<FrozenCaressComponent>()
        .Configure();

      var frozenCaress = ActivatableAbilityConfigurator.New(FrozenCaressAbility, Guids.WinterWitchFrozenCaressAbility)
        .SetDisplayName(FrozenCaressDisplayName)
        .SetDescription(FrozenCaressDescription)
        .SetBuff(buff)
        .SetActivationType(AbilityActivationType.WithUnitCommand)
        .SetActivateWithUnitCommand(CommandType.Swift)
        .SetDeactivateIfCombatEnded()
        .AddActivatableAbilityUnitCommand(type: CommandType.Swift)
        .Configure();

      FeatureConfigurator.New(FrozenCaress, Guids.WinterWitchFrozenCaress, FeatureGroup.WitchHex)
        .SetDisplayName(FrozenCaressDisplayName)
        .SetDescription(FrozenCaressDescription)
        .SetIsClassFeature()
        .AddPrerequisiteArchetypeLevel(
          archetype: Guids.WinterWitchArchetype, characterClass: CharacterClassRefs.WitchClass.ToString())
        .AddFacts(new() { frozenCaress })
        .Configure(delayed: true);
    }

    #region Spells
    private static BlueprintSpellbook CreateSpellbook()
    {
      return SpellbookConfigurator.New(Spellbook, Guids.WinterWitchSpellbook)
        .CopyFrom(SpellbookRefs.WitchSpellbook)
        .SetSpellList(CreateSpellList())
        .Configure();
    }

    private static BlueprintSpellList CreateSpellList()
    {
      return SpellListConfigurator.New(SpellList, Guids.WinterWitchSpellList)
        .OnConfigure(PopulateSpellList)
        .Configure(delayed: true);
    }

    private static void PopulateSpellList(BlueprintSpellList spellList)
    {
      var witchSpells = SpellListRefs.WitchSpellList.Reference.Get().SpellsByLevel;
      var winterWitchSpells = new SpellLevelList[witchSpells.Length];

      for (int i = 0; i < winterWitchSpells.Length; i++)
      {
        var spells = new SpellLevelList(i);
        spells.m_Spells =
          witchSpells[i].Spells.Where(
            spell =>
            {
              var descriptor = spell.GetComponent<SpellDescriptorComponent>();
              if (descriptor is null)
                return true;

              return !descriptor.Descriptor.HasAnyFlag(SpellDescriptor.Fire);
            })
          .Select(spell => spell.ToReference<BlueprintAbilityReference>())
          .ToList();

        winterWitchSpells[i] = spells;
      }

      winterWitchSpells[0].m_Spells.Add(AbilityRefs.RayOfFrost.Cast<BlueprintAbilityReference>().Reference);
      spellList.SpellsByLevel = winterWitchSpells;
    }

    private static BlueprintFeature CreateCantrips()
    {
      var witch = CharacterClassRefs.WitchClass.ToString();
      var rayOfFrost = AbilityRefs.RayOfFrost.ToString();
      return FeatureConfigurator.New(WinterWitchCantrips, Guids.WinterWitchCantrips)
        .CopyFrom(FeatureRefs.WitchCantripsFeature)
        .AddLearnSpells(characterClass: witch, spells: new() { rayOfFrost })
        .AddBindAbilitiesToClass(characterClass: witch, abilites: new() { rayOfFrost })
        .Configure();
    }
    #endregion

    private static BlueprintFeatureSelection CreateFamiliarSelection()
    {
      return FeatureSelectionConfigurator.New(FamiliarSelection, Guids.WinterWitchFamiliarSelection)
        .CopyFrom(FeatureSelectionRefs.WitchFamiliarSelection)
        .SetAllFeatures(
          FeatureRefs.CatFamiliarBondFeature.ToString(),
          FeatureRefs.HareFamiliarBondFeature.ToString(),
          FeatureRefs.RabbitFamiliarBondFeature.ToString(),
          FeatureRefs.RatFamiliarBondFeature.ToString(),
          FeatureRefs.RavenFamiliarBondFeature.ToString())
        .Configure();
    }

    private static BlueprintFeatureSelection CreatePatronSelection()
    {
      return FeatureSelectionConfigurator.New(PatronSelection, Guids.WinterWitchPatronSelection)
        .CopyFrom(FeatureSelectionRefs.WitchPatronSelection)
        .SetAllFeatures(
          ProgressionRefs.WitchAncestorsPatronProgression.ToString(),
          ProgressionRefs.WitchDeceptionPatronProgression.ToString(),
          ProgressionRefs.WitchEndurancePatronProgression.ToString(),
          ProgressionRefs.WitchTransformationhPatronProgression.ToString(),
          ProgressionRefs.WitchWinterPatronProgression.ToString())
        .Configure();
    }

    private static BlueprintFeature CreateIceMagic()
    {
      return FeatureConfigurator.New(IceMagic, Guids.WinterWitchIceMagic)
        .SetDisplayName(IceMagicDisplayName)
        .SetDescription(IceMagicDescription)
        .SetIsClassFeature()
        .AddIncreaseSpellDescriptorDC(descriptor: SpellDescriptor.Cold, bonusDC: 1, spellsOnly: true)
        .Configure();
    }

    #region Cold Flesh
    private static BlueprintFeature CreateColdFlesh4()
    {
      return FeatureConfigurator.New(ColdFlesh4, Guids.WinterWitchColdFlesh4)
        .SetDisplayName(ColdFleshDisplayName)
        .SetDescription(ColdFleshDescription)
        .SetIsClassFeature()
        .AddResistEnergy(type: DamageEnergyType.Cold, value: 5)
        .Configure();
    }

    private static BlueprintFeature CreateColdFlesh9()
    {
      return FeatureConfigurator.New(ColdFlesh9, Guids.WinterWitchColdFlesh9)
        .SetDisplayName(ColdFleshDisplayName)
        .SetDescription(ColdFleshDescription)
        .SetIsClassFeature()
        .AddResistEnergy(type: DamageEnergyType.Cold, value: 10)
        .Configure();
    }

    private static BlueprintFeature CreateColdFlesh14()
    {
      return FeatureConfigurator.New(ColdFlesh14, Guids.WinterWitchColdFlesh14)
        .SetDisplayName(ColdFleshDisplayName)
        .SetDescription(ColdFleshDescription)
        .SetIsClassFeature()
        .AddEnergyImmunity(type: DamageEnergyType.Cold)
        .Configure();
    }
    #endregion

    [TypeId("52af25ca-44f8-41fb-99a5-dc6e09933e5e")]
    private class FrozenCaressComponent :
      UnitBuffComponentDelegate,
      IInitiatorRulebookHandler<RuleCalculateAbilityParams>,
      IInitiatorRulebookHandler<RuleDealDamage>
    {
      public void OnEventAboutToTrigger(RuleCastSpell evt)
      {
        try
        {
          if (evt.Spell.Blueprint.Range != AbilityRange.Touch)
            return;

          evt.Context.AddSpellDescriptor(SpellDescriptor.Cold);
        }
        catch (Exception e)
        {
          Logger.LogException("FrozenCaressComponent.OnEventAboutToTrigger(RuleCastSpell)", e);
        }
      }

      public void OnEventDidTrigger(RuleCastSpell evt) { }

      public void OnEventAboutToTrigger(RuleDealDamage evt)
      {
        try
        {
          if (evt.Reason.Ability is null)
            return;

          if (evt.Reason.Context is null)
            return;

          var ability = evt.Reason.Ability.Blueprint;
          if (ability.Type != AbilityType.Spell)
            return;

          if (ability.Range != AbilityRange.Touch)
            return;

          var savingThrow = evt.Reason.Context.SavingThrow;
          if (savingThrow is not null && savingThrow.IsPassed)
            return;

          var damage = DamageTypes.Energy(DamageEnergyType.Cold);
          evt.Add(damage.CreateDamage(new DiceFormula(rollsCount: 1, diceType: DiceType.D4), bonus: 0));
        }
        catch (Exception e)
        {
          Logger.LogException("FrozenCaressComponent.OnEventAboutToTrigger(RuleDealDamage)", e);
        }
      }

      public void OnEventDidTrigger(RuleDealDamage evt) { }

      public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt)
      {
        try
        {
          if (evt.Spellbook == null)
            return;

          var spell = evt.Spell;
          if (spell.Range != AbilityRange.Touch)
            return;

          var component = spell.GetComponent<SpellDescriptorComponent>();
          if (component is not null)
          {
            var descriptor = UnitPartChangeSpellElementalDamage.ReplaceSpellDescriptorIfCan(Owner, component.Descriptor);
            if (descriptor.HasAnyFlag(SpellDescriptor.Cold))
              return;
          }

          evt.AddBonusDC(dc: 1); // Ice Magic won't apply since the cold descriptor isn't applied yet
        }
        catch (Exception e)
        {
          Logger.LogException("FrozenCaressComponent.OnEventAboutToTrigger(RuleCalculateAbilityParams)", e);
        }
      }

      public void OnEventDidTrigger(RuleCalculateAbilityParams evt) { }
    }
  }
}
