using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [Obsolete]
    public class SceneDependency : MonoBehaviour
    {
        public static SceneDependency Instance = default;

        [SerializeField]
        private NetworkSettings _networkSettings = default;

        protected DependencyManager _dependencyManager = default;

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

            _dependencyManager = new DependencyManager();
            _dependencyManager.Register<NetworkSettings>(_networkSettings);
            _dependencyManager.Register<HttpClient>(new HttpClient());
            _dependencyManager.Register<IJwtDecoder>(new JwtDecoder());
            _dependencyManager.Register<IJsonSerializer>(new Utf8JsonSerializer());
            SetupDependencies();
        }

        public void Unregister<T>()
        {
            _dependencyManager.Unregister<T>();
        }

        protected virtual void SetupDependencies() { }

        public T Resolve<T>() where T : class
        {
            return _dependencyManager.Resolve<T>();
        }

        public void Register<T>(T instance)
        {
            _dependencyManager.Register(instance);
        }

        public void Register<T>(Task<T> task)
        {
            _dependencyManager.Register(task);
        }

        public async Task<T> ResolveAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            return await _dependencyManager.ResolveAsync<T>(cancellationToken);
        }
    }
}