using System;
using System.IO;
using System.Net.Sockets;
using PretiaArCloud.RecordingPlayback.FileTransfer.Models;

namespace PretiaArCloud.RecordingPlayback.Editor.FileTransfer
{
    internal class TcpServiceClient
    {
#region Properties

        private TcpClient Client { get; set; }

        private NetworkStream Stream { get; set; }

        private TcpServiceState State { get; set; } 
            = TcpServiceState.Disconnected;
        
#endregion

#region Events
        
        internal event Action<TcpServiceState> OnStateChange;

        internal event Action<Exception> OnError;

#endregion

#region Methods
        
        internal void Connect(string ipAddress, int port)
        {
            try
            {
                Client = new TcpClient(ipAddress, port);
                Stream = Client.GetStream();
                SetState(TcpServiceState.Connected);
            }
            catch (Exception e)
            {
                if (Client != null)
                {
                    Client.Close();
                    Client = null;
                }

                if (Stream != null)
                {
                    Stream.Close();
                    Stream = null;
                }

                OnError?.Invoke(e);
            }
        }

        internal void SendRequest(
            int protocol, 
            byte[] body,
            Action<int, byte[], Exception> callback)
        {
            try
            {
                var packetSize = body.Length + sizeof(int) * 2;
                var packet = new byte[packetSize];

                var span = packet.AsSpan();
                var protocolSpan = span.Slice(sizeof(int), sizeof(int));

                SpanBasedBitConverter.TryWriteBytes(span, packetSize);
                SpanBasedBitConverter.TryWriteBytes(protocolSpan, protocol);
                Array.Copy(body, 0, packet, sizeof(int) * 2, body.Length);

                Stream.Write(packet, 0, packetSize);
                Stream.Flush();

                var memoryStream = new MemoryStream();
                var buffer = new byte[1000 * 1000];
                int bytesRead;
                
                while ((bytesRead = Stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                    var response = HandlePotentialPacket(memoryStream);
                    if (!response.HasValue) continue;
                    var item1 = response.Value.Item1;
                    var item2 = response.Value.Item2;
                    callback?.Invoke(item1, item2, null);
                    break;
                }
            }
            catch (Exception e)
            {
                callback?.Invoke(0, null, e);
                Disconnect();
            }
        }

        internal void Disconnect()
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }

            if (Client != null)
            {
                Client.Close();
                Client = null;
            }

            SetState(TcpServiceState.Disconnected);
        }

        internal TcpServiceState GetState()
        {
            return State;
        }

        private (int, byte[])? HandlePotentialPacket(MemoryStream memoryStream)
        {
            var potentialPacket = memoryStream.ToArray();
            
            if (potentialPacket.Length <= sizeof(int))
            {
                return null;
            }
            
            var sizeBytes = new byte[sizeof(int)];
            Array.Copy(potentialPacket, sizeBytes, sizeBytes.Length);
            var packetSize = SpanBasedBitConverter.ToInt32(sizeBytes);

            if (potentialPacket.Length < packetSize)
            {
                return null;
            }
            
            var bytes = new byte[sizeof(int)];
            Array.Copy(potentialPacket, sizeof(int), bytes, 0, bytes.Length);
            var protocol = SpanBasedBitConverter.ToInt32(bytes);

            var body = new byte[packetSize - sizeof(int) * 2];
            Array.Copy(potentialPacket, sizeof(int) * 2, body, 0, body.Length);

            return (protocol, body);
        }

        private void SetState(TcpServiceState state)
        {
            State = state;
            OnStateChange?.Invoke(state);
        }
        
#endregion
    }
}
