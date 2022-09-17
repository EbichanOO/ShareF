using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud.Networking
{
    public interface IPacketSourceFactory
    {
        Task<IPacketSource> CreateAsync(string address, int port, CancellationToken cancellationToken = default(CancellationToken));
        Task<IPacketSource> CreateAsync(IPAddress address, int port, CancellationToken cancellationToken = default(CancellationToken));
    }
}