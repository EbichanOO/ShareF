using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class EnduserClaims : BaseClaims
    {
        [DataMember(Name = "payload")]
        public string Payload;
    }
}