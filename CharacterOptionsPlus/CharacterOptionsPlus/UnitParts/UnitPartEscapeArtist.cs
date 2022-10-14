using CharacterOptionsPlus.Util;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs;
using Newtonsoft.Json;
using System.Collections.Generic;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.UnitParts
{
  internal class UnitPartEscapeArtist : UnitPart
  {
    private static readonly ModLogger Logger = Logging.GetLogger(nameof(UnitPartEscapeArtist));

    [JsonProperty]
    public List<Buff> BreakFreeBuffs = new();

    [JsonProperty]
    public List<Buff> SuppressBuffs = new();
  }
}
