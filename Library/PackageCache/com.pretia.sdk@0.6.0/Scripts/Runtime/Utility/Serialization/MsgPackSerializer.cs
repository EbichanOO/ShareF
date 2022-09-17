using System;
using System.Buffers;
using System.IO;
using MessagePack;

namespace PretiaArCloud.Networking
{
    public class MsgPackSerializer : ISerializer
    {
        private MessagePackSerializerOptions _options = default;
        public MessagePackSerializerOptions Options => _options;

        public MsgPackSerializer(IFormatterResolver appResolver)
        {
            _options = MessagePackSerializerOptions.Standard
                .WithCompression(MessagePackCompression.None)
                .WithResolver(new MsgPackPretiaResolver(appResolver));
        }

        public T Deserialize<T>(ReadOnlySpan<byte> byteArray)
        {
            return MessagePackSerializer.Deserialize<T>(byteArray.ToArray(), _options);
        }

        public byte[] Serialize<T>(T data)
        {
            return MessagePackSerializer.Serialize(data, _options);
        }

        public void Serialize<T>(Stream stream, T data)
        {
            MessagePackSerializer.Serialize<T>(stream, data, _options);
        }

        public void Serialize<T>(IBufferWriter<byte> writer, T data)
        {
            MessagePackSerializer.Serialize<T>(writer, data, _options);
        }
    }
}