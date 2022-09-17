using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class RegisterNetworkPrefab : MonoBehaviour
    {
        [SerializeField] private NetworkIdentity[] _prefabs = default;
        private IGameSession _gameSession = default;

        private async void Start()
        {
            _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();

            foreach (var prefab in _prefabs)
            {
                _gameSession.NetworkSpawner.Register(prefab);
            }
        }
    }
}