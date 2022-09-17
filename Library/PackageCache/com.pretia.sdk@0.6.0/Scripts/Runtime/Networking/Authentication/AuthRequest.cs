using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class AuthRequest
    {
        [DataMember(Name = "user_name")]
        public string Username;

        [DataMember(Name = "password")]
        public string Password;
    }
}