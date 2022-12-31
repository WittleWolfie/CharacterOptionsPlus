using BlueprintCore.Blueprints.Configurators.Classes.Spells;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus
{
  /// <summary>
  /// A collection of homebrew / optional rules. Some of these are TT rules that were ignored by Owlcat.
  /// </summary>
  internal class Homebrew
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(Homebrew));

    internal static void Configure()
    {
      if (Settings.IsEnabled(ConeOfColdWinterPatron))
        ConfigureConeOfColdWinterPatron();
      if (Settings.IsEnabled(ImplosionDestructionDomain))
        ConfigureImplosionDestructionDomain();
    }

    internal const string ConeOfColdWinterPatron = "cone-of-cold-winter-patron";
    internal static void ConfigureConeOfColdWinterPatron()
    {
      Logger.Log("Patching Cone of Cold Winter Patron");
      var coldIceStrike = AbilityRefs.ColdIceStrike.Reference;
      var coneOfCold = AbilityRefs.ConeOfCold.Cast<BlueprintAbilityReference>().Reference;
      ProgressionConfigurator.For(ProgressionRefs.WitchWinterPatronProgression)
        .EditComponent<AddSpellsToDescription>(
          c =>
          {
            bool replaced = false;
            for (int i = 0; i < c.m_Spells.Length; i++)
            {
              if (c.m_Spells[i].deserializedGuid == coldIceStrike.deserializedGuid)
              {
                replaced = true;
                c.m_Spells[i] = coneOfCold;
              }
            }
            if (!replaced)
              Logger.Warning("Cold Ice Strike was not found in the description");
          })
        .Configure();

      FeatureConfigurator.For(FeatureRefs.WitchPatronSpellLevel6_Winter)
        .EditComponent<AddKnownSpell>(c => c.m_Spell = coneOfCold)
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

    // Change is in GloriousHeat
    internal const string OriginalGloriousHeat = "glorious-heat-og";

    // Fixes are in EldritchHeritage
    internal const string SingleDraconicBloodline = "single-draconic-bloodline";
    internal const string SingleElementalBloodline = "single-elemental-bloodline";

    // Change is in FreezingSphere
    internal const string SelectiveFreezingSphere = "freezing-sphere-selective";

    internal static readonly List<(string key, string name, string description)> Entries =
      new()
      {
        (ConeOfColdWinterPatron, "ConeOfCold.WinterPatron.Name", "ConeOfCold.WinterPatron.Description"),
        (ImplosionDestructionDomain, "Implosion.DestructionDomain.Name", "Implosion.DestructionDomain.Description"),
        (SelectiveFreezingSphere, "Homebrew.FreezingSphere.Name", "Homebrew.FreezingSphere.Description"),
        (OriginalGloriousHeat, "Homebrew.GloriousHeat.Name", "Homebrew.GloriousHeat.Description"),
        (SingleDraconicBloodline, "SingleDraconicBloodline.Name", "SingleDraconicBloodline.Description"),
        (SingleElementalBloodline, "SingleElementalBloodline.Name", "SingleElementalBloodline.Description"),
      };
  }
}
