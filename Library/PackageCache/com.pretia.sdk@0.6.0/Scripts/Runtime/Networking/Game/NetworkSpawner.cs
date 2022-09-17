using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class NetworkSpawner : IDisposable
    {
        private IGameSessionEvent _gameSessionEvent;
        private Player _localPlayer;
        private IDictionary<uint, Player> _players;
        private HostMessageHandler _hostMsg;
        private HostToPlayerMessageHandler _hostToPlayerMsg;
        private PlayerMessageHandler _playerMsg;
        private PlayerToHostMessageHandler _playerToHostMsg;
        private IdentityManager _identityManager;
        private IdentityManager _existInSceneIdentityManager;

        private Dictionary<NetworkIdentity, ushort> _prefabIdMap;
        private List<NetworkIdentity> _prefabList;
        private Queue<TaskCompletionSource<NetworkIdentity>> _instantiationQueue;
        private UniqueUIntProvider _networkIdProvider;

        public delegate void InstantiateEvent(NetworkIdentity networkIdentity);
        public event InstantiateEvent OnLocalPlayerInstantiated;
        public event InstantiateEvent OnInstantiated;

        public delegate void SnapshotEvent();
        public event SnapshotEvent OnSnapshotApplied;

        internal NetworkSpawner(
            IGameSessionEvent gameSessionEvent,
            Player localPlayer,
            ref Dictionary<uint, Player> players,
            HostMessageHandler hostMsg,
            HostToPlayerMessageHandler hostToPlayerMsg,
            PlayerMessageHandler playerMsg,
            PlayerToHostMessageHandler playerToHostMsg,
            UniqueUIntProvider networkIdProvider,
            IdentityManager identityManager,
            IdentityManager existInSceneIdentityManager)
        {
            _gameSessionEvent = gameSessionEvent;
            _localPlayer = localPlayer;
            _players = players;
            _hostMsg = hostMsg;
            _hostToPlayerMsg = hostToPlayerMsg;
            _playerMsg = playerMsg;
            _playerToHostMsg = playerToHostMsg;
            _networkIdProvider = networkIdProvider;
            _identityManager = identityManager;
            _existInSceneIdentityManager = existInSceneIdentityManager;

            _prefabIdMap = new Dictionary<NetworkIdentity, ushort>();
            _prefabList = new List<NetworkIdentity>();
            _instantiationQueue = new Queue<TaskCompletionSource<NetworkIdentity>>();

            _hostMsg.Register<InstantiateMsg>(InstantiateInternal);
            _hostToPlayerMsg.Register<InstantiateMsg>(InstantiateFromSnapshot);
            _hostToPlayerMsg.Register<DeleteExistInSceneObjectsMsg>(DeleteExistInSceneObjects);
            _playerMsg.Register<NetworkDestroyMsg>(NetworkDestroy);
            _playerToHostMsg.Register<InstantiateMsg>(HOST_InstantiateInternal);

            _gameSessionEvent.OnDisconnected += DestroyObjectsOwnedByPlayer;
        }

        private void DeleteExistInSceneObjects(DeleteExistInSceneObjectsMsg msg)
        {
            foreach (var networkIdentity in _existInSceneIdentityManager.GetAll())
            {
                GameObject.Destroy(networkIdentity.gameObject);
            }

            OnSnapshotApplied?.Invoke();
        }

        internal void HOST_SendCurrentSnapshot(Player requester)
        {
            foreach (var networkIdentity in _identityManager.GetAll())
            {
                _hostToPlayerMsg.Enqueue(new InstantiateMsg
                {
                    NetworkId = networkIdentity.Value,
                    PrefabId = networkIdentity.PrefabId,
                    Position = networkIdentity.transform.localPosition.AsSerializable(),
                    Rotation = networkIdentity.transform.localRotation.AsSerializable(),
                    Active = networkIdentity.gameObject.activeInHierarchy,
                    Parent = networkIdentity.transform.parent == null ? 0 : networkIdentity.transform.parent.GetComponent<NetworkIdentity>().Value,
                    ForSnapshot = true,
                    Sender = requester.UserNumber,
                    Owner = networkIdentity.Owner == null ? 0 : networkIdentity.Owner.UserNumber,
                });

                var snapshotableComponents = networkIdentity.GetComponents<ISnapshot>();
                foreach (var component in snapshotableComponents)
                {
                    component.EnqueueSnapshot(_hostToPlayerMsg);
                }
            }

            _hostToPlayerMsg.Enqueue(new DeleteExistInSceneObjectsMsg());
            _hostToPlayerMsg.SendQueue(requester);
        }

        private void HOST_InstantiateInternal(InstantiateMsg msg, Player _)
        {
            msg.NetworkId = _networkIdProvider.Rent();
            _hostMsg.Send(msg);
        }

        private void RegisterSnapshotInitializers(NetworkIdentity networkIdentity)
        {
            var snapshotableComponents = networkIdentity.GetComponents<ISnapshot>();
            foreach (var component in snapshotableComponents)
            {
                component.RegisterSnapshotCallback(_hostToPlayerMsg);
            }
        }

        private void InstantiateInternal(InstantiateMsg msg)
        {
            NetworkIdentity prefab = _prefabList[msg.PrefabId];
            NetworkIdentity networkIdentity = default;
            if (msg.Parent == 0)
            {
                networkIdentity = GameObject.Instantiate<NetworkIdentity>(prefab, msg.Position.AsVector3(), msg.Rotation.AsQuaternion());
            }
            else
            {
                NetworkIdentity parent = _identityManager.Get(msg.Parent);
                networkIdentity = GameObject.Instantiate<NetworkIdentity>(prefab, msg.Position.AsVector3(), msg.Rotation.AsQuaternion(), parent.transform);
            }

            if (!_localPlayer.IsHost)
            {
                _networkIdProvider.Rent();
            }

            networkIdentity.Value = msg.NetworkId;
            networkIdentity.PrefabId = msg.PrefabId;
            if (_players.TryGetValue(msg.Owner, out Player player))
            {
                networkIdentity.Owner = player;
            }
            else
            {
                Debug.LogError($"Instantiate Error: No player found with userNumber {msg.Owner}");
            }
            networkIdentity.gameObject.name = $"{networkIdentity.Owner.UserNumber}:{networkIdentity.Value}";

            networkIdentity.gameObject.SetActive(msg.Active);
            _identityManager.Register(networkIdentity);

            if (msg.Owner == _localPlayer.UserNumber)
            {
                OnLocalPlayerInstantiated?.Invoke(networkIdentity);
            }

            OnInstantiated?.Invoke(networkIdentity);

            if (!msg.ForSnapshot && msg.Sender == _localPlayer.UserNumber && _instantiationQueue.Count > 0)
            {
                TaskCompletionSource<NetworkIdentity> instantiation = default;
                lock (_instantiationQueue)
                {
                    instantiation = _instantiationQueue.Dequeue();
                }

                instantiation.TrySetResult(networkIdentity);
            }
        }

        private void InstantiateFromSnapshot(InstantiateMsg msg)
        {
            if (_existInSceneIdentityManager.Contains(msg.NetworkId))
            {
                var existInSceneObject = _existInSceneIdentityManager.Get(msg.NetworkId);
                _existInSceneIdentityManager.Unregister(msg.NetworkId);
                _identityManager.Register(existInSceneObject);

                existInSceneObject.gameObject.SetActive(msg.Active);
                RegisterSnapshotInitializers(existInSceneObject);
                return;
            }

            NetworkIdentity prefab = _prefabList[msg.PrefabId];

            NetworkIdentity networkIdentity = default;
            if (msg.Parent == 0)
            {
                networkIdentity = GameObject.Instantiate<NetworkIdentity>(prefab, msg.Position.AsVector3(), msg.Rotation.AsQuaternion());
            }
            else
            {
                NetworkIdentity parent = _identityManager.Get(msg.Parent);
                networkIdentity = GameObject.Instantiate<NetworkIdentity>(prefab, msg.Position.AsVector3(), msg.Rotation.AsQuaternion(), parent.transform);
            }

            if (!_localPlayer.IsHost)
            {
                _networkIdProvider.Rent();
            }

            networkIdentity.gameObject.name = $"{msg.Owner}:{msg.NetworkId}";
            networkIdentity.Value = msg.NetworkId;
            networkIdentity.PrefabId = msg.PrefabId;
            if (_players.TryGetValue(msg.Owner, out Player player))
            {
                networkIdentity.Owner = player;
            }
            else
            {
                Debug.LogError($"Instantiate Error: No player found with userNumber {msg.Owner}");
            }

            networkIdentity.gameObject.SetActive(msg.Active);
            _identityManager.Register(networkIdentity);

            RegisterSnapshotInitializers(networkIdentity);
            OnInstantiated?.Invoke(networkIdentity);
        }

        private void NetworkDestroy(NetworkDestroyMsg msg, Player sender)
        {
            NetworkDestroyInternal(msg.NetworkId, msg.IsExistInScene, sender);
        }

        private void NetworkDestroyInternal(uint networkId, bool isExistInScene, Player sender)
        {
            var networkIdentity = _identityManager.Get(networkId);
            _identityManager.Unregister(networkId);

            if (!isExistInScene)
            {
                _networkIdProvider.Return(networkId);
            }

            if (sender != _localPlayer)
            {
                GameObject.Destroy(networkIdentity.gameObject);
            }
        }

        public void Register(NetworkIdentity prefab)
        {
            if (!_prefabIdMap.ContainsKey(prefab))
            {
                _prefabIdMap.Add(prefab, (ushort)_prefabList.Count);
                _prefabList.Add(prefab);
            }
        }

        public void Instantiate(NetworkIdentity prefab, Vector3 position, Quaternion rotation, Player owner)
        {
            Instantiate(prefab, position, rotation, null, owner);
        }

        public void Instantiate(NetworkIdentity prefab, Vector3 position, Quaternion rotation, NetworkIdentity parent, Player owner)
        {
            ushort prefabId = _prefabIdMap[prefab];

            var instantiateMsg = new InstantiateMsg
            {
                PrefabId = prefabId,
                Position = new Vector3Serializable(position),
                Rotation = new QuaternionSerializable(rotation),
                Active = true,
                Parent = parent == null ? 0 : parent.Value,
                Sender = _localPlayer.UserNumber,
                Owner = owner.UserNumber,
                ForSnapshot = false,
            };

            if (_localPlayer.IsHost)
            {
                instantiateMsg.NetworkId = _networkIdProvider.Rent();
                _hostMsg.Send(instantiateMsg);
            }
            else
            {
                _playerToHostMsg.Send(instantiateMsg);
            }
        }

        public async Task<NetworkIdentity> InstantiateAsync(NetworkIdentity prefab, Vector3 position, Quaternion rotation, Player owner, CancellationToken cancellationToken = default)
        {
            return await InstantiateAsync(prefab, position, rotation, null, owner, cancellationToken);
        }

        public async Task<NetworkIdentity> InstantiateAsync(NetworkIdentity prefab, Vector3 position, Quaternion rotation, NetworkIdentity parent, Player owner, CancellationToken cancellationToken = default)
        {
            Instantiate(prefab, position, rotation, parent, owner);

            var instantiation = new TaskCompletionSource<NetworkIdentity>();
            lock (_instantiationQueue)
            {
                _instantiationQueue.Enqueue(instantiation);
            }

            return await instantiation.Task;
        }

        private void DestroyObjectsOwnedByPlayer(Player player)
        {
            var identities = _identityManager.GetAll().ToList();
            for (int i = identities.Count - 1; i >= 0; i--)
            {
                var networkIdentity = identities[i];
                if (networkIdentity.Owner == player)
                {
                    NetworkDestroyInternal(networkIdentity.Value, isExistInScene: false, player);
                }
            }
        }

        public void Dispose()
        {
            _playerMsg.SendQueue();

            _hostMsg.Unregister<InstantiateMsg>(InstantiateInternal);
            _hostToPlayerMsg.Unregister<InstantiateMsg>(InstantiateFromSnapshot);
            _hostToPlayerMsg.Unregister<DeleteExistInSceneObjectsMsg>(DeleteExistInSceneObjects);
            _playerMsg.Unregister<NetworkDestroyMsg>(NetworkDestroy);
            _playerToHostMsg.Unregister<InstantiateMsg>(HOST_InstantiateInternal);

            _gameSessionEvent.OnDisconnected -= DestroyObjectsOwnedByPlayer;
        }
    }
}