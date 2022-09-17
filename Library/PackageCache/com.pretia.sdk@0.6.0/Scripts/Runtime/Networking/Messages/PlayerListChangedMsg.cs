using System.Collections.Generic;
using MessagePack;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class PlayerListSnapshotMsg
    {
        [Key(0)]
        public List<uint> UserNumbers;
        [Key(1)]
        public List<string> DisplayNames;

        public PlayerListSnapshotMsg(List<uint> userNumbers, List<string> displayNames)
        {
            UserNumbers = new List<uint>(userNumbers.Count);
            foreach (var n in userNumbers)
            {
                UserNumbers.Add(n);
            }

            DisplayNames = new List<string>(displayNames.Count);
            foreach (var s in displayNames)
            {
                DisplayNames.Add(s);
            }
        }

        internal PlayerListSnapshotMsg(IEnumerable<Player> players)
        {
            UserNumbers = new List<uint>();
            DisplayNames = new List<string>();

            foreach (var player in players)
            {
                UserNumbers.Add(player.UserNumber);
                DisplayNames.Add(player.DisplayName);
            }
        }
    }
}