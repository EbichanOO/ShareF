using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [System.Serializable]
    [MessagePackObject]
    public class QuaternionSerializable
    {
        [Key(0)]
        public float X;
        [Key(1)]
        public float Y;
        [Key(2)]
        public float Z;
        [Key(3)]
        public float W;

        public Quaternion AsQuaternion() => new Quaternion(X, Y, Z, W);

        public QuaternionSerializable(Quaternion rotation)
        {
            X = rotation.x;
            Y = rotation.y;
            Z = rotation.z;
            W = rotation.w;
        }

        public QuaternionSerializable(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}