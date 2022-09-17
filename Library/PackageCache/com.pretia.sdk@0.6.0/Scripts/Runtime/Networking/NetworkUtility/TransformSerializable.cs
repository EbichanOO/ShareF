using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [System.Serializable]
    [MessagePackObject]
    public struct TransformSerializable
    {
        [Key(0)]
        public Vector3Serializable LocalPosition;
        [Key(1)]
        public Vector3Serializable LocalEulerAngles;
        [Key(2)]
        public Vector3Serializable LocalScale;

        public TransformSerializable(Transform transform)
        {
            LocalPosition = transform.localPosition.AsSerializable();
            LocalEulerAngles = transform.localEulerAngles.AsSerializable();
            LocalScale = transform.localScale.AsSerializable();
        }

        public TransformSerializable(Vector3Serializable localPosition, Vector3Serializable localEulerAngles, Vector3Serializable localScale)
        {
            LocalPosition = localPosition;
            LocalEulerAngles = localEulerAngles;
            LocalScale = localScale;
        }
    }
}