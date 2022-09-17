using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public sealed class NetworkManager : IDisposable
    {
        public static NetworkManager Instance { get; internal set; }

        private PretiaSDKProjectSettings _projectSettings;
        private NetworkSettings _networkSettings;

        private LobbyClient _lobbyClient;

        private IPacketSourceFactory _packetSourceFactory;
        private IGameSessionFactory _gameSessionFactory;
        private IJsonSerializer _jsonSerializer;
        private IJwtDecoder _jwtDecoder;
        private bool _connected = false;

        public bool IsConnected => _connected;

        internal bool LatestSessionDisposed = false;
        private TaskCompletionSource<IGameSession> _prepareSession;

        public async Task<IGameSession> GetLatestSessionAsync(CancellationToken cancellationToken = default)
        {
            if (_prepareSession == null || LatestSessionDisposed)
            {
                _prepareSession = new TaskCompletionSource<IGameSession>();
                LatestSessionDisposed = false;
            }

            using(cancellationToken.Register(() => _prepareSession.TrySetCanceled()))
            {
                return await _prepareSession.Task;
            }
        }

        internal async Task<GameSession> GetLatestSessionInternalAsync(CancellationToken cancellationToken = default)
        {
            if (_prepareSession == null || LatestSessionDisposed)
            {
                _prepareSession = new TaskCompletionSource<IGameSession>();
                LatestSessionDisposed = false;
            }

            using(cancellationToken.Register(() => _prepareSession.TrySetCanceled()))
            {
                return await _prepareSession.Task as GameSession;
            }
        }

        internal NetworkManager(
            PretiaSDKProjectSettings projectSettings,
            NetworkSettings networkSettings,
            IPacketSourceFactory packetSourceFactory,
            IGameSessionFactory gameSessionFactory,
            IJsonSerializer jsonSerializer,
            IJwtDecoder jwtDecoder)
        {
            _projectSettings = projectSettings;
            _networkSettings = networkSettings;
            _packetSourceFactory = packetSourceFactory;
            _gameSessionFactory = gameSessionFactory;
            _jsonSerializer = jsonSerializer;
            _jwtDecoder = jwtDecoder;
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var packetSource = await _packetSourceFactory.CreateAsync(_projectSettings.LobbyServerAddress, _projectSettings.LobbyServerPort, cancellationToken);
            if (packetSource == null) return false;

            _lobbyClient = new LobbyClient(packetSource);
            _lobbyClient.StartReceiver();
            _connected = true;

            return true;
        }

        public async Task<(NetworkStatusCode statusCode, byte[] token, string displayName)> GuestLoginAsync(string displayName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var login = new TaskCompletionSource<(NetworkStatusCode, byte[], string)>();

            _lobbyClient.OnAuthError = error => login.TrySetResult((error, null, null));
            _lobbyClient.OnAuth = (t, d) =>
            {
                login.TrySetResult((NetworkStatusCode.Success, t, d));
            };

            try
            {
                using (cancellationToken.Register(() => login.TrySetException(new LoginCanceledException())))
                {
                    _lobbyClient.SendAnonymousLogin(displayName, _projectSettings.AppKey);
                    await login.Task;
                }
            }
            finally
            {
                _lobbyClient.OnAuth = null;
                _lobbyClient.OnAuthError = null;
            }

            return login.Task.Result;
        }

        public async Task<(NetworkStatusCode statusCode, byte[] token, string displayName)> LoginAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            var login = new TaskCompletionSource<(NetworkStatusCode, byte[], string)>();

            _lobbyClient.OnAuthError = error => login.TrySetResult((error, null, null));
            _lobbyClient.OnAuth = (t, d) =>
            {
                login.TrySetResult((NetworkStatusCode.Success, t, d));
            };

            try
            {
                using (cancellationToken.Register(() => login.TrySetException(new LoginCanceledException())))
                {
                    _lobbyClient.SendLogin(username, password, _projectSettings.AppKey);
                    await login.Task;
                }
            }
            finally
            {
                _lobbyClient.OnAuth = null;
                _lobbyClient.OnAuthError = null;
            }

            return login.Task.Result;
        }

        public async Task<(NetworkStatusCode statusCode, byte[] token, string displayName)> RegisterAsync(string username, string password, string displayName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var register = new TaskCompletionSource<(NetworkStatusCode, byte[], string)>();

            _lobbyClient.OnAuthError = error => register.TrySetResult((error, null, null));
            _lobbyClient.OnAuth = (t, d) =>
            {
                register.TrySetResult(((NetworkStatusCode.Success, t, d)));
            };

            try
            {
                using (cancellationToken.Register(() => register.TrySetException(new RegisterCanceledException())))
                {
                    _lobbyClient.SendRegister(username, password, displayName, _projectSettings.AppKey);
                    await register.Task;
                }
            }
            finally
            {
                _lobbyClient.OnAuth = null;
                _lobbyClient.OnAuthError = null;
            }

            return register.Task.Result;
        }

        public async Task<IGameSession> RequestRandomMatchAsync(string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await RequestRandomMatchAsync(StringEncoder.Instance.GetBytes(token), cancellationToken);
        }

        public async Task<IGameSession> RequestRandomMatchAsync(byte[] token, CancellationToken cancellationToken = default(CancellationToken))
        {
            IPEndPoint matchEndpoint;
            var randomMatchRequest = new TaskCompletionSource<IPEndPoint>();

            _lobbyClient.OnMatch = (endPoint) => randomMatchRequest.TrySetResult(endPoint);

            try
            {
                using (cancellationToken.Register(() => randomMatchRequest.TrySetException(new RequestRandomMatchCanceledException())))
                {
                    _lobbyClient.SendRandomMatchRequest();
                    matchEndpoint = await randomMatchRequest.Task;
                    if (matchEndpoint == default) return null;
                }
            }
            finally
            {
                _lobbyClient.OnMatch = null;
            }

            var gameSession = await _gameSessionFactory.CreateAsync(token, matchEndpoint, _networkSettings, cancellationToken);
            _prepareSession.SetResult(gameSession);

            return gameSession;
        }

        public void Logout()
        {
            _lobbyClient.SendLogout();
            _connected = false;
        }

        public bool IsTokenValid(string token)
        {
            string jsonString = _jwtDecoder.Decode(token);
            var claims = _jsonSerializer.Deserialize<EnduserClaims>(jsonString);
            return claims.ExpiredAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public bool IsTokenValid(byte[] token)
        {
            string jsonString = _jwtDecoder.Decode(StringEncoder.Instance.GetString(token));
            var claims = _jsonSerializer.Deserialize<EnduserClaims>(jsonString);
            return claims.ExpiredAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public async Task<IGameSession> CreateGameSessionAsync(IPEndPoint endPoint, string token, CancellationToken cancellationToken = default)
        {
            var gameSession = await _gameSessionFactory.CreateAsync(StringEncoder.Instance.GetBytes(token), endPoint, _networkSettings, cancellationToken);
            _prepareSession.SetResult(gameSession);

            return gameSession;
        }

        public async Task<IGameSession> CreateGameSessionAsync(IPEndPoint endPoint, byte[] token, CancellationToken cancellationToken = default)
        {
            var gameSession = await _gameSessionFactory.CreateAsync(token, endPoint, _networkSettings, cancellationToken);
            _prepareSession.SetResult(gameSession);

            return gameSession;
        }

        // TODO: There are many issues with the current implementation of network visibility,
        // so commenting out the public method to enable it
        // public void EnableNetworkVisibility(float maxDistance)
        // {
        //     _networkSettings.EnableNetworkVisibility = true;
        //     _networkSettings.NetworkVisibilityRange = maxDistance;
        // }

        public void Dispose()
        {
            if (_lobbyClient != null)
            {
                _lobbyClient.Dispose();
                _lobbyClient.ReceiverThread?.Join();
            }
        }
    }
}