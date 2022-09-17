using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace PretiaArCloud.Pcx
{
    public class PretiaPointCloudData
    {
        [DataMember(Name = "points")]
        public LandmarkData[] Points;

        public List<Vector3> GetVertices()
        {
            var vertices = new List<Vector3>(Points.Length);
            foreach (var point in Points)
            {
                vertices.Add(new Vector3(point.Position[0], -point.Position[1], point.Position[2]));
            }

            return vertices;
        }

        public List<Color32> GetColors()
        {
            var colors = new List<Color32>(Points.Length);
            foreach (var point in Points)
            {
                colors.Add(Color.white);
            }

            return colors;
        }
    }
}