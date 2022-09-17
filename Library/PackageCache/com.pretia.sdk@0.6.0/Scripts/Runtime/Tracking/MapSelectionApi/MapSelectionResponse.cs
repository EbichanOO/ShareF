using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class MapSelectionResponse : ApiResult
    {
        public struct SelectedMap 
        {
            [DataMember(Name = "gps_distance")]
            public float GpsDistance;

            [DataMember(Name = "time_distance")]
            public float TimeDistance;

            [DataMember(Name = "map_key")]
            public string MapKey;
        }

        [DataMember(Name = "maps")]
        public SelectedMap[] SelectedMaps;
    }
}