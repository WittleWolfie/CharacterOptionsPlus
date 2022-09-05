

using static UnityModManagerNet.UnityModManager.ModEntry;

namespace CharacterOptionsPlus.Util
{
  internal static class Logging
  {
    private const string BaseChannel = "COP";

    internal static ModLogger GetLogger(string channel)
    {
      return new($"{BaseChannel}+{channel}");
    }
  }
}
