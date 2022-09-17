using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PretiaArCloud;
using PretiaArCloud.Pcx;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloudEditor
{
    internal class MapDownloadWindow : EditorWindow
    {
        private static string[] _colorValues = new string[] {
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
            "400000", "004000", "000040", "404000", "400040", "004040", "404040",
            "200000", "002000", "000020", "202000", "200020", "002020", "202020",
            "600000", "006000", "000060", "606000", "600060", "006060", "606060",
            "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
            "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
        };

        private string _username;
        private string _token;
        private bool _processing;
        private bool _wasLoggedOut;
        private bool[] _foldoutStatuses;
        private Vector2 _scrollPos;

        private MeshFilter _planePrefab;
        private GameObject _anchorPrefab;

        private GroupService _groupService;
        private MapService _mapService;
        private DeveloperService _devService;

        private GroupService.Data[] _groups;
        private GroupService.Data[] _groupData;

        private CancellationTokenSource _cts;

        private bool _retrieveGroupsError = false;
        private bool _retrieveMapsError = false;
        private bool _retrieveSelectedMapError = false;

        [MenuItem("Pretia/Map Download")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<MapDownloadWindow>("Map Download");
            window.Show();
        }

        private void OnEnable()
        {
            _groupService = Services.GroupService;
            _mapService = Services.MapService;
            _devService = Services.DeveloperService;

            string prefabDir = Path.Combine("Assets", "PretiaSDK", "Prefabs", "EditorVisualization");

            bool isPackage = Directory.Exists(Path.Combine("Packages", "com.pretia.sdk"));
            if (isPackage)
            {
                prefabDir = Path.Combine("Packages", "com.pretia.sdk", "Prefabs", "EditorVisualization");
            }

            _planePrefab = AssetDatabase.LoadAssetAtPath<MeshFilter>(Path.Combine(prefabDir, "Plane.prefab"));
            _anchorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(prefabDir, "location_pin_prefab.prefab"));

            _cts = new CancellationTokenSource();
        }

        private void OnGUI()
        {
            _token = EditorPrefs.GetString(Constants.TOKEN);

            if (string.IsNullOrEmpty(_token))
            {
                BeforeLoginUI();
            }
            else
            {
                AfterLoginUI();
                var errorStyle = new GUIStyle();
                errorStyle.normal.textColor = Color.red;

                if (_retrieveGroupsError)
                {
                    EditorGUILayout.LabelField("There was an error retrieving groups. Please try again.", errorStyle);
                }

                if (_retrieveMapsError)
                {
                    EditorGUILayout.LabelField("There was an error retrieving maps. Please try again.", errorStyle);
                }

                if (_retrieveSelectedMapError)
                {
                    EditorGUILayout.LabelField("There was an error retrieving selected map. Please try again.", errorStyle);
                }
            }
        }

        private void BeforeLoginUI()
        {
            _wasLoggedOut = true;

            EditorGUILayout.LabelField("Please login in order to download maps");
            if (GUILayout.Button("Login"))
            {
                SdkSetupWindow.Instance.Focus();
            }
        }

        private async void AfterLoginUI()
        {
            if (_wasLoggedOut)
            {
                _wasLoggedOut = false;
                _groups = null;
                _groupData = null;
            }

            string token = EditorPrefs.GetString(Constants.TOKEN);
            if (!_devService.IsTokenValid(token))
            {
                EditorGUILayout.LabelField("Your session has expired. Please login again");
                if (GUILayout.Button("Login"))
                {
                    SdkSetupWindow.Instance.Focus();
                }

                return;
            }
 
            _username = EditorPrefs.GetString(Constants.USERNAME);
            EditorGUILayout.LabelField($"Logged in as {_username}");

            EditorGUI.BeginDisabledGroup(_processing);
            if (GUILayout.Button("Reload groups"))
            {
                _processing = true;
                try
                {
                    _groups = await _groupService.GetGroupedMapsAsync(PretiaSDKProjectSettings.Instance.AppKey, _cts.Token);
                }
                catch
                {
                    _retrieveGroupsError = true;
                    throw;
                }
                finally
                {
                    _processing = false;
                }
                return;
            }

            if (_groups != null)
            {
                EditorGUILayout.BeginVertical();
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, alwaysShowHorizontal: false, alwaysShowVertical: true);
                _retrieveGroupsError = false;
                EditorGUILayout.LabelField("Groups",
                    new GUIStyle
                    {
                        fontSize = 14,
                        fontStyle = FontStyle.Bold,
                    }
                );

                if (_foldoutStatuses == null)
                {
                    _foldoutStatuses = new bool[_groups.Length];
                }

                if (_groupData == null)
                {
                    _groupData = new GroupService.Data[_groups.Length];
                }

                for (int i = 0; i < _groups.Length; i++)
                {
                    var group = _groups[i];
                    _foldoutStatuses[i] = EditorGUILayout.Foldout(_foldoutStatuses[i], group.Name);

                    bool status = _foldoutStatuses[i];
                    if (status)
                    {
                        _groupData[i] = _groups[i];

                        if (_groupData[i] != null)
                        {
                            _retrieveMapsError = false;
                            EditorGUI.indentLevel++;
                            foreach (var map in _groupData[i].Maps)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($"{map.Name} ({map.MapKey})");

                                string prefabName = $"{map.Name}";
                                string prefabDir = Path.Combine("Assets", "Pretia", "MapGroups", $"{group.Name}", prefabName);
                                string prefabPath = Path.Combine(prefabDir, $"{prefabName}.prefab");
                                string mapContentsDir = Path.Combine(prefabDir, "MapContents");

                                if (GUILayout.Button("Import map"))
                                {
                                    _processing = true;
                                    if (Directory.Exists(prefabDir))
                                    {
                                        var files = Directory.EnumerateFiles(prefabDir);
                                        foreach (var file in files)
                                        {
                                            File.Delete(file);
                                        }
                                    }
                                    Directory.CreateDirectory(prefabDir);

                                    _ = ImportMap(prefabDir, prefabName, prefabPath, map).ContinueWith(
                                        t => Debug.LogException(t.Exception),
                                        cancellationToken: default,
                                        TaskContinuationOptions.OnlyOnFaulted,
                                        TaskScheduler.FromCurrentSynchronizationContext());
                                }

                                if (GUILayout.Button("Download Map Contents"))
                                {
                                    if (!Directory.Exists(mapContentsDir))
                                    {
                                        Directory.CreateDirectory(mapContentsDir);
                                    }

                                    _ = DownloadMapContents(mapContentsDir, map).ContinueWith(
                                        t => Debug.LogException(t.Exception),
                                        cancellationToken: default,
                                        TaskContinuationOptions.OnlyOnFaulted,
                                        TaskScheduler.FromCurrentSynchronizationContext());
                                }

                                GUILayout.EndHorizontal();
                            }
                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUILayout.Separator();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();
        }

        private async Task DownloadMapContents(string dir, GroupService.MapData map)
        {
            byte[] mapContents = await _mapService.GetMapContentsAsync(PretiaSDKProjectSettings.Instance.AppKey, map.MapKey, _cts.Token);
            string mapContentsPath = Path.Combine(dir, $"{map.MapKey}.map-contents.json");
            File.WriteAllBytes(mapContentsPath, mapContents);
            AssetDatabase.ImportAsset(mapContentsPath);
            ImportMapContents.CreateMapContentsPrefab(mapContentsPath);
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(dir, "ContentRoot.prefab"));
        }

        private async Task ImportMap(string prefabDir, string prefabName, string prefabPath, GroupService.MapData map)
        {
            string pointCloudString = await _mapService.GetPointCloudData(map.MapKey, _cts.Token);
            string pointCloudPath = Path.Combine(prefabDir, "PointCloud.map");
            File.WriteAllText(pointCloudPath, pointCloudString);
            AssetDatabase.Refresh();

            SdkVersion sdkVersion = null;
            try
            {
                sdkVersion = await _mapService.GetSdkVersionAsync(map.MapKey, _cts.Token);
            }
            catch
            {
                Debug.LogWarning("Importing anchor_data and external_data with format version v0.1.0");
                var generatedObject = await ImportMapDataAsync_V010(prefabDir, prefabName, map, _cts.Token);

                var pointCloudAsset = AssetDatabase.LoadAssetAtPath<GameObject>(pointCloudPath);
                PrefabUtility.InstantiatePrefab(pointCloudAsset, generatedObject.transform);

                PrefabUtility.SaveAsPrefabAssetAndConnect(generatedObject, prefabPath, InteractionMode.UserAction);
                _processing = false;
                return;
            }

            var gameObject = new GameObject();
            gameObject.name = prefabName;

            try
            {
                Task importAnchorDataTask = null;
                Task importExternalDataTask = null;

                if (sdkVersion.AnchorDataVersion == Constants.V1_0_0)
                {
                    importAnchorDataTask = ImportAnchorData(prefabDir, gameObject.transform, map, _cts.Token);
                }
                else
                {
                    importAnchorDataTask = ImportAnchorData_V010(prefabDir, gameObject.transform, map, _cts.Token);
                }

                if (sdkVersion.ExternalDataVersion == Constants.V1_0_0)
                {
                    importExternalDataTask = ImportExternalData(prefabDir, gameObject.transform, map, _cts.Token);
                }
                else
                {
                    importExternalDataTask = ImportExternalData_V010(prefabDir, gameObject.transform, map, _cts.Token);
                }
                
                var pointCloudAsset = AssetDatabase.LoadAssetAtPath<GameObject>(pointCloudPath);
                PrefabUtility.InstantiatePrefab(pointCloudAsset, gameObject.transform);

                await Task.WhenAll(importAnchorDataTask, importExternalDataTask);
                PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, prefabPath, InteractionMode.UserAction);
            }
            catch
            {
                DestroyImmediate(gameObject);
                throw;
            }
            finally
            {
                _processing = false;
            }
        }

        private async Task ImportExternalData(string prefabDir, Transform root, GroupService.MapData map, CancellationToken cancellationToken = default)
        {
            var planeMeshes = await _mapService.GetExternalDataAsync(map.MapKey, cancellationToken);
            foreach (var plane in planeMeshes)
            {
                string meshPath = Path.Combine(prefabDir, $"Plane-{plane.Id}.mesh");
                GenerateMesh(plane, meshPath, root);
            }
        }

        private async Task ImportExternalData_V010(string prefabDir, Transform root, GroupService.MapData map, CancellationToken cancellationToken = default)
        {
            var externalDataMap = await _mapService.GetExternalDataAsync_V010(map.MapKey, cancellationToken);
            foreach (var externalData in externalDataMap)
            {
                string meshPath = Path.Combine(prefabDir, $"Plane-{externalData.Key}.mesh");
                GenerateMesh(externalData.Value, meshPath, root);
            }
        }

        private async Task ImportAnchorData(string prefabDir, Transform root, GroupService.MapData map, CancellationToken cancellationToken)
        {
            var anchors = await _mapService.GetAnchorDataAsync(map.MapKey, cancellationToken);
            for (int i = 0; i < anchors.Length; i++)
            {
                var anchorData = anchors[i];
                string materialPath = Path.Combine(prefabDir, $"AnchorMat-{i}.mat");

                int colorId = anchorData.Id % _colorValues.Length;
                string colorHex = $"#{_colorValues[colorId]}";
                ColorUtility.TryParseHtmlString(colorHex, out Color color);

                GenerateAnchor(anchorData.Pose, color, materialPath, root);
            }
        }

        private async Task ImportAnchorData_V010(string prefabDir, Transform root, GroupService.MapData map, CancellationToken cancellationToken)
        {
            var anchors = await _mapService.GetAnchorDataAsync_V010(map.MapKey, cancellationToken);
            for (int i = 0; i < anchors.Count; i++)
            {
                var anchorData = anchors[i];
                string materialPath = Path.Combine(prefabDir, $"AnchorMat-{i}.mat");
                ColorUtility.TryParseHtmlString($"#{_colorValues[i % anchors.Count]}", out Color color);
                GenerateAnchor(anchorData, color, materialPath, root);
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private async Task<GameObject> ImportMapDataAsync_V010(string prefabDir, string prefabName, GroupService.MapData map, CancellationToken cancellationToken = default)
        {
            var downloadExternalDataTask = _mapService.GetExternalDataAsync_V010(map.MapKey, cancellationToken);
            var downloadAnchorsTask = _mapService.GetAnchorDataAsync_V010(map.MapKey, cancellationToken);
            try
            {
                await Task.WhenAll(downloadExternalDataTask, downloadAnchorsTask);
            }
            catch
            {
                _processing = false;
                _retrieveSelectedMapError = true;
                throw;
            }

            _retrieveSelectedMapError = false;
            var externalDataMap = downloadExternalDataTask.Result;
            var anchors = downloadAnchorsTask.Result;

            var gameObject = new GameObject();
            gameObject.name = prefabName;

            foreach (var externalData in externalDataMap)
            {
                string meshPath = Path.Combine(prefabDir, $"Plane-{externalData.Key}.mesh");
                GenerateMesh(externalData.Value, meshPath, gameObject.transform);
            }

            for (int i = 0; i < anchors.Count; i++)
            {
                var anchorData = anchors[i];
                string materialPath = Path.Combine(prefabDir, $"AnchorMat-{i}.mat");
                ColorUtility.TryParseHtmlString($"#{_colorValues[i % anchors.Count]}", out Color color);
                GenerateAnchor(anchorData, color, materialPath, gameObject.transform);
            }

            return gameObject;
        }

        private void GenerateMesh(MeshData meshData, string meshPath, Transform parent)
        {
            var vertices = new Vector3[meshData.Vertices.Length / 3];
            var normals = new Vector3[vertices.Length];
            var normal = new Vector3(meshData.Normal.x, -meshData.Normal.y, meshData.Normal.z);

            for (int j = 0; j < meshData.Vertices.Length; j += 3)
            {
                vertices[j / 3] = new Vector3(meshData.Vertices[j], -meshData.Vertices[j + 1], meshData.Vertices[j + 2]);
                normals[j / 3] = normal;
            }

            var meshFilter = Instantiate(_planePrefab, Vector3.zero, Quaternion.identity, parent);

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = meshData.Indices;
            mesh.normals = normals;

            AssetDatabase.CreateAsset(mesh, meshPath);

            meshFilter.sharedMesh = mesh;
        }

        private void GenerateAnchor(Pose anchorData, Color color, string materialPath, Transform parent)
        {
            var position = new Vector3(anchorData.position.x, -anchorData.position.y, anchorData.position.z);
            var rotation = new Quaternion(anchorData.rotation.x, -anchorData.rotation.y, anchorData.rotation.z, -anchorData.rotation.w);
            var anchorObject = Instantiate(_anchorPrefab, position, rotation, parent);
            var renderer = anchorObject.GetComponentInChildren<Renderer>();
            var sharedMaterial = renderer.sharedMaterial;

            var newMaterial = new Material(sharedMaterial.shader);
            newMaterial.SetTexture("_MainTex", sharedMaterial.GetTexture("_MainTex"));
            newMaterial.color = color;

            AssetDatabase.CreateAsset(newMaterial, materialPath);

            renderer.sharedMaterial = newMaterial;
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }
    }
}