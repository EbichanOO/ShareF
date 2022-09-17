// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using MessagePack;
using MessagePack.Formatters;

#pragma warning disable SA1312 // variable naming
#pragma warning disable SA1402 // one type per file
#pragma warning disable SA1649 // file name matches type name

namespace MessagePack.Unity
{
    public sealed class Vector2Formatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Vector2>
    {
        public int FieldCount => 2;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Vector2 value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public global::UnityEngine.Vector2 Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var x = default(float);
            var y = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = reader.ReadSingle();
                        break;
                    case 1:
                        y = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Vector2(x, y);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Vector2 value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Vector2 instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Vector2 before, UnityEngine.Vector2 after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            return dirtyMask;
        }
    }

    public sealed class Vector3Formatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Vector3>
    {
        public int FieldCount => 3;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Vector3 value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(3);
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public global::UnityEngine.Vector3 Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var x = default(float);
            var y = default(float);
            var z = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = reader.ReadSingle();
                        break;
                    case 1:
                        y = reader.ReadSingle();
                        break;
                    case 2:
                        z = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Vector3(x, y, z);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Vector3 value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.z);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Vector3 instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadSingle();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.z = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Vector3 before, UnityEngine.Vector3 after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.z != after.z)
            {
                dirtyMask |= 1 << 2;
            }

            return dirtyMask;
        }
    }

    public sealed class Vector4Formatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Vector4>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Vector4 value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public global::UnityEngine.Vector4 Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var x = default(float);
            var y = default(float);
            var z = default(float);
            var w = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = reader.ReadSingle();
                        break;
                    case 1:
                        y = reader.ReadSingle();
                        break;
                    case 2:
                        z = reader.ReadSingle();
                        break;
                    case 3:
                        w = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Vector4(x, y, z, w);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Vector4 value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.z);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.w);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Vector4 instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadSingle();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.z = reader.ReadSingle();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.w = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Vector4 before, UnityEngine.Vector4 after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.z != after.z)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.w != after.w)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class QuaternionFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Quaternion>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Quaternion value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public global::UnityEngine.Quaternion Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var x = default(float);
            var y = default(float);
            var z = default(float);
            var w = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = reader.ReadSingle();
                        break;
                    case 1:
                        y = reader.ReadSingle();
                        break;
                    case 2:
                        z = reader.ReadSingle();
                        break;
                    case 3:
                        w = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Quaternion(x, y, z, w);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Quaternion value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.z);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.w);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Quaternion instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadSingle();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.z = reader.ReadSingle();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.w = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Quaternion before, UnityEngine.Quaternion after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.z != after.z)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.w != after.w)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class ColorFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Color>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Color value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public global::UnityEngine.Color Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var r = default(float);
            var g = default(float);
            var b = default(float);
            var a = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        r = reader.ReadSingle();
                        break;
                    case 1:
                        g = reader.ReadSingle();
                        break;
                    case 2:
                        b = reader.ReadSingle();
                        break;
                    case 3:
                        a = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Color(r, g, b, a);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Color value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.r);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.g);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.b);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.a);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Color instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.r = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.g = reader.ReadSingle();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.b = reader.ReadSingle();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.a = reader.ReadSingle();
            }

        }

        public ulong GetDirtyMask(UnityEngine.Color before, UnityEngine.Color after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            if (before.r != after.r)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.g != after.g)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.b != after.b)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.a != after.a)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class BoundsFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Bounds>
    {
        public int FieldCount => 6;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Bounds value, global::MessagePack.MessagePackSerializerOptions options)
        {
            IFormatterResolver resolver = options.Resolver;
            writer.WriteArrayHeader(2);
            resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Serialize(ref writer, value.center, options);
            resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Serialize(ref writer, value.size, options);
        }

        public global::UnityEngine.Bounds Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            IFormatterResolver resolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var center = default(global::UnityEngine.Vector3);
            var size = default(global::UnityEngine.Vector3);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        center = resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        size = resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Bounds(center, size);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Bounds value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            var v3Formatter = resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>() as INetworkVarMessagePackFormatter<global::UnityEngine.Vector3>;

            v3Formatter.SerializeNetworkVar(ref writer, value.center, options, dirtyMask);
            v3Formatter.SerializeNetworkVar(ref writer, value.size, options, dirtyMask >> v3Formatter.FieldCount);
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Bounds instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            var v3Formatter = resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>() as INetworkVarMessagePackFormatter<global::UnityEngine.Vector3>;

            global::UnityEngine.Vector3 temp = instance.center;
            v3Formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
            instance.center = temp;

            temp = instance.size;
            v3Formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask >> v3Formatter.FieldCount);
            instance.size = temp;
        }

        public ulong GetDirtyMask(UnityEngine.Bounds before, UnityEngine.Bounds after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            IFormatterResolver resolver = options.Resolver;
            var v3Formatter = resolver.GetFormatterWithVerify<global::UnityEngine.Vector3>() as INetworkVarMessagePackFormatter<global::UnityEngine.Vector3>;

            dirtyMask |= v3Formatter.GetDirtyMask(before.center, after.center, options);
            dirtyMask |= v3Formatter.GetDirtyMask(before.size, after.size, options) << v3Formatter.FieldCount;

            return dirtyMask;
        }
    }

    public sealed class RectFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Rect>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Rect value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.width);
            writer.Write(value.height);
        }

        public global::UnityEngine.Rect Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var x = default(float);
            var y = default(float);
            var width = default(float);
            var height = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        x = reader.ReadSingle();
                        break;
                    case 1:
                        y = reader.ReadSingle();
                        break;
                    case 2:
                        width = reader.ReadSingle();
                        break;
                    case 3:
                        height = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var result = new global::UnityEngine.Rect(x, y, width, height);
            return result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Rect value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.width);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.height);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Rect instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadSingle();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.width = reader.ReadSingle();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.height = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Rect before, UnityEngine.Rect after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.width != after.width)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.height != after.height)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    // additional
    public sealed class WrapModeFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.WrapMode>
    {
        public int FieldCount => 1;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.WrapMode value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((int)value);
        }

        public global::UnityEngine.WrapMode Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::UnityEngine.WrapMode)reader.ReadInt32();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.WrapMode value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write((int)value);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.WrapMode instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = (UnityEngine.WrapMode)reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.WrapMode before, UnityEngine.WrapMode after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class GradientModeFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.GradientMode>
    {
        public int FieldCount => 1;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.GradientMode value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((int)value);
        }

        public global::UnityEngine.GradientMode Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::UnityEngine.GradientMode)reader.ReadInt32();
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.GradientMode value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write((int)value);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.GradientMode instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance = (UnityEngine.GradientMode)reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.GradientMode before, UnityEngine.GradientMode after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }

    public sealed class KeyframeFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Keyframe>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Keyframe value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.time);
            writer.Write(value.value);
            writer.Write(value.inTangent);
            writer.Write(value.outTangent);
        }

        public global::UnityEngine.Keyframe Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __time__ = default(float);
            var __value__ = default(float);
            var __inTangent__ = default(float);
            var __outTangent__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __time__ = reader.ReadSingle();
                        break;
                    case 1:
                        __value__ = reader.ReadSingle();
                        break;
                    case 2:
                        __inTangent__ = reader.ReadSingle();
                        break;
                    case 3:
                        __outTangent__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.Keyframe(__time__, __value__, __inTangent__, __outTangent__);
            ____result.time = __time__;
            ____result.value = __value__;
            ____result.inTangent = __inTangent__;
            ____result.outTangent = __outTangent__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Keyframe value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.time);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.value);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.inTangent);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.outTangent);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Keyframe instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.time = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.value = reader.ReadSingle();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.inTangent = reader.ReadSingle();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.outTangent = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Keyframe before, UnityEngine.Keyframe after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.time != after.time)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.value != after.value)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.inTangent != after.inTangent)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.outTangent != after.outTangent)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class AnimationCurveFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.AnimationCurve>
    {
        public int FieldCount => 3;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.AnimationCurve value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver resolver = options.Resolver;
            writer.WriteArrayHeader(3);
            resolver.GetFormatterWithVerify<global::UnityEngine.Keyframe[]>().Serialize(ref writer, value.keys, options);
            resolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Serialize(ref writer, value.postWrapMode, options);
            resolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Serialize(ref writer, value.preWrapMode, options);
        }

        public global::UnityEngine.AnimationCurve Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                return null;
            }

            IFormatterResolver resolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __keys__ = default(global::UnityEngine.Keyframe[]);
            var __postWrapMode__ = default(global::UnityEngine.WrapMode);
            var __preWrapMode__ = default(global::UnityEngine.WrapMode);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __keys__ = resolver.GetFormatterWithVerify<global::UnityEngine.Keyframe[]>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __postWrapMode__ = resolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __preWrapMode__ = resolver.GetFormatterWithVerify<global::UnityEngine.WrapMode>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.AnimationCurve();
            ____result.keys = __keys__;
            ____result.postWrapMode = __postWrapMode__;
            ____result.preWrapMode = __preWrapMode__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.AnimationCurve value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            if ((dirtyMask & 1UL) != 0)
            {
                var arrayFormatter = resolver.GetFormatterWithVerify<UnityEngine.Keyframe[]>() as INetworkVarMessagePackFormatter<UnityEngine.Keyframe[]>;
                arrayFormatter.SerializeNetworkVar(ref writer, value.keys, options, dirtyMask);
                fieldCounter += arrayFormatter.FieldCount;
            }

            if ((dirtyMask & 2UL) != 0)
            {
                var wrapModeFormatter = resolver.GetFormatterWithVerify<UnityEngine.WrapMode>() as INetworkVarMessagePackFormatter<UnityEngine.WrapMode>;
                wrapModeFormatter.SerializeNetworkVar(ref writer, value.postWrapMode, options, dirtyMask >> fieldCounter);
                fieldCounter += wrapModeFormatter.FieldCount;
            }

            if ((dirtyMask & 4UL) != 0)
            {
                var wrapModeFormatter = resolver.GetFormatterWithVerify<UnityEngine.WrapMode>() as INetworkVarMessagePackFormatter<UnityEngine.WrapMode>;
                wrapModeFormatter.SerializeNetworkVar(ref writer, value.preWrapMode, options, dirtyMask >> fieldCounter);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.AnimationCurve instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            if ((dirtyMask & 1UL) != 0)
            {
                var arrayFormatter = resolver.GetFormatterWithVerify<UnityEngine.Keyframe[]>() as INetworkVarMessagePackFormatter<UnityEngine.Keyframe[]>;
                var temp = instance.keys; 
                arrayFormatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
                instance.keys = temp;
                fieldCounter += arrayFormatter.FieldCount;
            }

            if ((dirtyMask & 2UL) != 0)
            {
                var wrapModeFormatter = resolver.GetFormatterWithVerify<UnityEngine.WrapMode>() as INetworkVarMessagePackFormatter<UnityEngine.WrapMode>;
                var temp = instance.postWrapMode;
                wrapModeFormatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask >> fieldCounter);
                fieldCounter += wrapModeFormatter.FieldCount;
            }

            if ((dirtyMask & 4UL) != 0)
            {
                var wrapModeFormatter = resolver.GetFormatterWithVerify<UnityEngine.WrapMode>() as INetworkVarMessagePackFormatter<UnityEngine.WrapMode>;
                var temp = instance.preWrapMode;
                wrapModeFormatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask >> fieldCounter);
                instance.preWrapMode = temp;
            }
        }

        public ulong GetDirtyMask(UnityEngine.AnimationCurve before, UnityEngine.AnimationCurve after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.keys != after.keys)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.postWrapMode != after.postWrapMode)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.preWrapMode != after.preWrapMode)
            {
                dirtyMask |= 1 << 2;
            }

            return dirtyMask;
        }
    }

    public sealed class Matrix4x4Formatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Matrix4x4>
    {
        public int FieldCount => 16;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Matrix4x4 value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(16);
            writer.Write(value.m00);
            writer.Write(value.m10);
            writer.Write(value.m20);
            writer.Write(value.m30);
            writer.Write(value.m01);
            writer.Write(value.m11);
            writer.Write(value.m21);
            writer.Write(value.m31);
            writer.Write(value.m02);
            writer.Write(value.m12);
            writer.Write(value.m22);
            writer.Write(value.m32);
            writer.Write(value.m03);
            writer.Write(value.m13);
            writer.Write(value.m23);
            writer.Write(value.m33);
        }

        public global::UnityEngine.Matrix4x4 Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __m00__ = default(float);
            var __m10__ = default(float);
            var __m20__ = default(float);
            var __m30__ = default(float);
            var __m01__ = default(float);
            var __m11__ = default(float);
            var __m21__ = default(float);
            var __m31__ = default(float);
            var __m02__ = default(float);
            var __m12__ = default(float);
            var __m22__ = default(float);
            var __m32__ = default(float);
            var __m03__ = default(float);
            var __m13__ = default(float);
            var __m23__ = default(float);
            var __m33__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __m00__ = reader.ReadSingle();
                        break;
                    case 1:
                        __m10__ = reader.ReadSingle();
                        break;
                    case 2:
                        __m20__ = reader.ReadSingle();
                        break;
                    case 3:
                        __m30__ = reader.ReadSingle();
                        break;
                    case 4:
                        __m01__ = reader.ReadSingle();
                        break;
                    case 5:
                        __m11__ = reader.ReadSingle();
                        break;
                    case 6:
                        __m21__ = reader.ReadSingle();
                        break;
                    case 7:
                        __m31__ = reader.ReadSingle();
                        break;
                    case 8:
                        __m02__ = reader.ReadSingle();
                        break;
                    case 9:
                        __m12__ = reader.ReadSingle();
                        break;
                    case 10:
                        __m22__ = reader.ReadSingle();
                        break;
                    case 11:
                        __m32__ = reader.ReadSingle();
                        break;
                    case 12:
                        __m03__ = reader.ReadSingle();
                        break;
                    case 13:
                        __m13__ = reader.ReadSingle();
                        break;
                    case 14:
                        __m23__ = reader.ReadSingle();
                        break;
                    case 15:
                        __m33__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = default(global::UnityEngine.Matrix4x4);
            ____result.m00 = __m00__;
            ____result.m10 = __m10__;
            ____result.m20 = __m20__;
            ____result.m30 = __m30__;
            ____result.m01 = __m01__;
            ____result.m11 = __m11__;
            ____result.m21 = __m21__;
            ____result.m31 = __m31__;
            ____result.m02 = __m02__;
            ____result.m12 = __m12__;
            ____result.m22 = __m22__;
            ____result.m32 = __m32__;
            ____result.m03 = __m03__;
            ____result.m13 = __m13__;
            ____result.m23 = __m23__;
            ____result.m33 = __m33__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Matrix4x4 value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & (1 << 0)) != 0)
            {
                writer.Write(value.m00);
            }

            if ((dirtyMask & (1 << 1)) != 0)
            {
                writer.Write(value.m10);
            }

            if ((dirtyMask & (1 << 2)) != 0)
            {
                writer.Write(value.m20);
            }

            if ((dirtyMask & (1 << 3)) != 0)
            {
                writer.Write(value.m30);
            }

            if ((dirtyMask & (1 << 4)) != 0)
            {
                writer.Write(value.m01);
            }

            if ((dirtyMask & (1 << 5)) != 0)
            {
                writer.Write(value.m11);
            }

            if ((dirtyMask & (1 << 6)) != 0)
            {
                writer.Write(value.m21);
            }

            if ((dirtyMask & (1 << 7)) != 0)
            {
                writer.Write(value.m31);
            }

            if ((dirtyMask & (1 << 8)) != 0)
            {
                writer.Write(value.m02);
            }

            if ((dirtyMask & (1 << 9)) != 0)
            {
                writer.Write(value.m12);
            }

            if ((dirtyMask & (1 << 10)) != 0)
            {
                writer.Write(value.m22);
            }

            if ((dirtyMask & (1 << 11)) != 0)
            {
                writer.Write(value.m32);
            }

            if ((dirtyMask & (1 << 12)) != 0)
            {
                writer.Write(value.m03);
            }

            if ((dirtyMask & (1 << 13)) != 0)
            {
                writer.Write(value.m13);
            }

            if ((dirtyMask & (1 << 14)) != 0)
            {
                writer.Write(value.m23);
            }

            if ((dirtyMask & (1 << 15)) != 0)
            {
                writer.Write(value.m33);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Matrix4x4 instance, ulong dirtyMask)
        {
            if ((dirtyMask & (1 << 0)) != 0)
            {
                instance.m00 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 1)) != 0)
            {
                instance.m10 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 2)) != 0)
            {
                instance.m20 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 3)) != 0)
            {
                instance.m30 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 4)) != 0)
            {
                instance.m01 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 5)) != 0)
            {
                instance.m11 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 6)) != 0)
            {
                instance.m21 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 7)) != 0)
            {
                instance.m31 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 8)) != 0)
            {
                instance.m02 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 9)) != 0)
            {
                instance.m12 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 10)) != 0)
            {
                instance.m22 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 11)) != 0)
            {
                instance.m32 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 12)) != 0)
            {
                instance.m03 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 13)) != 0)
            {
                instance.m13 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 14)) != 0)
            {
                instance.m23 = reader.ReadSingle();
            }

            if ((dirtyMask & (1 << 15)) != 0)
            {
                instance.m33 = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Matrix4x4 before, UnityEngine.Matrix4x4 after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.m00 != after.m00)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.m10 != after.m10)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.m20 != after.m20)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.m30 != after.m30)
            {
                dirtyMask |= 1 << 3;
            }

            if (before.m01 != after.m01)
            {
                dirtyMask |= 1 << 4;
            }

            if (before.m11 != after.m11)
            {
                dirtyMask |= 1 << 5;
            }

            if (before.m21 != after.m21)
            {
                dirtyMask |= 1 << 6;
            }

            if (before.m31 != after.m31)
            {
                dirtyMask |= 1 << 7;
            }

            if (before.m02 != after.m02)
            {
                dirtyMask |= 1 << 8;
            }

            if (before.m12 != after.m12)
            {
                dirtyMask |= 1 << 9;
            }

            if (before.m22 != after.m22)
            {
                dirtyMask |= 1 << 10;
            }

            if (before.m32 != after.m32)
            {
                dirtyMask |= 1 << 11;
            }

            if (before.m03 != after.m03)
            {
                dirtyMask |= 1 << 12;
            }

            if (before.m13 != after.m13)
            {
                dirtyMask |= 1 << 13;
            }

            if (before.m23 != after.m23)
            {
                dirtyMask |= 1 << 14;
            }

            if (before.m33 != after.m33)
            {
                dirtyMask |= 1 << 15;
            }

            return dirtyMask;
        }
    }

    public sealed class GradientColorKeyFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.GradientColorKey>
    {
        public int FieldCount => 5;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.GradientColorKey value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            options.Resolver.GetFormatterWithVerify<global::UnityEngine.Color>().Serialize(ref writer, value.color, options);
            writer.Write(value.time);
        }

        public global::UnityEngine.GradientColorKey Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __color__ = default(global::UnityEngine.Color);
            var __time__ = default(float);
            IFormatterResolver resolver = options.Resolver;
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __color__ = resolver.GetFormatterWithVerify<global::UnityEngine.Color>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __time__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.GradientColorKey(__color__, __time__);
            ____result.color = __color__;
            ____result.time = __time__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.GradientColorKey value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            
            var colorFormatter = resolver.GetFormatterWithVerify<UnityEngine.Color>() as INetworkVarMessagePackFormatter<UnityEngine.Color>;
            colorFormatter.SerializeNetworkVar(ref writer, value.color, options, dirtyMask);

            if ((dirtyMask & 16L) != 0)
            {
                writer.Write(value.time);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.GradientColorKey instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            
            var colorFormatter = resolver.GetFormatterWithVerify<UnityEngine.Color>() as INetworkVarMessagePackFormatter<UnityEngine.Color>;
            var temp = instance.color;
            colorFormatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
            instance.color = temp;

            if ((dirtyMask & 16L) != 0)
            {
                instance.time = reader.ReadSingle();
            }
        }

        public ulong GetDirtyMask(UnityEngine.GradientColorKey before, UnityEngine.GradientColorKey after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            IFormatterResolver resolver = options.Resolver;

            var colorFormatter = resolver.GetFormatterWithVerify<UnityEngine.Color>() as INetworkVarMessagePackFormatter<UnityEngine.Color>;
            dirtyMask |= colorFormatter.GetDirtyMask(before.color, after.color, options);

            if (before.time == after.time)
            {
                dirtyMask |= 1UL << colorFormatter.FieldCount;
            }

            return dirtyMask;
        }
    }

    public sealed class GradientAlphaKeyFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.GradientAlphaKey>
    {
        public int FieldCount => 2;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.GradientAlphaKey value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.Write(value.alpha);
            writer.Write(value.time);
        }

        public global::UnityEngine.GradientAlphaKey Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __alpha__ = default(float);
            var __time__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __alpha__ = reader.ReadSingle();
                        break;
                    case 1:
                        __time__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.GradientAlphaKey(__alpha__, __time__);
            ____result.alpha = __alpha__;
            ____result.time = __time__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.GradientAlphaKey value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.alpha);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.time);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.GradientAlphaKey instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.alpha = reader.ReadSingle();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.time = reader.ReadSingle();
            }
        }

        ulong INetworkVarMessagePackFormatter<UnityEngine.GradientAlphaKey>.GetDirtyMask(UnityEngine.GradientAlphaKey before, UnityEngine.GradientAlphaKey after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.alpha != after.alpha)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.time != after.time)
            {
                dirtyMask |= 1 << 1;
            }

            return dirtyMask;
        }
    }

    public sealed class GradientFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Gradient>
    {
        public int FieldCount => 3;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Gradient value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver resolver = options.Resolver;
            writer.WriteArrayHeader(3);
            resolver.GetFormatterWithVerify<global::UnityEngine.GradientColorKey[]>().Serialize(ref writer, value.colorKeys, options);
            resolver.GetFormatterWithVerify<global::UnityEngine.GradientAlphaKey[]>().Serialize(ref writer, value.alphaKeys, options);
            resolver.GetFormatterWithVerify<global::UnityEngine.GradientMode>().Serialize(ref writer, value.mode, options);
        }

        public global::UnityEngine.Gradient Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                return null;
            }

            IFormatterResolver resolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __colorKeys__ = default(global::UnityEngine.GradientColorKey[]);
            var __alphaKeys__ = default(global::UnityEngine.GradientAlphaKey[]);
            var __mode__ = default(global::UnityEngine.GradientMode);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __colorKeys__ = resolver.GetFormatterWithVerify<global::UnityEngine.GradientColorKey[]>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __alphaKeys__ = resolver.GetFormatterWithVerify<global::UnityEngine.GradientAlphaKey[]>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __mode__ = resolver.GetFormatterWithVerify<global::UnityEngine.GradientMode>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.Gradient();
            ____result.colorKeys = __colorKeys__;
            ____result.alphaKeys = __alphaKeys__;
            ____result.mode = __mode__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Gradient value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientColorKey[]>() as INetworkVarMessagePackFormatter<UnityEngine.GradientColorKey[]>;
                formatter.SerializeNetworkVar(ref writer, value.colorKeys, options, dirtyMask);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientAlphaKey[]>() as INetworkVarMessagePackFormatter<UnityEngine.GradientAlphaKey[]>;
                formatter.SerializeNetworkVar(ref writer, value.alphaKeys, options, dirtyMask >> fieldCounter);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientMode>() as INetworkVarMessagePackFormatter<UnityEngine.GradientMode>;
                formatter.SerializeNetworkVar(ref writer, value.mode, options, dirtyMask >> fieldCounter);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Gradient instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientColorKey[]>() as INetworkVarMessagePackFormatter<UnityEngine.GradientColorKey[]>;
                var temp = instance.colorKeys;
                formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientAlphaKey[]>() as INetworkVarMessagePackFormatter<UnityEngine.GradientAlphaKey[]>;
                var temp = instance.alphaKeys;
                formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask >> fieldCounter);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientMode>() as INetworkVarMessagePackFormatter<UnityEngine.GradientMode>;
                var temp = instance.mode;
                formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask >> fieldCounter);
                instance.mode = temp;
            }
        }

        public ulong GetDirtyMask(UnityEngine.Gradient before, UnityEngine.Gradient after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientColorKey[]>() as INetworkVarMessagePackFormatter<UnityEngine.GradientColorKey[]>;
                dirtyMask |= formatter.GetDirtyMask(before.colorKeys, after.colorKeys, options);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientAlphaKey[]>() as INetworkVarMessagePackFormatter<UnityEngine.GradientAlphaKey[]>;
                dirtyMask |= formatter.GetDirtyMask(before.alphaKeys, after.alphaKeys, options) << fieldCounter;
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.GradientMode>() as INetworkVarMessagePackFormatter<UnityEngine.GradientMode>;
                dirtyMask |= formatter.GetDirtyMask(before.mode, after.mode, options) << fieldCounter;
                fieldCounter += formatter.FieldCount;
            }

            return dirtyMask;
        }
    }

    public sealed class Color32Formatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Color32>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Color32 value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }

        public global::UnityEngine.Color32 Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __r__ = default(byte);
            var __g__ = default(byte);
            var __b__ = default(byte);
            var __a__ = default(byte);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __r__ = reader.ReadByte();
                        break;
                    case 1:
                        __g__ = reader.ReadByte();
                        break;
                    case 2:
                        __b__ = reader.ReadByte();
                        break;
                    case 3:
                        __a__ = reader.ReadByte();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.Color32(__r__, __g__, __b__, __a__);
            ____result.r = __r__;
            ____result.g = __g__;
            ____result.b = __b__;
            ____result.a = __a__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Color32 value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.r);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.g);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.b);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.a);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Color32 instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.r = reader.ReadByte();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.g = reader.ReadByte();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.b = reader.ReadByte();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.a = reader.ReadByte();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Color32 before, UnityEngine.Color32 after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.r != after.r)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.g != after.g)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.b != after.b)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.a != after.a)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class RectOffsetFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.RectOffset>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.RectOffset value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(4);
            writer.Write(value.left);
            writer.Write(value.right);
            writer.Write(value.top);
            writer.Write(value.bottom);
        }

        public global::UnityEngine.RectOffset Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                return null;
            }

            var length = reader.ReadArrayHeader();
            var __left__ = default(int);
            var __right__ = default(int);
            var __top__ = default(int);
            var __bottom__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __left__ = reader.ReadInt32();
                        break;
                    case 1:
                        __right__ = reader.ReadInt32();
                        break;
                    case 2:
                        __top__ = reader.ReadInt32();
                        break;
                    case 3:
                        __bottom__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.RectOffset();
            ____result.left = __left__;
            ____result.right = __right__;
            ____result.top = __top__;
            ____result.bottom = __bottom__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.RectOffset value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.left);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.right);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.top);
            }

            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.bottom);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.RectOffset instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.left = reader.ReadInt32();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.right = reader.ReadInt32();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.top = reader.ReadInt32();
            }

            if ((dirtyMask & 8UL) != 0)
            {
                instance.bottom = reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.RectOffset before, UnityEngine.RectOffset after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.left != after.right)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.right != after.right)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.top != after.top)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.bottom != after.bottom)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class LayerMaskFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.LayerMask>
    {
        public int FieldCount => 1;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.LayerMask value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(1);
            writer.Write(value.value);
        }

        public global::UnityEngine.LayerMask Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __value__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __value__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = default(global::UnityEngine.LayerMask);
            ____result.value = __value__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.LayerMask value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.value);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.LayerMask instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.value = reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.LayerMask before, UnityEngine.LayerMask after, MessagePackSerializerOptions options)
        {
            return before == after ? 0UL : 1UL;
        }
    }
#if UNITY_2017_2_OR_NEWER
    public sealed class Vector2IntFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Vector2Int>
    {
        public int FieldCount => 2;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Vector2Int value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
        }
        public global::UnityEngine.Vector2Int Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __x__ = default(int);
            var __y__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __x__ = reader.ReadInt32();
                        break;
                    case 1:
                        __y__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.Vector2Int(__x__, __y__);
            ____result.x = __x__;
            ____result.y = __y__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Vector2Int value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Vector2Int instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadInt32();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.Vector2Int before, UnityEngine.Vector2Int after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            return dirtyMask;
        }
    }

    public sealed class Vector3IntFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.Vector3Int>
    {
        public int FieldCount => 3;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.Vector3Int value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(3);
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
            writer.WriteInt32(value.z);
        }
        public global::UnityEngine.Vector3Int Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __x__ = default(int);
            var __y__ = default(int);
            var __z__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __x__ = reader.ReadInt32();
                        break;
                    case 1:
                        __y__ = reader.ReadInt32();
                        break;
                    case 2:
                        __z__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.Vector3Int(__x__, __y__, __z__);
            ____result.x = __x__;
            ____result.y = __y__;
            ____result.z = __z__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.Vector3Int value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.z);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.Vector3Int instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadInt32();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadInt32();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.z = reader.ReadInt32();
            }

        }

        public ulong GetDirtyMask(UnityEngine.Vector3Int before, UnityEngine.Vector3Int after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.z != after.z)
            {
                dirtyMask |= 1 << 2;
            }

            return dirtyMask;
        }
    }

    public sealed class RangeIntFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.RangeInt>
    {
        public int FieldCount => 2;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.RangeInt value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            writer.WriteInt32(value.start);
            writer.WriteInt32(value.length);
        }
        public global::UnityEngine.RangeInt Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __start__ = default(int);
            var __length__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __start__ = reader.ReadInt32();
                        break;
                    case 1:
                        __length__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.RangeInt(__start__, __length__);
            ____result.start = __start__;
            ____result.length = __length__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.RangeInt value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.start);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.length);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.RangeInt instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.start = reader.ReadInt32();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.length = reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.RangeInt before, UnityEngine.RangeInt after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.start != after.start)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.length != after.length)
            {
                dirtyMask |= 1 << 1;
            }

            return dirtyMask;
        }
    }

    public sealed class RectIntFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.RectInt>
    {
        public int FieldCount => 4;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.RectInt value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(4);
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
            writer.WriteInt32(value.width);
            writer.WriteInt32(value.height);
        }
        public global::UnityEngine.RectInt Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __x__ = default(int);
            var __y__ = default(int);
            var __width__ = default(int);
            var __height__ = default(int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __x__ = reader.ReadInt32();
                        break;
                    case 1:
                        __y__ = reader.ReadInt32();
                        break;
                    case 2:
                        __width__ = reader.ReadInt32();
                        break;
                    case 3:
                        __height__ = reader.ReadInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.RectInt(__x__, __y__, __width__, __height__);
            ____result.x = __x__;
            ____result.y = __y__;
            ____result.width = __width__;
            ____result.height = __height__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.RectInt value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                writer.Write(value.x);
            }

            if ((dirtyMask & 2UL) != 0)
            {
                writer.Write(value.y);
            }

            if ((dirtyMask & 4UL) != 0)
            {
                writer.Write(value.width);
            }
            
            if ((dirtyMask & 8UL) != 0)
            {
                writer.Write(value.height);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.RectInt instance, ulong dirtyMask)
        {
            if ((dirtyMask & 1UL) != 0)
            {
                instance.x = reader.ReadInt32();
            }

            if ((dirtyMask & 2UL) != 0)
            {
                instance.y = reader.ReadInt32();
            }

            if ((dirtyMask & 4UL) != 0)
            {
                instance.width = reader.ReadInt32();
            }
            
            if ((dirtyMask & 8UL) != 0)
            {
                instance.height = reader.ReadInt32();
            }
        }

        public ulong GetDirtyMask(UnityEngine.RectInt before, UnityEngine.RectInt after, MessagePackSerializerOptions options)
        {
            ulong dirtyMask = 0;

            if (before.x != after.x)
            {
                dirtyMask |= 1 << 0;
            }

            if (before.y != after.y)
            {
                dirtyMask |= 1 << 1;
            }

            if (before.width != after.width)
            {
                dirtyMask |= 1 << 2;
            }

            if (before.height != after.height)
            {
                dirtyMask |= 1 << 3;
            }

            return dirtyMask;
        }
    }

    public sealed class BoundsIntFormatter : global::MessagePack.Formatters.INetworkVarMessagePackFormatter<global::UnityEngine.BoundsInt>
    {
        public int FieldCount => 6;

        public void Serialize(ref MessagePackWriter writer, global::UnityEngine.BoundsInt value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(2);
            options.Resolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Serialize(ref writer, value.position, options);
            options.Resolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Serialize(ref writer, value.size, options);
        }
        public global::UnityEngine.BoundsInt Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }
            var length = reader.ReadArrayHeader();
            var __position__ = default(global::UnityEngine.Vector3Int);
            var __size__ = default(global::UnityEngine.Vector3Int);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __position__ = options.Resolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __size__ = options.Resolver.GetFormatterWithVerify<global::UnityEngine.Vector3Int>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::UnityEngine.BoundsInt(__position__, __size__);
            ____result.position = __position__;
            ____result.size = __size__;
            return ____result;
        }

        public void SerializeNetworkVar(ref MessagePackWriter writer, UnityEngine.BoundsInt value, MessagePackSerializerOptions options, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.Vector3Int>() as INetworkVarMessagePackFormatter<UnityEngine.Vector3Int>;
                formatter.SerializeNetworkVar(ref writer, value.position, options, dirtyMask);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.Vector3Int>() as INetworkVarMessagePackFormatter<UnityEngine.Vector3Int>;
                formatter.SerializeNetworkVar(ref writer, value.size, options, dirtyMask >> fieldCounter);
            }
        }

        public void DeserializeNetworkVar(ref MessagePackReader reader, MessagePackSerializerOptions options, ref UnityEngine.BoundsInt instance, ulong dirtyMask)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.Vector3Int>() as INetworkVarMessagePackFormatter<UnityEngine.Vector3Int>;
                var temp = instance.position;
                formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask);
                instance.position = temp;
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.Vector3Int>() as INetworkVarMessagePackFormatter<UnityEngine.Vector3Int>;
                var temp = instance.size;
                formatter.DeserializeNetworkVar(ref reader, options, ref temp, dirtyMask >> fieldCounter);
                instance.size = temp;
            }
        }

        public ulong GetDirtyMask(UnityEngine.BoundsInt before, UnityEngine.BoundsInt after, MessagePackSerializerOptions options)
        {
            IFormatterResolver resolver = options.Resolver;
            int fieldCounter = 0;

            ulong dirtyMask = 0;

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.Vector3Int>() as INetworkVarMessagePackFormatter<UnityEngine.Vector3Int>;
                dirtyMask |= formatter.GetDirtyMask(before.position, after.position, options);
                fieldCounter += formatter.FieldCount;
            }

            {
                var formatter = resolver.GetFormatterWithVerify<UnityEngine.Vector3Int>() as INetworkVarMessagePackFormatter<UnityEngine.Vector3Int>;
                dirtyMask |= formatter.GetDirtyMask(before.size, after.size, options) << fieldCounter;
                fieldCounter += formatter.FieldCount;
            }

            return dirtyMask;
        }
    }
#endif
}
