using System;
using System.Buffers;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkRigidbody : NetworkBehaviour
    {
        [Flags]
        private enum BitMask
        {
            None = 0,
            PosX = 1 << 0,
            PosY = 1 << 1,
            PosZ = 1 << 2,
            RotX = 1 << 3,
            RotY = 1 << 4,
            RotZ = 1 << 5,
            All = ~0,
        }

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private BitMask _syncMask = BitMask.All;

        private float[] _cachedValues;

        private void Awake()
        {
            _cachedValues = new float[6];
            _cachedValues[0] = _rigidbody.position.x;
            _cachedValues[1] = _rigidbody.position.y;
            _cachedValues[2] = _rigidbody.position.z;

            _cachedValues[3] = _rigidbody.rotation.eulerAngles.x;
            _cachedValues[4] = _rigidbody.rotation.eulerAngles.y;
            _cachedValues[5] = _rigidbody.rotation.eulerAngles.z;
        }

        public override void Serialize(ref MessagePackWriter writer, MessagePackSerializerOptions options)
        {
            Vector3 position = _rigidbody.position;
            Vector3 eulerAngles = _rigidbody.rotation.eulerAngles;

            float[] values = ArrayPool<float>.Shared.Rent(6);
            values[0] = position.x;
            values[1] = position.y;
            values[2] = position.z;

            values[3] = eulerAngles.x;
            values[4] = eulerAngles.y;
            values[5] = eulerAngles.z;

            for (int i = 0; i < _cachedValues.Length; i++)
            {
                ulong currentMask = 1UL << i;
                bool syncValue = (((ulong)_syncMask & currentMask) != 0) && (_cachedValues[i] != values[i]);

                if (syncValue)
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
            BitMask dirtyMask = (BitMask)_dirtyMask;

            var position = _rigidbody.position;
            var rotation = _rigidbody.rotation.eulerAngles;

            bool hasNewPosition = false;
            bool hasNewRotation = false;

            #region Sync Position
            if (dirtyMask.HasFlag(BitMask.PosX))
            {
                position.x = reader.ReadSingle();
                hasNewPosition = true;
            }

            if (dirtyMask.HasFlag(BitMask.PosY))
            {
                position.y = reader.ReadSingle();
                hasNewPosition = true;
            }

            if (dirtyMask.HasFlag(BitMask.PosZ))
            {
                position.z = reader.ReadSingle();
                hasNewPosition = true;
            }

            if (hasNewPosition)
            {
                _rigidbody.position = position;
            }
            #endregion

            #region Sync Rotation
            if (dirtyMask.HasFlag(BitMask.RotX))
            {
                rotation.x = reader.ReadSingle();
                hasNewRotation = true;
            }

            if (dirtyMask.HasFlag(BitMask.RotY))
            {
                rotation.y = reader.ReadSingle();
                hasNewRotation = true;
            }

            if (dirtyMask.HasFlag(BitMask.RotZ))
            {
                rotation.z = reader.ReadSingle();
                hasNewRotation = true;
            }

            if (hasNewRotation)
            {
                _rigidbody.rotation = Quaternion.Euler(rotation);
            }
            #endregion
        }
    }
}