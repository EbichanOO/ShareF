using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Media;
using UnityEngine;
using UnityEngine.Video;
using MessagePack;
using PretiaArCloud.RecordingPlayback.Serialization;

namespace PretiaArCloud.RecordingPlayback.Editor.Importers
{
    /// <summary>
    /// Importer for images.
    /// </summary>
    internal class ImageBaseVideoImporter : IImporter
    {

#region Properties

        /// <summary>
        /// Frame rate to set the clip to.
        /// </summary>
        private int FPS { get; set; }

        /// <summary>List of frames for Orientation.</summary>
        private List<Keyframe> OrientationRotation { get; } = new List<Keyframe>();

        /// <summary>
        /// Name of the recording.
        /// </summary>
        private string AssetName { get; }
        
        private string AssetFolderPath { get; set; }
        
        /// <summary>
        /// Temporal path.
        /// </summary>
        private string TempPath { get; set; }

        /// <summary>
        /// Timestamp used for decide the time of the keyframes.
        /// </summary>
        private TimestampImporter Timestamp { get; }

        /// <summary>
        /// Encoder to store the images.
        /// </summary>
        private MediaEncoder Encoder { get; set; }
        
        /// <summary>
        /// Texture with the data of the last loaded texture.
        /// </summary>
        private Texture2D LoadedTexture2D { get; set; }

        /// <summary>
        /// Texture with the fixed format for video encoding.
        /// </summary>
        private Texture2D FixedTexture2D { get; } =
            new Texture2D(0, 0, TextureFormat.RGBA32, false, true);
        
        /// <summary>
        /// Last time stored.
        /// </summary>
        private MediaTime? LastTime { get; set; }

#endregion

#region Methods

        /// <summary>
        /// Creates a new instance of the CameraImageImporter.
        /// </summary>
        /// <param name="clip">Target clip.</param>
        /// <param name="timestamp">Timestamp used for decide the time of
        /// the keyframes.</param>
        public ImageBaseVideoImporter(AnimationClip clip, TimestampImporter timestamp)
        {
            Timestamp = timestamp;
            AssetName = clip.name + ".mp4";
            AssetFolderPath = AssetDatabase.GetAssetPath(clip);
            AssetFolderPath = AssetFolderPath.Replace(
                "/" + clip.name + ".anim",
                string.Empty
            );
        }
        

        /// <inheritdoc cref="IImporter.CollectKeyframes"/>
        public async Task CollectKeyframes(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            if (stream == null) { return; }
            
            // Whether are settings stored.
            var readOp = await ImageSerialization.Read(
                stream, 
                options, 
                cancelToken
            );

            if (readOp.settings.HasChanged)
            {
                var orientation = readOp.settings.Orientation;
                OrientationRotation.Add(
                    new Keyframe(
                        Timestamp.Time, 
                        GetRotation(orientation) 
                    )
                );
                
                FPS = readOp.settings.FPS;
                var width = readOp.settings.Width;
                var height =  readOp.settings.Height;

                if (Encoder == null)
                {
                    TempPath = Path.Combine(
                        Application.dataPath.Replace("Assets", AssetFolderPath),
                        AssetName
                    );

                    var attributes = new VideoTrackAttributes
                    {
                        width = (uint) width,
                        height = (uint) height,
                        frameRate = new MediaRational(FPS),
                        bitRateMode = VideoBitrateMode.Medium,
                        includeAlpha = false
                    };

                    Encoder = new MediaEncoder(TempPath, attributes); 
                }

                if (LoadedTexture2D == null)
                {
                    LoadedTexture2D = new Texture2D(
                        width,
                        height,
                        readOp.settings.Format,
                        false
                    );
                }
                else
                {
                    LoadedTexture2D.Resize(width, height);
                }
                
                FixedTexture2D.Resize(width, height);
            }

            // Loads from the recorded format.
            LoadedTexture2D.LoadImage(readOp.data);
            LoadedTexture2D.Apply();
            
            // Converts to the only supported format RGBA32.
            FixedTexture2D.SetPixels(LoadedTexture2D.GetPixels());
            FixedTexture2D.Apply();
            
            var time = new MediaTime(
                (long) Mathf.Round(Timestamp.Time * FPS), 
                (uint)FPS
            );
            
            if (LastTime.HasValue)
            {
                if (time.count == LastTime.Value.count)
                {
                    return;
                }
            }

            Encoder.AddFrame(FixedTexture2D, time);
            LastTime = time;
        }

        /// <inheritdoc cref="IImporter.AddKeyFrames"/> 
        public void AddKeyFrames(AnimationClip clip)
        {
            // Finishes the encoder.
            Encoder?.Dispose();
            Encoder = null;
            
            // Adds a reference to the video to the animation clip.
            AssetDatabase.Refresh();
            var path = Path.Combine(TempPath.Replace(TempPath, AssetFolderPath), AssetName);
            var video = AssetDatabase.LoadAssetAtPath<VideoClip>(path);
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
            
            // Adds orientation values to the video.
            var curve = new AnimationCurve(OrientationRotation.ToArray());
            for (var i = 0; i < curve.length; i++)
            {
                curve.SmoothTangents(i,0);
            }
            AnimationUtility.SetEditorCurve(
                clip, 
                PlaybackUtility.OrientationRotationBinding, 
                curve
            );
            OrientationRotation.Clear();
        }

        private static float GetRotation(ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.Portrait:
                    return -90;
                case ScreenOrientation.PortraitUpsideDown:
                    return 90;
                case ScreenOrientation.LandscapeLeft:
                    return 0;
                case ScreenOrientation.LandscapeRight:
                    return 180;
                case ScreenOrientation.AutoRotation:
                    return 0;
                default:
                    return 0;
            }
        }

        #endregion
    }
}