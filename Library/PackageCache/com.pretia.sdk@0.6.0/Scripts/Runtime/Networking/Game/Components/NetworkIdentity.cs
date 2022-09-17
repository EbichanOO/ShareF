#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class NetworkIdentity : MonoBehaviour
    {
        private uint _value = default;
        public uint Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private ushort _prefabId = default;
        public ushort PrefabId
        {
            get { return _prefabId; }
            set { _prefabId = value; }
        }

        private Player _owner = default;
        public Player Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        [SerializeField]
        // [HideInInspector]
        private bool _existInScene = default;

        private IGameSession _gameSession = default;

#if UNITY_EDITOR
        private void Reset()
        {
            var prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
            var prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            Debug.Log($"{prefabAssetType}, {prefabInstanceStatus}, {currentPrefabStage}, {PrefabStageUtility.GetPrefabStage(gameObject)}, {gameObject.scene.name}");

            _existInScene =
                (currentPrefabStage == null) &&
                ((prefabAssetType == PrefabAssetType.NotAPrefab) ||
                (prefabAssetType != PrefabAssetType.NotAPrefab && prefabInstanceStatus == PrefabInstanceStatus.Connected));
        }
#endif

        private async void Awake()
        {
            if (_existInScene)
            {
                gameObject.SetActive(false);
                _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();
                Value = _gameSession.NetworkIdProvider.Rent();
                bool isHost = await _gameSession.WaitForConnectionAsync() && _gameSession.LocalPlayer.IsHost;

                if (isHost)
                {
                    _gameSession.IdentityManager.Register(this);
                    gameObject.SetActive(true);
                }
                else
                {
                    _gameSession.ExistInSceneIdentityManager.Register(this);
                }
            }
        }

        private void OnDestroy()
        {
            if (_existInScene && _gameSession != null && _gameSession.LocalPlayer.IsHost && !_gameSession.Disposed)
            {
                _gameSession.PlayerMsg.Send(new NetworkDestroyMsg
                {
                    NetworkId = Value,
                    IsExistInScene = true,
                });
            }
        }

    }
}