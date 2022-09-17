using System.Collections.Generic;

namespace PretiaArCloud.Networking
{
    public delegate void HostAppointmentEvent();
    public delegate void DisconnectionEvent(Player player);
    public delegate void PlayerListEvent(ICollection<Player> players);
    public delegate void PlayerEvent(Player player);
    public delegate void LocalPlayerEvent();
    public delegate void NetworkErrorEvent(NetworkStatusCode errorCode);

    public interface IGameSessionEvent
    {
        event HostAppointmentEvent OnHostAppointment;
        event DisconnectionEvent OnDisconnected;
        event LocalPlayerEvent OnLocalPlayerDisconnected;
        event PlayerListEvent OnPlayerListInitialized;
        event PlayerEvent OnPlayerJoined;
        event LocalPlayerEvent OnLocalPlayerJoined;
        event NetworkErrorEvent OnNetworkError;
    }
}