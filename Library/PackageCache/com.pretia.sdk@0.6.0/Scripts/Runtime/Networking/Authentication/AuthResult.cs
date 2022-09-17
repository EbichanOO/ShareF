using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class AuthResult : ApiResult
    {
        [DataMember(Name = "token")]
        public string Token;

        [DataMember(Name = "display_name")]
        public string DisplayName;
    }
}