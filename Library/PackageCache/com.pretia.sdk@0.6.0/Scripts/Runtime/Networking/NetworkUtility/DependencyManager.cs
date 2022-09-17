using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud.Networking
{
    public class DependencyManager
    {
        private Dictionary<Type, object> _instanceCache = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            if (!_instanceCache.ContainsKey(typeof(T)))
            {
                _instanceCache.Add(typeof(T), instance);
            }
        }

        public void Register<T>(Task<T> instance)
        {
            if (!_instanceCache.ContainsKey(typeof(T)))
            {
                _instanceCache.Add(typeof(T), instance);
            }
        }

        public T Resolve<T>() where T : class
        {
            return _instanceCache[typeof(T)] as T;
        }

        public void Unregister<T>()
        {
            var type = typeof(T);
            if (_instanceCache.ContainsKey(type))
            {
                _instanceCache.Remove(type);
            }
        }

        public async Task<T> ResolveAsync<T>(CancellationToken cancellationToken = default) where T : class
        {
            var resolve = new TaskCompletionSource<T>();
            if (_instanceCache.TryGetValue(typeof(T), out object asyncObj))
            {
                using (cancellationToken.Register(() => resolve.TrySetCanceled()))
                {
                    var instance = asyncObj as Task<T>;
                    resolve.TrySetResult(await instance);
                }

                return await resolve.Task;
            }
            else
            {
                throw new KeyNotFoundException($"Failed resolving async instance of type {typeof(T)}");
            }
        }
    }
}