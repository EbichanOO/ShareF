using System.Collections.Generic;

namespace PretiaArCloud.Networking
{
    internal interface IMessageHandlerFactory
    {
        ISerializer Serializer { get; }
        HostMessageHandler CreateHostMsgHandler(ReliableGameClient reliableGameClient, TickManager tickManager);
        PlayerMessageHandler CreatePlayerMsgHandler(ReliableGameClient reliableGameClient, Player localPlayer, ref Dictionary<uint, Player> players, TickManager tickManager);
        PlayerToHostMessageHandler CreatePlayerToHostMsgHandler(ReliableGameClient reliableGameClient, ref Dictionary<uint, Player> players, TickManager tickManager);
        HostToPlayerMessageHandler CreateHostToPlayerMsgHandler(ReliableGameClient reliableGameClient, TickManager tickManager);
    }
}