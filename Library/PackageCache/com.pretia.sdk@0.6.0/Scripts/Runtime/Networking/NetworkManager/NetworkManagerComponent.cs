using System.Collections;
using System.Threading;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public sealed class NetworkManagerComponent : MonoBehaviour
    {
        private static NetworkManagerComponent Singleton;

        [SerializeField]
        private NetworkSettings _settings = default;

        private GameSession _gameSession = default;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                DestroyImmediate(this);
                return;
            }

            var messageHandlerFactory = new MessageHandlerFactory();

            var tcpPacketSourceFactory = new TcpPacketSourceFactory();
            var sslStreamPacketSourceFactory = new SslPacketSourceFactory();
            var mainThreadExecutor = new UnityMainThreadExecutor(SynchronizationContext.Current);
            var gameSessionFactory = new GameSessionFactory(tcpPacketSourceFactory, messageHandlerFactory, mainThreadExecutor);
            var jsonSerializer = Utf8JsonSerializer.Instance;
            var jwtDecoder = new JwtDecoder();

            NetworkManager.Instance = new NetworkManager(
                PretiaSDKProjectSettings.Instance,
                _settings,
                sslStreamPacketSourceFactory,
                gameSessionFactory,
                jsonSerializer,
                jwtDecoder
            );

            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            if (Singleton == this)
            {
                NetworkManager.Instance.Dispose();
            }
        }
    }
}
