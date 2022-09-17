using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class HostOnlyComponent : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _components = default;

        private IGameSession _gameSession = default;

        private async void OnEnable()
        {
            SetEnable(false);
            _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();
            _gameSession.OnHostAppointment += EnableComponents;

            bool isHost = await _gameSession.WaitForConnectionAsync() && _gameSession.LocalPlayer.IsHost;
            SetEnable(isHost);
        }

        private void OnDisable()
        {
            if (_gameSession != null)
            {
                _gameSession.OnHostAppointment -= EnableComponents;
            }
        }

        private void EnableComponents()
        {
            SetEnable(true);
        }

        private void SetEnable(bool value)
        {
            foreach (var component in _components)
            {
                component.enabled = value;
            }
        }
    }
}
