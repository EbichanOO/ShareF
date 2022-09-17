using System.Collections.Generic;
using UnityEngine;
using Utf8Json;

namespace PretiaArCloud
{
    public class MapAnchorsFormatter : IJsonFormatter<List<UnityEngine.Pose>>
    {
        public void Serialize(ref JsonWriter writer, List<Pose> value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return;
            }

            var json = formatterResolver.GetFormatterWithVerify<List<Pose>>().ToJsonString(value, formatterResolver);
            writer.WriteString(json);
        }

        public List<Pose> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            var json = reader.ReadString();
            if (string.IsNullOrEmpty(json)) return null;

            return formatterResolver.GetFormatterWithVerify<List<Pose>>().Deserialize(ref reader, formatterResolver);
        }
    }
}