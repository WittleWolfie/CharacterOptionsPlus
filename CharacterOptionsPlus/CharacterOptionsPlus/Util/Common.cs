using BlueprintCore.Blueprints.Configurators.Classes;
using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Spells;
using System;

namespace CharacterOptionsPlus.Util
{
  /// <summary>
  /// Container for common utils or constants.
  /// </summary>
  internal static class Common
  {
    internal static readonly string DurationVaries = "Duration.Varies";
    internal static readonly string DurationRoundPerTwoLevels = "Duration.RoundPerTwoLevels";

    internal static readonly string SavingThrowVaries = "SavingThrow.Varies";
    internal static readonly string SavingThrowReflexHalfWillNegates = "SavingThrow.ReflexHalf.WillNegates";
    internal static readonly string SavingThrowFortPartialOrWillNegates = "SavingThrow.FortPartial.Or.WillNegates";

    /// <summary>
    /// Adds a new BlueprintFeatureReplaceSpellbook for an archetype which alters a spellbook.
    /// </summary>
    /// <param name="characterClass">Guid / name of class for archetype</param>
    /// <param name="archetype">Guid / name of archetype</param>
    /// <param name="baseReplacement">Base class spellbook replacement</param>
    /// <param name="sourceReplacement">Spellbook replacement to copy from</param>
    /// <param name="replacementName">New replacement name</param>
    /// <param name="replacementGuid">New replacement guid</param>
    /// <param name="spellbook">Spellbook for new replacement</param>
    /// <param name="replacementSelection">Guid / name of replacement selection bp</param>
    internal static void ConfigureArchetypeSpellbookReplacement(
      string characterClass,
      string archetype,
      string baseReplacement, 
      string sourceReplacement,
      string replacementName,
      string replacementGuid,
      BlueprintSpellbook spellbook,
      string replacementSelection,
      params Type[] typesToCopy)
    {
      FeatureReplaceSpellbookConfigurator.For(baseReplacement)
        .AddPrerequisiteNoArchetype(characterClass: characterClass, archetype: archetype)
        .Configure();
      var types = CommonTool.Append(typesToCopy, typeof(PrerequisiteClassSpellLevel));
      var replacement = FeatureReplaceSpellbookConfigurator.New(replacementName, replacementGuid)
        .CopyFrom(sourceReplacement, types)
        .AddPrerequisiteArchetypeLevel(characterClass: characterClass, archetype: archetype)
        .SetSpellbook(spellbook)
        .Configure();

      FeatureSelectionConfigurator.For(replacementSelection)
        .AddToAllFeatures(replacement)
        .Configure();
    }
  }
}
