﻿using CharacterOptionsPlus.Archetypes;
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
    internal const string ArrowsongMinstrelArcaneArchery = "23d3d217-93cc-4ae6-a034-860c89561253";
    internal const string ArrowsongMinstrelSpellbook = "b9e9804e-5361-43e7-87c2-ffb7b1195be5";
    internal const string ArrowsongMinstrelSpellList = "74f9286f-6e8e-46ba-8de8-995759759b2f";
    internal const string ArrowsongMinstrelSpellsPerDay = "32e41814-4688-42b6-87a7-4384f602a621";
    internal const string ArrowsongMinstrelSpellSelection = "3c9b1fb1-9466-45db-9fe8-08500b66ea50";
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
    //*******************//

    private static readonly List<string> GUIDS =
      new() { GUID_0, GUID_1, GUID_2, GUID_3, GUID_4, GUID_5, GUID_6, GUID_7, GUID_8, GUID_9 };

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