using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class MapContentTransform
    {
        [DataMember(Name = "pos")]
        public UnityEngine.Vector3 Position;

        [DataMember(Name = "rot")]
        public UnityEngine.Quaternion Rotation;

        [DataMember(Name = "scale")]
        public UnityEngine.Vector3 Scale;
    }

    public class MapContent
    {
        [DataMember(Name = "id")]
        public System.Guid Id;

        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "asset_path")]
        public string AssetPath;

        [DataMember(Name = "transform")]
        public MapContentTransform Transform;

        [DataMember(Name = "children")]
        public System.Guid[] Children;
    }

    public class MapContentCollection
    {
        [DataMember(Name = "contents")]
        public MapContent[] Contents;
    }
}