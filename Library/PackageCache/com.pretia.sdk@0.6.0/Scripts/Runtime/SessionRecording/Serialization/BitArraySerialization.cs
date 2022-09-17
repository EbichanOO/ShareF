using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;

namespace PretiaArCloud.RecordingPlayback.Serialization
{
    public static class BitArraySerialization
    {
        /// <summary>
        /// Writes the array into the given stream.
        /// </summary>
        public static void Write(
            Stream stream,
            MessagePackSerializerOptions options,
            BitArray array)
        {
            if (options == null || stream == null)
            {
                return;
            }

            MessagePackSerializer.Serialize(stream, array, options);
        }

        /// <summary>
        /// Reads the data from the given stream.
        /// </summary>
        public static BitArray Read(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            return MessagePackSerializer.Deserialize<BitArray>(
                stream, options, cancelToken);
        }
    }
}