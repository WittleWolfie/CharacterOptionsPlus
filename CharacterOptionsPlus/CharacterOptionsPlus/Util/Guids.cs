using CharacterOptionsPlus.Archetypes;
using CharacterOptionsPlus.Feats;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Util
{
  /// <summary>
  /// List of new Guids.
  /// </summary>
  internal static class Guids
  {
    private static ModLogger Logger = Logging.GetLogger(nameof(Guids));

    //** Archetypes **//
    internal const string ArrowsongMinstrelArchetype = "b704d577-abe5-4873-b922-8f56c2319b54";
    internal const string ArrowsongMinstrelProficiencies = "e24693cb-6e0b-4bbd-af5b-05de85f5ff88";

    internal const string ArrowsongMinstrelArcaneArchery = "23d3d217-93cc-4ae6-a034-860c89561253";
    internal const string ArrowsongMinstrelSpellStrike = "4c26bf5c-971c-4183-b3ec-3005702773d5";
    internal const string ArrowsongMinstrelSpellStrikeAbility = "5e9d97d0-6d80-4626-9102-4e0b1c907b1a";
    internal const string ArrowsongMinstrelSpellCombat = "647345f6-6a85-40cc-8776-f46165cd435d";
    internal const string ArrowsongMinstrelSpellCombatAbility = "708daf96-aff7-4082-a3b5-14854cf8287e";

    internal const string ArrowsongMinstrelSpellbook = "b9e9804e-5361-43e7-87c2-ffb7b1195be5";
    internal const string ArrowsongMinstrelSpellList = "74f9286f-6e8e-46ba-8de8-995759759b2f";
    internal const string ArrowsongMinstrelSpellsPerDay = "32e41814-4688-42b6-87a7-4384f602a621";
    internal const string ArrowsongMinstrelSpellSelection = "3c9b1fb1-9466-45db-9fe8-08500b66ea50";
    //****************//

    internal static readonly (string guid, string displayName)[] Archetypes =
      new (string, string)[]
      {
        (ArrowsongMinstrelArchetype, ArrowsongMinstrel.ArchetypeDisplayName),
      };

    //** Feats **//
    internal const string FuriousFocusFeat = "de9a75d3-1289-4098-a0b7-fda465a79576";

    internal const string GloriousHeatFeat = "860e19d3-a2ae-4b53-abd9-9673d635110f";
    internal const string GloriousHeatBuff = "ff8875f9-6ff2-4be6-b969-5b731d37b303";

    internal const string HurtfulFeat = "9474814d-363c-401d-9385-c2ce59fe2e3c";
    internal const string HurtfulAbility = "9ef73549-6706-4d33-abd0-bdfaa3ada6a8";
    internal const string HurtfulBuff = "4fd8ee4d-cf60-4b99-9316-e57a71be80ea";

    internal const string PairedOpportunistsFeat = "41df43af-78bc-477a-a33a-e57d86ba8928";

    internal const string SkaldsVigorFeat = "55dd527b-8721-426b-aaa2-036ccc9a0458";
    internal const string SkaldsVigorGreaterFeat = "ee4756c6-797f-4848-a814-4a27a159641d";
    internal const string SkaldsVigorBuff = "9e67121d-0433-4706-a107-7796187df3e1";
    //***********//

    internal static readonly (string guid, string displayName)[] Feats =
      new (string, string)[]
      {
        (FuriousFocusFeat, FuriousFocus.FeatDisplayName),
        (GloriousHeatFeat, GloriousHeat.FeatDisplayName),
        (HurtfulFeat, Hurtful.FeatDisplayName),
        (PairedOpportunistsFeat, PairedOpportunists.FeatDisplayName),
        (SkaldsVigorFeat, SkaldsVigor.FeatureDisplayName),
      };

    //** Dynamic GUIDs **//
    private const string GUID_0 = "4d802e91-ea0e-41bd-9ebe-1e92263605e8";
    private const string GUID_1 = "6ba9a23f-30af-4e26-9c4a-836cc7431c4f";
    private const string GUID_2 = "3da99391-21de-4c43-b3fa-34fe8dc822bb";
    private const string GUID_3 = "1e4b3c37-9746-48a0-8bdd-4abaf1dd8414";
    private const string GUID_4 = "69ece93c-ac3f-472e-8d50-1b994a1b294f";
    private const string GUID_5 = "00d4365c-dc04-4ca9-a465-5c6e778a8d18";
    private const string GUID_6 = "23534d14-8d85-45f3-bcee-c2c618582983";
    private const string GUID_7 = "187ad65f-5e2e-4c81-aa29-e42f70a4ffc5";
    private const string GUID_8 = "fd85b244-42fc-4d2c-90c9-d438a3797971";
    private const string GUID_9 = "fa0dc9ed-bd43-4f6f-85c1-02f1aaf2ce31";
    private const string GUID_10 = "f08e2782-af70-42ae-a3a8-8d3dc0167687";
    private const string GUID_11 = "50f92e9a-7cef-46b5-88c5-5525bb41df12";
    private const string GUID_12 = "a81e728e-31f5-4e80-9db0-689142d6b310";
    private const string GUID_13 = "89ededba-5096-4f40-bbe3-c9bd1695e5f8";
    private const string GUID_14 = "3553884f-d2f3-4caf-9ed4-68fc147e6315";
    private const string GUID_15 = "1305e710-3836-4fba-a18d-c1c71eb8ff8d";
    private const string GUID_16 = "2e3968fb-9292-4b53-a3b3-1e183699f64d";
    private const string GUID_17 = "76096c62-3f48-4f67-8e0a-51ff57f9f79b";
    private const string GUID_18 = "8de1e388-8a79-409e-922f-822d13b7fff6";
    private const string GUID_19 = "aa816f85-1832-4ba2-82b8-88aa147c941a";
    //*******************//

    private static readonly List<string> GUIDS =
      new()
      {
        GUID_0, GUID_1, GUID_2, GUID_3, GUID_4, GUID_5, GUID_6, GUID_7, GUID_8, GUID_9,
        GUID_10, GUID_11, GUID_12, GUID_13, GUID_14, GUID_15, GUID_16, GUID_17, GUID_18, GUID_19
      };

    /// <summary>
    /// Reserves and returns one of the cached GUIDs used for dynamic blueprint generation.
    /// </summary>
    /// 
    /// <remarks>Will generat new GUIDs if all cached GUIDs are reserved.</remarks>
    internal static string ReserveDynamic()
    {
      string guid;
      if (GUIDS.Any())
      {
        guid = GUIDS[0];
        GUIDS.RemoveAt(0);
        Logger.NativeLog($"Reserving dynamic guid {guid}");
        return guid;
      }
      guid = Guid.NewGuid().ToString();
      Logger.NativeLog($"Generating dynamic guid {guid}");
      return guid;
    }
  }
}