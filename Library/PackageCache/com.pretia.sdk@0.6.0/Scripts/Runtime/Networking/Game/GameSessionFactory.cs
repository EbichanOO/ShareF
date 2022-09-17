using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud.Networking
{
    internal class GameSessionFactory : IGameSessionFactory
    {
        IPacketSourceFactory _packetSourceFactory;
        IMessageHandlerFactory _messageHandlerFactory;
        IMainThreadExecutor _mainThreadExecutor;

        public GameSessionFactory(IPacketSourceFactory packetSourceFactory,
                                  IMessageHandlerFactory messageHandlerFactory,
                                  IMainThreadExecutor mainThreadExecutor = default)
        {
            _packetSourceFactory = packetSourceFactory;
            _messageHandlerFactory = messageHandlerFactory;
            _mainThreadExecutor = mainThreadExecutor;
        }

        public async Task<IGameSession> CreateAsync(byte[] token,
                                                    IPEndPoint endPoint,
                                                    NetworkSettings networkSettings,
                                                    CancellationToken cancellationToken = default)
        {
            var packetSource = await _packetSourceFactory.CreateAsync(endPoint.Address, endPoint.Port, cancellationToken);
            var reliableGameClient = new ReliableGameClient(packetSource, _mainThreadExecutor);
            return new GameSession(token, endPoint, reliableGameClient, networkSettings, _messageHandlerFactory);
        }
    }
}