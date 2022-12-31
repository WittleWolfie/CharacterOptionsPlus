using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Mechanics;
using System;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  [AllowedOn(typeof(BlueprintBuff))]
  [TypeId("cb1e92e7-0a39-4b5a-841b-bc6a723d67f2")]
  internal class SpellPenBonusAgainstTarget :
    UnitBuffComponentDelegate, ITargetRulebookHandler<RuleSpellResistanceCheck>
  {
    private static readonly Logging.Logger Logger = Logging.GetLogger(nameof(SpellPenBonusAgainstTarget));

    private readonly ContextValue Bonus;

    internal SpellPenBonusAgainstTarget(ContextValue bonus)
    {
      Bonus = bonus;
    }

    public void OnEventAboutToTrigger(RuleSpellResistanceCheck evt)
    {
      try
      {
        if (evt.Initiator != Context.MaybeCaster)
          return;

        var bonus = Bonus.Calculate(Context);
        evt.AddSpellPenetration(bonus);
      }
      catch (Exception e)
      {
        Logger.LogException("SpellPenBonusAgainstTarget", e);
      }
    }

    public void OnEventDidTrigger(RuleSpellResistanceCheck evt) { }
  }
}
