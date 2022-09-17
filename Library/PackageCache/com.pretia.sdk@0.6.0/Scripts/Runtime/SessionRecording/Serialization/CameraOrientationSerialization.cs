using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Serialization
{
    public static class CameraOrientationSerialization
    {
        /// <summary>
        /// Writes the position and rotation into the given stream.
        /// </summary>
        public static void Write(
            Stream stream,
            MessagePackSerializerOptions options,
            ScreenOrientation orientation)
        {
            if (options == null || stream == null)
            {
                return;
            }

            MessagePackSerializer.Serialize(stream, (int) orientation, options);
        }

        /// <summary>
        /// Reads the data from the given stream.
        /// </summary>
        public static async Task<ScreenOrientation> Read(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            if (stream == null)
            {
                return ScreenOrientation.Landscape;
            }

            var orientation = await MessagePackSerializer
                .DeserializeAsync<int>(stream, options, cancelToken);

            return (ScreenOrientation) orientation;
        }
    }
}