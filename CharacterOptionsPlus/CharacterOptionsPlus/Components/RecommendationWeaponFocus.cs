using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowMultipleComponents]
  [AllowedOn(typeof(BlueprintAbility))]
  [AllowedOn(typeof(BlueprintFeature))]
  [TypeId("d8da02d0-1483-49e6-b816-4767d14bf3aa")]
  internal class RecommendationWeaponFocus : LevelUpRecommendationComponent
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(RecommendationWeaponFocus));

    private static BlueprintFeature _weaponFocus;
    private static BlueprintFeature WeaponFocus
    {
      get
      {
        _weaponFocus ??= ParametrizedFeatureRefs.WeaponFocus.Reference.Get();
        return _weaponFocus;
      }
    }

    private readonly List<WeaponCategory> Categories = new();

    internal RecommendationWeaponFocus(params WeaponCategory[] categories)
    {
      Categories.AddRange(categories);
    }

    public override RecommendationPriority GetPriority([CanBeNull] LevelUpState levelUpState)
    {
      try
      {
        if (levelUpState is null)
          return RecommendationPriority.Same;

        foreach (var feature in levelUpState.Unit.Progression.Features)
        {
          if (feature.Blueprint == WeaponFocus && Categories.Contains(feature.Param.Value.WeaponCategory.Value))
            return RecommendationPriority.Good;
        }
      }
      catch (Exception e)
      {
        Logger.LogException("AddRecommendationWeaponFocus.GetPriority", e);
      }
      return RecommendationPriority.Same;
    }
  }
}
