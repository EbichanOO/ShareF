using System;
#if UNITY_EDITOR
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif

namespace PretiaArCloud.Networking
{
    public interface IPacketSource
    {
        byte[] GetNextPacket();
        void Send(ReadOnlySpan<byte> packet);
        void Close();
    }
}