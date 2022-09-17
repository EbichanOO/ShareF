using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PretiaArCloud;

namespace PretiaArCloudEditor
{
    internal class DeveloperService
    {
        private string _baseAddress => PretiaSDKProjectSettings.Instance.DeveloperServerAddress;
        private readonly HttpClient _httpClient;
        private readonly IJwtDecoder _jwtDecoder;
        private readonly IJsonSerializer _serializer;

        public DeveloperService(
            HttpClient httpClient,
            IJwtDecoder jwtDecoder,
            IJsonSerializer serializer)
        {
            _httpClient = httpClient;
            _serializer = serializer;
            _jwtDecoder = jwtDecoder;
        }

        internal async Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            return await AuthAsync($"{_baseAddress}/dev/v1.1/login", username, password, cancellationToken);
        }

        private async Task<AuthResult> AuthAsync(string endpoint, string username, string password, CancellationToken cancellationToken = default)
        {
            var dataJson = _serializer.ToJsonString(new AuthRequest { Username = username, Password = password });
            using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
            {
                request.Headers.AddTimestamp();
                request.Content = new StringContent(dataJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request, cancellationToken);

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return _serializer.Deserialize<AuthResult>(stream);
                }
            }
        }

        internal bool IsTokenValid(string token)
        {
            string jsonString = _jwtDecoder.Decode(token);
            var claims = _serializer.Deserialize<DeveloperClaims>(jsonString);
            return claims.ExpiredAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}