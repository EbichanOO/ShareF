using MessagePack;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class NetworkAnimatorSyncMsg
    {
        [Key(0)]
        public uint NetworkId;
        [Key(1)]
        public byte[] Data;
    }
}