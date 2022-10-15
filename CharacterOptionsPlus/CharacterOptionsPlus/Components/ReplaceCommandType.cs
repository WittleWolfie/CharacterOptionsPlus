using CharacterOptionsPlus.Util;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Components
{
  public interface ICalculateCommandType
  {
    CommandType Calculate(UnitEntityData unit); 
  }

  [TypeId("02a70e00-915c-4843-8851-b16517b67603")]
  internal class ReplaceCommandType : UnitFactComponentDelegate, IOwnerGainLevelHandler
  {
    private static readonly ModLogger Logger = Logging.GetLogger("ReplaceCommandType");

    private readonly ICalculateCommandType Calculation;
    private readonly BlueprintAbilityReference Ability;

    /// <summary>
    /// Calculation is done at level up so dynamic conditions won't work.
    /// </summary>
    public ReplaceCommandType(ICalculateCommandType calculation, BlueprintAbilityReference ability)
    {
      Calculation = calculation;
      Ability = ability;
    }

    public override void OnTurnOn()
    {
      Apply();
    }

    public override void OnTurnOff()
    {
      Remove();
    }

    public void HandleUnitGainLevel()
    {
      Apply();
    }

    private void Apply()
    {
      try
      {
        Remove();

        var commandType = Calculation.Calculate(Owner);
        Logger.Log($"Setting action cost for {Ability.Get().Name} to {commandType} for {Owner.CharacterName}");
        Owner.Ensure<UnitPartAbilityModifiers>().AddEntry(new(Fact, commandType, Ability));
      }
      catch (Exception e)
      {
        Logger.LogException("ReplaceCommandType.Apply", e);
      }
    }

    private void Remove()
    {
      try
      {
        Owner.Ensure<UnitPartAbilityModifiers>().RemoveEntry(Fact);
      }
      catch (Exception e)
      {
        Logger.LogException("ReplaceCommandType.Remove", e);
      }
    }
  }
}
