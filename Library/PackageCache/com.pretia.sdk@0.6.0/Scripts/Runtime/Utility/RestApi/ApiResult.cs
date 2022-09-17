using System.Runtime.Serialization;

namespace PretiaArCloud
{
    public class ApiResult
    {
        [DataMember(Name = "error")]
        public string Message;

        [DataMember(Name = "error_code")]
        public int StatusCode;
    }
}