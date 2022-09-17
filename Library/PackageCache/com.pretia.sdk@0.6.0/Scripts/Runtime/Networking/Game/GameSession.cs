using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;

namespace PretiaArCloud.Networking
{
    internal class GameSession : IGameSession
    {
        private byte[] _token;
        private ISerializer _serializer;
        private ReliableGameClient _reliableGameClient;
        private Dictionary<uint, Player> _playersMap;
        private TickManager _tickManager;
        private NetworkSettings _networkSettings;

        private TaskCompletionSource<bool> _sessionConnected;
        private Timer _clientNetworkSyncTimer;
        private AutoResetEvent _autoClientNetworkSyncEvent;

        private Timer _hostNetworkSyncTimer;
        private AutoResetEvent _autoHostNetworkSyncEvent;

        public UniqueUIntProvider NetworkIdProvider { get; private set; }
        public IdentityManager IdentityManager { get; private set; }
        public IdentityManager ExistInSceneIdentityManager { get; private set; }
        public NetworkSpawner NetworkSpawner { get; private set; }
        public NetworkSynchronizationManager HostSynchronizationManager { get; private set; }
        public NetworkSynchronizationManager PlayerSynchronizationManager { get; private set; }
        public NetworkSynchronizationManager PlayerToHostSynchronizationManager { get; private set; }
        public HostMessageHandler HostMsg { get; private set; }
        public PlayerMessageHandler PlayerMsg { get; private set; }
        public PlayerToHostMessageHandler PlayerToHostMsg { get; private set; }
        public HostToPlayerMessageHandler HostToPlayerMsg { get; private set; }

        public IPEndPoint EndPoint { get; private set; }
        public Player LocalPlayer { get; internal set; }
        public bool Disposed { get; private set; }
        public IDictionary<uint, Player> Players => _playersMap;

        public event HostAppointmentEvent OnHostAppointment;
        public event DisconnectionEvent OnDisconnected;
        public event LocalPlayerEvent OnLocalPlayerDisconnected;
        public event PlayerListEvent OnPlayerListInitialized;
        public event PlayerEvent OnPlayerJoined;
        public event LocalPlayerEvent OnLocalPlayerJoined;
        public event NetworkErrorEvent OnNetworkError;

        public GameSession(
            byte[] token,
            IPEndPoint endPoint,
            ReliableGameClient reliableGameClient,
            NetworkSettings networkSettings,
            IMessageHandlerFactory messageHandlerFactory)
        {
            _token = token;
            EndPoint = endPoint;
            _serializer = messageHandlerFactory.Serializer;
            _reliableGameClient = reliableGameClient;
            _networkSettings = networkSettings;
            _tickManager = new TickManager();

            _sessionConnected = new TaskCompletionSource<bool>();

            LocalPlayer = new Player();
            _playersMap = new Dictionary<uint, Player>();

            HostMsg = messageHandlerFactory.CreateHostMsgHandler(_reliableGameClient, _tickManager);
            PlayerMsg = messageHandlerFactory.CreatePlayerMsgHandler(_reliableGameClient, LocalPlayer, ref _playersMap, _tickManager);
            PlayerToHostMsg = messageHandlerFactory.CreatePlayerToHostMsgHandler(_reliableGameClient, ref _playersMap, _tickManager);
            HostToPlayerMsg = messageHandlerFactory.CreateHostToPlayerMsgHandler(_reliableGameClient, _tickManager);

            _reliableGameClient.OnError += InvokeNetworkError;
            _reliableGameClient.OnHostMessage += HostMsg.OnReceiveMessage;
            _reliableGameClient.OnPlayerMessage += PlayerMsg.OnReceiveMessage;
            _reliableGameClient.OnPlayerToHostMessage += PlayerToHostMsg.OnReceiveMessage;
            _reliableGameClient.OnHostToPlayerMessage += HostToPlayerMsg.OnReceiveMessage;

            _reliableGameClient.OnHostAppointment += RegisterHostCallbacks;
            _reliableGameClient.OnHostAppointment += SetLocalPlayerAsHost;
            _reliableGameClient.OnHostAppointment += InvokeOnHostAppointment;

            _reliableGameClient.OnSessionConnected += SetLocalPlayerData;
            _reliableGameClient.OnSessionDisconnected += InvokeOnDisconnected;
            _reliableGameClient.OnSessionDisconnected += RemoveFromPlayerList;

            PlayerMsg.Register<NotifyConnectionMsg>(InitializeConnectedPlayer);
            HostToPlayerMsg.Register<PlayerListSnapshotMsg>(InitializePlayerList);

            NetworkIdProvider = new UniqueUIntProvider();
            IdentityManager = new IdentityManager();
            ExistInSceneIdentityManager = new IdentityManager();
            NetworkSpawner = new NetworkSpawner(
                this,
                LocalPlayer,
                ref _playersMap,
                HostMsg,
                HostToPlayerMsg,
                PlayerMsg,
                PlayerToHostMsg,
                NetworkIdProvider,
                IdentityManager,
                ExistInSceneIdentityManager
            );

            NetworkSpawner.OnSnapshotApplied += InvokeLocalPlayerJoined;

            var byteArrayPool = ArrayPool<byte>.Shared;
            ArrayBufferWriter<byte> scratchBuffer = new ArrayBufferWriter<byte>(256);
            ArrayBufferWriter<byte> dirtyMaskBuffer = new ArrayBufferWriter<byte>(16);

            PlayerSynchronizationManager = new NetworkSynchronizationManager(
                scratchBuffer, dirtyMaskBuffer,
                _serializer,
                _reliableGameClient,
                ReliableGameClient.ProtocolId.PlayerMessage,
                IdentityManager,
                _tickManager,
                ref byteArrayPool,
                ref _playersMap,
                _networkSettings.EnableNetworkVisibility,
                _networkSettings.NetworkVisibilityRange
            );
            PlayerToHostSynchronizationManager = new NetworkSynchronizationManager(
                scratchBuffer, dirtyMaskBuffer,
                _serializer,
                _reliableGameClient,
                ReliableGameClient.ProtocolId.PlayerToHostMessage,
                IdentityManager,
                _tickManager,
                ref byteArrayPool,
                ref _playersMap,
                _networkSettings.EnableNetworkVisibility,
                _networkSettings.NetworkVisibilityRange
            );
            HostSynchronizationManager = new NetworkSynchronizationManager(
                scratchBuffer, dirtyMaskBuffer,
                _serializer,
                _reliableGameClient,
                ReliableGameClient.ProtocolId.HostMessage,
                IdentityManager,
                _tickManager,
                ref byteArrayPool,
                ref _playersMap,
                _networkSettings.EnableNetworkVisibility,
                _networkSettings.NetworkVisibilityRange
            );

            PlayerMsg.OnSyncMessage += PlayerSynchronizationManager.Deserialize;
            HostMsg.OnSyncMessage += HostSynchronizationManager.Deserialize;
            HostToPlayerMsg.OnSyncMessage += HostSynchronizationManager.Deserialize;
            _reliableGameClient.OnHostAppointment += StopSyncDeserialization;

            _autoClientNetworkSyncEvent = new AutoResetEvent(false);
            _clientNetworkSyncTimer = new Timer(SendClientSyncUpdate, _autoClientNetworkSyncEvent, 0, Math.Max(_networkSettings.ClientSynchronizationIntervalInMillis, 16));

            NetworkLifecycle.SyncUpdate += SyncUpdate;
        }

        private void InvokeLocalPlayerJoined()
        {
            OnPlayerJoined?.Invoke(LocalPlayer);
            OnLocalPlayerJoined?.Invoke();
        }

        private void InvokeNetworkError(NetworkStatusCode errorCode, string errorMessage)
        {
            UnityEngine.Debug.LogError($"{errorCode}: {errorMessage}");
            OnNetworkError?.Invoke(errorCode);
        }

        private void StopSyncDeserialization()
        {
            HostMsg.OnSyncMessage -= HostSynchronizationManager.Deserialize;
        }

        private void InitializePlayerList(PlayerListSnapshotMsg msg)
        {
            for (int i = 0; i < msg.UserNumbers.Count; i++)
            {
                if (msg.UserNumbers[i] != LocalPlayer.UserNumber)
                {
                    var player = new Player { UserNumber = msg.UserNumbers[i], DisplayName = msg.DisplayNames[i] };
                    _playersMap[msg.UserNumbers[i]] = player;
                }
            }

            OnPlayerListInitialized?.Invoke(_playersMap.Values);
        }

        public async Task ConnectSessionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Guard to prevent executing connection flow when already connected
            if (_sessionConnected.Task.IsCompleted)
            {
                return;
            }

            var sessionConnection = new TaskCompletionSource<bool>();

            Action<uint, string> connectionCallback = null;
            _reliableGameClient.OnSessionConnected += connectionCallback = (_, __) =>
            {
                sessionConnection.TrySetResult(true);
            };

            try
            {
                using (cancellationToken.Register(() => sessionConnection.TrySetException(new ConnectSessionCanceledException())))
                {
                    _reliableGameClient.StartReceiver();
                    _reliableGameClient.SendConnection(_token);
                    await sessionConnection.Task;
                }
            }
            finally
            {
                _reliableGameClient.OnSessionConnected -= connectionCallback;
            }


            if (LocalPlayer.IsHost)
            {
                OnPlayerJoined?.Invoke(LocalPlayer);
                OnLocalPlayerJoined?.Invoke();
            }
            else
            {
                PlayerMsg.Send(new NotifyConnectionMsg { DisplayName = LocalPlayer.DisplayName });
            }

            _sessionConnected.TrySetResult(true);
        }

        public async Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken = default)
        {
            using (cancellationToken.Register(() => _sessionConnected.TrySetCanceled()))
            {
                return await _sessionConnected.Task;
            }
        }

        private void InvokeOnHostAppointment()
        {
            OnHostAppointment?.Invoke();
        }

        private void SetLocalPlayerData(uint userNumber, string displayName)
        {
            LocalPlayer.UserNumber = userNumber;
            LocalPlayer.DisplayName = displayName;

            _playersMap.Add(LocalPlayer.UserNumber, LocalPlayer);
        }

        private void RegisterHostCallbacks()
        {
            _autoHostNetworkSyncEvent = new AutoResetEvent(false);
            _hostNetworkSyncTimer = new Timer(SendHostSyncUpdate, _autoHostNetworkSyncEvent, 0, Math.Max(_networkSettings.HostSynchronizationIntervalInMillis, 16));

            PlayerToHostMsg.OnSyncMessage += PlayerToHostSynchronizationManager.Deserialize;
        }

        private void UnregisterHostCallbacks()
        {
            PlayerToHostMsg.OnSyncMessage -= PlayerToHostSynchronizationManager.Deserialize;
            _hostNetworkSyncTimer.Dispose();
        }

        private void InitializeConnectedPlayer(NotifyConnectionMsg msg, Player sender)
        {
            if (LocalPlayer.IsHost)
            {
                HostToPlayerMsg.Send(new PlayerListSnapshotMsg(_playersMap.Values.Where(p => p != sender)), sender);
                NetworkSpawner.HOST_SendCurrentSnapshot(sender);
            }

            if (sender != LocalPlayer)
            {
                sender.DisplayName = msg.DisplayName;
                OnPlayerJoined?.Invoke(sender);
            }
        }

        private void SetLocalPlayerAsHost()
        {
            LocalPlayer.IsHost = true;
        }

        private void InvokeOnDisconnected(uint userNumber)
        {
            OnDisconnected?.Invoke(_playersMap[userNumber]);
            if (LocalPlayer.UserNumber == userNumber)
            {
                OnLocalPlayerDisconnected?.Invoke();
            }
        }

        private void RemoveFromPlayerList(uint userNumber)
        {
            _playersMap.Remove(userNumber);
        }

        internal void SyncUpdate()
        {
            PlayerSynchronizationManager.Serialize(_tickManager.Tick);
            PlayerToHostSynchronizationManager.Serialize(_tickManager.Tick);

            if (LocalPlayer.IsHost)
            {
                HostSynchronizationManager.HostSerialize(_tickManager.Tick);
            }

            _tickManager.Increment();
        }

        internal void SendClientSyncUpdate(Object stateInfo)
        {
            PlayerSynchronizationManager.SendSyncUpdate();
            PlayerToHostSynchronizationManager.SendSyncUpdate();
        }

        internal void SendHostSyncUpdate(Object stateInfo)
        {
            HostSynchronizationManager.SendHostSyncUpdate();
        }

        public void Dispose()
        {
            NetworkManager.Instance.LatestSessionDisposed = true;
            Disposed = true;
            NetworkSpawner.Dispose();

            _reliableGameClient.SendDisconnect();
            OnLocalPlayerDisconnected?.Invoke();

            _clientNetworkSyncTimer.Dispose();

            PlayerMsg.OnSyncMessage -= PlayerSynchronizationManager.Deserialize;
            HostToPlayerMsg.OnSyncMessage -= HostSynchronizationManager.Deserialize;

            _reliableGameClient.OnError -= InvokeNetworkError;
            _reliableGameClient.OnHostMessage -= HostMsg.OnReceiveMessage;
            _reliableGameClient.OnPlayerMessage -= PlayerMsg.OnReceiveMessage;
            _reliableGameClient.OnPlayerToHostMessage -= PlayerToHostMsg.OnReceiveMessage;
            _reliableGameClient.OnHostToPlayerMessage -= HostToPlayerMsg.OnReceiveMessage;

            _reliableGameClient.OnHostAppointment -= SetLocalPlayerAsHost;
            _reliableGameClient.OnHostAppointment -= RegisterHostCallbacks;
            _reliableGameClient.OnHostAppointment -= InvokeOnHostAppointment;
            _reliableGameClient.OnHostAppointment -= StopSyncDeserialization;

            _reliableGameClient.OnSessionConnected -= SetLocalPlayerData;
            _reliableGameClient.OnSessionDisconnected -= InvokeOnDisconnected;
            _reliableGameClient.OnSessionDisconnected -= RemoveFromPlayerList;

            PlayerMsg.Unregister<NotifyConnectionMsg>(InitializeConnectedPlayer);
            HostToPlayerMsg.Unregister<PlayerListSnapshotMsg>(InitializePlayerList);
            NetworkSpawner.OnSnapshotApplied -= InvokeLocalPlayerJoined;

            if (LocalPlayer.IsHost)
            {
                UnregisterHostCallbacks();
            }

            NetworkLifecycle.SyncUpdate -= SyncUpdate;
        }
    }
}