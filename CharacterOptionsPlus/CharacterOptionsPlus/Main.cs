using BlueprintCore.Blueprints.Configurators.Root;
using BlueprintCore.Utils;
using CharacterOptionsPlus.Archetypes;
using CharacterOptionsPlus.ClassFeatures;
using CharacterOptionsPlus.Feats;
using CharacterOptionsPlus.Spells;
using CharacterOptionsPlus.Util;
using HarmonyLib;
using Kingmaker.PubSubSystem;
using System;
using TabletopTweaks.Core.NewEvents;
using UnityModManagerNet;

namespace CharacterOptionsPlus
{
  public static class Main
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(Main));

    public static bool Load(UnityModManager.ModEntry modEntry)
    {
      try
      {
        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll();

        EventBus.Subscribe(new BlueprintCacheInitHandler());

        Logger.Log("Finished patching.");
      }
      catch (Exception e)
      {
        Logger.LogException("Failed to patch", e);
      }
      return true;
    }

    class BlueprintCacheInitHandler : IBlueprintCacheInitHandler
    {
      private static bool Initialized = false;
      private static bool InitializeDelayed = false;

      public void AfterBlueprintCachePatches()
      {
        try
        {
          if (InitializeDelayed)
          {
            Logger.Log("Already initialized blueprints cache.");
            return;
          }
          InitializeDelayed = true;

          ConfigureFeatsDelayed();

          RootConfigurator.ConfigureDelayedBlueprints();

          ConfigureArchetypesDelayed();
        }
        catch (Exception e)
        {
          Logger.LogException("Delayed blueprint configuration failed.", e);
        }
      }

      public void BeforeBlueprintCacheInit() { }

      public void BeforeBlueprintCachePatches() { }

      public void AfterBlueprintCacheInit()
      {
        try
        {
          if (Initialized)
          {
            Logger.Log("Already initialized blueprints cache.");
            return;
          }
          Initialized = true;

          // First strings
          LocalizationTool.LoadEmbeddedLocalizationPacks(
            "CharacterOptionsPlus.Strings.Archetypes.json",
            "CharacterOptionsPlus.Strings.BugFixes.json",
            "CharacterOptionsPlus.Strings.ClassFeatures.json",
            "CharacterOptionsPlus.Strings.Components.json",
            "CharacterOptionsPlus.Strings.Feats.json",
            "CharacterOptionsPlus.Strings.Homebrew.json",
            "CharacterOptionsPlus.Strings.Settings.json",
            "CharacterOptionsPlus.Strings.Spells.json");

          // Then settings
          Settings.Init();

          CommonBlueprints.Configure();
          BugFixes.Configure();
          Homebrew.Configure();

          ConfigureArchetypes();
          ConfigureClassFeatures();
          ConfigureFeats();
          ConfigureSpells();
        }
        catch (Exception e)
        {
          Logger.LogException("Failed to initialize.", e);
        }
      }

      private static void ConfigureArchetypes()
      {
        Logger.Log("Configuring archetypes.");

        ArrowsongMinstrel.Configure();
        WinterWitch.Configure();
      }

      private static void ConfigureArchetypesDelayed()
      {
        Logger.Log($"Configuring Archetypes delayed.");

        WinterWitch.ConfigureDelayed();
      }

      private static void ConfigureClassFeatures()
      {
        Logger.Log("Configuring class features.");

        ArmoredMarauder.Configure();
        IceTomb.Configure();
        ShadowDuplicate.Configure();
        ShimmeringMirage.Configure();
        SlowingStrike.Configure();
        Suffocate.Configure();
      }

      private static void ConfigureFeats()
      {
        Logger.Log("Configuring feats.");

        DazingAssault.Configure();
        DivineFightingTechnique.Configure();
        EldritchHeritage.Configure();
        EnergyChannel.Configure();
        FuriousFocus.Configure();
        GloriousHeat.Configure();
        Hurtful.Configure();
        PairedOpportunists.Configure();
        PurifyingChannel.Configure();
        SignatureSkill.Configure();
        SkaldsVigor.Configure();
      }

      private static void ConfigureFeatsDelayed()
      {
        Logger.Log($"Configuring feats delayed.");

        EldritchHeritage.ConfigureDelayed();
      }

      private static void ConfigureSpells()
      {
        Logger.Log("Configuring spells.");

        BladedDash.Configure();
        BurningDisarm.Configure();
        Consecrate.Configure();
        Desecrate.Configure();
        DimensionalBlade.Configure();
        FleshwormInfestation.Configure();
        FreezingSphere.Configure();
        Frostbite.Configure();
        FrostFall.Configure();
        FrozenNote.Configure();
        HedgingWeapons.Configure();
        HorrificDoubles.Configure();
        IceSlick.Configure();
        IceSpears.Configure();
        Implosion.Configure();
        InvisibilityPurge.Configure();
        JudgementLight.Configure();
        KeenEdge.Configure();
        MortalTerror.Configure();
        NineLives.Configure();
        ScreamingFlames.Configure();
        ShadowTrap.Configure();
        StrickenHeart.Configure();
        TouchOfBlindness.Configure();
        UnshakableZeal.Configure();
        WeaponOfAwe.Configure();
        Wrath.Configure();
      }
    }
  }
}
