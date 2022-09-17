using System;
using System.IO;
using System.Text;
using PretiaArCloud.RecordingPlayback.Editor.FileTransfer;
using PretiaArCloud.RecordingPlayback.FileTransfer.Models;
using PretiaArCloud.RecordingPlayback.FileTransfer;
using PretiaArCloud;

namespace PretiaArCloudEditor.RecordingPlayback
{
    /// <summary>
    /// Downloader
    /// </summary>
    public class PlaybackDownloader : IDisposable
    {
#region Consts

        /// <summary>
        /// Protocol used for list downloadable content.
        /// </summary>
        private const int ListProtocol = (int) FileTransferProtocol.ListFiles;
        
        /// <summary>
        /// Protocol used to download content.
        /// </summary>
        private const int DownloadProtocol = (int) FileTransferProtocol.GetFile;
        
        /// <summary>
        /// Protocol used to download content.
        /// </summary>
        private const int RemoveProtocol = (int) FileTransferProtocol.RemoveFile;

        #endregion

        #region Preperties

        /// <summary>
        /// Serializer used to serialize/deserialize JSON.
        /// </summary>
        private IJsonSerializer Serializer { get; }

        /// <summary>
        /// Client for the tcp connection.
        /// </summary>
        private TcpServiceClient Client { get; } 
            = new TcpServiceClient();
        
        /// <summary>
        /// IP of the device where to request info.
        /// </summary>
        internal string IP { get; set; }
        
        /// <summary>
        /// Port of the connection to request info.
        /// </summary>
        internal int Port { get; set; }
        
        /// <summary>
        /// Path of the file to download relative to the device.
        /// </summary>
        private string RelativePath { get; set; }
        
        /// <summary>
        /// Index of the current piece being downloaded.
        /// </summary>
        private int CurrentPiece { get; set; }
        
        /// <summary>
        /// Total amount of pieces to download.
        /// </summary>
        private int TotalPieces { get; set; }
        
        /// <summary>
        /// Stream used to write the downloaded content into a file.
        /// </summary>
        private Stream Stream { get; set; }
        
        /// <summary>
        /// Method called after the Update list process finishes.
        /// </summary>
        private Action<ListFilesResponse> UpdateFilesCallback { get; set; }
        
        /// <summary>
        /// Method called to update the percentage of the downloading process. 
        /// </summary>
        private Action<float> DownloadFileProgressCallback { get; set; }
        
        /// <summary>
        /// Delegate called on error.
        /// </summary>
        public Action<Exception> OnError { get; set; }
        
        /// <summary>
        /// Delegate called on client status changed.
        /// </summary>
        public Action<TcpServiceState> OnStateChange { get; set; }

#endregion

#region Methods
        public PlaybackDownloader(IJsonSerializer serializer)
        {
            Serializer = serializer;
        }
        
        /// <summary>
        /// Downloads the file at the given relative path to the
        /// destination path.
        /// </summary>
        /// <param name="relativePath">Path of the file to download relative
        /// to the device.</param>
        /// <param name="dest">Path where to put write the downloaded file.
        /// </param>
        /// <param name="callback">Method called to update the percentage of
        /// the downloading process.</param>
        public void DownloadFile(
            string relativePath,
            string dest, 
            Action<float> callback)
        {
            RelativePath = relativePath;
            CurrentPiece = 1;
            TotalPieces = int.MaxValue;
            Stream = new FileStream(dest, FileMode.Create, FileAccess.Write);
            
            Connect();
            if (Client.GetState() != TcpServiceState.Connected)
            {
                Disconnect();
                return;
            }

            DownloadFileProgressCallback = callback;
            DownloadNextPiece();
        }
        
        /// <summary>
        /// Downloads a single piece of the hole file.
        /// </summary>
        private void DownloadNextPiece()
        {
            var pathBytes = Encoding.UTF8.GetBytes(RelativePath);
            var body = new byte[sizeof(int) + pathBytes.Length];
            SpanBasedBitConverter.TryWriteBytes(body, CurrentPiece);
            Array.Copy(pathBytes, 0, body, sizeof(int), pathBytes.Length);
            Client.SendRequest(DownloadProtocol, body, OnDownloadedPiece);
        }
        
        /// <summary>
        /// Event called when a piece has been completely downloaded.
        /// </summary>
        /// <param name="protocol">Protocol used.</param>
        /// <param name="body">Body of the response.</param>
        /// <param name="e">Exception of the response.</param>
        private void OnDownloadedPiece(int protocol, byte[] body, Exception e)
        {
            if (e != null) {  Disconnect(); OnError?.Invoke(e); return; }

            if (protocol == (int) FileTransferProtocol.Error)
            {
                var json = Encoding.UTF8.GetString(body);
                var error = Serializer.Deserialize<Error>(json);
                Disconnect();
                if (error != null)
                {
                    OnError?.Invoke(new SystemException(error.message));
                }
                return;
            }
            
            if (protocol != (int) FileTransferProtocol.GetFile)
            {
                Disconnect();
                return;
            }
            
            var currentPieceBytes = new byte[sizeof(int)];
            Array.Copy(body, currentPieceBytes, currentPieceBytes.Length);
            var currentPiece = SpanBasedBitConverter.ToInt32(currentPieceBytes);

            if (currentPiece != CurrentPiece)
            {
                Disconnect();
                return;
            }
            
            var totalPiecesBytes = new byte[sizeof(int)];
            Array.Copy(body, sizeof(int), totalPiecesBytes, 0, totalPiecesBytes.Length);
            TotalPieces = SpanBasedBitConverter.ToInt32(totalPiecesBytes);

            var fileBytes = new byte[body.Length - sizeof(int) * 2];
            Array.Copy(body, sizeof(int) * 2, fileBytes, 0, fileBytes.Length);
            Stream.Write(fileBytes, 0, fileBytes.Length);

            DownloadFileProgressCallback?.Invoke(currentPiece/(float)TotalPieces);
            if (CurrentPiece >= TotalPieces || Client.GetState() != TcpServiceState.Connected)
            {
                Disconnect();
                return;
            }
            
            CurrentPiece++;
            DownloadNextPiece();
        }

        /// <summary>
        /// Removes a file from the device.
        /// </summary>
        /// <param name="relativePath">Relative path to the file.</param>
        /// <param name="callback">Method to call after delete file.</param>
        public void RemoveFile(string relativePath, Action<ListFilesResponse> callback)
        {
            Connect();
            
            if (Client.GetState() != TcpServiceState.Connected)
            {
                Disconnect();
                return;
            }

            var pathBytes = Encoding.UTF8.GetBytes(relativePath);
            UpdateFilesCallback = callback;
            Client.SendRequest(RemoveProtocol, pathBytes, OnUpdatedList);
        }
        
        /// <summary>
        /// Lists the files possible to download throw the given callback.
        /// </summary>
        /// <param name="callback">Method called with the list o</param>
        public void ListFiles(Action<ListFilesResponse> callback)
        {
            Connect();
            
            if (Client.GetState() != TcpServiceState.Connected)
            {
                Disconnect();
                return;
            }

            UpdateFilesCallback = callback;
            Client.SendRequest(ListProtocol, Array.Empty<byte>(), OnUpdatedList);
        }
        
        /// <summary>
        /// Event called to receive the response of listed elements to download.
        /// </summary>
        /// <param name="protocol">Protocol used.</param>
        /// <param name="body">Body of the response.</param>
        /// <param name="e">Exception of the response.</param>
        private void OnUpdatedList(int protocol, byte[] body, Exception e)
        {
            Disconnect();
            
            if (e != null) { OnError?.Invoke(e); return; }
            if (protocol != (int) FileTransferProtocol.ListFiles) { return; }
            if(UpdateFilesCallback == null) { return; }
            
            var json = Encoding.UTF8.GetString(body);
            var files = Serializer.Deserialize<ListFilesResponse>(json);
            UpdateFilesCallback.Invoke(files);
        }
        
        /// <summary>
        /// Connects to the client.
        /// </summary>
        private void Connect()
        {
            Client.OnStateChange += OnStateChangeCalled; 
            Client.OnError += OnErrorCalled;
            Client.Connect(IP, Port);
        }

        /// <summary>
        /// Disconnects from the client.
        /// </summary>
        private void Disconnect()
        {
            Client.Disconnect();
            Client.OnStateChange -= OnStateChangeCalled; 
            Client.OnError -= OnErrorCalled;
            
            if (Stream == null)
            {
                return;
            }
            
            Stream.Close();
            Stream.Dispose();
            Stream = null;
            RelativePath = string.Empty;
            CurrentPiece = 0;
            TotalPieces = 0;
        }

        private void OnErrorCalled(Exception e) => 
            OnError?.Invoke(e);

        private void OnStateChangeCalled(TcpServiceState state) =>
            OnStateChange?.Invoke(state);
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

#endregion

    }
}
