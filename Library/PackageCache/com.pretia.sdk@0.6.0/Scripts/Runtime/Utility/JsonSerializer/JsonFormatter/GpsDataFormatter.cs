using System.Text;
using Utf8Json;

namespace PretiaArCloud
{
    public class GpsDataFormatter : IJsonFormatter<double[]>
    {
        public void Serialize(ref JsonWriter writer, double[] value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var json = formatterResolver.GetFormatterWithVerify<double[]>().ToJsonString(value, formatterResolver);
            writer.WriteString(json);
        }

        public double[] Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            var json = reader.ReadString();
            if (string.IsNullOrEmpty(json)) return null;
            if (json[0] != '[') return null;

            var newReader = new Utf8Json.JsonReader(Encoding.UTF8.GetBytes(json));
            return formatterResolver.GetFormatterWithVerify<double[]>().Deserialize(ref newReader, formatterResolver);
        }
    }
}