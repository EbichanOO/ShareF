using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud.Networking
{
    internal class TcpPacketSourceFactory : IPacketSourceFactory
    {
        private TcpClient CreateTcpClient()
        {
            TcpClient client = new TcpClient();

            client.NoDelay = true;
            client.ReceiveBufferSize = ushort.MaxValue;
            client.SendBufferSize = ushort.MaxValue;

            return client;
        }

        public async Task<IPacketSource> CreateAsync(string address, int port, CancellationToken cancellationToken = default)
        {
            TcpClient client = CreateTcpClient();
            cancellationToken.Register(client.Close);

            await client.ConnectAsync(address, port);

            if (client.Connected)
            {
                var packetSource = new TcpPacketSource(client.GetStream(), client);
                return packetSource;
            }
            else
            {
                throw new System.NullReferenceException();
            }
        }

        public async Task<IPacketSource> CreateAsync(IPAddress address, int port, CancellationToken cancellationToken = default)
        {
            TcpClient client = CreateTcpClient();
            client.NoDelay = true;

            cancellationToken.Register(client.Close);

            await client.ConnectAsync(address, port);

            if (client.Connected)
            {
                var packetSource = new TcpPacketSource(client.GetStream(), client);
                return packetSource;
            }
            else
            {
                throw new System.NullReferenceException();
            }
        }

    }
}
