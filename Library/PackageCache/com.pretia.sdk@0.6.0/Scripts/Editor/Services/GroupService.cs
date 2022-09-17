using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PretiaArCloud;

namespace PretiaArCloudEditor
{

    public class GroupService
    {
        public class Data
        {
            [DataMember(Name = "group_key")]
            public string Key;

            [DataMember(Name = "group_name")]
            public string Name;

            [DataMember(Name = "description")]
            public string Description;

            [DataMember(Name = "public_yn")]
            public bool? IsPublic;

            [DataMember(Name = "maps")]
            public MapData[] Maps;
        }

        public struct MapData
        {
            [DataMember(Name = "map_name")]
            public string Name;

            [DataMember(Name = "map_key")]
            public string MapKey;
        }

        private const string PATCH = "PATCH";


        private string _baseAddress => $"{PretiaSDKProjectSettings.Instance.PublicMapServerAddress}/maps/v1.1/group";
        private HttpClient _client;
        private IJsonSerializer _serializer;

        public string Token { get; set; }

        public GroupService(HttpClient client, IJsonSerializer serializer)
        {
            _client = client;
            _serializer = serializer;
        }

        private struct PostResponse
        {
            [DataMember(Name = "group_key")]
            public string Key;

            public PostResponse(string key)
            {
                Key = key;
            }
        }

        public async Task<string> PostAsync(Data data, CancellationToken cancellationToken = default(CancellationToken))
        {
            string dataJson = await IJsonSerializerExtensions.ToJsonStringAsync(_serializer, data);

            using (var request = new HttpRequestMessage(HttpMethod.Post, _baseAddress))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                request.Content = new StringContent(dataJson, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return (await _serializer.DeserializeAsync<PostResponse>(json)).Key;
            }
        }

        public class GetAllResponse : ApiResult
        {
            [DataMember(Name = "groups")]
            public Data[] Groups;
        }

        public class GetGroupedMapsResponse : ApiResult
        {
            [DataMember(Name = "grouped_maps")]
            public Data[] GroupedMaps;
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
                    var result = await _serializer.DeserializeAsync<GetAllResponse>(stream);
                    return result.Groups;
                }
            }
        }

        public async Task<Data[]> GetGroupedMapsAsync(string appKey, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{PretiaSDKProjectSettings.Instance.PublicMapServerAddress}/maps/v1.1/app/grouped-maps/{appKey}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                var response = await _client.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var result = await _serializer.DeserializeAsync<GetGroupedMapsResponse>(stream);
                    return result.GroupedMaps;
                }
            }
        }

        public async Task<Data> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/{key}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                var response = await _client.SendAsync(request, cancellationToken);

                string respJson = await response.Content.ReadAsStringAsync();
                // if (response.IsSuccessStatusCode)
                {
                    return await _serializer.DeserializeAsync<Data>(respJson);
                }
                // else
                // {
                //     throw new Exception(respJson);
                // }
            }
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseAddress}/{key}"))
            {
                request.Headers.AddTimestamp();
                request.Headers.AddToken(Token);
                var response = await _client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    string respJson = await response.Content.ReadAsStringAsync();
                    throw new Exception(respJson);
                }
            }
        }

        public async Task PutAsync(string key, Data data, CancellationToken cancellationToken = default(CancellationToken))
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
    }
}
