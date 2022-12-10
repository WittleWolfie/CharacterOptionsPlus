using CharacterOptionsPlus.Archetypes;
using CharacterOptionsPlus.ClassFeatures;
using CharacterOptionsPlus.Feats;
using CharacterOptionsPlus.Spells;
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
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(Guids));

    #region Archetypes
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
    

    internal static readonly (string guid, string displayName)[] Archetypes =
      new (string, string)[]
      {
        (ArrowsongMinstrelArchetype, ArrowsongMinstrel.ArchetypeDisplayName),
      };
    #endregion

    #region Class Features
    internal const string IceTombHex = "7c7f44cb-76b9-49a4-a40d-57fee16140ba";
    internal const string IceTombAbility = "595790fd-4b57-49e5-8e99-664dcca67092";
    internal const string IceTombBuff = "66e31ecf-ced9-4527-b985-52ec33e64c12";
    internal const string IceTombCooldown = "00a72a90-dd68-4981-9d99-d0780b4c5a20";

    internal const string ShadowDuplicateTalent = "07219f30-3565-426f-ae53-24d72873a8e7";
    internal const string ShadowDuplicateAbility = "575dff1b-36a9-4d67-8a93-7e7c70ec6d16";
    internal const string ShadowDuplicateResource = "7f948006-fc71-4cfb-8ce1-e34a5f0e3d92";
    internal const string ShadowDuplicateHiddenBuff = "05ab8c51-cfa9-49bc-896a-95d8d5797ded";
    internal const string ShadowDuplicateBuff = "41e68502-7aac-4635-a66e-d0220619c759";

    internal const string SlowingStrikeTalent = "1fda2ef4-8e4f-4698-bca0-6af85d5e5307";
    internal const string SlowingStrikeBuff = "00650b0e-e4e4-478c-a03b-63d03d53a0ad";

    internal static readonly (string guid, string displayName)[] ClassFeatures =
      new (string, string)[]
      {
        (IceTombHex, IceTomb.DisplayName),
        (ShadowDuplicateTalent, ShadowDuplicate.DisplayName),
        (SlowingStrikeTalent, SlowingStrike.DisplayName),
      };
    #endregion

    #region Feats
    internal const string DazingAssaultFeat = "98556744-6715-4654-8323-d0f823003ca1";
    internal const string DazingAssaultToggle = "2a67d113-cc03-42ba-8141-15e359cc9e0f";
    internal const string DazingAssaultBuff = "9da65905-1d5e-41c5-a85b-7e0117d66a08";

    internal const string DivineFightingTechniqueFeat = "96f784ce-7660-40d4-9cf3-29bc289a8be5";

    internal const string EldritchHeritageFeat = "f9d9eb8b-01d9-4799-bb27-f5ad1a7f7e31";
    internal const string ImprovedEldritchHeritageFeat = "553ed7eb-e1f2-4ba2-a3cf-1c66e7c33bcd";
    internal const string GreaterEldritchHeritageFeat = "e29ab08b-0b87-432d-a773-f0d3a39aac5a";
    internal const string EldritchHeritageEffectiveLevel = "64c2f8cd-be15-47d8-bb1b-638d04417f4b";

    internal const string FuriousFocusFeat = "de9a75d3-1289-4098-a0b7-fda465a79576";

    internal const string GloriousHeatFeat = "860e19d3-a2ae-4b53-abd9-9673d635110f";
    internal const string GloriousHeatBuff = "ff8875f9-6ff2-4be6-b969-5b731d37b303";

    internal const string HurtfulFeat = "9474814d-363c-401d-9385-c2ce59fe2e3c";
    internal const string HurtfulAbility = "9ef73549-6706-4d33-abd0-bdfaa3ada6a8";
    internal const string HurtfulBuff = "4fd8ee4d-cf60-4b99-9316-e57a71be80ea";

    internal const string PairedOpportunistsFeat = "41df43af-78bc-477a-a33a-e57d86ba8928";
    internal const string PairedOpportunistsAbility = "37b08883-ccae-477b-bde8-2ed440b96af5";
    internal const string PairedOpportunistsBuff = "4fc40ae3-136e-4850-9491-b2db1ae7d399";
    internal const string PairedOpportunistsCavalier = "9eee9a72-304f-4b99-a59c-492f06d24953";
    internal const string PairedOpportunistsVanguardBuff = "b9ef85c6-9ece-40b6-b829-46a56517fe02";
    internal const string PairedOpportunistsVanguardAbility = "97238ed6-c81f-45c3-91bc-a03bef6507a7";
    internal const string PairedOpportunistsRagerBuff = "eea566b1-625f-4e0a-9e8e-0cf364bd87dd";
    internal const string PairedOpportunistsRagerArea = "47db0206-edd2-4a23-8a79-d7695d37c15d";
    internal const string PairedOpportunistsRagerAreaBuff = "a341739e-4a61-481d-9018-63b654bdfb53";
    internal const string PairedOpportunistsRagerToggleBuff = "1bb0952d-fec5-412f-9495-5b7043548aaf";
    internal const string PairedOpportunistsRagerToggle = "68a15456-1076-408f-bd0f-9205c913f0ed";

    internal const string SignatureSkillFeat = "7bb79f1f-8a90-4cf0-9322-c81961fae21a";

    internal const string SignatureSkillAthletics = "1a436197-9e8d-44df-bf08-33afa9b025cd";
    internal const string SignatureSkillAthleticsBreakFree = "cc7c219a-f684-49a4-8386-be7a390c63d5";
    internal const string SignatureSkillAthleticsSuppressPassiveBuff = "ba838ccd-8bec-44c4-a98d-80aef50f1dae";
    internal const string SignatureSkillAthleticsSuppressPassive = "b6a7878a-ef3e-4a9f-9ede-a688f2358921";
    internal const string SignatureskillAthleticsSuppressActive = "2f375a7f-317c-439d-acc2-8ead9f34e4ac";
    internal const string SignatureSkillAthleticsSuppressParalyzeBuff = "b1e07337-a211-4ba8-af93-767c82e0c957";
    internal const string SignatureSkillAthleticsSuppressSlowBuff = "8ce7f623-57be-49dd-a2db-8d13d8a6825a";

    internal const string SignatureSkillKnowledgeAbility = "3ffba240-5446-4dfa-92b7-cd82de34e6b9";
    internal const string SignatureSkillKnowledgeBuff = "8a1cd964-541b-4c2b-8506-47cb80780b92";
    internal const string SignatureSkillKnowledgeArcana = "d36369ae-3f05-4c00-aee1-5e46f09771d5";
    internal const string SignatureSkillKnowledgeWorld = "c2f152a0-c7e4-4f8b-8634-1af3061cde29";
    internal const string SignatureSkillLoreNature = "e3258da0-d4d4-4150-8049-09b3c726a232";
    internal const string SignatureSkillLoreReligion = "74442dfb-6deb-4700-bac0-2b99b6fe91d6";

    internal const string SignatureSkillMobility = "c48a21a8-8c32-464b-b1a9-b6a39953fc48";
    internal const string SignatureSkillMobilityAbility = "52518d3d-c23d-469a-af03-a9daf990d296";
    internal const string SignatureSkillMobilityAbilityBuff = "44d89421-c539-49c0-ab0d-c69391a52cb8";

    internal const string SignatureSkillPerception = "bfad5d33-ea63-4451-8342-1bc56425a6d4";

    internal const string SignatureSkillPersuasion = "70d0cbae-811f-47aa-87c8-593d2a6d1a9e";

    internal const string SignatureSkillStealth = "8f798047-b6ce-4d7e-bdc7-1f68eeec0d1f";
    internal const string SignatureSkillStealthConcealment = "0bb1836e-b35b-420d-bf98-ecbddf176d73";
    internal const string SignatureSkillStealthConcealmentGreater = "0e4960e4-a427-4176-be9a-ff96eb027093";
    internal const string SignatureSkillStealthSurprise = "3aa58bf8-b3dc-45fb-80f0-e03b9e5a95e6";

    internal const string SkaldsVigorFeat = "55dd527b-8721-426b-aaa2-036ccc9a0458";
    internal const string SkaldsVigorGreaterFeat = "ee4756c6-797f-4848-a814-4a27a159641d";
    internal const string SkaldsVigorBuff = "9e67121d-0433-4706-a107-7796187df3e1";


    internal static readonly (string guid, string displayName)[] Feats =
      new (string, string)[]
      {
        (DazingAssaultFeat, DazingAssault.FeatDisplayName),
        (DivineFightingTechniqueFeat, DivineFightingTechnique.FeatDisplayName),
        (EldritchHeritageFeat, EldritchHeritage.FeatDisplayName),
        (FuriousFocusFeat, FuriousFocus.FeatDisplayName),
        (GloriousHeatFeat, GloriousHeat.FeatDisplayName),
        (HurtfulFeat, Hurtful.FeatDisplayName),
        (PairedOpportunistsFeat, PairedOpportunists.FeatDisplayName),
        (SignatureSkillFeat, SignatureSkill.FeatDisplayName),
        (SkaldsVigorFeat, SkaldsVigor.FeatureDisplayName),
      };
    #endregion

    #region Divine Fighting Technique
    internal const string AbadarTechnique = "5e76b6e8-6acf-49ab-9193-6762b27ae20f";
    internal const string AbadarAdvancedTechnique = "b205b319-71f2-4cdb-9c3a-d9b7b2ada7cf";
    internal const string AbadarTechniqueAbility = "33523593-e0f8-4bb2-a6d9-d598806a96d2";
    internal const string AbadarAdvancedTechniqueAbility = "5d19da09-2d3e-4031-a744-fb3f443129c6";

    internal const string AsmodeusTechnique = "2389fcf7-c0c8-47a1-8b4c-624834afff5d";
    internal const string AsmodeusAdvancedTechnique = "833bfe20-0e00-435f-8cd5-10aeb46c59e0";
    internal const string AsmodeusAdvancedToggle = "e07d5bae-7ec4-4b1b-9a7f-479ae40847fc";
    internal const string AsmodeusBlind = "a1c959a9-8389-4fca-b633-6c14bc863b20";
    internal const string AsmodeusBlindBuff = "2e3f95b1-5edb-4836-a05a-6d7a4c754e83";
    internal const string AsmodeusEntangle = "5071f5d4-46d5-4229-9774-739fef34cf52";
    internal const string AsmodeusEntangleBuff = "1ee58b95-afe9-4e1b-8d72-8b7cce119b22";
    internal const string AsmodeusSicken = "451b7770-6ce9-448a-82d9-45a61271e7a6";
    internal const string AsmodeusSickenBuff = "9c733126-3370-4501-b6a2-fdfc8d0de647";

    internal const string ErastilTechnique = "072292be-e9e1-4364-8f69-3adc57c1e6be";
    internal const string ErastilAdvancedTechnique = "d6d184d9-5d54-4372-8f09-879fb99e0f1d";
    internal const string ErastilAbility = "2cb10b4f-9034-45c8-a914-f46167abc9b1";
    internal const string ErastilDistracted = "6fb9eda4-2369-4d6e-836d-5fc3976e9221";
    internal const string ErastilFocused = "37f95bcf-5759-41cc-8343-6b1f77422565";
    internal const string ErastilFocusedAdvanced = "b8d4b4ef-2882-438f-bdbb-e366bbb6275a";

    internal const string GorumTechnique = "76e56fd3-fb80-41cf-96e8-b7861e334e30";
    internal const string GorumAdvancedTechnique = "ce972a0c-d262-4f41-ae0b-33a7d0c02cc8";
    internal const string GorumTechniqueBuff = "8a3f1314-79c1-4bd1-963c-ecd1a93dea3f";
    internal const string GorumAdvancedTechniqueBuff = "e0c38513-045b-49df-b6cb-a3ae8bf93a43";

    internal const string IomedaeTechnique = "edc643bd-d52b-4073-af42-a3199e864f04";
    internal const string IomedaeAdvancedTechnique = "2f03af39-8c1d-4e23-bfa6-78860328432f";
    internal const string IomedaeTechniqueBuff = "8e8f3b0c-f01b-4c01-8329-c448fd95cfe8";
    internal const string IomedaeInspireAbility = "0ffd236b-f887-4964-914a-4d57250dbe24";
    internal const string IomedaeQuickInspireAbility = "3bd567e3-8f3b-4535-abe1-ba63349f218e";

    internal const string IroriTechnique = "25864b67-1266-4f08-bafd-c584c2eb1b4d";
    internal const string IroriAdvancedTechnique = "0504f175-bef9-4c95-a59c-a2b6d39e3328";
    internal const string IroriTechniqueToggle = "ed32bdeb-e1ab-487c-b4c0-9f7a61f28038";
    internal const string IroriTechniqueBuff = "e6d2330e-fa36-4c0f-bd90-5ac7b0ba919a";

    internal const string LamashtuTechnique = "72337519-24f4-49ee-8f21-76e1fd4c34b4";
    internal const string LamashtuAdvancedTechnique = "9e894abe-64a5-4520-8a08-5ebc810cf51e";
    internal const string LamashtuTechniqueAbility = "8f3930a3-dcf7-45c9-b875-0cd5840f513e";
    internal const string LamashtuTechniqueBuff = "0609e814-53f0-448d-a2b8-f50f36eaf1a8";
    internal const string LamashtuTechniqueImmunityBuff = "10f3d18c-a7c4-4e0a-9e76-5524fa23b4d0";

    internal const string NorgorberTechnique = "6546d666-23ad-457e-a90a-38a176e2df89";
    internal const string NorgorberAdvancedTechnique = "f356e5cf-c180-4191-879e-e792fcf8c374";
    internal const string NorgorberTechniqueToggle = "24d670da-07fc-4a79-bd44-11c8a4728804";
    internal const string NorgorberTechniqueSelfBuff = "7295c09e-a631-4213-87b3-040f55339341";
    internal const string NorgorberTechniqueBuff = "b11516b2-20c7-4139-a71e-b9faf3012ffb";

    internal const string RovagugTechnique = "a8f30595-08ee-42e4-ac54-bef8fd5cc396";
    internal const string RovagugAdvancedTechnique = "7593c79c-9e42-49df-a5df-a0fe36c234d1";

    internal const string ToragTechnique = "71f98f2c-9df1-4627-9714-d8cc426ecb43";
    internal const string ToragAdvancedTechnique = "a27514f6-cd73-4905-af35-ccabb1b676b8";

    internal const string UrgathoaTechnique =         "f2de6a4b-65a6-4351-b9b4-330f74ab2511";
    internal const string UrgathoaAdvancedTechnique = "f118d4e3-4111-4f94-8c2e-6fcc81cc310c";
    internal const string UrgathoaTechniqueToggle =   "183809ac-4480-4959-bbce-d43269fd4e0e";
    internal const string UrgathoaTechniqueBuff =     "40729a22-35b7-47a7-80a9-4b8288365d1f";
    internal const string UrgathoaTechniqueResource = "104e8d24-f47d-492f-b5b0-3a6f336ad35c";
    internal const string UrgathoaTechniqueLifeBuff = "1b39f151-2e24-4a8d-8251-5762c1394026";
    internal const string UrgathoaAdvancedTechniqueToggle =   "9e69d470-2e63-4b1d-90ce-84d943a92385";
    internal const string UrgathoaAdvancedTechniqueBuff =     "228118fa-bd2e-45f4-b71f-abc7126411cc";
    internal const string UrgathoaAdvancedTechniqueResource = "3839fdd0-6410-482e-bcaa-0269d399d8fd";
    #endregion

    #region Eldritch Heritage
    internal const string AberrantHeritage = "1b8ca09f-2c71-4086-9494-4a897b671695";
    internal const string AberrantHeritageRay = "eca24f6b-02ea-483f-b665-6a5083c4e4b4";
    internal const string AberrantHeritageLimbs = "93298d67-0448-4cdf-b32a-412c8ba95a09";
    internal const string AberrantHeritageAnatomy = "ec076951-432e-42da-beac-15c940ff7ccd";
    internal const string AberrantHeritageResistance = "739cc880-42cd-4724-a995-4f6bf9ad2267";

    internal const string AbyssalHeritage = "8f070fcb-bf77-4a22-8a0e-9fdf769dcf6e";
    internal const string AbyssalHeritageResistance = "99bfdfcf-c1d1-47e4-8550-d925e45d1cc3";
    internal const string AbyssalHeritageStrength = "e01ed944-bb6d-46e4-be8d-32acee0da462";
    internal const string AbyssalHeritageSummons = "252d7e8a-09d5-42d6-ab3e-9aa0368cf2cd";

    internal const string ArcaneHeritage = "439add6b-ed93-4759-bba5-d43e8fb6b306";
    internal const string ArcaneHeritageAdept = "8a0b28de-c62e-4d0b-9197-2d11a5e68e8f";
    internal const string ArcaneHeritageFocus = "c2ac8341-2cd8-4968-bcc0-d4f0ed48940b";

    internal const string CelestialHeritage = "786fbf80-9db7-4107-986a-8ce791bf62b6";
    internal const string CelestialHeritageRay = "e72fa9d9-c1df-4c85-b7cc-66a876380e5a";
    internal const string CelestialHeritageResistances = "000fc6b5-1926-4702-9c05-b4df8940930a";
    internal const string CelestialHeritageAura = "a73b418a-00da-462d-8d79-475ee091ab03";
    internal const string CelestialHeritageAuraResource = "9d1256f4-e872-4be2-a588-391814b811eb";
    internal const string CelestialHeritageAuraAbility = "5d1681ec-cdd8-4f04-899c-af0a238254eb";
    internal const string CelestialHeritageConviction = "989e19d2-efdc-436f-a1c7-fb9b5247dbad";

    internal const string DestinedHeritage = "b4f5d946-d21f-448e-b44b-c1747d98c071";
    internal const string DestinedHeritageTouch = "8593c16d-d6a4-4ed5-b7cd-aa6270696197";
    internal const string DestinedHeritageTouchBuff = "bded7cba-fddc-4a28-8a7a-ef75f228fa01";
    internal const string DestinedHeritageFated = "e59c79b0-bc04-4a6d-8d34-5cb93c264ae4";
    internal const string DestinedHeritageFatedBuff = "98b34436-8ced-48c2-8040-16924192dd4b";
    internal const string DestinedHeritageReroll = "8beb5beb-5c51-40cf-8823-416b96fa2737";
    internal const string DestinedHeritageReach = "1728ed97-0145-4f3f-8fa1-05d8fce4b05a";

    #region Draconic Heritage
    // Used to prevent multiple selections
    internal const string DraconicHeritage = "86bfc14d-08bd-4a5b-9d53-521ca22a5d99";
    internal const string DraconicHeritageRequisite = "f20409c5-343e-40a4-9ebe-7189a40e3a63";

    internal const string DraconicBlackHeritage = "cb5eaa9d-9ccc-4b55-bd05-88caffc38139";
    internal const string DraconicBlackHeritageResistance = "4cc57a5d-17c1-4988-be61-32652b002439";
    internal const string DraconicBlackHeritageBreath = "ff8d2050-ff8d-4b57-9f98-6b267bbfb273";
    internal const string DraconicBlackHeritageBreathAbility = "3bec8d09-ffa1-422f-8ed8-7d579ed224ef";
    internal const string DraconicBlackHeritageWings = "07329973-8023-4f8c-a545-648481e25605";

    internal const string DraconicBlueHeritage = "e7f180f6-51c1-4725-8cca-5306a87d7d1e";
    internal const string DraconicBlueHeritageResistance = "978a2c36-55e2-43c8-a568-e526ff06e7c9";
    internal const string DraconicBlueHeritageBreath = "1687e6b5-6840-4c80-a736-210f2bd5ab7f";
    internal const string DraconicBlueHeritageBreathAbility = "dcdad9d3-9437-4c51-868e-aa17e769da46";
    internal const string DraconicBlueHeritageWings = "afcb3080-ef55-4685-a08c-241d5c990c69";

    internal const string DraconicBrassHeritage = "6c3f1ccf-29db-4c8f-960e-131570c745d8";
    internal const string DraconicBrassHeritageResistance = "66c9a304-d741-42ff-ae11-a873acf27f3c";
    internal const string DraconicBrassHeritageBreath = "1858e25c-68d5-4142-8e63-75faefd5ca1f";
    internal const string DraconicBrassHeritageBreathAbility = "796c9ee8-0ccb-4dee-a74c-3073266ca22e";
    internal const string DraconicBrassHeritageWings = "6688b160-5c04-4691-b1bb-506be64f91ff";

    internal const string DraconicBronzeHeritage = "43475fb4-8cf1-4b63-ad69-2695f61d0d63";
    internal const string DraconicBronzeHeritageResistance = "3c8105c1-b09b-454e-a49e-a5bc741994cc";
    internal const string DraconicBronzeHeritageBreath = "291b8055-97f7-4ec6-834c-f51ca7c223d6";
    internal const string DraconicBronzeHeritageBreathAbility = "72adb890-36d7-4067-aec3-af62c54e9978";
    internal const string DraconicBronzeHeritageWings = "b238e257-c6ab-4593-a0ed-da1702a9f3ae";

    internal const string DraconicCopperHeritage = "bf7d8d37-7205-4307-88f0-3a823871bd5b";
    internal const string DraconicCopperHeritageResistance = "430b87d2-8c6a-4c7d-b749-29bf6d4e80ea";
    internal const string DraconicCopperHeritageBreath = "1c9d33de-7fa3-4a48-9a04-d604f071dc4e";
    internal const string DraconicCopperHeritageBreathAbility = "deffdcf2-c4cd-4b2f-8904-c31a6e7b65bd";
    internal const string DraconicCopperHeritageWings = "942eef14-e0b2-48b3-82d3-22005f4df6da";

    internal const string DraconicGoldHeritage = "b1a0bce7-e5e0-48f6-82ae-ed91ffd58805";
    internal const string DraconicGoldHeritageResistance = "54e59cfb-c133-45ce-96cd-775f16f213a9";
    internal const string DraconicGoldHeritageBreath = "999ca134-5c85-4876-88f7-1a0cf91e1a72";
    internal const string DraconicGoldHeritageBreathAbility = "aa7c1ba0-be6b-4530-a7df-939b79f714b3";
    internal const string DraconicGoldHeritageWings = "90dff65b-ff12-4a6c-a796-8643d4d3a315";

    internal const string DraconicGreenHeritage = "f39601f8-fd68-472c-a4a1-c33e5890f8d9";
    internal const string DraconicGreenHeritageResistance = "e78c3df2-9c05-4e7b-b9bc-8aaed51230bf";
    internal const string DraconicGreenHeritageBreath = "eaa860d8-26ae-469e-80a9-fd1b07c549de";
    internal const string DraconicGreenHeritageBreathAbility = "25ef87d3-babb-4818-91b2-038470b66812";
    internal const string DraconicGreenHeritageWings = "095dc84f-e42c-408b-8c19-9b2923953286";

    internal const string DraconicRedHeritage = "8696a6cd-895e-4b62-9b77-d2b87a03fe2c";
    internal const string DraconicRedHeritageResistance = "d8c11f13-0f71-4dd1-934b-09dd558d85c8";
    internal const string DraconicRedHeritageBreath = "d96e2d29-35c0-4f70-aa19-3119d9898ea1";
    internal const string DraconicRedHeritageBreathAbility = "7d4f09b1-bb5e-4f74-a6d3-997b5a239df0";
    internal const string DraconicRedHeritageWings = "c89de55a-437a-43e5-b69d-0d34e5ba32bc";

    internal const string DraconicSilverHeritage = "438949f8-8a35-4404-acc5-dd7b2cac0021";
    internal const string DraconicSilverHeritageResistance = "77791d4b-f473-4351-945f-bffc4e144cea";
    internal const string DraconicSilverHeritageBreath = "0096aed1-49f2-438e-af2b-6c0bf0207fc4";
    internal const string DraconicSilverHeritageBreathAbility = "c64845a7-79a9-4447-8424-e8fc9080b340";
    internal const string DraconicSilverHeritageWings = "2f4f6ee2-df54-43d8-809f-df830f2c54c8";

    internal const string DraconicWhiteHeritage = "1d842b4e-3ef4-48dc-bd8f-1f61d04284dd";
    internal const string DraconicWhiteHeritageResistance = "3597adad-461f-4245-adec-3acbe621464c";
    internal const string DraconicWhiteHeritageBreath = "c45145fc-7907-4c21-bb90-bdd12ef9ea87";
    internal const string DraconicWhiteHeritageBreathAbility = "bb1d342b-b962-4d27-9543-fc9d2c8b9822";
    internal const string DraconicWhiteHeritageWings = "c7c528fc-cc04-473a-ac01-73af8a600105";
    #endregion

    #region Elemental Heritage
    // Used to prevent multiple selections
    internal const string ElementalHeritage = "b77ff128-3a16-4da7-9f91-afd24c9fd6ae";
    internal const string ElementalHeritageRequisite = "9dd9b2a4-f4b4-47db-a1d1-d74901e3f095";

    internal const string ElementalAirHeritage = "10f79138-da20-4202-b09f-941261f7d212";
    internal const string ElementalAirHeritageRay = "6460bd7b-19f1-4356-82c7-137f05c4ee11";
    internal const string ElementalAirHeritageResistance = "ed4bedb5-489e-495d-b1ac-be8222521324";
    internal const string ElementalAirHeritageBlast = "2e6bdc74-2340-4687-a482-9b2ad7a1ffd6";
    internal const string ElementalAirHeritageBlastAbility = "daf339a0-92de-4619-816a-f3dda3cd42ff";
    internal const string ElementalAirHeritageMovement = "a360a135-b122-4346-b98d-f7f1373de739";

    internal const string ElementalEarthHeritage = "4652cd5e-64a3-4dfb-9c32-8cd5d8f2bcd5";
    internal const string ElementalEarthHeritageRay = "7366e6d5-4ae4-4878-823f-8379d840e576";
    internal const string ElementalEarthHeritageResistance = "079f7532-a65e-4fb5-9da7-255be89ef229";
    internal const string ElementalEarthHeritageBlast = "8ed30065-aad9-4b41-93d4-a1fee73734e5";
    internal const string ElementalEarthHeritageBlastAbility = "664a9fe9-de8e-4ec1-869b-ab8d1ccc41d5";
    internal const string ElementalEarthHeritageMovement = "c876b04c-a6dd-45b8-929c-c5145b5129c3";

    internal const string ElementalFireHeritage = "8914f1d1-a0c3-4c4a-a054-e383b2d14ecd";
    internal const string ElementalFireHeritageRay = "f2579bee-1226-406f-8535-3141b5dccc82";
    internal const string ElementalFireHeritageResistance = "b7754b56-45d6-478a-bd6b-df73622973e8";
    internal const string ElementalFireHeritageBlast = "a2f4e224-8c80-4f8c-927b-9cb776683343";
    internal const string ElementalFireHeritageBlastAbility = "80c5069a-bf6b-47e0-a976-ac528eeba0b3";
    internal const string ElementalFireHeritageMovement = "4fdda62d-7f49-4823-9905-45fc7c86c00d";

    internal const string ElementalWaterHeritage = "a61189b1-ed4c-4af1-b07a-071bfa1e5104";
    internal const string ElementalWaterHeritageRay = "9a56963d-aad2-440b-b5dc-3a151921d7d3";
    internal const string ElementalWaterHeritageResistance = "fdeb361d-d430-49aa-aa0d-96688009a316";
    internal const string ElementalWaterHeritageBlast = "07d4f7eb-e903-4ee3-94ac-9cc7230eb1b3";
    internal const string ElementalWaterHeritageBlastAbility = "37b617c8-eee9-4231-b65a-ba8e3a1498ab";
    internal const string ElementalWaterHeritageMovement = "d6fe85fa-148a-4324-8adb-ced6a458010e";
    #endregion

    internal const string FeyHeritage = "cbcb9798-995a-4cf6-b1aa-44f7b0593896";
    internal const string FeyHeritageStride = "5e330bbc-2e93-4d93-a541-5307a4f7950e";
    internal const string FeyHeritageGlance = "48440cbc-7f20-4a14-b3aa-00c040b012a5";
    internal const string FeyHeritageGlanceAbility = "d4f6a90d-e4d1-4161-ad40-c2478f41b57e";
    internal const string FeyHeritageGlanceResource = "bf5fbe6b-45d0-4658-8e95-7331a22e10d0";
    internal const string FeyHeritageMagic = "2de28bc0-71cc-41ab-9b6d-7660ad6ec0de";

    internal const string InfernalHeritage = "a93ad7b3-fad6-42a8-9661-d30a74f04fbf";
    internal const string InfernalHeritageTouch = "4ca71c74-0709-4974-a9b9-196723d3768b";
    internal const string InfernalHeritageResistance = "8bd41448-c754-4ba0-9b19-496b3ae32e44";
    internal const string InfernalHeritageBlast = "804c18e1-d381-45af-997c-5cd974d57f7b";
    internal const string InfernalHeritageBlastAbility = "fb2393f4-1106-487d-8d51-98a51c31f8dd";
    internal const string InfernalHeritageWings = "b09f3fea-12a8-405e-98ec-8f708bee03bc";

    internal const string SerpentineHeritage = "0c9720f7-786f-4ac9-8acd-4cd26e3f9f0e";
    internal const string SerpentineHeritageFriend = "16a89124-e8a5-4f9e-b0d7-0e9bd7e8ce3a";
    internal const string SerpentineHeritageSkin = "bf6a7039-9489-47c1-864c-42ab853f7b1a";
    internal const string SerpentineHeritageSpiders = "f12226e6-cd1f-4bdb-90e9-3bd598bd4d39";

    internal const string UndeadHeritage = "e62b8778-4afd-4d8a-99f8-348f5e4076ba";
    internal const string UndeadHeritageTouch = "d771e3ca-330c-4e84-a0f1-866e5401d1de";
    internal const string UndeadHeritageResistance = "4082741d-e3be-4cb7-a31b-f5c5f8e95c9d";
    internal const string UndeadHeritageBlast = "527ceb3c-2d70-42fd-99d2-c2a1ba01e123";
    internal const string UndeadHeritageBlastAbility = "348a4515-bef2-4aea-8155-e92f9dd5e745";
    internal const string UndeadHeritageIncorporeal = "d0f79e44-5583-4c4d-bdf8-fa5b62b22fe0";
    internal const string UndeadHeritageIncorporealAbility = "fac3a233-d710-4934-8bc0-01dbb315c0bc";
    #endregion

    #region Spells
    internal const string BladedDashSpell =        "4b1d7167-c4dc-4eda-b48b-af4dd5c00473";
    internal const string BladedDashGreaterSpell = "cc5b9eba-b761-42d4-8566-5e5c7a420f18";
    internal const string BladedDashBuff =         "8425b8d0-ecbc-4905-bcba-e3890ed519a5";

    internal const string BurningDisarmSpell = "4445e61d-3dff-4985-9eb5-37169a0e85bc";

    internal const string ConsecrateSpell = "5bc013d5-d014-4502-af6a-e14141d6c35e";
    internal const string ConsecrateAoE =   "ba25ca0e-e0e7-4199-967d-b79cfae32318";
    internal const string ConsecrateBuff =  "ab7079a6-6bff-427e-8257-1f2457c04c14";

    internal const string DesecrateSpell = "ba47baf2-982a-4c4c-82fc-af65dab915af";
    internal const string DesecrateAoE =   "372f94fb-0a45-4813-8e85-46783792ec3d";
    internal const string DesecrateBuff =  "6bf4c34c-8e54-448f-8921-11f93f21aac7";

    internal const string FrozenNoteSpell = "82bbafd5-6525-41bb-bf22-176b6fdeff1b";
    internal const string FrozenNoteBuff = "51f066c1-f2c9-49de-bb58-0c4a65ef22be";
    internal const string FrozenNoteArea = "96d0ccaf-3730-4a58-8d92-50f81dab92fb";

    internal const string HedgingWeaponsSpell = "9d39ffeb-6d18-4f0b-8ec2-2a7272539d85";
    internal const string HedgingWeaponsThrow = "ed1fdeb5-033e-400b-8ce9-d127ce218c9d";
    internal const string HedgingWeaponsBuff = "baf45d67-c5e8-4e4f-9f22-75dc08303b72";

    internal const string HorrificDoublesSpell = "79b7eb84-9dfd-4184-9d0d-490c370d7fc3";
    internal const string HorrificDoublesBuff = "cd85fc3c-2fea-4da7-b02e-418227a6d016";
    internal const string HorrificDoublesShaken = "f0d27bcd-d791-405f-ad9e-d252c3c0abed";
    internal const string HorrificDoublesShakenImmunity = "30331609-c2b9-40c6-b0ba-a48129b34793";

    internal const string IceSlickSpell =   "ff0cd827-3f35-4ed7-a811-18cd716124ed";
    internal const string IceSlickAoE =     "b94a289c-49c2-4754-83a5-e171ad39fba6";
    internal const string IceSlickAoEBuff = "e3dcd204-3216-4ed1-9c49-708106e8191c";

    internal const string MortalTerrorSpell = "a33f2bff-0db5-471f-b2ae-3d78d35ecbc3";
    internal const string MortalTerrorBuff =  "2ea610bf-ff22-4bec-9cb3-cc87702c4c40";

    internal const string ShadowTrapSpell =     "a3a7e60e-7866-46c4-99b8-ad685e625c06";
    internal const string ShadowTrapBuff =      "09062a14-4a01-41dc-8408-abb0cd38da69";
    internal const string ShadowTrapDelayBuff = "46f235b2-95b1-425e-ae14-8dd54173b039";

    internal const string StrickenHeartSpell =  "8e4e9469-8fa1-42a9-8cfc-44ad05ff8c91";
    internal const string StrickenHeartEffect = "99b047cc-658a-43d1-9b8b-7a922ccdaa1d";

    internal const string TouchOfBlindnessSpell =  "6177af1b-a096-4f58-a0a0-c02778e95483";
    internal const string TouchOfBlindnessEffect = "a8bb445d-74ce-4075-b4f0-875d1b73715b";

    internal const string WrathSpell = "c31b0784-8523-43ce-a5f5-cb3a6121c43a";
    internal const string WrathBuff = "f47ab8be-368a-44a5-8448-fb1b8d756130";
    internal const string WrathSelfBuff = "b4d4b58c-0147-4a93-bb45-2eae439e5787";

    internal static readonly (string guid, string displayName)[] Spells =
      new (string, string)[]
      {
        (BladedDashSpell, BladedDash.DisplayName),
        (BurningDisarmSpell, BurningDisarm.DisplayName),
        (ConsecrateSpell, Consecrate.DisplayName),
        (DesecrateSpell, Desecrate.DisplayName),
        (FrozenNoteSpell, FrozenNote.DisplayName),
        (HedgingWeaponsSpell, HedgingWeapons.DisplayName),
        (HorrificDoublesSpell, HorrificDoubles.DisplayName),
        (IceSlickSpell, IceSlick.DisplayName),
        (MortalTerrorSpell, MortalTerror.DisplayName),
        (ShadowTrapSpell, ShadowTrap.DisplayName),
        (StrickenHeartSpell, StrickenHeart.DisplayName),
        (TouchOfBlindnessSpell, TouchOfBlindness.DisplayName),
        (WrathSpell, Wrath.DisplayName),
      };
    #endregion

    #region TTT

    internal const string AberrantBloodline = "1ff2d53c-2bee-429a-801e-eebb1197bcd1";
    internal const string AberrantBloodragerBloodline = "540390e4-7356-4601-9137-625a37910df0";
    internal const string AberrantCrossbloodedBloodline = "d27567f7-5d04-43b4-bad9-b1db9278fea0";
    internal const string AberrantSeekerBloodline = "65128d5d-9d27-4c7a-9a1d-7bbefb021c96";
    internal const string AberrantBloodlineRequisiteFeature = "2ded002a-790f-4727-8b20-ad73b9c0adf0";

    internal const string AberrantAcidicRay = "d3726f4b-599d-4c4d-b5a8-2abd961097d5";
    internal const string AberrantAcidicRayAbility = "89ae4738-2cdd-4b95-ad03-76ca74e6bf71";
    internal const string AberrantAcidicRayResource = "30fad156-6224-4062-8c38-2fb178e1f611";
    internal const string AberrantLongLimbs = "e9bc0888-007e-48be-9281-cf75f4390fea";
    internal const string AberrantUnusualAnatomy = "5df18db4-9b61-4148-af96-4fb9e9d5a1c6";
    internal const string AberrantAlienResistance = "ba8c2c55-f005-4e35-b0de-1ff47d180b43";

    internal const string DestinedBloodline = "62f017f0-7222-4574-89ee-d83165e25682";
    internal const string DestinedBloodragerBloodline = "7076fd5a-ffd1-4207-9ac6-c9ff4882c16e";
    internal const string DestinedCrossbloodedBloodline = "388c5979-4add-45ac-9314-2656fb2e429b";
    internal const string DestinedSeekerBloodline = "1a62c90a-bfa8-426f-8f6c-7962cb162447";
    internal const string DestinedBloodlineRequisiteFeature = "facf9101-fd98-4ad0-8384-6b159be802cd";

    internal const string DestinedTouchOfDestiny = "2363df02-d1c2-4cbd-96cb-13b6200a6c37";
    internal const string DestinedTouchOfDestinyAbility = "f9772da9-d0fc-43b4-9215-c5dc81a8f4bd";
    internal const string DestinedTouchOfDestinyBuff = "b9772da9-d0fc-43b4-6545-c5dc81c8f4bd";
    internal const string DestinedTouchOfDestinyResource = "d871c8ea-dddb-472d-a2de-977c35cecede";
    internal const string DestinedFated = "830f610e-df82-49a8-a68b-b224536e3efe";
    internal const string DestinedFatedBuff = "33e1f401-09a4-4044-b47d-c3fe557f3985";
    internal const string DestinedItWasMeantToBe = "2ca5b40b-9d06-45de-a55c-baa29c301d0f";
    internal const string DestinedItWasMeantToBeResourceIncrease = "95e4d84e-67da-4f4c-b6da-265c720518a7";
    internal const string DestinedWithinReach = "9ccea48d-deba-4b14-b30d-f3bbd382f843";

    #endregion

    #region Common Blueprints
    internal const string BlankProjectile = "5172c4d9-a5a1-4fea-8600-fc5619d9f1de";

    internal const string PanickedBuff = "22c82942-931a-4ca8-955d-78ce86944218";
    #endregion

    #region Dynamic GUIDs
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
    #endregion
  }
}