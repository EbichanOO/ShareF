using System.IO;
using PretiaArCloud;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PretiaArCloudEditor
{
    internal sealed class PretiaSDKProjectSettingsProvider : SettingsProvider
    {
        private const string OLD_PATH = "Assets/Resources/PretiaSDKProjectSettings.asset";
        private const string PATH = "Assets/Pretia/PretiaSDKProjectSettings.asset";

        private static SerializedObject _settings;

        public PretiaSDKProjectSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) {}

        public override void OnGUI(string searchContext)
        {
            _settings = GetSerializedSettings();

            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(_settings.FindProperty("_appKey"), new GUIContent("App Key"));
            EditorGUILayout.PropertyField(_settings.FindProperty("_initializeOnStartup"), new GUIContent("Initialize On Startup"));
            _settings.ApplyModifiedPropertiesWithoutUndo();
        }

        [SettingsProvider]
        public static SettingsProvider CreatePretiaSDKProjectSettingsProvider()
        {
            _settings = GetSerializedSettings();

            if (File.Exists(PATH))
            {
                var provider = new PretiaSDKProjectSettingsProvider("Project/Pretia AR Cloud", SettingsScope.Project);
                provider.keywords = new string[] { "pretia", "arc", "pretiasdk", "pretiaarcloud", "arcloud", "appkey", "app key" };
                return provider;
            }

            return null;
        }

        private static PretiaSDKProjectSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PretiaSDKProjectSettings>(OLD_PATH);
            if (settings != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PATH));

                File.Move(OLD_PATH, PATH);
                AssetDatabase.Refresh();
            }

            settings = AssetDatabase.LoadAssetAtPath<PretiaSDKProjectSettings>(PATH);
            if (settings == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PATH));

                settings = ScriptableObject.CreateInstance<PretiaSDKProjectSettings>();
                AssetDatabase.CreateAsset(settings, PATH);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}