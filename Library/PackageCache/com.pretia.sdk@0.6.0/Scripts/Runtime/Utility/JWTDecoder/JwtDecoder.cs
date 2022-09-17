using System;
using System.Text;

namespace PretiaArCloud
{
    public class JwtDecoder : IJwtDecoder
    {
        public string Decode(string token)
        {
            string[] parts = token.Split('.');
            string payload = parts[1];
            switch (payload.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    payload += "==";
                    break;
                case 3:
                    payload += "=";
                    break;
                case 1:
                    throw new System.Exception("Illegal base64url string");
            }

            byte[] bytes = Convert.FromBase64String(payload);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}