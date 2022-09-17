using System.Runtime.Serialization;
using UnityEngine;

namespace PretiaArCloudEditor
{
    internal struct MeshData
    {
        [DataMember(Name = "id")]
        public int Id;

        [DataMember(Name = "indexes")]
        public int[] Indices;

        [DataMember(Name = "normal")]
        public float[] NormalData;

        public Vector3 Normal => new Vector3(NormalData[0], NormalData[1], NormalData[2]);

        [DataMember(Name = "vertices")]
        public float[] Vertices;
    }
}