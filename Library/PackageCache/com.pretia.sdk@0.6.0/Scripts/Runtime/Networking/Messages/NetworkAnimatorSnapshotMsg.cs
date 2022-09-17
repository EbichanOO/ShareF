using MessagePack;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class NetworkAnimatorSnapshotMsg
    {
        [Key(0)]
        public uint NetworkId;
        [Key(1)]
        public int[] FullPathHash;
        [Key(2)]
        public float[] NormalizedTime;
    }
}