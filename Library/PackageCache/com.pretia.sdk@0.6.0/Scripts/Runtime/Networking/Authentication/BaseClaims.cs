using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class BaseClaims
    {
        [DataMember(Name = "exp")]
        public long ExpiredAt;

        [DataMember(Name = "iat")]
        public long IssuedAt;
    }
}