using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PretiaArCloud.RecordingPlayback.FileTransfer.Models
{
    public class FileDetail
    {
        [DataMember(Name = "name")]
        public string name;

        [DataMember(Name = "relativePath")]
        public string relativePath;

        [DataMember(Name = "isDirectory")]
        public bool isDirectory;

        [DataMember(Name = "fileSize")]
        public long fileSize;

        [DataMember(Name = "fileDetails")]
        public List<FileDetail> fileDetails;
    }
}