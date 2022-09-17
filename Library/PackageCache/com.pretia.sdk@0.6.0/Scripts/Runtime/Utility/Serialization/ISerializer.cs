using System;
using System.Buffers;
using System.IO;
using MessagePack;

namespace PretiaArCloud.Networking
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T data);
        void Serialize<T>(Stream stream, T data);
        void Serialize<T>(IBufferWriter<byte> stream, T data);
        T Deserialize<T>(ReadOnlySpan<byte> byteArray);
    }
}