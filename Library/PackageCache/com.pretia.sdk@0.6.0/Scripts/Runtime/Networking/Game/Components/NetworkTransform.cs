using System;
using System.Buffers;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class NetworkTransform : NetworkBehaviour
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
            ScaleX = 1 << 6,
            ScaleY = 1 << 7,
            ScaleZ = 1 << 8,
            All = ~0,
        }

        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private BitMask _syncMask = BitMask.All;

        private float[] _cachedValues;

        private void Awake()
        {
            _cachedValues = new float[9];
            _cachedValues[0] = _transform.localPosition.x;
            _cachedValues[1] = _transform.localPosition.y;
            _cachedValues[2] = _transform.localPosition.z;

            _cachedValues[3] = _transform.localEulerAngles.x;
            _cachedValues[4] = _transform.localEulerAngles.y;
            _cachedValues[5] = _transform.localEulerAngles.z;

            _cachedValues[6] = _transform.localScale.x;
            _cachedValues[7] = _transform.localScale.y;
            _cachedValues[8] = _transform.localScale.z;
        }

        public override void Serialize(ref MessagePackWriter writer, MessagePackSerializerOptions options)
        {
            Vector3 position = _transform.localPosition;
            Vector3 eulerAngles = _transform.localEulerAngles;
            Vector3 scale = _transform.localScale;

            float[] values = ArrayPool<float>.Shared.Rent(9);
            values[0] = position.x;
            values[1] = position.y;
            values[2] = position.z;

            values[3] = eulerAngles.x;
            values[4] = eulerAngles.y;
            values[5] = eulerAngles.z;

            values[6] = scale.x;
            values[7] = scale.y;
            values[8] = scale.z;

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

            var position = _transform.localPosition;
            var rotation = _transform.localEulerAngles;
            var scale = _transform.localScale;

            bool hasNewPosition = false;
            bool hasNewRotation = false;
            bool hasNewScale = false;

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
                _transform.localPosition = position;
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
                _transform.localEulerAngles = rotation;
            }
            #endregion

            #region Sync Scale
            if (dirtyMask.HasFlag(BitMask.ScaleX))
            {
                scale.x = reader.ReadSingle();
                hasNewScale = true;
            }

            if (dirtyMask.HasFlag(BitMask.ScaleY))
            {
                scale.y = reader.ReadSingle();
                hasNewScale = true;
            }

            if (dirtyMask.HasFlag(BitMask.ScaleZ))
            {
                scale.z = reader.ReadSingle();
                hasNewScale = true;
            }

            if (hasNewScale)
            {
                _transform.localScale = scale;
            }
            #endregion
        }
    }
}