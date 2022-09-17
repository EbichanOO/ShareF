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
    internal class CameraOrientationImporter : IImporter
    {
        /// <summary>
        /// Timestamp used for decide the time of the keyframes.
        /// </summary>
        private TimestampImporter Timestamp { get; }
        
        /// <summary>List of frames for Orientation.</summary>
        private List<Keyframe> Orientation { get; } = new List<Keyframe>();
        
        /// <summary>
        /// Creates a new instance of the CameraPoseImporter.
        /// </summary>
        /// <param name="timestamp">Timestamp used for decide the time of
        /// the keyframes.</param>
        public CameraOrientationImporter(TimestampImporter timestamp)
        {
            Timestamp = timestamp;
        }
        
        /// <inheritdoc cref="IImporter.CollectKeyframes"/>
        public async Task CollectKeyframes(
            Stream stream, 
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            if (stream == null || Timestamp == null)
            {
                return;
            }

            var screenOrientation = await CameraOrientationSerialization.Read(
                stream,
                options,
                cancelToken
            );

            var orientation = GetDegrees(screenOrientation);
            
            Orientation.Add(
                new Keyframe(
                    Timestamp.Time, 
                    orientation
                )
            );
        }
        
        /// <inheritdoc cref="IImporter.AddKeyFrames"/> 
        public void AddKeyFrames(AnimationClip clip)
        {
            var curve = new AnimationCurve(Orientation.ToArray());
            for (var i = 0; i < curve.length; i++)
            {
                curve.SmoothTangents(i,0);
            }
            AnimationUtility.SetEditorCurve(
                clip, 
                PlaybackUtility.OrientationRotationBinding, 
                curve
            );
            Orientation.Clear();
        }

        /// <summary>
        /// Converts the given orientation into degrees.
        /// </summary>
        /// <param name="orientation">Screen orientation.</param>
        public float GetDegrees(ScreenOrientation orientation)
        {
            float result;
            switch(orientation)
            {
                case ScreenOrientation.Portrait: result = -90; break;
                case ScreenOrientation.PortraitUpsideDown: result = 90; break;
                case ScreenOrientation.LandscapeLeft: result = 0; break;
                case ScreenOrientation.LandscapeRight: result = 180; break;
                case ScreenOrientation.AutoRotation: result = 0; break;
                default: result = 0; break;
            };

            return result;
        }
    }
}