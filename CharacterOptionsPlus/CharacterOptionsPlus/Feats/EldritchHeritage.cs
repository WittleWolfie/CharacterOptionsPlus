using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.Configurators.Classes.Selection;
using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.BasicEx;
using BlueprintCore.Utils.Types;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using System;
using System.Linq;
using TabletopTweaks.Core.NewComponents.Prerequisites;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Feats
{
  public class EldritchHeritage
  {
    internal const string FeatName = "EldritchHeritage";

    internal const string FeatDisplayName = "EldritchHeritage.Name";
    private const string FeatDescription = "EldritchHeritage.Description";

    private const string AbyssalHeritageName = "EldrichHeritage.Abyssal";
    private const string AbyssalHeritageClaws = "EldritchHeritage.Abyssal.Claws";
    private const string AbyssalHeritageClawsBuff = "EldritchHeritage.Abyssal.Claws.Buff";
    private const string AbyssalHeritageClawsResource = "EldrtichHeritrage.Abyssal.Claws.Resource";

    private const string IconPrefix = "assets/icons/";
    private const string Icon = IconPrefix + "eldritchheritage.png";

    private static readonly ModLogger Logger = Logging.GetLogger(FeatName);

    internal static void Configure()
    {
      try
      {
        if (Settings.IsEnabled(Guids.EldritchHeritageFeat))
          ConfigureEnabled();
        else
          ConfigureDisabled();
      }
      catch (Exception e)
      {
        Logger.LogException("EldritchHeritage.Configure", e);
      }
    }

    private static void ConfigureDisabled()
    {
      Logger.Log($"Configuring {FeatName} (disabled)");

      ParametrizedFeatureConfigurator.New(FeatName, Guids.EldritchHeritageFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(Icon)
        .Configure();
    }

    private static void ConfigureEnabled()
    {
      Logger.Log($"Configuring {FeatName}");

      FeatureSelectionConfigurator.New(FeatName, Guids.EldritchHeritageFeat)
        .SetDisplayName(FeatDisplayName)
        .SetDescription(FeatDescription)
        .SetIcon(Icon)
        .SetIsClassFeature()
        .AddFeatureTagsComponent(featureTags: FeatureTag.Magic)
        .AddPrerequisiteStatValue(StatType.Charisma, 13)
        .AddPrerequisiteCharacterLevel(3)
        .AddToAllFeatures(ConfigureAbyssalHeritage())
        .Configure();

      // Since feature selection logic is only in FeatureConfigurator, do this instead of trying to do in parametrized
      // configurator.
      FeatureConfigurator.For(FeatName).AddToGroups(FeatureGroup.Feat).Configure(delayed: true);
    }

    private static BlueprintFeature ConfigureAbyssalHeritage()
    {
      var abyssalClawsBuff = BuffRefs.BloodlineAbyssalClawsBuffLevel1.Reference.Get();
      var buff = BuffConfigurator.New(AbyssalHeritageClawsBuff, Guids.AbyssalHeritageClawsBuff)
        .SetFlags(BlueprintBuff.Flags.HiddenInUi)
        .AddComponent<AbyssalHeritageClawsComponent>()
        .Configure();

      // TODO: Resources!

      var abyssalClaws = ActivatableAbilityRefs.BloodlineAbyssalClawsAbililyLevel1.Reference.Get();
      var ability = ActivatableAbilityConfigurator.New(AbyssalHeritageClaws, Guids.AbyssalHeritageClawsAbility)
        .SetDisplayName(abyssalClaws.m_DisplayName)
        .SetDescription(abyssalClaws.m_Description)
        .SetIcon(abyssalClaws.m_Icon)
        .SetDeactivateIfCombatEnded()
        .SetDeactivateIfOwnerDisabled()
        .SetDeactivateImmediately()
        .SetActivationType(AbilityActivationType.WithUnitCommand)
        .AddActivatableAbilityResourceLogic(null)
        .SetBuff(buff)
        .Configure();

      var abyssalBloodline = ProgressionRefs.BloodlineAbyssalProgression.Reference.Get();
      return FeatureConfigurator.New(AbyssalHeritageName, Guids.AbyssalHeritage)
        .SetDisplayName(abyssalBloodline.m_DisplayName)
        .SetDescription(abyssalClaws.m_Description)
        .SetIcon(abyssalBloodline.m_Icon)
        .SetIsClassFeature()
        .AddPrerequisiteFeature(FeatureRefs.SkillFocusPhysique.ToString())
        .AddPrerequisiteNoFeature(FeatureRefs.AbyssalBloodlineRequisiteFeature.ToString())
        .AddFacts(new() { ability })
        .Configure();
    }

    [TypeId("83224ddf-2f92-48c3-bf2d-9a8d26a5432e")]
    private class AbyssalHeritageClawsComponent : UnitBuffComponentDelegate
    {
      public override void OnActivate()
      {
        var characterLevel = Owner.Descriptor.Progression.CharacterLevel;
        Buff buff;
        if (characterLevel < 7)
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel1.Reference.Get(), Context);
        else if (characterLevel < 9)
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel2.Reference.Get(), Context);
        else
          buff = Owner.AddBuff(BuffRefs.BloodlineAbyssalClawsBuffLevel4.Reference.Get(), Context);

        // Links the buff to this one so they get removed at the same time
        Buff.StoreFact(buff);
      }
    }
  }
}
