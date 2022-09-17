using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Utf8Json;
using PretiaArCloud;
using PretiaArCloud.Pcx;
using System.Net;

namespace PretiaArCloudEditor
{
    internal class MapService
    {
        public class Data
        {
            [DataMember(Name = "name")]
            public string Name;

            [DataMember(Name = "map_key")]
            public string MapKey;

            [IgnoreDataMember]
            public sbyte[] MapData;

            [DataMember(Name = "map_data")]
            private string Base64MapData
            {
                get
                {
                    if (MapData == null) return null;

                    var bytes = (byte[])(Array)MapData;
                    return System.Convert.ToBase64String(bytes);
                }
                set
                {
                    MapData = (sbyte[])(Array)System.Convert.FromBase64String(value);
                }
            }

            [DataMember(Name = "anchor_data")]
            [JsonFormatter(typeof(MapAnchorsFormatter))]
            public List<UnityEngine.Pose> MapAnchors;

            [IgnoreDataMember]
            public double GpsData1;

            [IgnoreDataMember]
            public double GpsData2;

            [IgnoreDataMember]
            public double GpsData3;

            [DataMember(Name = "gps_data")]
            [JsonFormatter(typeof(GpsDataFormatter))]
            public double[] GpsData
            {
                get
                {
                    return new double[] { GpsData1, GpsData2, GpsData3 };
                }
                set
                {
                    if (value != null)
                    {
                        GpsData1 = value[0];
                        GpsData2 = value[1];
                        GpsData3 = value[2];
                    }
                }
            }

            [DataMember(Name = "time_env")]
            public string TimeEnv;

            [DataMember(Name = "sdk_version")]
            public string SdkVersion;

            [DataMember(Name = "public_yn")]
            public bool? IsPublic;
        }

        private const string PATCH = "PATCH";

        private string _baseAddress => $"{PretiaSDKProjectSettings.Instance.PublicMapServerAddress}/maps/v1.1/map";
        private string _pointCloudAddress => $"{PretiaSDKProjectSettings.Instance.PublicMapServerAddress}/maps/v1.1/point-cloud";
        private string _mapContentsAddress => $"{PretiaSDKProjectSettings.Instance.PublicMapServerAddress}/maps/v1.1/platform-storage/map-content";
        private readonly HttpClient _client;
        private readonly IJsonSerializer _serializer;

        internal string Token { get; set; }

        public MapService(HttpClient client, IJsonSerializer serializer)
        {
            _client = client;
            _serializer = serializer;
        }

        public class DataWithGroupKey : Data
        {
            [DataMember(Name = "group_key")]
            public string GroupKey;

            public DataWithGroupKey()
            {
            }

            public DataWithGroupKey(string groupKey, Data data)
            {
                GroupKey = groupKey;
                Name = data.Name;
                MapData = data.MapData;
                MapAnchors = data.MapAnchors;
                GpsData1 = data.GpsData1;
                GpsData2 = data.GpsData2;
                GpsData3 = data.GpsData3;
                TimeEnv = data.TimeEnv;
                IsPublic = data.IsPublic;
                SdkVersion = data.SdkVersion;
            }
        }

        private struct PostResponse
        {
            [DataMember(Name = "map_key")]
            public ulong Key;

            public PostResponse(ulong key)
            {
                Key = key;
            }
        }

        public async Task<string> PostAsync(string groupKey, Data data, CancellationToken cancellationToken = default(CancellationToken))
        {
            string dataJson = await IJsonSerializerExtensions.ToJsonStringAsync(_serializer, new DataWithGroupKey(groupKey, data));

            using (var request = new HttpRequestMessage(HttpMethod.Post, _baseAddress))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                request.Content = new StringContent(dataJson, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                string respJson = await response.Content.ReadAsStringAsync();
                return (await _serializer.DeserializeAsync<PostResponse>(respJson)).Key.ToString();
            }
        }

        public struct GetAllResponse
        {
            [DataMember(Name = "name")]
            public string Name;

            [DataMember(Name = "map_key")]
            public string MapKey;
        }

        public async Task<Data[]> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, _baseAddress))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return await _serializer.DeserializeAsync<Data[]>(stream);
                }
            }
        }

        public async Task<Data> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/binary/{key}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return await _serializer.DeserializeAsync<Data>(stream);
                }
            }
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseAddress}/{key}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                {
                    string respJson = await response.Content.ReadAsStringAsync();
                    throw new Exception(respJson);
                }
            }
        }

        public async Task PutAsync(string key, DataWithGroupKey data, CancellationToken cancellationToken = default(CancellationToken))
        {
            string dataJson = await IJsonSerializerExtensions.ToJsonStringAsync(_serializer, data);

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"{_baseAddress}/{key}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                request.Content = new StringContent(dataJson, Encoding.UTF8, "application/json");
                var response = await _client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    string respJson = await response.Content.ReadAsStringAsync();
                    throw new Exception(respJson);
                }
            }
        }

        public async Task PatchAsync(string key, Data data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dataJson = await IJsonSerializerExtensions.ToJsonStringAsync(_serializer, data);

            using (var request = new HttpRequestMessage(new HttpMethod(PATCH), $"{_baseAddress}/{key}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                request.Content = new StringContent(dataJson, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    string respJson = await response.Content.ReadAsStringAsync();
                    throw new Exception(respJson);
                }
            }
        }

        [Obsolete("This function is obsolete, and only exists for supporting v0.1.0 anchor_data format. Please use GetAnchorDataAsync instead")]
        internal async Task<List<UnityEngine.Pose>> GetAnchorDataAsync_V010(string mapKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/anchor/{mapKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var getAnchorResponse = await _serializer.DeserializeAsync<GetAnchorResponse>(jsonString, cancellationToken);
                var result = await _serializer.DeserializeAsync<List<UnityEngine.Pose>>(getAnchorResponse.AnchorDataString);
                return result;
            }
        }

        internal async Task<AnchorData[]> GetAnchorDataAsync(string mapKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/anchor/{mapKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var getAnchorResponse = await _serializer.DeserializeAsync<GetAnchorResponse>(jsonString, cancellationToken);
                var result = await _serializer.DeserializeAsync<ArrayOfAnchors>(getAnchorResponse.AnchorDataString);
                return result.Anchors;
            }
        }

        [Obsolete("This function is obsolete, and only exists for supporting v0.1.0 external_data format. Please use GetExternalDataAsync instead")]
        internal async Task<Dictionary<int, MeshData>> GetExternalDataAsync_V010(string mapKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/external/{mapKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var getExternalDataResponse = await _serializer.DeserializeAsync<GetExternalDataResponse>(jsonString, cancellationToken);
                var result = await _serializer.DeserializeAsync<Dictionary<int, MeshData>>(getExternalDataResponse.ExternalDataString);
                return result;
            }
        }

        internal async Task<MeshData[]> GetExternalDataAsync(string mapKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/external/{mapKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var getExternalDataResponse = await _serializer.DeserializeAsync<GetExternalDataResponse>(jsonString, cancellationToken);
                var result = await _serializer.DeserializeAsync<ArrayOfMeshes>(getExternalDataResponse.ExternalDataString);
                return result.Meshes;
            }
        }

        internal async Task<SdkVersion> GetSdkVersionAsync(string mapKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/version/{mapKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var getSdkVersionResponse = await _serializer.DeserializeAsync<GetSdkVersionResponse>(jsonString, cancellationToken);
                return await _serializer.DeserializeAsync<SdkVersion>(getSdkVersionResponse.SdkVersionString, cancellationToken);
            }
        }

        internal async Task<string> GetPointCloudData(string mapKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_pointCloudAddress}/{mapKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                string jsonString = await response.Content.ReadAsStringAsync();
                var getPointCloudResponse = await _serializer.DeserializeAsync<GetPointCloudResponse>(jsonString, cancellationToken);
                return getPointCloudResponse.PointCloudString;
            }
        }

        internal async Task<byte[]> GetMapContentsAsync(string appKey, string mapKey, CancellationToken cancellationToken = default)
        {
            string downloadUrl = default;
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_mapContentsAddress}/{appKey}?path={mapKey}.json"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                request.Headers.Add("API-KEY", appKey);

                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var getMapContentsResponse = await _serializer.DeserializeAsync<MapContentsResponse>(stream);
                    if (getMapContentsResponse.StatusCode != 0)
                    {
                        UnityEngine.Debug.LogError($"Unable to retrieve custom objects get urls: {getMapContentsResponse.Message}");
                        return null;
                    }

                    downloadUrl = getMapContentsResponse.Urls[$"{mapKey}.json"];
                }
            }

            using (var request = new HttpRequestMessage(HttpMethod.Get, downloadUrl))
            {
                var response = await _client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        internal class GetAnchorResponse : ApiResult
        {
            [DataMember(Name = "anchor_data")]
            public string AnchorDataString;
        }

        internal class ArrayOfAnchors
        {
            [DataMember(Name = "anchors")]
            public AnchorData[] Anchors;
        }

        internal class ArrayOfMeshes
        {
            [DataMember(Name = "plane_meshes")]
            public MeshData[] Meshes;
        }

        internal class GetExternalDataResponse : ApiResult
        {
            [DataMember(Name = "external_data")]
            public string ExternalDataString;
        }

        internal class GetSdkVersionResponse : ApiResult
        {
            [DataMember(Name = "sdk_version")]
            public string SdkVersionString;
        }

        internal class GetPointCloudResponse : ApiResult
        {
            [DataMember(Name = "point_cloud")]
            public string PointCloudString;
        }

        internal class MapContentsResponse : ApiResult
        {
            [DataMember(Name = "urls")]
            public Dictionary<string, string> Urls;
        }
    }
}
