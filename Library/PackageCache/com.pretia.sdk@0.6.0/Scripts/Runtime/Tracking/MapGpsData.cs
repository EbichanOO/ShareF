using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class MapGpsData
    {
        private double latitude_, longitude_, altitude_;
        private float bearing_, accuracy_, vertical_accuracy_;
        private int num_sats_;
        private ulong timestamp_;

        [Utf8Json.SerializationConstructor]
        public MapGpsData() { }

        public MapGpsData(
            double latitude,
            double longitude,
            double altitude,
            float bearing,
            float accuracy,
            float vertical_accuracy)
        {
            latitude_ = latitude;
            longitude_ = longitude;
            altitude_ = altitude;
            bearing_ = bearing;
            accuracy_ = accuracy;
            vertical_accuracy_ = vertical_accuracy;
        }

        public MapGpsData(double latitude, double longitude, double altitude,
            float bearing, ulong timestamp, float accuracy, float vertical_accuracy,
            int num_sats)
        {
            latitude_ = latitude;
            longitude_ = longitude;
            altitude_ = altitude;
            bearing_ = bearing;
            timestamp_ = timestamp;
            accuracy_ = accuracy;
            vertical_accuracy_ = vertical_accuracy;
            num_sats_ = num_sats;
        }

        [DataMember(Name = "lat")]
        public double Latitude
        {
            get { return latitude_; }
        }

        [DataMember(Name = "long")]
        public double Longitude
        {
            get { return longitude_; }
        }

        [DataMember(Name = "alt")]
        public double Altitude
        {
            get { return altitude_; }
        }

        [DataMember(Name = "bear")]
        public float Bearing
        {
            get { return bearing_; }
        }

        [IgnoreDataMember]
        public ulong Timestamp
        {
            get { return timestamp_; }
        }

        [DataMember(Name = "acc")]
        public float Accuracy
        {
            get { return accuracy_; }
        }

        [DataMember(Name = "vacc")]
        public float VerticalAccuracy
        {
            get { return vertical_accuracy_; }
        }

        [IgnoreDataMember]
        public int NumSats
        {
            get { return num_sats_; }
        }
    }
}
