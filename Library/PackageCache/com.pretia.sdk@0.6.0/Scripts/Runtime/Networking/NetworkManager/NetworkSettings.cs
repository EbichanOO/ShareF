using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [CreateAssetMenu(fileName = "Network Settings", menuName = "Pretia ArCloud/Network/NetworkSettings")]
    public class NetworkSettings : ScriptableObject
    {
        public int ClientSynchronizationIntervalInMillis = 33;
        public int HostSynchronizationIntervalInMillis = 33;

        internal bool EnableNetworkVisibility = false;
        internal float NetworkVisibilityRange;
    }
}