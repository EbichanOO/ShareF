using System.Runtime.Serialization;

namespace PretiaArCloud.Pcx
{
    public class LandmarkData
    {
        [DataMember(Name = "pos")]
        public float[] Position;
    }
}