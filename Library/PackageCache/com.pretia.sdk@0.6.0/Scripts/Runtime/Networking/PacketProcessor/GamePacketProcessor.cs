using System;
using System.Buffers;

namespace PretiaArCloud.Networking
{
    using BitConverter = SpanBasedBitConverter;

    internal class GamePacketProcessor
    {
        internal void ParseConnection(ReadOnlySpan<byte> buffer, out uint userNumber, out string displayName)
        {
            userNumber = BitConverter.ToUInt32(buffer);
            displayName = StringEncoder.Instance.GetString(buffer.Slice(sizeof(uint)));
        }

        internal void ParsePlayerMessage(ReadOnlyMemory<byte> buffer, out ReadOnlyMemory<byte> msg, out uint userNumber)
        {
            msg = buffer.Slice(0, buffer.Length - sizeof(uint));
            userNumber = BitConverter.ToUInt32(buffer.Slice(msg.Length).Span);
        }

        internal void ParseDisconnect(ReadOnlySpan<byte> buffer, out uint userNumber)
        {
            userNumber = BitConverter.ToUInt32(buffer);
        }

        internal void ParseError(ReadOnlySpan<byte> buffer, out NetworkStatusCode errorCode, out string errorMessage)
        {
            errorCode = (NetworkStatusCode)BitConverter.ToUInt16(buffer);
            errorMessage = StringEncoder.Instance.GetString(buffer.Slice(sizeof(ushort)));
        }

        internal byte[] BuildPacket(byte[] buffer, ushort protocolId)
        {
            byte[] packet = new byte[4 + buffer.Length];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), protocolId);
            buffer.CopyTo(packetSpan.Slice(4));

            return packet;
        }

        internal void BuildAndWriteMessagePacket(ArrayBufferWriter<byte> bufferWriter, byte[] buffer, ushort messageType, ushort tick)
        {
            int packetLength = 4 + buffer.Length;
            var packetSpan = bufferWriter.GetSpan(packetLength);

            SpanBasedBitConverter.TryWriteBytes(packetSpan, messageType);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)buffer.Length);
            buffer.CopyTo(packetSpan.Slice(4));

            bufferWriter.Advance(packetLength);

            var headerSpan = bufferWriter.Buffer.AsSpan();
            SpanBasedBitConverter.TryWriteBytes(headerSpan, (ushort)bufferWriter.Index);
            SpanBasedBitConverter.TryWriteBytes(headerSpan.Slice(6), tick);
            headerSpan[8]++;
        }

        internal byte[] BuildMessagePacket(ReadOnlySpan<byte> buffer, ushort protocolId, ushort messageType, ushort tick)
        {
            int packetLength = 8 + buffer.Length;
            byte[] packet = new byte[packetLength];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packetLength);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), protocolId);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(4), messageType);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(6), tick);
            buffer.CopyTo(packetSpan.Slice(8));

            return packet;
        }

        internal void BuildMessageHeader(Span<byte> destination, ushort packetLength, ushort protocolId, ushort messageType, ushort tick)
        {
            SpanBasedBitConverter.TryWriteBytes(destination, packetLength);
            SpanBasedBitConverter.TryWriteBytes(destination.Slice(2), protocolId);
            SpanBasedBitConverter.TryWriteBytes(destination.Slice(4), messageType);
            SpanBasedBitConverter.TryWriteBytes(destination.Slice(6), tick);
        }

        internal byte[] BuildHostToPlayerMessagePacket(byte[] buffer, ushort protocolId, ushort messageType, uint userNumber, ushort tick)
        {
            byte[] packet = new byte[12 + buffer.Length];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), protocolId);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(4), messageType);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(6), tick);
            buffer.CopyTo(packetSpan.Slice(8));
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(packetSpan.Length - 4), userNumber);

            return packet;
        }

        internal void AppendUserNumber(ArrayBufferWriter<byte> bufferWriter, uint userNumber)
        {
            var span = bufferWriter.GetSpan(sizeof(uint));
            SpanBasedBitConverter.TryWriteBytes(span, userNumber);
            bufferWriter.Advance(sizeof(uint));

            var headerSpan = bufferWriter.Buffer.AsSpan();
            SpanBasedBitConverter.TryWriteBytes(headerSpan, (ushort)bufferWriter.Index);
        }

        internal byte[] BuildDisconnectPacket(ushort protocolId)
        {
            byte[] packet = new byte[4];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), protocolId);

            return packet;
        }

        internal void ResetArrayBuffer(ArrayBufferWriter<byte> bufferWriter, ushort protocolId)
        {
            bufferWriter.Clear();
            var span = bufferWriter.GetSpan(9);
            SpanBasedBitConverter.TryWriteBytes(span.Slice(2), protocolId);
            SpanBasedBitConverter.TryWriteBytes(span.Slice(4), SpecialMessage.COMPOUND_MESSAGE);
            bufferWriter.Advance(9);
        }

        internal void BuildSynchronizationTick(Span<byte> destination, ushort tick)
        {
            SpanBasedBitConverter.TryWriteBytes(destination, tick);
        }
    }
}