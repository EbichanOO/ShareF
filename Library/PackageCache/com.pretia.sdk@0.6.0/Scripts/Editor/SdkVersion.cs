using System.Runtime.Serialization;

namespace PretiaArCloudEditor
{
    public class SdkVersion
    {
        [DataMember(Name = "map")]
        public string MapVersion;

        [DataMember(Name = "map_orig")]
        public string MapOriginalVersion;

        [DataMember(Name = "anchor_data")]
        public string AnchorDataVersion;

        [DataMember(Name = "gps_data")]
        public string GpsDataVersion;

        [DataMember(Name = "external_data")]
        public string ExternalDataVersion;

        [DataMember(Name = "time_env")]
        public string TimeEnvVersion;

        [DataMember(Name = "android")]
        public string AndroidLibVersion;

        [DataMember(Name = "ios")]
        public string IOSLibVersion;

        [DataMember(Name = "unity")]
        public string UnitySdkVersion;

        [DataMember(Name = "scanner_app")]
        public string ScannerAppVersion;
    }
}
