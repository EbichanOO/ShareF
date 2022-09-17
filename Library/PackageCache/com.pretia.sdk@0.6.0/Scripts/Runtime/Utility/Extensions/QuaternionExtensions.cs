using UnityEngine;

namespace PretiaArCloud.Networking
{
    public static class QuaternionExtensions
    {
        public static QuaternionSerializable AsSerializable(this Quaternion q)
        {
            return new QuaternionSerializable(q);
        }
    }
}