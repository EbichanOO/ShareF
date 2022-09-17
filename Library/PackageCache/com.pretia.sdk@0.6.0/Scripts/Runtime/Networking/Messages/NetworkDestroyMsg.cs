using MessagePack;

namespace PretiaArCloud.Networking
{
    [System.Serializable]
    [NetworkMessage]
    [MessagePackObject]
    public class NetworkDestroyMsg
    {
        [Key(0)]
        public uint NetworkId;
        [Key(1)]
        public bool IsExistInScene;
    }
}