// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MessagePack.Formatters
{
    public class NilFormatter : INetworkVarMessagePackFormatter<Nil>
    {
        public static readonly IMessagePackFormatter<Nil> Instance = new NilFormatter();

        public int FieldCount => 1;

        private NilFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Nil value, MessagePackSerializerOptions options)
        {
            writer.WriteNil();
        }

        public Nil Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadNil();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Nil value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.WriteNil();
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Nil instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = reader.ReadNil();
            }
        }

        public ulong GetDirtyMask(Nil before, Nil after, MessagePackSerializerOptions options)
        {
            return 0UL;
        }
    }

    // NullableNil is same as Nil.
    public class NullableNilFormatter : INetworkVarMessagePackFormatter<Nil?>
    {
        public static readonly IMessagePackFormatter<Nil?> Instance = new NullableNilFormatter();

        public int FieldCount => 1;

        private NullableNilFormatter()
        {
        }

        public void Serialize(ref MessagePackWriter writer, Nil? value, MessagePackSerializerOptions options)
        {
            writer.WriteNil();
        }

        public Nil? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return reader.ReadNil();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, Nil? value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.WriteNil();
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref Nil? instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = reader.ReadNil();
            }
        }

        public ulong GetDirtyMask(Nil? before, Nil? after, MessagePackSerializerOptions options)
        {
            return before.Equals(after) ? 0UL : 1UL;
        }
    }
}
