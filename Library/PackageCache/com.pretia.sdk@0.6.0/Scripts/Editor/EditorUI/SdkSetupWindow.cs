using UnityEditor;
using UnityEngine;

namespace PretiaArCloudEditor
{
    internal class SdkSetupWindow : EditorWindow
    {
        private static SdkSetupWindow _instance;
        internal static SdkSetupWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = EditorWindow.GetWindow<SdkSetupWindow>("Sdk Setup");
                }

                return _instance;
            }
        }

        private string _username;
        private string _password;
        private string _token;
        private bool _hasErrors;
        private bool _processingLogin;

        private DeveloperService _devService;
        private GroupService _groupService;
        private MapService _mapService;
        private SerializedObject _pretiaSDKSettings;

        [MenuItem("Pretia/Sdk Setup")]
        static void Init()
        {
            _instance = EditorWindow.GetWindow<SdkSetupWindow>("Sdk Setup");
            _instance.Show();
        }

        private void OnEnable()
        {
            _username = EditorPrefs.GetString(Constants.USERNAME);
            _password = EditorPrefs.GetString(Constants.PASSWORD);
            _token = EditorPrefs.GetString(Constants.TOKEN);

            _devService = Services.DeveloperService;
            _groupService = Services.GroupService;
            _mapService = Services.MapService;
        }

        private void OnGUI()
        {
            _pretiaSDKSettings = PretiaSDKProjectSettingsProvider.GetSerializedSettings();
            EditorGUILayout.PropertyField(_pretiaSDKSettings.FindProperty("_appKey"), new GUIContent("App Key"));

            if (string.IsNullOrEmpty(_token))
            {
                BeforeLoginUI();
                if (_hasErrors)
                {
                    var errorStyle = new GUIStyle();
                    errorStyle.normal.textColor = Color.red;
                    EditorGUILayout.LabelField("There was an error authenticating your credentials. Please try again.", errorStyle);
                }
            }
            else
            {
                AfterLoginUI();
            }

            _pretiaSDKSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        private async void BeforeLoginUI()
        {
            _username = EditorGUILayout.TextField("Username", _username);
            _password = EditorGUILayout.PasswordField("Password", _password);

            EditorGUI.BeginDisabledGroup(_processingLogin);
            if (GUILayout.Button("Login"))
            {
                _processingLogin = true;
                var authResult = await _devService.LoginAsync(_username, _password);
                if (authResult.StatusCode == 0)
                {
                    _hasErrors = false;
                    _token = authResult.Token;

                    _groupService.Token = _token;
                    _mapService.Token = _token;

                    EditorPrefs.SetString(Constants.TOKEN, _token);
                }
                else
                {
                    _hasErrors = true;
                }
                _processingLogin = false;
            }
            EditorGUI.EndDisabledGroup();
        }

        private void AfterLoginUI()
        {
            if (_devService.IsTokenValid(_token))
            {
                EditorGUILayout.LabelField($"Logged in as {_username}");
                if (GUILayout.Button("Logout"))
                {
                    _token = null;
                    _groupService.Token = null;
                    _mapService.Token = null;

                    EditorPrefs.DeleteKey(Constants.TOKEN);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Your session has expired. Please login again");
                BeforeLoginUI();
            }
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(Constants.USERNAME, _username);
            EditorPrefs.SetString(Constants.PASSWORD, _password);
        }
    }
}