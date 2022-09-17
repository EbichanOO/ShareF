using System.Net;
using System.Net.Sockets;

namespace PretiaArCloud.Networking
{
    internal class UdpClientProxy : IUdpClientProxy
    {
        private UdpClient _udpClient;

        internal UdpClientProxy()
        {
            _udpClient = new UdpClient();
        }

        public void Connect(IPEndPoint endPoint)
        {
            _udpClient.Connect(endPoint);
        }

        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            try
            {
                return _udpClient.Receive(ref remoteEP);
            }
            catch
            {
                return null;
            }
        }

        public int Send(byte[] dgram, int bytes)
        {
            return _udpClient.Send(dgram, bytes);
        }

        public void Close()
        {
            _udpClient.Close();
        }
    }
}