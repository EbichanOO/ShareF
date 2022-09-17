using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static MapSelectionCriteria;

namespace PretiaArCloud
{
    /// <summary>
    /// Main API to select maps on the server based on a list of criteria
    /// </summary>
    public static class MapSelection
    {   
        /// <summary>
        /// Start the device location service, necessary to select maps
        /// </summary>
        public static IEnumerator StartLocationService()
        {
            yield return LocationProvider.StartLocationService();
        }

        /// <summary>
        /// Select maps on the server based on the given criteria.
        /// The location service must be running or initializing.
        /// If the location service is initializing, this process will wait up to a few seconds until the location data are available.
        /// </summary>
        public static async Task<MapSelectionResponse> SelectMapsAsync(MapSelectionCriteria criteria, CancellationToken cancellationToken = default)
        {
            // Make sure that the location service is running or initializing
            var locationStatus = UnityEngine.Input.location.status;
            if(locationStatus != LocationServiceStatus.Running && locationStatus != LocationServiceStatus.Initializing)
            {
                Debug.Log("Select Maps Error: Location service has not been started");
                return null;
            }

            // Wait for location service initialization
            for (int i = 0; i < 30 && UnityEngine.Input.location.status != LocationServiceStatus.Running; i++)
            {
                await Task.Delay(500);
            }

            if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
            {
                Debug.Log("Select Maps Error: Location service initialization timed out");
                return null;
            }
            
            // Get location data
            var location = UnityEngine.Input.location.lastData;
            var gps = new MapGpsData
            (
                latitude: location.latitude,
                longitude: location.longitude,
                altitude: location.altitude,
                bearing: 0,
                timestamp: 0L,
                accuracy: location.horizontalAccuracy,
                vertical_accuracy: location.verticalAccuracy,
                num_sats: 0
            );

            // Setup request

            bool privateMaps = (criteria.Public == PublicStatus.ANY || criteria.Public == PublicStatus.PRIVATE_ONLY);
            bool publicMaps = (criteria.Public == PublicStatus.ANY || criteria.Public == PublicStatus.PUBLIC_ONLY);

            var groups = (criteria.GroupKeys == null) ? new string[0] : criteria.GroupKeys.ToArray();

            string mergeStatus = "";
            switch (criteria.Merged)
            {
                case MergeStatus.ANY:
                mergeStatus = "";
                break;
                case MergeStatus.MERGED_MAPS_ONLY:
                mergeStatus = "merged_only";
                break;
                case MergeStatus.NOT_MERGED_MAPS_ONLY:
                mergeStatus = "not_merged";
                break;
                case MergeStatus.MERGED_MAPS_IF_EXISTS:
                mergeStatus = "merged_if_exists";
                break;

                default:
                break;
            }

            var time = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");

            string sortKey = "";
            switch (criteria.Sorting)
            {
                case ResultOrder.NO_SORTING:
                sortKey = "";
                break;
                case ResultOrder.SORT_BY_GPS_DISTANCE:
                sortKey = "gps";
                break;
                case ResultOrder.SORT_BY_TIME:
                sortKey = "time";
                break;

                default:
                break;
            }

            var requestBody = new MapSelectionNetworkRequest
            {
                OnlyPrivateMaps = privateMaps,
                OnlyPublicMaps = publicMaps,
                Groups = groups,
                MergeStatus = mergeStatus,
                Gps = gps,
                GpsThreshold = criteria.GpsThreshold,
                TimeEnv = time,
                TimeThreshold = criteria.TimeThreshold,
                SortKey = sortKey,
            };

            // Send request and return response
            var response = await MapSelectionNetworkApi.GetAsync(requestBody, cancellationToken);
            return response;
        }

        /// <summary>
        /// Stop the device location service
        /// </summary>
        public static void StopLocationService()
        {
            LocationProvider.StopLocationUpdates();
        }
    }
}
