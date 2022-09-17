using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class MapSelectionNetworkRequest
    {
        [DataMember(Name = "private_yn")]
        public bool OnlyPrivateMaps;

        [DataMember(Name = "public_yn")]
        public bool OnlyPublicMaps;

        [DataMember(Name = "groups")]
        public string[] Groups;

        [DataMember(Name = "merge_status")]
        public string MergeStatus;

        [DataMember(Name = "gps")]
        public PretiaArCloud.MapGpsData Gps;

        [DataMember(Name = "gps_threshold")]
        public float GpsThreshold;
        
        [DataMember(Name = "time_env")]
        public string TimeEnv;

        [DataMember(Name = "time_threshold")]
        public float TimeThreshold;

        [DataMember(Name = "sort_key")]
        public string SortKey;
    }
}