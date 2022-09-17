using System;
using System.Net.Http.Headers;

namespace PretiaArCloud
{
    public static class HttpRequestHeadersExtensions
    {
        public static void AddTimestamp(this HttpRequestHeaders headers)
        {
            headers.Add("timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }

        public static void AddAppKey(this HttpRequestHeaders headers, string appKey)
        {
            headers.Add("app_key", appKey);
        }

        public static void AddToken(this HttpRequestHeaders headers, string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            headers.Add("token", token);
        }
    }
}