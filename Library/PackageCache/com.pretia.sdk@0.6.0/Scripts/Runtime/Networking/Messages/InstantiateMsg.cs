using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [NetworkMessage]
    [MessagePackObject]
    public class InstantiateMsg
    {
        [Key(0)]
        public uint NetworkId;
        [Key(1)]
        public ushort PrefabId;

        [Key(2)]
        public Vector3Serializable Position;

        [Key(3)]
        public QuaternionSerializable Rotation;

        [Key(4)]
        public uint Parent;

        [Key(5)]
        public bool Active;

        [Key(6)]
        public uint Sender;

        [Key(7)]
        public uint Owner;

        [Key(8)]
        public bool ForSnapshot;
    }
}