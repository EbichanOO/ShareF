using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud.Networking
{
    public interface IGameSessionFactory
    {
        Task<IGameSession> CreateAsync(byte[] token,
                                       IPEndPoint endPoint,
                                       NetworkSettings networkSettings,
                                       CancellationToken cancellationToken = default(CancellationToken));
    }
}