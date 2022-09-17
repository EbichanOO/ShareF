using System.Runtime.Serialization;
using UnityEngine;

namespace PretiaArCloudEditor
{
    internal struct AnchorData
    {
        [DataMember(Name = "id")]
        public int Id;

        [DataMember(Name = "position")]
        public Vector3 Position;

        [DataMember(Name = "rotation")]
        public Quaternion Rotation;

        public Pose Pose => new Pose(Position, Rotation);
    }
}