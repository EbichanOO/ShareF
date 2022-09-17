// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using MessagePack.Internal;

#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters
{
    // NET40 -> BigInteger, Complex, Tuple

    // byte[] is special. represents bin type.
    public sealed class ByteArrayFormatter : INetworkVarMessagePackFormatter<byte[]>
    {
        public static readonly ByteArrayFormatter Instance = new ByteArrayFormatter();

        public int FieldCount => 1;

        private ByteArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, byte[] value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public byte[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadBytes()?.ToArray();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, byte[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref byte[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = reader.ReadBytes()?.ToArray();
            }
        }

        public ulong GetDirtyMask(byte[] before, byte[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableStringFormatter : INetworkVarMessagePackFormatter<String>
    {
        public static readonly NullableStringFormatter Instance = new NullableStringFormatter();

        public int FieldCount => 1;

        private NullableStringFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, string value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public string Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadString();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, string value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref string instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = reader.ReadString();
            }
        }

        public ulong GetDirtyMask(string before, string after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableStringArrayFormatter : INetworkVarMessagePackFormatter<String[]>
    {
        public static readonly NullableStringArrayFormatter Instance = new NullableStringArrayFormatter();

        public int FieldCount => 1;

        private NullableStringArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, String[] value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.WriteArrayHeader(value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    writer.Write(value[i]);
                }
            }
        }

        public String[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new String[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadString();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, string[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref string[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(string[] before, string[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class DecimalFormatter : INetworkVarMessagePackFormatter<Decimal>
    {
        public static readonly DecimalFormatter Instance = new DecimalFormatter();

        public int FieldCount => 1;

        private DecimalFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, decimal value, MessagePackSerializerOptions options)
        {
            var dest = writer.GetSpan(MessagePackRange.MaxFixStringLength);
            if (System.Buffers.Text.Utf8Formatter.TryFormat(value, dest.Slice(1), out var written))
            {
                // write header
                dest[0] = (byte)(MessagePackCode.MinFixStr | written);
                writer.Advance(written + 1);
            }
            else
            {
                // reset writer's span previously acquired that does not use
                writer.Advance(0);
                writer.Write(value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public decimal Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (!(reader.ReadStringSequence() is ReadOnlySequence<byte> sequence))
            {
                throw new MessagePackSerializationException(string.Format("Unexpected msgpack code {0} ({1}) encountered.", MessagePackCode.Nil, MessagePackCode.ToFormatName(MessagePackCode.Nil)));
            }

            if (sequence.IsSingleSegment)
            {
                var span = sequence.First.Span;
                if (System.Buffers.Text.Utf8Parser.TryParse(span, out decimal result, out var bytesConsumed))
                {
                    if (span.Length != bytesConsumed)
                    {
                        throw new MessagePackSerializationException("Unexpected length of string.");
                    }

                    return result;
                }
            }
            else
            {
                // sequence.Length is not free
                var seqLen = (int)sequence.Length;
                if (seqLen < 128)
                {
                    Span<byte> span = stackalloc byte[seqLen];
                    sequence.CopyTo(span);
                    if (System.Buffers.Text.Utf8Parser.TryParse(span, out decimal result, out var bytesConsumed))
                    {
                        if (seqLen != bytesConsumed)
                        {
                            throw new MessagePackSerializationException("Unexpected length of string.");
                        }

                        return result;
                    }
                }
                else
                {
                    var rentArray = ArrayPool<byte>.Shared.Rent(seqLen);
                    try
                    {
                        sequence.CopyTo(rentArray);
                        if (System.Buffers.Text.Utf8Parser.TryParse(rentArray.AsSpan(0, seqLen), out decimal result, out var bytesConsumed))
                        {
                            if (seqLen != bytesConsumed)
                            {
                                throw new MessagePackSerializationException("Unexpected length of string.");
                            }

                            return result;
                        }
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(rentArray);
                    }
                }
            }

            throw new MessagePackSerializationException("Can't parse to decimal, input string was not in a correct format.");
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, decimal value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref decimal instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }

        }

        public ulong GetDirtyMask(decimal before, decimal after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class TimeSpanFormatter : INetworkVarMessagePackFormatter<TimeSpan>
    {
        public static readonly IMessagePackFormatter<TimeSpan> Instance = new TimeSpanFormatter();

        public int FieldCount => 1;

        private TimeSpanFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, TimeSpan value, MessagePackSerializerOptions options)
        {
            writer.Write(value.Ticks);
            return;
        }

        public TimeSpan Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, TimeSpan value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref TimeSpan instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(TimeSpan before, TimeSpan after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class DateTimeOffsetFormatter : INetworkVarMessagePackFormatter<DateTimeOffset>
    {
        public static readonly IMessagePackFormatter<DateTimeOffset> Instance = new DateTimeOffsetFormatter();

        public int FieldCount => 1;

        private DateTimeOffsetFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, DateTimeOffset value, MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.Write(new DateTime(value.Ticks, DateTimeKind.Utc)); // current ticks as is
            writer.Write((short)value.Offset.TotalMinutes); // offset is normalized in minutes
            return;
        }

        public DateTimeOffset Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var count = reader.ReadArrayHeader();

            if (count != 2)
            {
                throw new MessagePackSerializationException("Invalid DateTimeOffset format.");
            }

            DateTime utc = reader.ReadDateTime();

            var dtOffsetMinutes = reader.ReadInt16();

            return new DateTimeOffset(utc.Ticks, TimeSpan.FromMinutes(dtOffsetMinutes));
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, DateTimeOffset value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref DateTimeOffset instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(DateTimeOffset before, DateTimeOffset after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class GuidFormatter : INetworkVarMessagePackFormatter<Guid>
    {
        public static readonly IMessagePackFormatter<Guid> Instance = new GuidFormatter();

        public int FieldCount => 1;

        private GuidFormatter()
        {
        }

        public unsafe void Serialize(ref MessagePackWriter writer, Guid value, MessagePackSerializerOptions options)
        {
            byte* pBytes = stackalloc byte[36];
            Span<byte> bytes = new Span<byte>(pBytes, 36);
            new GuidBits(ref value).Write(bytes);
            writer.WriteString(bytes);
        }

        public Guid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            ReadOnlySequence<byte> segment = reader.ReadStringSequence().Value;
            if (segment.Length != 36)
            {
                throw new MessagePackSerializationException("Unexpected length of string.");
            }

            GuidBits result;
            if (segment.IsSingleSegment)
            {
                result = new GuidBits(segment.First.Span);
            }
            else
            {
                Span<byte> bytes = stackalloc byte[36];
                segment.CopyTo(bytes);
                result = new GuidBits(bytes);
            }

            return result.Value;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Guid value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Guid instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(Guid before, Guid after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UriFormatter : INetworkVarMessagePackFormatter<Uri>
    {
        public static readonly IMessagePackFormatter<Uri> Instance = new UriFormatter();

        public int FieldCount => 1;

        private UriFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Uri value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.ToString());
            }
        }

        public Uri Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                return new Uri(reader.ReadString(), UriKind.RelativeOrAbsolute);
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Uri value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Uri instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(Uri before, Uri after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class VersionFormatter : INetworkVarMessagePackFormatter<Version>
    {
        public static readonly IMessagePackFormatter<Version> Instance = new VersionFormatter();

        public int FieldCount => 1;

        private VersionFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Version value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.ToString());
            }
        }

        public Version Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                return new Version(reader.ReadString());
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Version value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Version instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(Version before, Version after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class KeyValuePairFormatter<TKey, TValue> : INetworkVarMessagePackFormatter<KeyValuePair<TKey, TValue>>
    {
        private int _fieldCount;
        public int FieldCount => _fieldCount;

        public void Serialize(ref MessagePackWriter writer, KeyValuePair<TKey, TValue> value, MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            IFormatterResolver resolver = options.Resolver;
            resolver.GetFormatterWithVerify<TKey>().Serialize(ref writer, value.Key, options);
            resolver.GetFormatterWithVerify<TValue>().Serialize(ref writer, value.Value, options);
            return;
        }

        public KeyValuePair<TKey, TValue> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var count = reader.ReadArrayHeader();

            if (count != 2)
            {
                throw new MessagePackSerializationException("Invalid KeyValuePair format.");
            }

            IFormatterResolver resolver = options.Resolver;
            options.Security.DepthStep(ref reader);
            try
            {
                TKey key = resolver.GetFormatterWithVerify<TKey>().Deserialize(ref reader, options);
                TValue value = resolver.GetFormatterWithVerify<TValue>().Deserialize(ref reader, options);
                return new KeyValuePair<TKey, TValue>(key, value);
            }
            finally
            {
                reader.Depth--;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, KeyValuePair<TKey, TValue> value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            int fieldCount = 0;
            IFormatterResolver resolver = options.Resolver;

            {
                var formatter = resolver.GetFormatterWithVerify<TKey>() as INetworkVarMessagePackFormatter<TKey>;
                formatter.SerializeNetworkVar(ref writer, value.Key, options, dirtyMask);
                fieldCount += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<TValue>() as INetworkVarMessagePackFormatter<TValue>;
                formatter.SerializeNetworkVar(ref writer, value.Value, options, dirtyMask >> fieldCount);
                fieldCount += formatter.FieldCount;
            }

            _fieldCount = fieldCount;
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref KeyValuePair<TKey, TValue> instance, ulong dirtyMask)
        {
            int fieldCount = 0;
            IFormatterResolver resolver = options.Resolver;

            TKey key = instance.Key;
            TValue value = instance.Value;

            {
                var formatter = resolver.GetFormatterWithVerify<TKey>() as INetworkVarMessagePackFormatter<TKey>;
                formatter.DeserializeNetworkVar(ref reader, options, ref key, dirtyMask);
                fieldCount += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<TValue>() as INetworkVarMessagePackFormatter<TValue>;
                formatter.DeserializeNetworkVar(ref reader, options, ref value, dirtyMask >> fieldCount);
                fieldCount += formatter.FieldCount;
            }

            instance = new KeyValuePair<TKey, TValue>(key, value);
        }

        public ulong GetDirtyMask(KeyValuePair<TKey, TValue> before, KeyValuePair<TKey, TValue> after, MessagePackSerializerOptions options)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            ulong dirtyMask = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<TKey>() as INetworkVarMessagePackFormatter<TKey>;
                dirtyMask |= formatter.GetDirtyMask(before.Key, after.Key, options);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<TValue>() as INetworkVarMessagePackFormatter<TValue>;
                dirtyMask |= formatter.GetDirtyMask(before.Value, after.Value, options);
                fieldCounter += formatter.FieldCount;
            }

            return dirtyMask;
        }
    }

    public sealed class StringBuilderFormatter : INetworkVarMessagePackFormatter<StringBuilder>
    {
        public static readonly IMessagePackFormatter<StringBuilder> Instance = new StringBuilderFormatter();

        public int FieldCount => 1;

        private StringBuilderFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, StringBuilder value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.ToString());
            }
        }

        public StringBuilder Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                return new StringBuilder(reader.ReadString());
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, StringBuilder value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref StringBuilder instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(StringBuilder before, StringBuilder after, MessagePackSerializerOptions options)
        {
            return before.ToString() == after.ToString() ? 0UL : 1UL;
        }
    }

    public sealed class BitArrayFormatter : INetworkVarMessagePackFormatter<BitArray>
    {
        public static readonly IMessagePackFormatter<BitArray> Instance = new BitArrayFormatter();

        public int FieldCount => 1;

        private BitArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, BitArray value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                var len = value.Length;
                writer.WriteArrayHeader(len);
                for (int i = 0; i < len; i++)
                {
                    writer.Write(value.Get(i));
                }

                return;
            }
        }

        public BitArray Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                var len = reader.ReadArrayHeader();

                var array = new BitArray(len);
                for (int i = 0; i < len; i++)
                {
                    array[i] = reader.ReadBoolean();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, BitArray value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref BitArray instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(BitArray before, BitArray after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class BigIntegerFormatter : INetworkVarMessagePackFormatter<System.Numerics.BigInteger>
    {
        public static readonly IMessagePackFormatter<System.Numerics.BigInteger> Instance = new BigIntegerFormatter();

        public int FieldCount => 1;

        private BigIntegerFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, System.Numerics.BigInteger value, MessagePackSerializerOptions options)
        {
#if NETCOREAPP2_1
            if (!writer.OldSpec)
            {
                // try to get bin8 buffer.
                var span = writer.GetSpan(byte.MaxValue);
                if (value.TryWriteBytes(span.Slice(2), out var written))
                {
                    span[0] = MessagePackCode.Bin8;
                    span[1] = (byte)written;

                    writer.Advance(written + 2);
                    return;
                }
                else
                {
                    // reset writer's span previously acquired that does not use
                    writer.Advance(0);
                }
            }
#endif

            writer.Write(value.ToByteArray());
            return;
        }

        public System.Numerics.BigInteger Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            ReadOnlySequence<byte> bytes = reader.ReadBytes().Value;
#if NETCOREAPP2_1
            if (bytes.IsSingleSegment)
            {
                return new System.Numerics.BigInteger(bytes.First.Span);
            }
            else
            {
                byte[] bytesArray = ArrayPool<byte>.Shared.Rent((int)bytes.Length);
                try
                {
                    bytes.CopyTo(bytesArray);
                    return new System.Numerics.BigInteger(bytesArray.AsSpan(0, (int)bytes.Length));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(bytesArray);
                }
            }
#else
            return new System.Numerics.BigInteger(bytes.ToArray());
#endif
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, BigInteger value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref BigInteger instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(BigInteger before, BigInteger after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class ComplexFormatter : INetworkVarMessagePackFormatter<System.Numerics.Complex>
    {
        public static readonly IMessagePackFormatter<System.Numerics.Complex> Instance = new ComplexFormatter();

        public int FieldCount => 1;

        private ComplexFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, System.Numerics.Complex value, MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.Write(value.Real);
            writer.Write(value.Imaginary);
            return;
        }

        public System.Numerics.Complex Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var count = reader.ReadArrayHeader();

            if (count != 2)
            {
                throw new MessagePackSerializationException("Invalid Complex format.");
            }

            var real = reader.ReadDouble();

            var imaginary = reader.ReadDouble();

            return new System.Numerics.Complex(real, imaginary);
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Complex value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Complex instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(Complex before, Complex after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class LazyFormatter<T> : INetworkVarMessagePackFormatter<Lazy<T>>
    {
        private int _fieldCount;
        public int FieldCount => _fieldCount;

        public void Serialize(ref MessagePackWriter writer, Lazy<T> value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                IFormatterResolver resolver = options.Resolver;
                resolver.GetFormatterWithVerify<T>().Serialize(ref writer, value.Value, options);
            }
        }

        public Lazy<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }
            else
            {
                options.Security.DepthStep(ref reader);
                try
                {
                    // deserialize immediately(no delay, because capture byte[] causes memory leak)
                    IFormatterResolver resolver = options.Resolver;
                    T v = resolver.GetFormatterWithVerify<T>().Deserialize(ref reader, options);
                    return new Lazy<T>(() => v);
                }
                finally
                {
                    reader.Depth--;
                }
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Lazy<T> value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
        
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

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Lazy<T> instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
        

            if (reader.TryReadNil())
            {
                instance = null;
            }
            else
            {
                var formatter = resolver.GetFormatterWithVerify<T>() as INetworkVarMessagePackFormatter<T>;
                var temp = default(T);
                formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
                instance = new Lazy<T>(() => temp);
            }
        }

        public ulong GetDirtyMask(Lazy<T> before, Lazy<T> after, MessagePackSerializerOptions options)
        {
            if (before == after || (before.IsValueCreated && after.IsValueCreated))
                return 0UL;

            if ((before.IsValueCreated && !after.IsValueCreated)
            || (!before.IsValueCreated && after.IsValueCreated))
                return 1UL;

            return before.Value.Equals(after.Value) ? 0UL : 1UL;
        }
    }
}
