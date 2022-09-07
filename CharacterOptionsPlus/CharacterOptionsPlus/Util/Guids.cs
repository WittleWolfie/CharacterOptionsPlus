using CharacterOptionsPlus.Archetypes;
using CharacterOptionsPlus.Feats;

namespace CharacterOptionsPlus.Util
{
  /// <summary>
  /// List of new Guids.
  /// </summary>
  internal static class Guids
  {
    //** Archetypes **//
    internal const string ArrowsongMinstrelArchetype = "b704d577-abe5-4873-b922-8f56c2319b54";
    internal const string ArrowsongMinstrelArcaneArchery = "23d3d217-93cc-4ae6-a034-860c89561253";
    internal const string ArrowsongMinstrelSpellbook = "b9e9804e-5361-43e7-87c2-ffb7b1195be5";
    internal const string ArrowsongMinstrelSpellList = "74f9286f-6e8e-46ba-8de8-995759759b2f";
    internal const string ArrowsongMinstrelSpellsPerDay = "32e41814-4688-42b6-87a7-4384f602a621";
    internal const string ArrowsongMinstrelProficiencies = "e24693cb-6e0b-4bbd-af5b-05de85f5ff88";
    //****************//

    internal static readonly (string guid, string displayName)[] Archetypes =
      new (string, string)[]
      {
        (ArrowsongMinstrelArchetype, ArrowsongMinstrel.ArchetypeDisplayName),
      };

    //** Feats **//
    internal const string FuriousFocusFeat = "de9a75d3-1289-4098-a0b7-fda465a79576";

    internal const string HurtfulFeat = "9474814d-363c-401d-9385-c2ce59fe2e3c";
    internal const string HurtfulAbility = "9ef73549-6706-4d33-abd0-bdfaa3ada6a8";
    internal const string HurtfulBuff = "4fd8ee4d-cf60-4b99-9316-e57a71be80ea";

    internal const string SkaldsVigorFeat = "55dd527b-8721-426b-aaa2-036ccc9a0458";
    internal const string SkaldsVigorGreaterFeat = "ee4756c6-797f-4848-a814-4a27a159641d";
    internal const string SkaldsVigorBuff = "9e67121d-0433-4706-a107-7796187df3e1";
    //***********//

    internal static readonly (string guid, string displayName)[] Feats =
      new (string, string)[]
      {
        (FuriousFocusFeat, FuriousFocus.FeatDisplayName),
        (HurtfulFeat, Hurtful.FeatDisplayName),
        (SkaldsVigorFeat, SkaldsVigor.FeatureDisplayName),
      };
  }
}