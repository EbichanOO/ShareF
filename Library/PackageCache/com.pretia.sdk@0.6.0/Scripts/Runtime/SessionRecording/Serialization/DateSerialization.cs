using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;

namespace PretiaArCloud.RecordingPlayback.Serialization
{
    public class DateSerialization
    {
        /// <summary>
        /// Formatting for the timestamp.
        /// </summary>
        private const string DateFormat = "yyyyMMddHHmmssffff";

        /// <summary>
        /// Writes the date into the given stream.
        /// </summary>
        public static void Write(
            Stream stream,
            MessagePackSerializerOptions options,
            System.DateTime dateTime)
        {
            if (options == null || stream == null)
            {
                return;
            }

            var culture = System.Globalization.CultureInfo.InvariantCulture;
            var s = dateTime.ToUniversalTime()
                .ToString(DateFormat, culture);

            if (!long.TryParse(s, System.Globalization.NumberStyles.None,
                    culture, out var result))
            {
                throw new System.ArgumentException("Incorrect date time.");
            }

            MessagePackSerializer.Serialize(stream, result, options);
        }

        /// <summary>
        /// Reads the data from the given stream.
        /// </summary>
        public static System.DateTime Read(
            Stream stream,
            MessagePackSerializerOptions options = null,
            CancellationToken cancelToken = default)
        {

            var longDate = MessagePackSerializer
                .Deserialize<long>(stream, options, cancelToken);

            var culture = System.Globalization.CultureInfo.InvariantCulture;

            var date = System.DateTime.ParseExact(
                longDate.ToString(culture),
                DateFormat,
                culture
            );
            
            return date;
        }
    }
}