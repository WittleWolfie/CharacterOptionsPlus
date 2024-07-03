using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Spells;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.UnitLogic.FactLogic;
using System.Collections.Generic;

namespace CharacterOptionsPlus
{
  /// <summary>
  /// A collection of homebrew / optional rules. Some of these are TT rules that were ignored by Owlcat.
  /// </summary>
  internal class Homebrew
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(Homebrew));

    internal static void Configure()
    {
      // Not needed anymore
      //if (Settings.IsEnabled(DreadfulCarnagePrereq))
      //  ConfigureDreadfulCarnagePrereq();
      if (Settings.IsEnabled(ImplosionDestructionDomain))
        ConfigureImplosionDestructionDomain();
      if (Settings.IsEnabled(WinterPatronSpells))
        ConfigureWinterPatronSpells();
    }

    internal const string DreadfulCarnagePrereq = "dreadful-carnage-prereq";
    internal static void ConfigureDreadfulCarnagePrereq()
    {
      Logger.Log("Patching Dreadful Carnage prerequisites");
      var dazzlingDisplay = FeatureRefs.DazzlingDisplayFeature.Reference;
      FeatureConfigurator.For(FeatureRefs.DreadfulCarnage)
        .RemoveComponents(
          c =>
          {
            if (c is not PrerequisiteFeature prereq)
              return false;
            return prereq.m_Feature.deserializedGuid == dazzlingDisplay.deserializedGuid;
          })
        .AddPrerequisiteFeature(Guids.FuriousFocusFeat)
        .Configure();
    }

    internal const string ImplosionDestructionDomain = "implosion-destruction-domain";
    internal static void ConfigureImplosionDestructionDomain()
    {
      Logger.Log("Patching Implosion - Destruction Domain");
      var implosion = BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.ImplosionSpell);
      SpellListConfigurator.For(SpellListRefs.DestructionDomainSpellList)
        .OnConfigure(bp => bp.SpellsByLevel[9].m_Spells = new() { implosion })
        .Configure();
    }

    internal const string WinterPatronSpells = "winter-patron-spells";
    internal static void ConfigureWinterPatronSpells()
    {
      // Level 1 - Snowball
      // Level 2 - Resist Fire
      // Level 3 - Ice Storm (New) [Protection from Fire]
      // Level 4 - Icy Prison (New) [Ice Storm]
      // Level 5 - Cone of Cold (New) [Icy Prison]
      // Level 6 - Freezing Sphere (New) [Cold Ice Strike]
      // Level 7 - Icy Body
      // Level 8 - Polar Ray
      // Level 9 - Polar Midnight
      Logger.Log("Patching Winter Patron Spells");

      // First fix the spells description
      var protectionFromFire = AbilityRefs.ProtectionFromFire.Reference;
      var freezingSphere = BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.FreezingSphereSpell);

      var coldIceStrike = AbilityRefs.ColdIceStrike.Reference;
      var coneOfCold = AbilityRefs.ConeOfCold.Cast<BlueprintAbilityReference>().Reference;
      ProgressionConfigurator.For(ProgressionRefs.WitchWinterPatronProgression)
        .EditComponent<AddSpellsToDescription>(
          c =>
          {
            for (int i = 0; i < c.m_Spells.Length; i++)
            {
              var guid = c.m_Spells[i].deserializedGuid;
              if (guid == coldIceStrike.deserializedGuid)
                c.m_Spells[i] = coneOfCold;
              else if (guid == protectionFromFire.deserializedGuid)
                c.m_Spells[i] = freezingSphere;
            }
          })
        .Configure();

      FeatureConfigurator.For(FeatureRefs.WitchPatronSpellLevel3_Winter)
        .EditComponent<AddKnownSpell>(
          c => c.m_Spell = AbilityRefs.IceStorm.Cast<BlueprintAbilityReference>().Reference)
        .Configure();

      FeatureConfigurator.For(FeatureRefs.WitchPatronSpellLevel4_Winter)
        .EditComponent<AddKnownSpell>(
          c => c.m_Spell = AbilityRefs.IcyPrison.Cast<BlueprintAbilityReference>().Reference)
        .Configure();

      FeatureConfigurator.For(FeatureRefs.WitchPatronSpellLevel5_Winter)
        .EditComponent<AddKnownSpell>(
          c => c.m_Spell = coneOfCold)
        .Configure();

      FeatureConfigurator.For(FeatureRefs.WitchPatronSpellLevel6_Winter)
        .EditComponent<AddKnownSpell>(
          c => c.m_Spell = BlueprintTool.GetRef<BlueprintAbilityReference>(Guids.FreezingSphereSpell))
        .Configure();
    }

    // Change is in GloriousHeat
    internal const string OriginalGloriousHeat = "glorious-heat-og";

    // Change is in CompanionShareSpells
    internal const string CompanionShareSpells = "companion-share-spells";

    // Fixes are in EldritchHeritage
    internal const string SingleDraconicBloodline = "single-draconic-bloodline";
    internal const string SingleElementalBloodline = "single-elemental-bloodline";

    // Change is in FreezingSphere
    internal const string SelectiveFreezingSphere = "freezing-sphere-selective";

    internal static readonly List<(string key, string name, string description)> Entries =
      new()
      {
        (CompanionShareSpells, "CompanionShareSpells.Name", "CompanionShareSpells.Description"),
     //   (DreadfulCarnagePrereq, "DreadfulCarnagePrereq.Name", "DreadfulCarnagePrereq.Description"),
        (ImplosionDestructionDomain, "Implosion.DestructionDomain.Name", "Implosion.DestructionDomain.Description"),
        (SelectiveFreezingSphere, "Homebrew.FreezingSphere.Name", "Homebrew.FreezingSphere.Description"),
        (OriginalGloriousHeat, "Homebrew.GloriousHeat.Name", "Homebrew.GloriousHeat.Description"),
        (SingleDraconicBloodline, "SingleDraconicBloodline.Name", "SingleDraconicBloodline.Description"),
        (SingleElementalBloodline, "SingleElementalBloodline.Name", "SingleElementalBloodline.Description"),
        (WinterPatronSpells, "WinterPatronSpells.Name", "WinterPatronSpells.Description"),
      };
  }
}
