using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud
{
    public static class MapSelectionNetworkApi
    {
        private static string _baseAddress => $"{PretiaSDKProjectSettings.Instance.PublicMapServerAddress}/maps/v1.1/public/select";

        private static IJsonSerializer _jsonSerializer;
        private static HttpClient _httpClient;

        static MapSelectionNetworkApi()
        {
            _jsonSerializer = Utf8JsonSerializer.Instance;
            _httpClient = new HttpClient();
        }

        public static async Task<MapSelectionResponse> GetAsync(MapSelectionNetworkRequest requestBody, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(PretiaSDKProjectSettings.Instance.AppKey))
            {
                throw new System.Exception("Unable to perform map selection request. AppKey is null or empty.");
            }

            return await GetAsync(requestBody, PretiaSDKProjectSettings.Instance.AppKey, cancellationToken);
        }

        public static async Task<MapSelectionResponse> GetAsync(MapSelectionNetworkRequest requestBody, string appKey, CancellationToken cancellationToken = default)
        {
            byte[] parameterBytes = await _jsonSerializer.SerializeAsync(requestBody);
            string base64Parameter = System.Convert.ToBase64String(parameterBytes);

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseAddress}/{base64Parameter}"))
            {
                request.Headers.Add("app-key", appKey);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return await _jsonSerializer.DeserializeAsync<MapSelectionResponse>(json);
            }
        }
    }
}