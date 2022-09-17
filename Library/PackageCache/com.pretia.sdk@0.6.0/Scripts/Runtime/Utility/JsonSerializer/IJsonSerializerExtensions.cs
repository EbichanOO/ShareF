using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PretiaArCloud
{
    public static class IJsonSerializerExtensions
    {
        public static async Task<T> DeserializeAsync<T>(this IJsonSerializer jsonSerializer, byte[] data, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => jsonSerializer.Deserialize<T>(data), cancellationToken);
        }

        public static async Task<T> DeserializeAsync<T>(this IJsonSerializer jsonSerializer, string json, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => jsonSerializer.Deserialize<T>(json), cancellationToken);
        }

        public static async Task<T> DeserializeAsync<T>(this IJsonSerializer jsonSerializer, Stream stream, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => jsonSerializer.Deserialize<T>(stream), cancellationToken);
        }

        public static async Task<string> ToJsonStringAsync<T>(this IJsonSerializer jsonSerializer, T data, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => jsonSerializer.ToJsonString(data), cancellationToken);
        }

        public static async Task<byte[]> SerializeAsync<T>(this IJsonSerializer jsonSerializer, T data, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => jsonSerializer.Serialize(data), cancellationToken);
        }
    }
}