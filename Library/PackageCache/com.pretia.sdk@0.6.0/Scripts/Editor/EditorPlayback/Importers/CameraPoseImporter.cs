using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using PretiaArCloud.RecordingPlayback.Serialization;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Editor.Importers
{
    /// <summary>
    /// Importer for the camera pose.
    /// </summary>
    internal class CameraPoseImporter : IImporter
    {
#region Properties

        /// <summary>
        /// Timestamp used for decide the time of the keyframes.
        /// </summary>
        private TimestampImporter Timestamp { get; }

        /// <summary>List of frames for Position.X</summary>
        private List<Keyframe> PositionX { get; } = new List<Keyframe>();

        /// <summary>List of frames for Position.Y</summary>
        private List<Keyframe> PositionY { get; } = new List<Keyframe>();

        /// <summary>List of frames for Position.Z</summary>
        private List<Keyframe> PositionZ { get; } = new List<Keyframe>();

        /// <summary>List of frames for Rotation.X</summary>
        private List<Keyframe> RotationX { get; } = new List<Keyframe>();

        /// <summary>List of frames for Rotation.Y</summary>
        private List<Keyframe> RotationY { get; } = new List<Keyframe>();

        /// <summary>List of frames for Rotation.Z</summary>
        private List<Keyframe> RotationZ { get; } = new List<Keyframe>();

        /// <summary>List of frames for Rotation.W</summary>
        private List<Keyframe> RotationW { get; } = new List<Keyframe>();

#endregion

#region Methods

        /// <summary>
        /// Creates a new instance of the CameraPoseImporter.
        /// </summary>
        /// <param name="timestamp">Timestamp used for decide the time of
        /// the keyframes.</param>
        public CameraPoseImporter(TimestampImporter timestamp)
        {
            Timestamp = timestamp;
        }

        /// <inheritdoc cref="IImporter.CollectKeyframes"/>
        public Task CollectKeyframes(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            if (stream == null || Timestamp == null)
            {
                return Task.CompletedTask;
            }

            var readOp = PoseSerialization.Read(
                stream, 
                options, 
                cancelToken
            );

            PositionX.Add(new Keyframe(Timestamp.Time, readOp.position.x));
            PositionY.Add(new Keyframe(Timestamp.Time, readOp.position.y));
            PositionZ.Add(new Keyframe(Timestamp.Time, readOp.position.z));
            RotationX.Add(new Keyframe(Timestamp.Time, readOp.rotation.x));
            RotationY.Add(new Keyframe(Timestamp.Time, readOp.rotation.y));
            RotationZ.Add(new Keyframe(Timestamp.Time, readOp.rotation.z));
            RotationW.Add(new Keyframe(Timestamp.Time, readOp.rotation.w));

            return Task.CompletedTask;
        }

        /// <inheritdoc cref="IImporter.AddKeyFrames"/> 
        public void AddKeyFrames(AnimationClip clip)
        {
            AddFrames(clip, PlaybackUtility.GetPositionBinding('x'), PositionX);
            AddFrames(clip, PlaybackUtility.GetPositionBinding('y'), PositionY);
            AddFrames(clip, PlaybackUtility.GetPositionBinding('z'), PositionZ);
            AddFrames(clip, PlaybackUtility.GetRotationBinding('x'), RotationX);
            AddFrames(clip, PlaybackUtility.GetRotationBinding('y'), RotationY);
            AddFrames(clip, PlaybackUtility.GetRotationBinding('z'), RotationZ);
            AddFrames(clip, PlaybackUtility.GetRotationBinding('w'), RotationW);
            
            clip.EnsureQuaternionContinuity();
        }

        /// <summary>
        /// Adds keyframes to the given key on the given binding.
        /// </summary>
        /// <param name="clip">Clip where to add keyframes.</param>
        /// <param name="binding">Binding where to add data.</param>
        /// <param name="keys">Keys to add.</param>
        private void AddFrames(
            AnimationClip clip, 
            EditorCurveBinding binding, 
            List<Keyframe> keys
        ){
            var curve = new AnimationCurve(keys.ToArray());
            for (var i = 0; i < curve.length; i++)
            {
                curve.SmoothTangents(i, 1);
            }
            keys.Clear();
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }


#endregion
    }
}