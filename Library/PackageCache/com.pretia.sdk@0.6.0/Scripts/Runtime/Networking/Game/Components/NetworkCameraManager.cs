using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace PretiaArCloud.Networking
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(ARPoseDriver))]
    public class NetworkCameraManager : MonoBehaviour
    {
        public static NetworkCameraManager Instance;

        [SerializeField]
        [HideInInspector]
        private NetworkIdentity _networkCameraProxyPrefab = default;
        private IGameSession _gameSession;

        private Dictionary<uint, Transform> _networkCameraProxies = new Dictionary<uint, Transform>();
        public Transform LocalProxy { get; private set; }

        public delegate void LocalProxyReadyEvent(Transform localProxy);
        public event LocalProxyReadyEvent OnLocalProxyReady;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private async void OnEnable()
        {
            _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();
            _gameSession.OnLocalPlayerJoined += SpawnNetworkCameraProxy;
            _gameSession.NetworkSpawner.OnInstantiated += RegisterPlayerCamera;
            _gameSession.OnDisconnected += UnregisterPlayerCamera;
        }

        private void OnDisable()
        {
            if (_gameSession != null)
            {
                _gameSession.OnLocalPlayerJoined -= SpawnNetworkCameraProxy;
                _gameSession.NetworkSpawner.OnInstantiated -= RegisterPlayerCamera;
                _gameSession.OnDisconnected -= UnregisterPlayerCamera;
            }
        }

        private void RegisterPlayerCamera(NetworkIdentity networkIdentity)
        {
            var networkCameraProxy = networkIdentity.GetComponent<NetworkCameraProxy>();
            if (networkCameraProxy != null)
            {
                if (_networkCameraProxies.ContainsKey(networkIdentity.Owner.UserNumber))
                {
                    throw new Exception($"NetworkCameraProxy owned by player {networkIdentity.Owner} already exists");
                }

                networkCameraProxy.SetCameraTransform(transform);
                _networkCameraProxies.Add(networkIdentity.Owner.UserNumber, networkIdentity.transform);
                if (networkIdentity.Owner == _gameSession.LocalPlayer)
                {
                    LocalProxy = networkIdentity.transform;
                    OnLocalProxyReady?.Invoke(LocalProxy);
                }
            }
        }

        private void UnregisterPlayerCamera(Player player)
        {
            if (_networkCameraProxies.ContainsKey(player.UserNumber))
            {
                _networkCameraProxies.Remove(player.UserNumber);
            }
        }

        public bool TryGetProxyByOwner(Player owner, out Transform cameraProxy)
        {
            if (_networkCameraProxies.TryGetValue(owner.UserNumber, out cameraProxy))
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"NetworkCameraProxy owned by player {owner.UserNumber} is not found");
                return false;
            }
        }

        private void SpawnNetworkCameraProxy()
        {
            _gameSession.NetworkSpawner.Instantiate(
                prefab: _networkCameraProxyPrefab,
                position: transform.position,
                rotation: transform.rotation,
                owner: _gameSession.LocalPlayer);
        }
    }
}