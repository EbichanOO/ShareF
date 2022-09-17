using System;
using System.Collections.Generic;

namespace PretiaArCloud.Networking
{
    public class PlayerToHostMessageHandler
    {
        protected ISerializer _serializer;
        private ReliableGameClient _gameClient;
        private Dictionary<uint, Player> _players;
        private TickManager _tickManager;

        private OpType OP_TYPE => OpType.PlayerToHost;
        internal event PlayerSyncMessageEvent OnSyncMessage;

        private delegate void MsgHandler(ReadOnlySpan<byte> msgBody, Player player);
        private Dictionary<ushort, MsgHandler> _handlersMap = new Dictionary<ushort, MsgHandler>();
        private Dictionary<ushort, Delegate> _delegatesMap = new Dictionary<ushort, Delegate>();

        public PlayerToHostMessageHandler(ISerializer serializer, ReliableGameClient gameClient, ref Dictionary<uint, Player> players, TickManager tickManager)
        {
            _serializer = serializer;
            _gameClient = gameClient;
            _players = players;
            _tickManager = tickManager;
        }

        public void Enqueue<T>(T data)
        {
            byte[] body = _serializer.Serialize(data);
            ushort msgType = TypeResolver.Get<T>();

            _gameClient.EnqueuePlayerToHostMessage(body, msgType, _tickManager.Tick);
        }

        public void SendQueue()
        {
            _gameClient.SendPlayerToHostMessage(_tickManager.Tick);
        }

        public void Send<T>(T data)
        {
            byte[] body = _serializer.Serialize(data);
            ushort msgType = TypeResolver.Get<T>();

            _gameClient.SendPlayerToHostMessage(body, msgType, _tickManager.Tick);
        }

        public void Register<T>(Action<T, Player> callback)
        {
            ushort typeId = TypeResolver.Get<T>();

            if (_delegatesMap.TryGetValue(typeId, out Delegate delegateFunction))
            {
                var typedDelegateFunction = (Action<T, Player>)delegateFunction;
                typedDelegateFunction += callback;
                _delegatesMap[typeId] = typedDelegateFunction;
            }
            else
            {
                _delegatesMap[typeId] = callback;

                MsgHandler handler = (msgBody, sender) =>
                {
                    var typedDelegateFunction = (Action<T, Player>)_delegatesMap[typeId];
                    typedDelegateFunction?.Invoke(_serializer.Deserialize<T>(msgBody), sender);
                };

                _handlersMap[typeId] = handler;
            }
        }

        public void Unregister<T>(Action<T, Player> callback)
        {
            ushort typeId = TypeResolver.Get<T>();
            if (_delegatesMap.TryGetValue(typeId, out Delegate delegateFunction))
            {
                var typedDelegateFunction = (Action<T, Player>)delegateFunction;
                typedDelegateFunction -= callback;
                _delegatesMap[typeId] = typedDelegateFunction;
            }
            else
            {
                throw new KeyNotFoundException($"There is no message handler found for type {typeof(T)} in HostMsg");
            }
        }

        public void OnReceiveMessage(ReadOnlyMemory<byte> memory, uint userNumber)
        {
            bool playerExists = _players.TryGetValue(userNumber, out Player sender);
            
            if (!playerExists)
            {
                throw new Exception($"There is no player found with userNumber: {userNumber}");
            }
            
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

                        InvokeHandler(compoundedMsgType, packet.Slice(index, compoundedMsgLength), sender);
                        index += compoundedMsgLength;
                    }
                    break;

                case SpecialMessage.SYNC_MESSAGE:
                    OnSyncMessage?.Invoke(tick, memory.Slice(4), sender);
                    break;

                default:
                    InvokeHandler(msgType, packet.Slice(4), sender);
                    break;
            }
        }

        private void InvokeHandler(ushort msgType, ReadOnlySpan<byte> msgBody, Player player)
        {
            if (_handlersMap.TryGetValue(msgType, out MsgHandler handler))
            {
                handler.Invoke(msgBody, player);
            }
        }
    }
}