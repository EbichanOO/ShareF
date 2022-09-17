using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace PretiaArCloud.RecordingPlayback.Editor.Importers
{
    internal class VideoImporter : IImporter
    {
        
        /// <summary>
        /// Relative path to the video.
        /// </summary>
        private string RelativePath { get; }

        /// <summary>
        /// Creates a new instance of the CameraPoseImporter.
        /// </summary>
        public VideoImporter( string relativePath)
        {
            RelativePath = relativePath;
        }
        
        /// <inheritdoc cref="IImporter.CollectKeyframes"/>
        public async Task CollectKeyframes(
            Stream stream, 
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default) { }

        
        /// <inheritdoc cref="IImporter.AddKeyFrames"/> 
        public void AddKeyFrames(AnimationClip clip)
        {
            var video = AssetDatabase.LoadAssetAtPath<VideoClip>(RelativePath);
            var objReferenceKeyframe = new ObjectReferenceKeyframe
            {
                time = 0,
                value = video
            };
            AnimationUtility.SetObjectReferenceCurve(
                clip,
                PlaybackUtility.VideoClipBinding,
                new[]{ objReferenceKeyframe }
            );
        }
    }
}