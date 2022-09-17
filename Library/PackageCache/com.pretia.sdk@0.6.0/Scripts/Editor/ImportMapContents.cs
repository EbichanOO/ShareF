using System;
using System.Collections.Generic;
using System.IO;
using PretiaArCloud;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace PretiaArCloudEditor
{
    public class ImportMapContents
    {
        [MenuItem(itemName: "Assets/Create/Pretia ArCloud/Prefab from Map Contents", isValidateFunction: true)]
        private static bool ValidateCreateMapContentsPrefab()
        {
            if (Selection.activeInstanceID == 0)
                return false;

            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            return path.EndsWith(".map-contents.json");
        }

        [MenuItem(itemName: "Assets/Create/Pretia ArCloud/Prefab from Map Contents")]
        private static void CreateMapContentsPrefab()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            CreateMapContentsPrefab(path);
        }

        public static void CreateMapContentsPrefab(string path)
        {
            string fileName = "ContentRoot.prefab";
            string prefabPath = Path.Combine(Path.GetDirectoryName(path), fileName);
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            byte[] bytes = asset.bytes;
            var collection = new Utf8JsonEditorSerializer().Deserialize<MapContentCollection>(bytes);

            Dictionary<Guid, Transform> childParentMap = new Dictionary<Guid, Transform>();

            GameObject root = SpawnGameObject(collection.Contents[0]);
            SetupMapContent(root, collection.Contents[0], childParentMap);
            var controller = root.AddComponent<MapContentsController>();
            controller.MapContentsAsset = asset;

            for (int i = 1; i < collection.Contents.Length; i++)
            {
                var content = collection.Contents[i];
                var go = SpawnGameObject(content);
                if (go != null)
                    SetupMapContent(go, content, childParentMap);
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(root, prefabPath, InteractionMode.AutomatedAction);
            GameObject.DestroyImmediate(root);
        }

        private static GameObject SpawnGameObject(MapContent mapContent)
        {
            GameObject go = default;

            if (string.IsNullOrEmpty(mapContent.AssetPath))
            {
                go = new GameObject();
            }
            else
            {
                if (mapContent.AssetPath.StartsWith("Primitive"))
                {
                    string primitiveTypeString = Path.GetFileNameWithoutExtension(mapContent.AssetPath);
                    if (primitiveTypeString.Equals("Directional Light"))
                    {
                        go = new GameObject();
                        var light = go.AddComponent<Light>();
                        light.type = LightType.Directional;
                        light.shadows = LightShadows.Soft;
                        light.shadowStrength = .4f;
                    }
                    else if (primitiveTypeString.Equals("Point Light"))
                    {
                        go = new GameObject();
                        var light = go.AddComponent<Light>();
                        light.type = LightType.Point;
                        light.shadows = LightShadows.None;
                    }
                    else
                    { 
                        PrimitiveType primitiveType = default;
                        switch (primitiveTypeString)
                        {
                            case "Sphere":
                                primitiveType = PrimitiveType.Sphere;
                                break;
                            case "Capsule":
                                primitiveType = PrimitiveType.Capsule;
                                break;
                            case "Cylinder":
                                primitiveType = PrimitiveType.Cylinder;
                                break;
                            case "Cube":
                                primitiveType = PrimitiveType.Cube;
                                break;
                            case "Plane":
                                primitiveType = PrimitiveType.Plane;
                                break;
                            case "Quad":
                                primitiveType = PrimitiveType.Quad;
                                break;
                        }

                        go = GameObject.CreatePrimitive(primitiveType);
                    }
                }
                else if (mapContent.AssetPath.StartsWith("PretiaCustomObjects/"))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine("Assets", "Pretia", mapContent.AssetPath));
                    if (prefab == null)
                    {
                        go = new GameObject(mapContent.Name);
                    }
                    else
                    {
                        go = GameObject.Instantiate(prefab);
                    }
                }
            }

            go.name = mapContent.Name;
            return go;
        }

        private static void SetupMapContent(
            GameObject go,
            MapContent mapContent,
            Dictionary<Guid, Transform> childParentMap)
        {
            go.name = mapContent.Name;

            if (childParentMap.TryGetValue(mapContent.Id, out Transform parent))
            {
                go.transform.SetParent(parent);
            }

            go.transform.localPosition = mapContent.Transform.Position;
            go.transform.localRotation = mapContent.Transform.Rotation;
            go.transform.localScale = mapContent.Transform.Scale;

            foreach (var childId in mapContent.Children)
            {
                childParentMap[childId] = go.transform;
            }

            var mapContentComponent = go.AddComponent<MapContentComponent>();
            mapContentComponent.AssetPath = mapContent.AssetPath;
            mapContentComponent.Id = mapContent.Id.ToString();
        }
    }
}