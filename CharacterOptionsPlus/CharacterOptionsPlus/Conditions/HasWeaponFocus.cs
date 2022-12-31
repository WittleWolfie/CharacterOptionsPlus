using BlueprintCore.Blueprints.References;
using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Conditions
{
  [TypeId("19bb9236-efc6-45d0-b793-eea016fc5dc6")]
  internal class HasWeaponFocus : ContextCondition
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(HasWeaponFocus));

    private static BlueprintFeature _weaponFocus;
    private static BlueprintFeature WeaponFocus
    {
      get
      {
        _weaponFocus ??= ParametrizedFeatureRefs.WeaponFocus.Reference.Get();
        return _weaponFocus;
      }
    }

    internal WeaponCategory Category;

    public override bool CheckCondition()
    {
      try
      {
        foreach (var feature in Target.Unit.Progression.Features)
        {
          if (feature.Blueprint == WeaponFocus && Category.Equals(feature.Param.Value.WeaponCategory.Value))
            return true;
        }
      }
      catch (Exception e)
      {
        Logger.LogException("HasWeaponFocus.CheckCondition", e);
      }
      return false;
    }

    public override string GetConditionCaption()
    {
      return "Checks for a specific Weapon Focus";
    }
  }
}
