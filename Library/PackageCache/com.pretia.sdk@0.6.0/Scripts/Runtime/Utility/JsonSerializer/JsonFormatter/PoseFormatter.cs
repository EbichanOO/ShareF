using UnityEngine;
using Utf8Json;

namespace PretiaArCloud
{
    public class PoseFormatter : IJsonFormatter<UnityEngine.Pose>
    {
        public Pose Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var v3Formatter = formatterResolver.GetFormatterWithVerify<UnityEngine.Vector3>();
            var qFormatter = formatterResolver.GetFormatterWithVerify<UnityEngine.Quaternion>();

            var pose = new Pose();

            reader.ReadIsBeginObject();

            reader.ReadPropertyName();
            pose.position = v3Formatter.Deserialize(ref reader, formatterResolver);

            reader.ReadIsValueSeparator();

            reader.ReadPropertyName();
            pose.rotation = qFormatter.Deserialize(ref reader, formatterResolver);

            reader.ReadIsEndObject();

            return pose;
        }

        public void Serialize(ref JsonWriter writer, Pose value, IJsonFormatterResolver formatterResolver)
        {
            var v3Formatter = formatterResolver.GetFormatterWithVerify<UnityEngine.Vector3>();
            var qFormatter = formatterResolver.GetFormatterWithVerify<UnityEngine.Quaternion>();

            writer.WriteBeginObject();

            writer.WritePropertyName("position");
            v3Formatter.Serialize(ref writer, value.position, formatterResolver);

            writer.WriteValueSeparator();

            writer.WritePropertyName("rotation");
            qFormatter.Serialize(ref writer, value.rotation, formatterResolver);

            writer.WriteEndObject();
        }
    }
}
