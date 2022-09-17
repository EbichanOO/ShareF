using System.Buffers;
using MessagePack;

namespace PretiaArCloud.Networking
{
    public interface ISynchronizable
    {
        void Serialize(ref MessagePackWriter writer, MessagePackSerializerOptions options);
        void Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options);
    }
}