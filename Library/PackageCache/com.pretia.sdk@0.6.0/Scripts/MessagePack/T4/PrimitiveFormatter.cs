// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/* THIS (.cs) FILE IS GENERATED. DO NOT CHANGE IT.
 * CHANGE THE .tt FILE INSTEAD. */

using System;
using System.Buffers;

#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters
{
    public sealed class Int16Formatter : INetworkVarMessagePackFormatter<Int16>
    {
        public static readonly Int16Formatter Instance = new Int16Formatter();

        public int FieldCount => 1;

        private Int16Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int16 value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Int16 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadInt16();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, short value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref short instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(short before, short after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableInt16Formatter : INetworkVarMessagePackFormatter<Int16?>
    {
        public static readonly NullableInt16Formatter Instance = new NullableInt16Formatter();

        public int FieldCount => 1;

        private NullableInt16Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int16? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Int16? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadInt16();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, short? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref short? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(short? before, short? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class Int16ArrayFormatter : INetworkVarMessagePackFormatter<Int16[]>
    {
        public static readonly Int16ArrayFormatter Instance = new Int16ArrayFormatter();

        public int FieldCount => 1;

        private Int16ArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int16[] value, MessagePackSerializerOptions options)
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

        public Int16[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Int16[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadInt16();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, short[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref short[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(short[] before, short[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class Int32Formatter : INetworkVarMessagePackFormatter<Int32>
    {
        public static readonly Int32Formatter Instance = new Int32Formatter();

        public int FieldCount => 1;

        private Int32Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int32 value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Int32 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadInt32();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, int value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref int instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(int before, int after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableInt32Formatter : INetworkVarMessagePackFormatter<Int32?>
    {
        public static readonly NullableInt32Formatter Instance = new NullableInt32Formatter();

        public int FieldCount => 1;

        private NullableInt32Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int32? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Int32? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadInt32();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, int? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref int? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(int? before, int? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class Int32ArrayFormatter : INetworkVarMessagePackFormatter<Int32[]>
    {
        public static readonly Int32ArrayFormatter Instance = new Int32ArrayFormatter();

        public int FieldCount => 1;

        private Int32ArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int32[] value, MessagePackSerializerOptions options)
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

        public Int32[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Int32[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadInt32();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, int[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref int[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(int[] before, int[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class Int64Formatter : INetworkVarMessagePackFormatter<Int64>
    {
        public static readonly Int64Formatter Instance = new Int64Formatter();

        public int FieldCount => 1;

        private Int64Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int64 value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Int64 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadInt64();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, long value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref long instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(long before, long after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableInt64Formatter : INetworkVarMessagePackFormatter<Int64?>
    {
        public static readonly NullableInt64Formatter Instance = new NullableInt64Formatter();

        public int FieldCount => 1;

        private NullableInt64Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int64? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Int64? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadInt64();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, long? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref long? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(long? before, long? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class Int64ArrayFormatter : INetworkVarMessagePackFormatter<Int64[]>
    {
        public static readonly Int64ArrayFormatter Instance = new Int64ArrayFormatter();

        public int FieldCount => 1;

        private Int64ArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Int64[] value, MessagePackSerializerOptions options)
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

        public Int64[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Int64[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadInt64();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, long[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref long[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(long[] before, long[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UInt16Formatter : INetworkVarMessagePackFormatter<UInt16>
    {
        public static readonly UInt16Formatter Instance = new UInt16Formatter();

        public int FieldCount => 1;

        private UInt16Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt16 value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public UInt16 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadUInt16();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, ushort value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref ushort instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(ushort before, ushort after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableUInt16Formatter : INetworkVarMessagePackFormatter<UInt16?>
    {
        public static readonly NullableUInt16Formatter Instance = new NullableUInt16Formatter();

        public int FieldCount => 1;

        private NullableUInt16Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt16? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public UInt16? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadUInt16();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, ushort? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref ushort? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(ushort? before, ushort? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UInt16ArrayFormatter : INetworkVarMessagePackFormatter<UInt16[]>
    {
        public static readonly UInt16ArrayFormatter Instance = new UInt16ArrayFormatter();

        public int FieldCount => 1;

        private UInt16ArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt16[] value, MessagePackSerializerOptions options)
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

        public UInt16[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new UInt16[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadUInt16();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, ushort[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref ushort[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(ushort[] before, ushort[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UInt32Formatter : INetworkVarMessagePackFormatter<UInt32>
    {
        public static readonly UInt32Formatter Instance = new UInt32Formatter();

        public int FieldCount => 1;

        private UInt32Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt32 value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public UInt32 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadUInt32();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, uint value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref uint instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(uint before, uint after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableUInt32Formatter : INetworkVarMessagePackFormatter<UInt32?>
    {
        public static readonly NullableUInt32Formatter Instance = new NullableUInt32Formatter();

        public int FieldCount => 1;

        private NullableUInt32Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt32? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public UInt32? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadUInt32();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, uint? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref uint? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(uint? before, uint? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UInt32ArrayFormatter : INetworkVarMessagePackFormatter<UInt32[]>
    {
        public static readonly UInt32ArrayFormatter Instance = new UInt32ArrayFormatter();

        public int FieldCount => 1;

        private UInt32ArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt32[] value, MessagePackSerializerOptions options)
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

        public UInt32[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new UInt32[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadUInt32();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, uint[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref uint[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(uint[] before, uint[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UInt64Formatter : INetworkVarMessagePackFormatter<UInt64>
    {
        public static readonly UInt64Formatter Instance = new UInt64Formatter();

        public int FieldCount => 1;

        private UInt64Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt64 value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public UInt64 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadUInt64();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, ulong value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref ulong instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(ulong before, ulong after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableUInt64Formatter : INetworkVarMessagePackFormatter<UInt64?>
    {
        public static readonly NullableUInt64Formatter Instance = new NullableUInt64Formatter();

        public int FieldCount => 1;

        private NullableUInt64Formatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt64? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public UInt64? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadUInt64();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, ulong? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref ulong? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(ulong? before, ulong? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class UInt64ArrayFormatter : INetworkVarMessagePackFormatter<UInt64[]>
    {
        public static readonly UInt64ArrayFormatter Instance = new UInt64ArrayFormatter();

        public int FieldCount => 1;

        private UInt64ArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, UInt64[] value, MessagePackSerializerOptions options)
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

        public UInt64[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new UInt64[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadUInt64();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, ulong[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref ulong[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(ulong[] before, ulong[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class SingleFormatter : INetworkVarMessagePackFormatter<Single>
    {
        public static readonly SingleFormatter Instance = new SingleFormatter();

        public int FieldCount => 1;

        private SingleFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Single value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Single Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadSingle();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, float value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref float instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(float before, float after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableSingleFormatter : INetworkVarMessagePackFormatter<Single?>
    {
        public static readonly NullableSingleFormatter Instance = new NullableSingleFormatter();

        public int FieldCount => 1;

        private NullableSingleFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Single? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Single? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadSingle();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, float? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref float? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(float? before, float? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class SingleArrayFormatter : INetworkVarMessagePackFormatter<Single[]>
    {
        public static readonly SingleArrayFormatter Instance = new SingleArrayFormatter();

        public int FieldCount => 1;

        private SingleArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Single[] value, MessagePackSerializerOptions options)
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

        public Single[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Single[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadSingle();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, float[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref float[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(float[] before, float[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class DoubleFormatter : INetworkVarMessagePackFormatter<Double>
    {
        public static readonly DoubleFormatter Instance = new DoubleFormatter();

        public int FieldCount => 1;

        private DoubleFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Double value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Double Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadDouble();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, double value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref double instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(double before, double after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableDoubleFormatter : INetworkVarMessagePackFormatter<Double?>
    {
        public static readonly NullableDoubleFormatter Instance = new NullableDoubleFormatter();

        public int FieldCount => 1;

        private NullableDoubleFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Double? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Double? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadDouble();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, double? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref double? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(double? before, double? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class DoubleArrayFormatter : INetworkVarMessagePackFormatter<Double[]>
    {
        public static readonly DoubleArrayFormatter Instance = new DoubleArrayFormatter();

        public int FieldCount => 1;

        private DoubleArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Double[] value, MessagePackSerializerOptions options)
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

        public Double[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Double[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadDouble();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, double[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref double[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(double[] before, double[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class BooleanFormatter : INetworkVarMessagePackFormatter<Boolean>
    {
        public static readonly BooleanFormatter Instance = new BooleanFormatter();

        public int FieldCount => 1;

        private BooleanFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Boolean value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Boolean Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadBoolean();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, bool value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref bool instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(bool before, bool after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableBooleanFormatter : INetworkVarMessagePackFormatter<Boolean?>
    {
        public static readonly NullableBooleanFormatter Instance = new NullableBooleanFormatter();

        public int FieldCount => 1;

        private NullableBooleanFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Boolean? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Boolean? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadBoolean();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, bool? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref bool? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(bool? before, bool? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class BooleanArrayFormatter : INetworkVarMessagePackFormatter<Boolean[]>
    {
        public static readonly BooleanArrayFormatter Instance = new BooleanArrayFormatter();

        public int FieldCount => 1;

        private BooleanArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Boolean[] value, MessagePackSerializerOptions options)
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

        public Boolean[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Boolean[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadBoolean();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, bool[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref bool[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(bool[] before, bool[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class ByteFormatter : INetworkVarMessagePackFormatter<Byte>
    {
        public static readonly ByteFormatter Instance = new ByteFormatter();

        public int FieldCount => 1;

        private ByteFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Byte value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Byte Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadByte();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, byte value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref byte instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(byte before, byte after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableByteFormatter : INetworkVarMessagePackFormatter<Byte?>
    {
        public static readonly NullableByteFormatter Instance = new NullableByteFormatter();

        public int FieldCount => 1;

        private NullableByteFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Byte? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Byte? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadByte();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, byte? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref byte? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(byte? before, byte? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class SByteFormatter : INetworkVarMessagePackFormatter<SByte>
    {
        public static readonly SByteFormatter Instance = new SByteFormatter();

        public int FieldCount => 1;

        private SByteFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, SByte value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public SByte Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadSByte();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, sbyte value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref sbyte instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(sbyte before, sbyte after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableSByteFormatter : INetworkVarMessagePackFormatter<SByte?>
    {
        public static readonly NullableSByteFormatter Instance = new NullableSByteFormatter();

        public int FieldCount => 1;

        private NullableSByteFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, SByte? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public SByte? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadSByte();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, sbyte? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref sbyte? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(sbyte? before, sbyte? after, MessagePackSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class SByteArrayFormatter : INetworkVarMessagePackFormatter<SByte[]>
    {
        public static readonly SByteArrayFormatter Instance = new SByteArrayFormatter();

        public int FieldCount => 1;

        private SByteArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, SByte[] value, MessagePackSerializerOptions options)
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

        public SByte[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new SByte[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadSByte();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, sbyte[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref sbyte[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(sbyte[] before, sbyte[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class CharFormatter : INetworkVarMessagePackFormatter<Char>
    {
        public static readonly CharFormatter Instance = new CharFormatter();

        public int FieldCount => 1;

        private CharFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Char value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public Char Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadChar();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, char value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref char instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(char before, char after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableCharFormatter : INetworkVarMessagePackFormatter<Char?>
    {
        public static readonly NullableCharFormatter Instance = new NullableCharFormatter();

        public int FieldCount => 1;

        private NullableCharFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Char? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public Char? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadChar();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, char? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref char? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(char? before, char? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class CharArrayFormatter : INetworkVarMessagePackFormatter<Char[]>
    {
        public static readonly CharArrayFormatter Instance = new CharArrayFormatter();

        public int FieldCount => 1;

        private CharArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Char[] value, MessagePackSerializerOptions options)
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

        public Char[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new Char[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadChar();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, char[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref char[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(char[] before, char[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class DateTimeFormatter : INetworkVarMessagePackFormatter<DateTime>
    {
        public static readonly DateTimeFormatter Instance = new DateTimeFormatter();

        public int FieldCount => 1;

        private DateTimeFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options)
        {
            writer.Write(value);
        }

        public DateTime Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadDateTime();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref DateTime instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(DateTime before, DateTime after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class NullableDateTimeFormatter : INetworkVarMessagePackFormatter<DateTime?>
    {
        public static readonly NullableDateTimeFormatter Instance = new NullableDateTimeFormatter();

        public int FieldCount => 1;

        private NullableDateTimeFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, DateTime? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Value);
            }
        }

        public DateTime? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                return reader.ReadDateTime();
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, DateTime? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref DateTime? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(DateTime? before, DateTime? after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class DateTimeArrayFormatter : INetworkVarMessagePackFormatter<DateTime[]>
    {
        public static readonly DateTimeArrayFormatter Instance = new DateTimeArrayFormatter();

        public int FieldCount => 1;

        private DateTimeArrayFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, DateTime[] value, MessagePackSerializerOptions options)
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

        public DateTime[] Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var len = reader.ReadArrayHeader();
                var array = new DateTime[len];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = reader.ReadDateTime();
                }

                return array;
            }
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, DateTime[] value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                Serialize(ref writer, value, options);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref DateTime[] instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = Deserialize(ref reader, options);
            }
        }

        public ulong GetDirtyMask(DateTime[] before, DateTime[] after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }
}
