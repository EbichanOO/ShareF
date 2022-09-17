using System.IO;
using Pcx;

using UnityEngine;
using System;
using UnityEngine.Rendering;
using System.Linq;
using Utf8Json;

namespace PretiaArCloud.Pcx
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "map")]
    public class MapImporter : PlyImporter
    {
        protected override Mesh ImportAsMesh(string path)
        {
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var pretiaMapData = JsonSerializer.Deserialize<PretiaPointCloudData>(stream);

                var mesh = new Mesh();
                mesh.name = Path.GetFileNameWithoutExtension(path);

                mesh.indexFormat = pretiaMapData.Points.Length > 65535 ?
                    IndexFormat.UInt32 : IndexFormat.UInt16;

                mesh.SetVertices(pretiaMapData.GetVertices());
                mesh.SetColors(pretiaMapData.GetColors());

                mesh.SetIndices(
                    Enumerable.Range(0, pretiaMapData.Points.Length).ToArray(),
                    MeshTopology.Points, 0
                );

                mesh.UploadMeshData(true);
                return mesh;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }

        protected override global::Pcx.PointCloudData ImportAsPointCloudData(string path)
        {
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var pretiaMapData = JsonSerializer.Deserialize<PretiaPointCloudData>(stream);

                var data = ScriptableObject.CreateInstance<global::Pcx.PointCloudData>();
                data.Initialize(pretiaMapData.GetVertices(), pretiaMapData.GetColors());
                data.name = Path.GetFileNameWithoutExtension(path);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }

        protected override BakedPointCloud ImportAsBakedPointCloud(string path)
        {
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var pretiaMapData = JsonSerializer.Deserialize<PretiaPointCloudData>(stream);

                var data = ScriptableObject.CreateInstance<BakedPointCloud>();
                data.Initialize(pretiaMapData.GetVertices(), pretiaMapData.GetColors());
                data.name = Path.GetFileNameWithoutExtension(path);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }
    }
}