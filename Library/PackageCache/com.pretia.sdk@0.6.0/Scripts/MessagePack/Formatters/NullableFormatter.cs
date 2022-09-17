// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;

#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters
{
    public sealed class NullableFormatter<T> : INetworkVarMessagePackFormatter<T?>
        where T : struct
    {
        private int _fieldCount;
        public int FieldCount => _fieldCount;

        public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                options.Resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, options);
            }
        }

        public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                reader.ReadNil();
                return null;
            }
            else
            {
                return options.Resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;

            {
                var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
                _fieldCount = formatter.FieldCount;
                if (value == null)
                {
                    writer.WriteNil();
                }
                else
                {
                    formatter.SerializeNetworkVar(ref writer, value.Value, options, dirtyMask);
                }
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref T? instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;

            {
                if (reader.TryReadNil())
                {
                    instance = null;
                }
                else
                {
                    var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
                    var temp = default(T);
                    formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
                    instance = temp;
                }
            }
        }

        public ulong GetDirtyMask(T? before, T? after, MessagePackSerializerOptions options)
        {
            if (before.Equals(after)) return 0UL;

            IFormatterResolver resolver = options.Resolver;
            var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
            return formatter.GetDirtyMask(before.Value, after.Value, options);
        }
    }

    public sealed class StaticNullableFormatter<T> : INetworkVarMessagePackFormatter<T?>
        where T : struct
    {
        private readonly IMessagePackFormatter<T> underlyingFormatter;

        private int _fieldCount;

        public int FieldCount => _fieldCount;

        public StaticNullableFormatter(IMessagePackFormatter<T> underlyingFormatter)
        {
            this.underlyingFormatter = underlyingFormatter;
        }

        public void Serialize(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                this.underlyingFormatter.Serialize(ref writer, value.Value, options);
            }
        }

        public T? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                return this.underlyingFormatter.Deserialize(ref reader, options);
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, T? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;

            {
                var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
                _fieldCount = formatter.FieldCount;
                if (value == null)
                {
                    writer.WriteNil();
                }
                else
                {
                    formatter.SerializeNetworkVar(ref writer, value.Value, options, dirtyMask);
                }
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref T? instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;

            {
                if (reader.TryReadNil())
                {
                    instance = null;
                }
                else
                {
                    var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
                    var temp = default(T);
                    formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
                    instance = temp;
                }
            }
        }

        public ulong GetDirtyMask(T? before, T? after, MessagePackSerializerOptions options)
        {
            if (before.Equals(after)) return 0UL;

            IFormatterResolver resolver = options.Resolver;
            var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
            return formatter.GetDirtyMask(before.Value, after.Value, options);
        }
    }
}
