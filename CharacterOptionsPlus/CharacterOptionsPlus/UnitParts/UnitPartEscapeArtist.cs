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

    // TODO: Keep in mind, when suppresing conditions they are counted. So the suppress
    // needs to reverse itself (only if the buff is still active tho)
    // e.g. Buff: +1 Slowed, Suppress: -1 Slowed
    // When it ends, Buff: -1 Slowed, so Suppress: +1 Slowed
    [JsonProperty]
    public List<Buff> SuppressBuffs = new();
  }
}
