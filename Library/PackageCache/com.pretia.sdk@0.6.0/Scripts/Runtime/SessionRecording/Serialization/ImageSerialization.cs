using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.RecordingPlayback.Serialization
{
    public class ImageSerialization
    {
        /// <summary>
        /// Structure to give the image settings.
        /// </summary>
        public struct ImageSettings
        {
            public bool HasChanged;
            public int Width;
            public int Height;
            public TextureFormat Format;
            public int FPS;
            public ScreenOrientation Orientation;

            public float OrientationDegrees
            {
                get
                {
                    float result;
                    switch(Orientation)
                    {
                        case ScreenOrientation.Portrait: result = -90; break;
                        case ScreenOrientation.PortraitUpsideDown: result = 90; break;
                        case ScreenOrientation.LandscapeLeft: result = 0; break;
                        case ScreenOrientation.LandscapeRight: result = 180; break;
                        case ScreenOrientation.AutoRotation: result = 0; break;
                        default: result = 0; break;
                    };

                    return result;
                }
            }
        }

        /// <summary>
        /// Writes the Image data and image settings into the given stream.
        /// </summary>
        public static void Write(
            Stream stream,
            MessagePackSerializerOptions options,
            byte[] imageData,
            bool settingsChanged,
            int width,
            int height,
            TextureFormat format,
            int fps,
            ScreenOrientation orientation)
        {
            if (options == null || stream == null)
            {
                return;
            }

            MessagePackSerializer.Serialize(stream, settingsChanged, options);
            if (settingsChanged)
            {
                MessagePackSerializer.Serialize(stream, width, options);
                MessagePackSerializer.Serialize(stream, height, options);
                MessagePackSerializer.Serialize(stream, (int) format, options);
                MessagePackSerializer.Serialize(stream, fps, options);
                MessagePackSerializer.Serialize(stream, (int) orientation, options);
            }

            MessagePackSerializer.Serialize(stream, imageData, options);
        }

        /// <summary>
        /// Writes the Image data and image settings into the given stream.
        /// </summary>
        public static void Write(
            Stream stream,
            MessagePackSerializerOptions options,
            byte[] imageData,
            ImageSettings settings)
        {
            Write(
                stream, 
                options,
                imageData, 
                settings.HasChanged, 
                settings.Width, 
                settings.Height, 
                settings.Format, 
                settings.FPS, 
                settings.Orientation
            );
        }

        /// <summary>
        /// Reads the data from the given stream.
        /// </summary>
        public static async Task<(ImageSettings settings, byte[] data)> Read(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {
            if (stream == null)
            {
                return (new ImageSettings(), System.Array.Empty<byte>());
            }

            // Whether are settings stored.
            var storedSettings = await MessagePackSerializer
                .DeserializeAsync<bool>(stream, options, cancelToken);

            var settings = new ImageSettings();
            if (storedSettings)
            {
                settings.HasChanged = true;
                settings.Width = await MessagePackSerializer
                    .DeserializeAsync<int>(stream, options, cancelToken);

                settings.Height = await MessagePackSerializer
                    .DeserializeAsync<int>(stream, options, cancelToken);

                settings.Format = (TextureFormat) await MessagePackSerializer
                    .DeserializeAsync<int>(stream, options, cancelToken);

                settings.FPS = await MessagePackSerializer
                    .DeserializeAsync<int>(stream, options, cancelToken);

                settings.Orientation = (ScreenOrientation) await MessagePackSerializer
                    .DeserializeAsync<int>(stream, options, cancelToken);
            }

            // Reads at the same order it was stored.
            var data = await MessagePackSerializer
                .DeserializeAsync<byte[]>(stream, options, cancelToken);

            return (settings, data);
        }
    }
}