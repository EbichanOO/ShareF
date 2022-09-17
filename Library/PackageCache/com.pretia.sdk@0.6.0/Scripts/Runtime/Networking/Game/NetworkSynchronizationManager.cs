using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using MessagePack;

namespace PretiaArCloud.Networking
{
    internal delegate void SyncMessageEvent(ushort tick, ReadOnlyMemory<byte> bodyMemory);
    internal delegate void PlayerSyncMessageEvent(ushort tick, ReadOnlyMemory<byte> bodyMemory, Player player);

    public class NetworkSynchronizationManager
    {
        private readonly ArrayBufferWriter<byte> _networkWriter;
        private readonly ArrayBufferWriter<byte> _scratchBuffer;
        private readonly ArrayBufferWriter<byte> _dirtyMaskBuffer;
        private readonly ISerializer _serializer;
        private readonly ReliableGameClient _gameClient;
        private readonly ReliableGameClient.ProtocolId _protocolId;
        private readonly IdentityManager _identityManager;
        private readonly MessagePackSerializerOptions _options;
        private readonly TickManager _tickManager;
        private readonly ArrayPool<byte> _byteArrayPool;
        private readonly Dictionary<uint, Player> _players;
        private readonly bool _enableNetworkVisibility;
        private readonly float _networkVisibilityRange;

        private SortedDictionary<uint, LinkedList<NetworkBehaviour>> _cachedNetworkBehaviours;
        private ConcurrentQueue<SynchronizationWriter[]> _synchronizationPerTick;
        private object _synchronizationObject = new object();

        struct SynchronizationWriter : IDisposable
        {
            private const int HEADER_LENGTH = 8;

            public Player Player;
            public ushort Tick;

            private float _maxDistance;
            private int _diffMaskLength;
            private ArrayPool<byte> _arrayPool;
            private ArrayBufferWriter<byte> _headerWriter;
            private ArrayBufferWriter<byte> _diffMaskWriter;
            private ArrayBufferWriter<byte> _bodyWriter;
            private Dictionary<uint, bool> _networkVisibility;

            internal SynchronizationWriter(Player player, ushort tick, float maxDistance, int diffMaskLength, ArrayPool<byte> arrayPool)
            {
                Player = player;
                Tick = tick;

                _maxDistance = maxDistance;
                _diffMaskLength = diffMaskLength;
                _arrayPool = arrayPool;

                _headerWriter = new ArrayBufferWriter<byte>(HEADER_LENGTH, _arrayPool);
                _diffMaskWriter = new ArrayBufferWriter<byte>(_diffMaskLength, _arrayPool);
                _bodyWriter = new ArrayBufferWriter<byte>(256, _arrayPool);
                _networkVisibility = new Dictionary<uint, bool>();
            }

            internal Span<byte> GetDiffMask()
            {
                return _diffMaskWriter.GetSpan(_diffMaskLength);
            }

            internal Span<byte> GetHeader()
            {
                var span = _headerWriter.GetSpan(HEADER_LENGTH);
                _headerWriter.Advance(HEADER_LENGTH);
                return span;
            }

            internal bool HasVisibilityOn(uint networkId)
            {
                return _networkVisibility.ContainsKey(networkId) && _networkVisibility[networkId];
            }

            internal void CalculateNetworkVisibility(NetworkIdentity networkIdentity)
            {
                if (NetworkCameraManager.Instance == null)
                {
                    _networkVisibility[networkIdentity.Value] = true;
                }
                else
                {
                    if (NetworkCameraManager.Instance.TryGetProxyByOwner(Player, out var playerProxy))
                    {
                        _networkVisibility[networkIdentity.Value] = UnityEngine.Vector3.Distance(playerProxy.position, networkIdentity.transform.position) <= _maxDistance;
                    }
                }
            }

            internal void WriteBody(ArrayBufferWriter<byte> tempWriter)
            {
                var messagePackWriter = new MessagePackWriter(_bodyWriter);
                messagePackWriter.WriteRaw(tempWriter.WrittenSpan);
                messagePackWriter.Flush();
            }

            public void Dispose()
            {
                _headerWriter.Dispose();
                _diffMaskWriter.Dispose();
                _bodyWriter.Dispose();
            }

            internal void WriteAll(ArrayBufferWriter<byte> networkWriter)
            {
                _diffMaskWriter.Advance(_diffMaskLength);

                var messagePackWriter = new MessagePackWriter(networkWriter);
                messagePackWriter.WriteRaw(_headerWriter.WrittenSpan);
                messagePackWriter.WriteRaw(_diffMaskWriter.WrittenSpan);
                messagePackWriter.WriteRaw(_bodyWriter.WrittenSpan);
                messagePackWriter.Flush();

                var userNumberSpan = networkWriter.GetSpan(sizeof(uint));
                SpanBasedBitConverter.TryWriteBytes(userNumberSpan, Player.UserNumber);
                networkWriter.Advance(sizeof(uint));
            }

            internal ushort GetTotalLength()
            {
                return (ushort)(HEADER_LENGTH + _diffMaskLength + _bodyWriter.Index + sizeof(uint));
            }
        }

        internal NetworkSynchronizationManager(
            ArrayBufferWriter<byte> scratchBuffer,
            ArrayBufferWriter<byte> dirtyMaskBuffer,
            ISerializer serializer,
            ReliableGameClient gameClient,
            ReliableGameClient.ProtocolId protocolId,
            IdentityManager identityManager,
            TickManager tickManager,
            ref ArrayPool<byte> byteArrayPool,
            ref Dictionary<uint, Player> players,
            bool enableNetworkVisibility,
            float networkVisibilityRange)
        {
            _scratchBuffer = scratchBuffer;
            _dirtyMaskBuffer = dirtyMaskBuffer;
            _tickManager = tickManager;
            _byteArrayPool = byteArrayPool;
            _players = players;
            _enableNetworkVisibility = enableNetworkVisibility;
            _networkVisibilityRange = networkVisibilityRange;

            _networkWriter = new ArrayBufferWriter<byte>(1024);
            _cachedNetworkBehaviours = new SortedDictionary<uint, LinkedList<NetworkBehaviour>>();
            _synchronizationPerTick = new ConcurrentQueue<SynchronizationWriter[]>();

            _serializer = serializer;
            if (_serializer is MsgPackSerializer)
            {
                _options = ((MsgPackSerializer)_serializer).Options;
            }

            _gameClient = gameClient;
            _protocolId = protocolId;
            _identityManager = identityManager;
        }

        internal bool HostSerialize(ushort tick)
        {
            if (!_enableNetworkVisibility) return Serialize(tick);

            if (_players.Count < 2) return false;

            if (_cachedNetworkBehaviours.Count == 0) return false;

            bool result = false;

            SynchronizationWriter[] playerWriters = new SynchronizationWriter[_players.Count-1];
            int differCount = (_cachedNetworkBehaviours.Count + 7) / 8;
            {
                int i = 0;
                foreach (var player in _players)
                {
                    if (!player.Value.IsHost)
                    {
                        playerWriters[i] = new SynchronizationWriter(player.Value, tick, _networkVisibilityRange, differCount, _byteArrayPool);
                        i++;
                    }
                }
            }

            int it = 0;
            foreach (var networkObject in _cachedNetworkBehaviours)
            {
                for (int i = 0; i < playerWriters.Length; i++)
                {
                    playerWriters[i].CalculateNetworkVisibility(_identityManager.Get(networkObject.Key));
                }

                bool hasBeenWritten = false;
                var tempWriter = new ArrayBufferWriter<byte>(128, _byteArrayPool);
                int bhvDifferCount = (networkObject.Value.Count + 7) / 8;
                var bhvDiffMask = tempWriter.GetSpan(bhvDifferCount);

                int bhvIt = 0;
                foreach (var networkBehaviour in networkObject.Value)
                {
                    var scratchWriter = new MessagePackWriter(_scratchBuffer);
                    var dirtyMaskWriter = new MessagePackWriter(_dirtyMaskBuffer);
                    if (networkBehaviour.TrySerialize(ref scratchWriter, ref dirtyMaskWriter, _options, tick))
                    {
                        scratchWriter.Flush();
                        dirtyMaskWriter.Flush();

                        bhvDiffMask[bhvIt / 8] |= (byte)(1 << (bhvIt % 8));
                        if (!hasBeenWritten)
                        {
                            tempWriter.Advance(bhvDifferCount);
                            hasBeenWritten = true;
                        }

                        for (int i = 0; i < playerWriters.Length; i++)
                        {
                            if (playerWriters[i].HasVisibilityOn(networkObject.Key))
                            {
                                var diffMask = playerWriters[i].GetDiffMask();
                                diffMask[it / 8] |= (byte)(1 << (it % 8));
                            }
                        }

                        var writer = new MessagePackWriter(tempWriter);
                        writer.WriteRaw(_dirtyMaskBuffer.WrittenSpan);
                        writer.WriteRaw(_scratchBuffer.WrittenSpan);
                        writer.Flush();
                    }

                    _scratchBuffer.Clear();
                    _dirtyMaskBuffer.Clear();
                    bhvIt++;
                }

                for (int i = 0; i < playerWriters.Length; i++)
                {
                    if (tempWriter.Index > 0 && playerWriters[i].HasVisibilityOn(networkObject.Key))
                    {
                        result = true;
                        playerWriters[i].WriteBody(tempWriter);
                    }
                }
                
                tempWriter.Dispose();

                it++;
            }

            if (result)
            {
                _synchronizationPerTick.Enqueue(playerWriters);
            }

            return result;
        }

        internal bool Serialize(ushort tick)
        {
            bool result = false;

            var tempWriter = new ArrayBufferWriter<byte>(1024, _byteArrayPool);
            var headerSpan = tempWriter.GetSpan(8);
            tempWriter.Advance(8);

            int differCount = (_cachedNetworkBehaviours.Count + 7) / 8;
            var diffMask = tempWriter.GetSpan(differCount);
            tempWriter.Advance(differCount);

            int it = 0;
            foreach (var cachedBehaviours in _cachedNetworkBehaviours.Values)
            {
                bool hasBeenWritten = false;
                int bhvDifferCount = (cachedBehaviours.Count + 7) / 8;
                var bhvDiffMask = tempWriter.GetSpan(bhvDifferCount);

                int bhvIt = 0;
                foreach (var networkBehaviour in cachedBehaviours)
                {
                    var scratchWriter = new MessagePackWriter(_scratchBuffer);
                    var dirtyMaskWriter = new MessagePackWriter(_dirtyMaskBuffer);
                    if (networkBehaviour.TrySerialize(ref scratchWriter, ref dirtyMaskWriter, _options, tick))
                    {
                        scratchWriter.Flush();
                        dirtyMaskWriter.Flush();

                        bhvDiffMask[bhvIt / 8] |= (byte)(1 << (bhvIt % 8));
                        diffMask[it / 8] |= (byte)(1 << (it % 8));

                        if (!hasBeenWritten)
                        {
                            tempWriter.Advance(bhvDifferCount);
                            hasBeenWritten = true;
                        }

                        var writer = new MessagePackWriter(tempWriter);
                        writer.WriteRaw(_dirtyMaskBuffer.WrittenSpan);
                        writer.WriteRaw(_scratchBuffer.WrittenSpan);
                        writer.Flush();
                    }

                    _scratchBuffer.Clear();
                    _dirtyMaskBuffer.Clear();
                    bhvIt++;
                }

                it++;
            }

            if (tempWriter.Index > (8 + differCount))
            {
                result = true;
                _gameClient.WriteHeader(headerSpan, (ushort)tempWriter.Index, (ushort)_protocolId, SpecialMessage.SYNC_MESSAGE, _tickManager.Tick);

                _networkWriter.GetSpan(tempWriter.Index);
                lock(_synchronizationObject)
                {
                    Buffer.BlockCopy(tempWriter.Buffer, 0, _networkWriter.Buffer, _networkWriter.Index, tempWriter.Index);
                    _networkWriter.Advance(tempWriter.Index);
                }
            }
            
            tempWriter.Dispose();

            return result;
        }

        internal void SendHostSyncUpdate()
        {
            if (!_enableNetworkVisibility) SendSyncUpdate();

            while (_synchronizationPerTick.TryDequeue(out var playerWriters))
            {
                for (int i = 0; i < playerWriters.Length; i++)
                {
                    var headerSpan = playerWriters[i].GetHeader();
                    _gameClient.WriteHeader(headerSpan, playerWriters[i].GetTotalLength(), (ushort)ReliableGameClient.ProtocolId.HostToPlayerMessage, SpecialMessage.SYNC_MESSAGE, playerWriters[i].Tick);

                    playerWriters[i].WriteAll(_networkWriter);
                    _gameClient.SendBody(_networkWriter.WrittenSpan);
                    _networkWriter.Clear();
                }

                playerWriters = null;
            }
        }

        internal void SendSyncUpdate()
        {
            lock(_synchronizationObject)
            {
                if (_networkWriter.Index > 0)
                {
                    _gameClient.SendBody(_networkWriter.WrittenSpan);
                    _networkWriter.Clear();
                }
            }
        }

        internal void Deserialize(ushort tick, ReadOnlyMemory<byte> msgBody, Player sender)
        {
            // if (sender == _localPlayer) return;

            int differCount = (_cachedNetworkBehaviours.Count + 7) / 8;
            var diffMask = msgBody.Slice(0, differCount).Span;

            int totalBytesRead = differCount;

            List<NetworkBehaviour> corrections = new List<NetworkBehaviour>();
            int it = 0;
            foreach (var kv in _cachedNetworkBehaviours)
            {
                var cachedBehaviours = kv.Value;
                byte idMask = diffMask[it / 8];
                byte currentIdMask = (byte)(1 << (it % 8));

                if ((idMask & currentIdMask) != 0)
                {
                    var identity = _identityManager.Get(kv.Key);
                    if (identity.Owner != sender)
                    {
                        throw new Exception($"Error trying to synchronize {identity.name} that's not owned by {sender.UserNumber}");
                    }

                    int bhvDifferCount = (cachedBehaviours.Count + 7) / 8;
                    var bhvDiffMask = msgBody.Slice(totalBytesRead, bhvDifferCount).Span;
                    totalBytesRead += bhvDifferCount;

                    int bhvIt = 0;
                    foreach (var networkBehaviour in cachedBehaviours)
                    {
                        byte bhvMask = bhvDiffMask[bhvIt / 8];
                        byte currentBhvMask = (byte)(1 << (bhvIt % 8));

                        if ((bhvMask & currentBhvMask) != 0)
                        {
                            var buffer = msgBody.Slice(totalBytesRead);
                            var reader = new MessagePackReader(buffer);
                            networkBehaviour.TryDeserialize(ref reader, _options, tick);
                            totalBytesRead += buffer.Slice(0, (int)reader.Consumed).Length;
                        }
                        bhvIt++;
                    }
                }
                it++;
            }
        }

        internal void Deserialize(ushort tick, ReadOnlyMemory<byte> msgBody)
        {
            int differCount = (_cachedNetworkBehaviours.Count + 7) / 8;
            var diffMask = msgBody.Slice(0, differCount).Span;

            int totalBytesRead = differCount;

            List<NetworkBehaviour> corrections = new List<NetworkBehaviour>();
            int it = 0;
            foreach (var cachedBehaviours in _cachedNetworkBehaviours.Values)
            {
                byte idMask = diffMask[it / 8];
                byte currentIdMask = (byte)(1 << (it % 8));

                if ((idMask & currentIdMask) != 0)
                {
                    int bhvDifferCount = (cachedBehaviours.Count + 7) / 8;
                    var bhvDiffMask = msgBody.Slice(totalBytesRead, bhvDifferCount).Span;
                    totalBytesRead += bhvDifferCount;

                    int bhvIt = 0;
                    foreach (var networkBehaviour in cachedBehaviours)
                    {
                        byte bhvMask = bhvDiffMask[bhvIt / 8];
                        byte currentBhvMask = (byte)(1 << (bhvIt % 8));

                        if ((bhvMask & currentBhvMask) != 0)
                        {
                            var buffer = msgBody.Slice(totalBytesRead);
                            var reader = new MessagePackReader(buffer);
                            long consumed = networkBehaviour.TryDeserialize(ref reader, _options, tick);
                            totalBytesRead += (int)consumed;
                        }
                        bhvIt++;
                    }
                }
                it++;
            }
        }

        public void AddNetworkBehaviour(uint networkIdentity, NetworkBehaviour networkBehaviour)
        {
            if (_cachedNetworkBehaviours.TryGetValue(networkIdentity, out LinkedList<NetworkBehaviour> networkBehaviours))
            {
                networkBehaviour.Node = networkBehaviours.AddLast(networkBehaviour);
            }
            else
            {
                var linkedList = new LinkedList<NetworkBehaviour>();
                networkBehaviour.Node = linkedList.AddLast(networkBehaviour);
                _cachedNetworkBehaviours.Add(networkIdentity, linkedList);
            }
        }

        public void RemoveNetworkBehaviour(uint networkIdentity, NetworkBehaviour networkBehaviour)
        {
            if (_cachedNetworkBehaviours.TryGetValue(networkIdentity, out LinkedList<NetworkBehaviour> networkBehaviours))
            {
                networkBehaviours.Remove(networkBehaviour.Node);
                if (networkBehaviours.Count == 0)
                {
                    _cachedNetworkBehaviours.Remove(networkIdentity);
                }
            }
        }
    }
}