using System;
using System.Collections.Generic;

namespace PretiaArCloud.Networking
{
    public class PlayerMessageHandler
    {
        protected ISerializer _serializer;
        private ReliableGameClient _gameClient;
        private Player _localPlayer;
        private Dictionary<uint, Player> _players;
        private TickManager _tickManager;
        private Action _queuedCallbacks;

        private OpType OP_TYPE => OpType.Player;
        internal event PlayerSyncMessageEvent OnSyncMessage;

        private delegate void MsgHandler(ReadOnlySpan<byte> msgBody, Player player);
        private Dictionary<ushort, MsgHandler> _handlersMap = new Dictionary<ushort, MsgHandler>();
        private Dictionary<ushort, Delegate> _delegatesMap = new Dictionary<ushort, Delegate>();

        public PlayerMessageHandler(ISerializer serializer,
                                    ReliableGameClient gameClient,
                                    Player localPlayer,
                                    ref Dictionary<uint, Player> players,
                                    TickManager tickManager)
        {
            _serializer = serializer;
            _gameClient = gameClient;
            _localPlayer = localPlayer;
            _players = players;
            _tickManager = tickManager;
        }

        public void Enqueue<T>(T data)
        {
            byte[] body = _serializer.Serialize(data);
            ushort msgType = TypeResolver.Get<T>();

            _gameClient.EnqueuePlayerMessage(body, msgType, _tickManager.Tick);
            _queuedCallbacks += () => InvokeHandler(msgType, body, _localPlayer);
        }

        public void SendQueue()
        {
            _gameClient.SendPlayerMessage(_tickManager.Tick);

            _queuedCallbacks?.Invoke();
            _queuedCallbacks = null;
        }

        public void Send<T>(T data)
        {
            byte[] body = _serializer.Serialize(data);
            ushort msgType = TypeResolver.Get<T>();

            _gameClient.SendPlayerMessage(body, msgType, _tickManager.Tick);
            InvokeHandler(msgType, body, _localPlayer);
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

                MsgHandler handler = (msgBody, player) =>
                {
                    var typedDelegateFunction = (Action<T, Player>)_delegatesMap[typeId];
                    typedDelegateFunction?.Invoke(_serializer.Deserialize<T>(msgBody), player);
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
            Player sender;
            if (!_players.TryGetValue(userNumber, out sender))
            {
                sender = new Player{ UserNumber = userNumber };
                _players.Add(userNumber, sender);
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