using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace PretiaArCloud.Networking
{
    internal class UdpPacketSource : IPacketSource
    {
        private IUdpClientProxy _udpClient;
        private IPEndPoint _endPoint;

        internal UdpPacketSource(IUdpClientProxy udpClient, IPEndPoint endPoint)
        {
            _udpClient = udpClient;
            _endPoint = endPoint;

            _udpClient.Connect(_endPoint);
        }

        public byte[] GetNextPacket()
        {
            return _udpClient.Receive(ref _endPoint);
        }

        public void Send(ReadOnlySpan<byte> buffer)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(sharedBuffer);
                _udpClient.Send(sharedBuffer, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public void Close()
        {
            _udpClient.Close();
        }
    }
}