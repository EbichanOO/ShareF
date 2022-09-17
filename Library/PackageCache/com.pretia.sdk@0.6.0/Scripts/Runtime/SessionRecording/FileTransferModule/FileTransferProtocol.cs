using System;

namespace PretiaArCloud.RecordingPlayback.FileTransfer
{
    public enum FileTransferProtocol
    {
        Error,
        ListFiles,
        GetFile,
        RemoveFile
    }
}