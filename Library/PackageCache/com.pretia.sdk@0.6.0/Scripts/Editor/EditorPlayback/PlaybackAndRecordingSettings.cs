using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloudEditor.RecordingPlayback
{

    /// <summary>
    /// Settings for the usage of the playback and recording.
    /// </summary>
    internal class PlaybackAndRecordingSettings : ScriptableObject
    {
        #region Consts

        /// <summary>
        /// Path where the asset will be created.
        /// </summary>
        private const string AssetPath =
            "Assets/Pretia/PretiaSDKPlaybackAndRecordingSettings.asset";

        /// <summary>
        /// Path of the window on the Project Settings.
        /// </summary>
        internal const string ProjectSettingsPath =
            "Project/Pretia AR Cloud/Playback & Recording";

        /// <summary>
        /// Default path where to locate the downloaded records relative to the
        /// assets folder.
        /// </summary>
        internal const string DefaultDownloadsRelativePath = "Pretia/Records";

        #endregion

        #region Fields

        /// <summary>
        /// Override to where to locate the downloaded files inside the assets
        /// folder.
        /// </summary>
        [SerializeField] private string downloadsPath 
            = DefaultDownloadsRelativePath;

        #endregion

        #region Properties

        /// <summary>
        /// Where to locate the downloaded files inside the assets
        /// folder.
        /// </summary>
        public string DownloadPath
        {
            get => Path.Combine(
                Application.dataPath, 
                string.IsNullOrEmpty(downloadsPath) 
                    ? DefaultDownloadsRelativePath 
                    : downloadsPath
            );
            set => downloadsPath = value;
        }
        
        /// <summary>
        /// Gets the current settings used.
        /// </summary>
        public static PlaybackAndRecordingSettings CurrentSettings =>
            GetOrCreateSettings();
        
#endregion

#region Methods

        /// <summary>
        /// Gets or creates a new asset if it is needed.
        /// </summary>
        /// <returns>Always return a new setting.</returns>
        private static PlaybackAndRecordingSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase
                .LoadAssetAtPath<PlaybackAndRecordingSettings>(AssetPath);

            if (settings != null) return settings;

            settings = CreateInstance<PlaybackAndRecordingSettings>();
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        /// <summary>
        /// Gets the serialized object from the settings.
        /// </summary>
        /// <returns></returns>
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

#endregion

    }

    /// <summary>
    /// Register for the Playback adn recording settings.
    /// </summary>
    internal static class PlaybackAndRecordingSettingsRegister
    {
        /// <summary>
        /// Keywords for search tool.
        /// </summary>
        private static readonly HashSet<string> Keywords = new HashSet<string>()
        {
            "Playback",
            "Recording"
        };

        /// <summary>
        /// Creates a new settings provider.
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // Creates a new setting window.
            var provider = new SettingsProvider(
                PlaybackAndRecordingSettings.ProjectSettingsPath,
                SettingsScope.Project
            ) {guiHandler = GUIHandler, keywords = Keywords};

            return provider;
        }

        /// <summary>
        /// Handles the GUI drawing.
        /// </summary>
        private static void GUIHandler(string _)
        {
            var settings = PlaybackAndRecordingSettings.GetSerializedSettings();

            var downloadsPathProperty = settings.FindProperty("downloadsPath");

            var path = EditorGUILayout.TextField(
                new GUIContent("Downloads Path"),
                downloadsPathProperty.stringValue
            );

            downloadsPathProperty.stringValue = string.IsNullOrWhiteSpace(path)
                ? PlaybackAndRecordingSettings.DefaultDownloadsRelativePath 
                : path;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Download Window", EditorStyles.miniButton))
            {
                PlaybackDownloaderWindow.ShowWindow();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();


            settings.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// Validates whether an IP is valid or not.
        /// </summary>
        /// <param name="ip">IP address to validate.</param>
        /// <returns>Whether the IP is valid or not.</returns>
        internal static bool IsValidIp(string ip)
        {
            var validIpRegex = new Regex("^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$");
            return validIpRegex.IsMatch(ip);
        }
    }
}