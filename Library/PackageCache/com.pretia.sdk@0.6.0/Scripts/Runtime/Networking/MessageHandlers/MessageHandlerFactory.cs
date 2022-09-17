using System.Collections.Generic;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    internal class MessageHandlerFactory : IMessageHandlerFactory
    {
        public ISerializer Serializer { get; protected set; }
        public MessageHandlerFactory()
        {
            Serializer = new MsgPackSerializer(FormatterResolver.Instance);
        }

        public HostMessageHandler CreateHostMsgHandler(ReliableGameClient reliableGameClient, TickManager tickManager)
        {
            return new HostMessageHandler(Serializer, reliableGameClient, tickManager);
        }

        public PlayerMessageHandler CreatePlayerMsgHandler(ReliableGameClient reliableGameClient, Player localPlayer, ref Dictionary<uint, Player> players, TickManager tickManager)
        {
            return new PlayerMessageHandler(Serializer, reliableGameClient, localPlayer, ref players, tickManager);
        }

        public PlayerToHostMessageHandler CreatePlayerToHostMsgHandler(ReliableGameClient reliableGameClient, ref Dictionary<uint, Player> players, TickManager tickManager)
        {
            return new PlayerToHostMessageHandler(Serializer, reliableGameClient, ref players, tickManager);
        }

        public HostToPlayerMessageHandler CreateHostToPlayerMsgHandler(ReliableGameClient reliableGameClient, TickManager tickManager)
        {
            return new HostToPlayerMessageHandler(Serializer, reliableGameClient, tickManager);
        }
    }
}