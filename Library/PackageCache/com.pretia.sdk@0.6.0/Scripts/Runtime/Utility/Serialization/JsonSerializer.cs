using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    public class JsonSerializer : ISerializer
    {
        private class CollectionWrapper<T>
        {
            public T Collection;

            public CollectionWrapper(T collection)
            {
                Collection = collection;
            }
        }

        public T Deserialize<T>(ReadOnlySpan<byte> byteArray)
        {
            Type type = typeof(T);
            string jsonString = StringEncoder.Instance.GetString(byteArray);
            // Debug.Log(string.Format("Deserialize: {0}", jsonString));

            if (type.IsArray
            || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return JsonUtility.FromJson<CollectionWrapper<T>>(jsonString).Collection;
            }
            else
            {
                return JsonUtility.FromJson<T>(jsonString);
            }
        }

        public byte[] Serialize<T>(T data)
        {
            string jsonString = null;
            if (typeof(T).IsArray
            || data is IList)
            {
                jsonString = JsonUtility.ToJson(new CollectionWrapper<T>(data));
            }
            else
            {
                jsonString = JsonUtility.ToJson(data);
            }

            // Debug.Log(string.Format("Serialize: {0}", jsonString));
            return StringEncoder.Instance.GetBytes(jsonString);
        }

        public void Serialize<T>(Stream stream, T data)
        {
            throw new NotImplementedException();
        }

        public void Serialize<T>(IBufferWriter<byte> stream, T data)
        {
            throw new NotImplementedException();
        }
    }
}