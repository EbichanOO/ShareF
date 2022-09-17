using System;

namespace PretiaArCloud.Networking
{
    [Flags]
    public enum OpType
    {
        Host = 1,
        Player = 2,
        PlayerToHost = 4,
        HostToPlayer = 8,
        All = Host | Player | PlayerToHost | HostToPlayer,
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false)]
    public class NetworkMessageAttribute : Attribute
    {
        public NetworkMessageAttribute() { }
    }
}
