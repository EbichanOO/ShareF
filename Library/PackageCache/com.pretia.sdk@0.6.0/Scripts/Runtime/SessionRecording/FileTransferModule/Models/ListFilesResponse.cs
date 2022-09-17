using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PretiaArCloud.RecordingPlayback.FileTransfer.Models
{

    public class ListFilesResponse
    {
        [DataMember(Name = "fileDetails")]
        public List<FileDetail> fileDetails;
    }
}