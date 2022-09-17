using System;
using System.Collections.Generic;

namespace PretiaArCloud.Networking
{
    public class IdentityManager
    {
        private Dictionary<uint, NetworkIdentity> _networkIdentityMap = default;

        public int Count => _networkIdentityMap.Count;

        public IdentityManager()
        {
            _networkIdentityMap = new Dictionary<uint, NetworkIdentity>();
        }

        public void Register(NetworkIdentity networkIdentity)
        {
            _networkIdentityMap.Add(networkIdentity.Value, networkIdentity);
        }

        public void Unregister(uint networkId)
        {
            _networkIdentityMap.Remove(networkId);
        }

        public NetworkIdentity Get(uint networkId)
        {
            return _networkIdentityMap[networkId];
        }

        public IEnumerable<NetworkIdentity> GetAll()
        {
            return _networkIdentityMap.Values;
        }

        public bool Contains(uint networkId)
        {
            return _networkIdentityMap.ContainsKey(networkId);
        }
    }
}