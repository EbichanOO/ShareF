using System.IO;
using System.Net.Sockets;

namespace PretiaArCloud.Networking
{
    internal class TcpPacketSource : StreamPacketSource
    {
        TcpClient _client;

        internal TcpPacketSource(Stream stream, TcpClient client) : base(stream, 2)
        {
            _client = client;
        }

        public override void Close()
        {
            base.Close();
            _client.Close();
        }
    }
}