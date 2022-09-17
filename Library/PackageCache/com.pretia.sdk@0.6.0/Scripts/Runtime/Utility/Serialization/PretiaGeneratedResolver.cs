// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace PretiaArCloud.Networking.Resolvers
{
    using System;

    public class PretiaGeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new PretiaGeneratedResolver();

        private PretiaGeneratedResolver()
        {
        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                var f = PretiaGeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class PretiaGeneratedResolverGetFormatterHelper
    {
        private static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static PretiaGeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(15)
            {
                { typeof(global::System.Collections.Generic.List<string>), 0 },
                { typeof(global::System.Collections.Generic.List<uint>), 1 },
                { typeof(global::PretiaArCloud.Networking.DeleteExistInSceneObjectsMsg), 2 },
                { typeof(global::PretiaArCloud.Networking.InstantiateMsg), 3 },
                { typeof(global::PretiaArCloud.Networking.NetworkAnimatorSnapshotMsg), 4 },
                { typeof(global::PretiaArCloud.Networking.NetworkAnimatorSyncMsg), 5 },
                { typeof(global::PretiaArCloud.Networking.NetworkDestroyMsg), 6 },
                { typeof(global::PretiaArCloud.Networking.NetworkRigidbodySyncMsg), 7 },
                { typeof(global::PretiaArCloud.Networking.NetworkTransformSyncMsg), 8 },
                { typeof(global::PretiaArCloud.Networking.NotifyConnectionMsg), 9 },
                { typeof(global::PretiaArCloud.Networking.PlayerJoinedMsg), 10 },
                { typeof(global::PretiaArCloud.Networking.PlayerListSnapshotMsg), 11 },
                { typeof(global::PretiaArCloud.Networking.QuaternionSerializable), 12 },
                { typeof(global::PretiaArCloud.Networking.TransformSerializable), 13 },
                { typeof(global::PretiaArCloud.Networking.Vector3Serializable), 14 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key))
            {
                return null;
            }

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ListFormatter<string>();
                case 1: return new global::MessagePack.Formatters.ListFormatter<uint>();
                case 2: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.DeleteExistInSceneObjectsMsgFormatter();
                case 3: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.InstantiateMsgFormatter();
                case 4: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.NetworkAnimatorSnapshotMsgFormatter();
                case 5: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.NetworkAnimatorSyncMsgFormatter();
                case 6: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.NetworkDestroyMsgFormatter();
                case 7: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.NetworkRigidbodySyncMsgFormatter();
                case 8: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.NetworkTransformSyncMsgFormatter();
                case 9: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.NotifyConnectionMsgFormatter();
                case 10: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.PlayerJoinedMsgFormatter();
                case 11: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.PlayerListChangedMsgFormatter();
                case 12: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.QuaternionSerializableFormatter();
                case 13: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.TransformSerializableFormatter();
                case 14: return new PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking.Vector3SerializableFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1649 // File name should match first type name




// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace PretiaArCloud.Networking.Formatters.PretiaArCloud.Networking
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class DeleteExistInSceneObjectsMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.DeleteExistInSceneObjectsMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.DeleteExistInSceneObjectsMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(0);
        }

        public global::PretiaArCloud.Networking.DeleteExistInSceneObjectsMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.DeleteExistInSceneObjectsMsg();
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class InstantiateMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.InstantiateMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.InstantiateMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(9);
            writer.Write(value.NetworkId);
            writer.Write(value.PrefabId);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Serialize(ref writer, value.Position, options);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.QuaternionSerializable>().Serialize(ref writer, value.Rotation, options);
            writer.Write(value.Parent);
            writer.Write(value.Active);
            writer.Write(value.Sender);
            writer.Write(value.Owner);
            writer.Write(value.ForSnapshot);
        }

        public global::PretiaArCloud.Networking.InstantiateMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __NetworkId__ = default(uint);
            var __PrefabId__ = default(ushort);
            var __Position__ = default(global::PretiaArCloud.Networking.Vector3Serializable);
            var __Rotation__ = default(global::PretiaArCloud.Networking.QuaternionSerializable);
            var __Parent__ = default(uint);
            var __Active__ = default(bool);
            var __Sender__ = default(uint);
            var __Owner__ = default(uint);
            var __ForSnapshot__ = default(bool);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __NetworkId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __PrefabId__ = reader.ReadUInt16();
                        break;
                    case 2:
                        __Position__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Deserialize(ref reader, options);
                        break;
                    case 3:
                        __Rotation__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.QuaternionSerializable>().Deserialize(ref reader, options);
                        break;
                    case 4:
                        __Parent__ = reader.ReadUInt32();
                        break;
                    case 5:
                        __Active__ = reader.ReadBoolean();
                        break;
                    case 6:
                        __Sender__ = reader.ReadUInt32();
                        break;
                    case 7:
                        __Owner__ = reader.ReadUInt32();
                        break;
                    case 8:
                        __ForSnapshot__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.InstantiateMsg();
            ____result.NetworkId = __NetworkId__;
            ____result.PrefabId = __PrefabId__;
            ____result.Position = __Position__;
            ____result.Rotation = __Rotation__;
            ____result.Parent = __Parent__;
            ____result.Active = __Active__;
            ____result.Sender = __Sender__;
            ____result.Owner = __Owner__;
            ____result.ForSnapshot = __ForSnapshot__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkAnimatorSnapshotMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.NetworkAnimatorSnapshotMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.NetworkAnimatorSnapshotMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            writer.Write(value.NetworkId);
            formatterResolver.GetFormatterWithVerify<int[]>().Serialize(ref writer, value.FullPathHash, options);
            formatterResolver.GetFormatterWithVerify<float[]>().Serialize(ref writer, value.NormalizedTime, options);
        }

        public global::PretiaArCloud.Networking.NetworkAnimatorSnapshotMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __NetworkId__ = default(uint);
            var __FullPathHash__ = default(int[]);
            var __NormalizedTime__ = default(float[]);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __NetworkId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __FullPathHash__ = formatterResolver.GetFormatterWithVerify<int[]>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __NormalizedTime__ = formatterResolver.GetFormatterWithVerify<float[]>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.NetworkAnimatorSnapshotMsg();
            ____result.NetworkId = __NetworkId__;
            ____result.FullPathHash = __FullPathHash__;
            ____result.NormalizedTime = __NormalizedTime__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkAnimatorSyncMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.NetworkAnimatorSyncMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.NetworkAnimatorSyncMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            writer.Write(value.NetworkId);
            writer.Write(value.Data);
        }

        public global::PretiaArCloud.Networking.NetworkAnimatorSyncMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __NetworkId__ = default(uint);
            var __Data__ = default(byte[]);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __NetworkId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __Data__ = reader.ReadBytes()?.ToArray();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.NetworkAnimatorSyncMsg();
            ____result.NetworkId = __NetworkId__;
            ____result.Data = __Data__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkDestroyMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.NetworkDestroyMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.NetworkDestroyMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(2);
            writer.Write(value.NetworkId);
            writer.Write(value.IsExistInScene);
        }

        public global::PretiaArCloud.Networking.NetworkDestroyMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __NetworkId__ = default(uint);
            var __IsExistInScene__ = default(bool);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __NetworkId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __IsExistInScene__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.NetworkDestroyMsg();
            ____result.NetworkId = __NetworkId__;
            ____result.IsExistInScene = __IsExistInScene__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkRigidbodySyncMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.NetworkRigidbodySyncMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.NetworkRigidbodySyncMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.NetworkId);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.TransformSerializable>().Serialize(ref writer, value.Transform, options);
        }

        public global::PretiaArCloud.Networking.NetworkRigidbodySyncMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __NetworkId__ = default(uint);
            var __Transform__ = default(global::PretiaArCloud.Networking.TransformSerializable);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __NetworkId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __Transform__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.TransformSerializable>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.NetworkRigidbodySyncMsg();
            ____result.NetworkId = __NetworkId__;
            ____result.Transform = __Transform__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NetworkTransformSyncMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.NetworkTransformSyncMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.NetworkTransformSyncMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            writer.Write(value.NetworkId);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.TransformSerializable>().Serialize(ref writer, value.Transform, options);
        }

        public global::PretiaArCloud.Networking.NetworkTransformSyncMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __NetworkId__ = default(uint);
            var __Transform__ = default(global::PretiaArCloud.Networking.TransformSerializable);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __NetworkId__ = reader.ReadUInt32();
                        break;
                    case 1:
                        __Transform__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.TransformSerializable>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.NetworkTransformSyncMsg();
            ____result.NetworkId = __NetworkId__;
            ____result.Transform = __Transform__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class NotifyConnectionMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.NotifyConnectionMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.NotifyConnectionMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(1);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.DisplayName, options);
        }

        public global::PretiaArCloud.Networking.NotifyConnectionMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __DisplayName__ = default(string);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __DisplayName__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.NotifyConnectionMsg();
            ____result.DisplayName = __DisplayName__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class PlayerJoinedMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.PlayerJoinedMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.PlayerJoinedMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(1);
            writer.Write(value.UserNumber);
        }

        public global::PretiaArCloud.Networking.PlayerJoinedMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __UserNumber__ = default(uint);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __UserNumber__ = reader.ReadUInt32();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.PlayerJoinedMsg();
            ____result.UserNumber = __UserNumber__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class PlayerListChangedMsgFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.PlayerListSnapshotMsg>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.PlayerListSnapshotMsg value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<uint>>().Serialize(ref writer, value.UserNumbers, options);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.DisplayNames, options);
        }

        public global::PretiaArCloud.Networking.PlayerListSnapshotMsg Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __UserNumbers__ = default(global::System.Collections.Generic.List<uint>);
            var __DisplayNames__ = default(global::System.Collections.Generic.List<string>);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __UserNumbers__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<uint>>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __DisplayNames__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.PlayerListSnapshotMsg(__UserNumbers__, __DisplayNames__);
            ____result.UserNumbers = __UserNumbers__;
            ____result.DisplayNames = __DisplayNames__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class QuaternionSerializableFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.QuaternionSerializable>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.QuaternionSerializable value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            writer.WriteArrayHeader(4);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
            writer.Write(value.W);
        }

        public global::PretiaArCloud.Networking.QuaternionSerializable Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __X__ = default(float);
            var __Y__ = default(float);
            var __Z__ = default(float);
            var __W__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __X__ = reader.ReadSingle();
                        break;
                    case 1:
                        __Y__ = reader.ReadSingle();
                        break;
                    case 2:
                        __Z__ = reader.ReadSingle();
                        break;
                    case 3:
                        __W__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.QuaternionSerializable(__X__, __Y__, __Z__, __W__);
            ____result.X = __X__;
            ____result.Y = __Y__;
            ____result.Z = __Z__;
            ____result.W = __W__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class TransformSerializableFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.TransformSerializable>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.TransformSerializable value, global::MessagePack.MessagePackSerializerOptions options)
        {
            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(3);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Serialize(ref writer, value.LocalPosition, options);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Serialize(ref writer, value.LocalEulerAngles, options);
            formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Serialize(ref writer, value.LocalScale, options);
        }

        public global::PretiaArCloud.Networking.TransformSerializable Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __LocalPosition__ = default(global::PretiaArCloud.Networking.Vector3Serializable);
            var __LocalEulerAngles__ = default(global::PretiaArCloud.Networking.Vector3Serializable);
            var __LocalScale__ = default(global::PretiaArCloud.Networking.Vector3Serializable);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __LocalPosition__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __LocalEulerAngles__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __LocalScale__ = formatterResolver.GetFormatterWithVerify<global::PretiaArCloud.Networking.Vector3Serializable>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.TransformSerializable(__LocalPosition__, __LocalEulerAngles__, __LocalScale__);
            ____result.LocalPosition = __LocalPosition__;
            ____result.LocalEulerAngles = __LocalEulerAngles__;
            ____result.LocalScale = __LocalScale__;
            reader.Depth--;
            return ____result;
        }
    }

    public sealed class Vector3SerializableFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::PretiaArCloud.Networking.Vector3Serializable>
    {

        public void Serialize(ref MessagePackWriter writer, global::PretiaArCloud.Networking.Vector3Serializable value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(3);
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
        }

        public global::PretiaArCloud.Networking.Vector3Serializable Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            options.Security.DepthStep(ref reader);
            var length = reader.ReadArrayHeader();
            var __X__ = default(float);
            var __Y__ = default(float);
            var __Z__ = default(float);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __X__ = reader.ReadSingle();
                        break;
                    case 1:
                        __Y__ = reader.ReadSingle();
                        break;
                    case 2:
                        __Z__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::PretiaArCloud.Networking.Vector3Serializable(__X__, __Y__, __Z__);
            ____result.X = __X__;
            ____result.Y = __Y__;
            ____result.Z = __Z__;
            reader.Depth--;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name

