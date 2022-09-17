using System.IO;

namespace PretiaArCloud
{
    public interface IJsonSerializer
    {
        byte[] Serialize<T>(T data);
        string ToJsonString<T>(T data);
        T Deserialize<T>(string json);
        T Deserialize<T>(byte[] data);
        T Deserialize<T>(Stream stream);
    }
}