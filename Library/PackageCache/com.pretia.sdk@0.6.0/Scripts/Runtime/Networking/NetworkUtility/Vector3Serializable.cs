using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [System.Serializable]
    [MessagePackObject]
    public struct Vector3Serializable
    {
        [Key(0)]
        public float X;
        [Key(1)]
        public float Y;
        [Key(2)]
        public float Z;

        public Vector3 AsVector3() => new Vector3(X, Y, Z);

        public Vector3Serializable(Vector3 vector)
        {
            X = vector.x;
            Y = vector.y;
            Z = vector.z;
        }

        public Vector3Serializable(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}