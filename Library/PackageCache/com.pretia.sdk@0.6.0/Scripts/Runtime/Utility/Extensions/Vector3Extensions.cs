using UnityEngine;

namespace PretiaArCloud.Networking
{
    public static class Vector3Extensions
    {
        public static Vector3Serializable AsSerializable(this Vector3 v)
        {
            return new Vector3Serializable(v);
        }
    }
}