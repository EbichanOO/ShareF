using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace PretiaArCloud.Networking
{
    public class FormatterResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new FormatterResolver();

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (IMessagePackFormatter<T>)f;
                }
            }
        }

        internal static class GeneratedResolverGetFormatterHelper
        {
            private static readonly Dictionary<Type, object> _lookup = new Dictionary<Type, object>();

            static GeneratedResolverGetFormatterHelper()
            {
            }

            internal static object GetFormatter(Type t)
            {
                if (_lookup.TryGetValue(t, out object f))
                {
                    return f;
                }
                else
                {
                    return null;
                }
            }

            internal static void Add(Type type, object formatter)
            {
                _lookup.Add(type, formatter);
            }
        }

        public static void Add(Type type, object formatter)
        {
            GeneratedResolverGetFormatterHelper.Add(type, formatter);
        }
    }

    public class ClassStub { }

    public sealed class ObjectFormatterTemplate : IMessagePackFormatter<ClassStub>
    {
        public void Serialize(ref MessagePackWriter writer, ClassStub value, MessagePackSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public ClassStub Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StringIndexedFormatterTemplate : IMessagePackFormatter<ClassStub>
    {
        private readonly AutomataDictionary ____keyMapping;
        private readonly byte[][] ____stringByteKeys;

        public StringIndexedFormatterTemplate()
        {
            ____keyMapping = new AutomataDictionary();
        }

        public void Serialize(ref MessagePackWriter writer, ClassStub value, MessagePackSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public ClassStub Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class UnionFormatterTemplate : IMessagePackFormatter<ClassStub>
    {
        private readonly Dictionary<Type, int> typeToKey;

        public UnionFormatterTemplate()
        {
            typeToKey = new Dictionary<Type, int>();
        }

        public void Serialize(ref MessagePackWriter writer, ClassStub value, MessagePackSerializerOptions options)
        {
            if (value != null && typeToKey.TryGetValue(value.GetType(), out int key))
            {
                writer.WriteArrayHeader(2);
                writer.WriteInt32(key);
                SerializeInternal(ref writer, value, options, key);
                return;
            }

            writer.WriteNil();
        }

        private void SerializeInternal(ref MessagePackWriter writer, ClassStub value, MessagePackSerializerOptions options, int key)
        {
            throw new NotImplementedException();
        }

        public ClassStub Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            if (reader.ReadArrayHeader() != 2)
            {
                throw new InvalidOperationException("Invalid Union data was detected");
            }

            options.Security.DepthStep(ref reader);
            var key = reader.ReadInt32();

            ClassStub result = DeserializeInternal(ref reader, options, key);
            reader.Depth--;
            return result;
        }

        private ClassStub DeserializeInternal(ref MessagePackReader reader, MessagePackSerializerOptions options, int key)
        {
            throw new NotImplementedException();
        }
    }
}