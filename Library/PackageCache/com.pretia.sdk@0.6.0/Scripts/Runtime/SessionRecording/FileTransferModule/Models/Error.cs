using System;
using System.Runtime.Serialization;

namespace PretiaArCloud.RecordingPlayback.FileTransfer.Models
{
    public class Error
    {
        public enum Code : Int32
        {
            FileNotFound,
            InvalidSequnce,
        }

        [DataMember(Name = "code")]
        public Code code;

        [DataMember(Name = "message")]
        public string message;

        public Error(Code code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}