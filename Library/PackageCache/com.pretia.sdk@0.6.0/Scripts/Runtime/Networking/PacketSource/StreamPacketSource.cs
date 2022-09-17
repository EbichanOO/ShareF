using System;
using System.Buffers;
using System.IO;

namespace PretiaArCloud.Networking
{
    internal class StreamPacketSource : IPacketSource
    {
        private Stream _stream;
        private int _packetSizeLength;
        protected byte[] _packetSizeBuffer;

        internal StreamPacketSource(Stream stream, int packetSizeLength)
        {
            _stream = stream;
            _packetSizeLength = packetSizeLength;
            _packetSizeBuffer = new byte[_packetSizeLength];
        }

        public byte[] GetNextPacket()
        {
            int readBytes = _stream.ReadExactly(_packetSizeBuffer, 0, _packetSizeLength);
            if (readBytes == 0) return null;

            int packetSize = GetPacketSize();
            int remainingSize = packetSize - _packetSizeLength;
            byte[] packet = new byte[remainingSize];

            readBytes = _stream.ReadExactly(packet, 0, remainingSize);
            if (readBytes == 0) return null;

            return packet;
        }

        public void Send(ReadOnlySpan<byte> packet)
        {
            _stream.Write(packet);
        }

        public virtual void Close()
        {
            _stream.Close();
        }

        protected virtual int GetPacketSize()
        {
            return SpanBasedBitConverter.ToUInt16(_packetSizeBuffer);
        }
    }

    internal class RelocPacketSource : StreamPacketSource
    {
        internal RelocPacketSource(Stream stream) : base(stream, sizeof(Int32))
        {
        }

        protected override int GetPacketSize()
        {
            return SpanBasedBitConverter.ToInt32(_packetSizeBuffer);
        }
    }
}