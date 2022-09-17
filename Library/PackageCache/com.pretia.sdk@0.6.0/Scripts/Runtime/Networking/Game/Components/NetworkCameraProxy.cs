using System.Buffers;
using MessagePack;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PretiaArCloud.Networking
{
    public class NetworkCameraProxy : NetworkBehaviour
    {
        private Transform _networkCameraTransform;
        private float[] _cachedValues;

        protected override bool ClientAuthority => true;

        private void Awake()
        {
            _cachedValues = new float[6];
            _cachedValues[0] = transform.localPosition.x;
            _cachedValues[1] = transform.localPosition.y;
            _cachedValues[2] = transform.localPosition.z;

            _cachedValues[3] = transform.localEulerAngles.x;
            _cachedValues[4] = transform.localEulerAngles.y;
            _cachedValues[5] = transform.localEulerAngles.z;
        }

        public void SetCameraTransform(Transform cameraTransform)
        {
            _networkCameraTransform = cameraTransform;
        }

        private void FixedUpdate()
        {
            if (isOwned)
            {
                transform.localPosition = _networkCameraTransform.position;
                transform.localEulerAngles = _networkCameraTransform.eulerAngles;
            }
        }

        public override void Serialize(ref MessagePackWriter writer, MessagePackSerializerOptions options)
        {
            Vector3 position = transform.localPosition;
            Vector3 eulerAngles = transform.localEulerAngles;

            float[] values = ArrayPool<float>.Shared.Rent(9);
            values[0] = position.x;
            values[1] = position.y;
            values[2] = position.z;

            values[3] = eulerAngles.x;
            values[4] = eulerAngles.y;
            values[5] = eulerAngles.z;

            for (int i = 0; i < _cachedValues.Length; i++)
            {
                ulong currentMask = 1UL << i;
                if (_cachedValues[i] != values[i])
                {
                    _dirtyMask |= currentMask;
                    writer.Write(values[i]);
                    _cachedValues[i] = values[i];
                }
            }

            ArrayPool<float>.Shared.Return(values);
        }

        public override void Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var position = transform.localPosition;
            var rotation = transform.localEulerAngles;

            bool hasNewPosition = false;
            bool hasNewRotation = false;

            #region Sync Position
            if ((_dirtyMask & (1 << 0)) != 0)
            {
                position.x = reader.ReadSingle();
                hasNewPosition = true;
            }

            if ((_dirtyMask & (1 << 1)) != 0)
            {
                position.y = reader.ReadSingle();
                hasNewPosition = true;
            }

            if ((_dirtyMask & (1 << 2)) != 0)
            {
                position.z = reader.ReadSingle();
                hasNewPosition = true;
            }

            if (hasNewPosition)
            {
                transform.localPosition = position;
            }
            #endregion

            #region Sync Rotation
            if ((_dirtyMask & (1 << 3)) != 0)
            {
                rotation.x = reader.ReadSingle();
                hasNewRotation = true;
            }

            if ((_dirtyMask & (1 << 4)) != 0)
            {
                rotation.y = reader.ReadSingle();
                hasNewRotation = true;
            }

            if ((_dirtyMask & (1 << 5)) != 0)
            {
                rotation.z = reader.ReadSingle();
                hasNewRotation = true;
            }

            if (hasNewRotation)
            {
                transform.localEulerAngles = rotation;
            }
            #endregion
        }
    }
}
