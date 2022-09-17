using UnityEngine;
using PretiaArCloud.Networking;

namespace PretiaArCloud.Networking
{
    public class NetworkUpdateSender : MonoBehaviour
    {
        private IGameSession _gameSession = default;

        private async void Awake()
        {
            _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();
        }

        private void FixedUpdate()
        {
            if (_gameSession != null)
            {
                _gameSession.HostMsg.SendQueue();
            }
        }
    }
}
