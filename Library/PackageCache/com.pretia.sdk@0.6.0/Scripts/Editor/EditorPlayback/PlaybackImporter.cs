using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using PretiaArCloud.Networking;
using UnityEditor;
using UnityEngine;
using PretiaArCloud.RecordingPlayback.Editor.Importers;
using PretiaArCloud.RecordingPlayback.Serialization;
using System.Diagnostics;

namespace PretiaArCloudEditor.RecordingPlayback
{
    /// <summary>
    /// Class with methods useful for import recordings.
    /// </summary>
    public static class PlaybackImporter
    {
        internal const string META_FILE_EXT = "record";
        internal const string MP4_FILE_EXT = "mp4";

        /// <summary>
        /// Imports a playback animation from a selected record file.
        /// </summary>
        //[MenuItem("Pretia/Import AR Record")]
        public static async void Import()
        {
            // Selects a record to import.
            var binaryFilePath = EditorUtility.OpenFilePanel(
                "Select the record to import.",
                Application.dataPath,
                META_FILE_EXT
            );
            if (string.IsNullOrEmpty(binaryFilePath)) { return; }
            await Import(binaryFilePath);
        }
        
        /// <summary>
        /// Imports a file at the given full path. The file MUST be inside the
        /// Assets folder.
        /// </summary>
        /// <param name="dataFileFullPath">Full path of the file on the system.</param>
        internal static async Task Import(string dataFileFullPath)
        {
            // Relative path.
            var relativeDataFilePath = "Assets" + dataFileFullPath.Replace(
                Application.dataPath, 
                string.Empty
            );

            // Creates stream and serializer.
            var serializer = new MsgPackSerializer(FormatterResolver.Instance);
            var stream = File.OpenRead(dataFileFullPath);
            stream.Position = 0;
            
            // Proceed with the import.
            await Import(stream, relativeDataFilePath, serializer.Options);
            
            // Closes stream.
            stream.Close();
            stream.Dispose();

            // Saves the clip.
            AssetDatabase.DeleteAsset(relativeDataFilePath);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Imports a playback animation from a given stream.
        /// </summary>
        /// <param name="stream">Stream where to read the data from.</param>
        /// <param name="dataPath">Relative path to the data
        /// file.</param>
        /// <param name="options">Options for serialization.</param>
        /// <param name="cancelToken">Token to cancel the task.</param>
        private static async Task Import(
            Stream stream,
            string dataPath,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            var videoPath = dataPath.Replace(
                META_FILE_EXT,
                MP4_FILE_EXT
            );
            
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(videoPath);
            AssetDatabase.ImportAsset(dataPath);
            
            // Creates clip for the animation.
            var clipPath = dataPath.Replace(
                META_FILE_EXT, 
                "anim"
            );
            var clip = GetAnimationClip(clipPath);
            
            // Creates importers.
            var timestampImporter = new TimestampImporter();
            var importers = new IImporter[]
            {
                timestampImporter,
                new CameraPoseImporter(timestampImporter),
                new VideoImporter(videoPath),
                new CameraOrientationImporter(timestampImporter)
            };

            // Reads keyframes.
            var didReadData = true;
            while (didReadData)
            {
                if (DisplayCancelableImportProgress(stream))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(clip));
                    EditorUtility.ClearProgressBar();
                    return;
                }

                didReadData = await TryReadData(
                    importers,
                    stream,
                    options,
                    cancelToken
                );
            }
            EditorUtility.ClearProgressBar();

            // Adds keyframes to animation clip.
            for (var i = 0; i < importers.Length; i++)
            {
                importers[i].AddKeyFrames(clip);
            }
            
            var clipSettings = new AnimationClipSettings
            {
                loopTime = true,
                stopTime = timestampImporter.Time
            };
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
        }

        /// <summary>
        /// Tries to read the data on the given stream for the given importers.
        /// </summary>
        /// <param name="importers">Importers that may read the stream.</param>
        /// <param name="stream">Stream where to read the data from.</param>
        /// <param name="options">Options for serialization.</param>
        /// <param name="cancelToken">Cancel token.</param>
        /// <returns>Task with the result of the attempt.</returns>
        private static async Task<bool> TryReadData(
            IImporter[] importers,
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            BitArray shouldRead;
            // Gets the should read array.
            try
            {
                shouldRead = BitArraySerialization.Read(
                    stream, 
                    options, 
                    cancelToken
                );
            }
            catch (Exception)
            {
                return false;
            }

            if (shouldRead == null) { return false; }

            // Reads data with each importer.
            for (var i = 0; i < shouldRead.Length; i++)
            {
                if (!shouldRead[i] || importers.Length <= i) { continue; }

                try
                {
                    await importers[i].CollectKeyframes(
                        stream, 
                        options, 
                        cancelToken
                    );
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a new animation clip at the given path.
        /// </summary>
        /// <param name="path">Path of the animation clip file relative to the
        /// project.</param>
        /// <returns>The animation at the given path.</returns>
        private static AnimationClip GetAnimationClip(string path)
        {
            // Loads the clip at the given path.
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null) return clip;
            
            // Creates a new animation clip.
            clip = new AnimationClip { wrapMode = WrapMode.Loop };

            AssetDatabase.CreateAsset(clip, path);
            AssetDatabase.SaveAssets();
            return clip;
        }

        /// <summary>
        /// Displays a cancelable progress bar for importing.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Whether the process should be canceled.</returns>
        private static bool DisplayCancelableImportProgress(Stream stream)
        {
            var position = stream.Position;
            var lenght = stream.Length;
            var progress = position / (float) lenght;
            var friendlyProgress = Mathf.Round(progress * 100);
            
            return EditorUtility.DisplayCancelableProgressBar(
                "Importing", 
                $"Progress: {friendlyProgress}%", 
                progress
            );
        }
    }
    
}