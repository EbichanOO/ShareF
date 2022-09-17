using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Serialization
{
    public class PoseSerialization
    {
        /// <summary>
        /// Writes the position and rotation into the given stream.
        /// </summary>
        public static void Write(
            Stream stream,
            MessagePackSerializerOptions options,
            Vector3 position,
            Quaternion rotation)
        {
            if (options == null || stream == null)
            {
                return;
            }

            MessagePackSerializer.Serialize(stream, position, options);
            MessagePackSerializer.Serialize(stream, rotation, options);
        }

        /// <summary>
        /// Reads the data from the given stream.
        /// </summary>
        public static (Vector3 position, Quaternion rotation) Read(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            if (stream == null)
            {
                return (Vector3.zero, Quaternion.identity);
            }

            var position = MessagePackSerializer
                .Deserialize<Vector3>(stream, options, cancelToken);

            var rotation = MessagePackSerializer
                .Deserialize<Quaternion>(stream, options, cancelToken);

            return (position, rotation);
        }
    }
}