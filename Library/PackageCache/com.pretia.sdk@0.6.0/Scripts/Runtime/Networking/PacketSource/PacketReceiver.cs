using System;
using System.Threading;

namespace PretiaArCloud.Networking
{
    internal class PacketReceiver
    {
        private IPacketSource _packetSource;
        private Action<byte[]> _callback;

        internal PacketReceiver(IPacketSource packetSource, Action<byte[]> callback)
        {
            _packetSource = packetSource;
            _callback = callback;
        }

        internal Thread Start()
        {
            Thread thread = new Thread(ReceiveLoop);
            thread.IsBackground = true;
            thread.Start();

            return thread;
        }

        private void ReceiveLoop()
        {
            while (true)
            {
                byte[] packet = _packetSource.GetNextPacket();
                if (packet == null) break;

                _callback.Invoke(packet);
            }
        }
    }
}