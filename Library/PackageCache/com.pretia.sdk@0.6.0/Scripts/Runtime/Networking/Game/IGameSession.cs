using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud.Networking
{
    public interface IGameSession : IGameSessionEvent, System.IDisposable
    {
        IPEndPoint EndPoint { get; }
        Player LocalPlayer { get; }
        bool Disposed { get; }

        UniqueUIntProvider NetworkIdProvider { get; }
        IdentityManager IdentityManager { get; }
        IdentityManager ExistInSceneIdentityManager { get; }
        NetworkSpawner NetworkSpawner { get; }
        NetworkSynchronizationManager HostSynchronizationManager { get; }
        NetworkSynchronizationManager PlayerSynchronizationManager { get; }
        NetworkSynchronizationManager PlayerToHostSynchronizationManager { get; }
        HostMessageHandler HostMsg { get; }
        PlayerMessageHandler PlayerMsg { get; }
        PlayerToHostMessageHandler PlayerToHostMsg { get; }
        HostToPlayerMessageHandler HostToPlayerMsg { get; }

        IDictionary<uint, Player> Players { get; }

        Task ConnectSessionAsync(CancellationToken cancellationToken = default);
        Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken = default);
    }
}