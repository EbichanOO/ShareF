using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class HostOnlyObject : MonoBehaviour
    {
        private GameObject[] _children = default;
        private IGameSession _gameSession = default;
        private bool[] _initialValues = default;

        private async void Awake()
        {
            _children = new GameObject[transform.childCount];
            _initialValues = new bool[transform.childCount];
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i] = transform.GetChild(i).gameObject;
                _initialValues[i] = _children[i].activeSelf;
                _children[i].SetActive(false);
            }

            _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();
            _gameSession.OnHostAppointment += EnableChildren;
        }

        private void OnDisable()
        {
            _gameSession.OnHostAppointment -= EnableChildren;
        }

        private void EnableChildren()
        {
            SetEnable(true);
        }

        private void SetEnable(bool value)
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].SetActive(_initialValues[i]);
            }
        }
    }
}