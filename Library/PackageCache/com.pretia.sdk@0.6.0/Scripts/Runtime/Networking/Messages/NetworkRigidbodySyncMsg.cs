using MessagePack;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class NetworkRigidbodySyncMsg
    {
        [Key(0)]
        public uint NetworkId;
        [Key(1)]
        public TransformSerializable Transform;
    }
}