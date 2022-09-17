using MessagePack;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class PlayerJoinedMsg
    {
        [Key(0)]
        public uint UserNumber;
    }
}