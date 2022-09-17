using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PretiaArCloud.Networking
{
    public class HostMessageHandler
    {
        protected ISerializer _serializer;
        private ReliableGameClient _gameClient;
        private TickManager _tickManager;

        private OpType OP_TYPE => OpType.Host;

        internal event SyncMessageEvent OnSyncMessage;

        private delegate void MsgHandler(ReadOnlySpan<byte> msgBody);
        private Dictionary<ushort, MsgHandler> _handlersMap = new Dictionary<ushort, MsgHandler>();
        private Dictionary<ushort, Delegate> _delegatesMap = new Dictionary<ushort, Delegate>();

        public HostMessageHandler(ISerializer serializer, ReliableGameClient gameClient, TickManager tickManager)
        {
            _serializer = serializer;
            _gameClient = gameClient;
            _tickManager = tickManager;
        }

        public void Enqueue<T>(T data)
        {
            byte[] body = _serializer.Serialize(data);
            ushort msgType = TypeResolver.Get<T>();

            _gameClient.EnqueueHostMessage(body, msgType, _tickManager.Tick);
        }

        public virtual void Send<T>(T data)
        {
            byte[] body = _serializer.Serialize(data);
            ushort msgType = TypeResolver.Get<T>();

            _gameClient.SendHostMessage(body, msgType, _tickManager.Tick);
        }

        public virtual void SendQueue()
        {
            _gameClient.SendHostMessage(_tickManager.Tick);
        }

        public void Register<T>(Action<T> callback)
        {
            ushort typeId = TypeResolver.Get<T>();

            if (_delegatesMap.TryGetValue(typeId, out Delegate delegateFunction))
            {
                var typedDelegateFunction = (Action<T>)delegateFunction;
                typedDelegateFunction += callback;
                _delegatesMap[typeId] = typedDelegateFunction;
            }
            else
            {
                _delegatesMap[typeId] = callback;

                MsgHandler handler = (msgBody) =>
                {
                    var typedDelegateFunction = (Action<T>)_delegatesMap[typeId];
                    typedDelegateFunction?.Invoke(_serializer.Deserialize<T>(msgBody));
                };

                _handlersMap[typeId] = handler;
            }
        }

        public void Unregister<T>(Action<T> callback)
        {
            ushort typeId = TypeResolver.Get<T>();
            if (_delegatesMap.TryGetValue(typeId, out Delegate delegateFunction))
            {
                var typedDelegateFunction = (Action<T>)delegateFunction;
                typedDelegateFunction -= callback;
                _delegatesMap[typeId] = typedDelegateFunction;
            }
            else
            {
                throw new KeyNotFoundException($"There is no message handler found for type {typeof(T)} in HostMsg");
            }
        }

        public void OnReceiveMessage(ReadOnlyMemory<byte> memory)
        {
            var packet = memory.Span;
            ushort msgType = SpanBasedBitConverter.ToUInt16(packet);
            ushort tick = SpanBasedBitConverter.ToUInt16(packet.Slice(2));

            switch (msgType)
            {
                case SpecialMessage.COMPOUND_MESSAGE:
                    int index = 4;

                    byte msgCount = packet[index];
                    index++;

                    for (int i = 0; i < msgCount; i++)
                    {
                        ushort compoundedMsgType = SpanBasedBitConverter.ToUInt16(packet.Slice(index));
                        index += sizeof(ushort);

                        ushort compoundedMsgLength = SpanBasedBitConverter.ToUInt16(packet.Slice(index));
                        index += sizeof(ushort);

                        InvokeHandler(compoundedMsgType, packet.Slice(index, compoundedMsgLength));
                        index += compoundedMsgLength;
                    }
                    break;

                case SpecialMessage.SYNC_MESSAGE:
                    OnSyncMessage?.Invoke(tick, memory.Slice(4));
                    break;

                default:
                    InvokeHandler(msgType, packet.Slice(4));
                    break;
            }
        }

        private void InvokeHandler(ushort msgType, ReadOnlySpan<byte> msgBody)
        {
            if (_handlersMap.TryGetValue(msgType, out MsgHandler handler))
            {
                handler.Invoke(msgBody);
            }
        }
    }
}