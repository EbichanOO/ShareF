using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace PretiaArCloud.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkBehaviour : MonoBehaviour, ISynchronizable
    {
        protected NetworkIdentity _networkIdentity = default;
        protected IGameSession _gameSession = default;
        protected ulong _dirtyMask = 0;

        protected enum SynchronizationTarget
        {
            All,
            HostOnly
        }

        protected bool isOwned => _gameSession != null && _gameSession.LocalPlayer == _networkIdentity.Owner;

        protected virtual bool ClientAuthority => false;
        protected virtual SynchronizationTarget SyncTarget => SynchronizationTarget.All;
        protected virtual bool NetSyncV2 => false;

        internal LinkedListNode<NetworkBehaviour> Node { get; set; }

        protected async virtual void Start()
        {
            _networkIdentity = GetComponent<NetworkIdentity>();

            if (_gameSession == null)
            {
                _gameSession = await NetworkManager.Instance.GetLatestSessionAsync();
                await _gameSession.WaitForConnectionAsync();
                if (ClientAuthority)
                {
                    if (SyncTarget == SynchronizationTarget.All)
                    {
                        _gameSession.PlayerSynchronizationManager.AddNetworkBehaviour(_networkIdentity.Value, this);
                    }
                    else
                    {
                        _gameSession.PlayerToHostSynchronizationManager.AddNetworkBehaviour(_networkIdentity.Value, this);
                    }
                }
                else
                {
                    _gameSession.HostSynchronizationManager.AddNetworkBehaviour(_networkIdentity.Value, this);
                }

                NetworkStart();
            }
        }

        protected virtual void NetworkStart() { }

        protected virtual void OnDisable()
        {
            if (_gameSession == null) return;

            if (ClientAuthority)
            {
                if (SyncTarget == SynchronizationTarget.All)
                {
                    _gameSession.PlayerSynchronizationManager.RemoveNetworkBehaviour(_networkIdentity.Value, this);
                }
                else
                {
                    _gameSession.PlayerToHostSynchronizationManager.RemoveNetworkBehaviour(_networkIdentity.Value, this);
                }
            }
            else
            {
                _gameSession.HostSynchronizationManager.RemoveNetworkBehaviour(_networkIdentity.Value, this);
            }
        }

        public bool TrySerialize(ref MessagePackWriter writer, ref MessagePackWriter maskWriter, MessagePackSerializerOptions options, ushort tick)
        {
            if (ClientAuthority)
            {
                if (!isOwned) return false;
                if (SyncTarget == SynchronizationTarget.HostOnly && _gameSession.LocalPlayer.IsHost && !NetSyncV2) return false;
            }

            if (NetSyncV2)
            {
                var networkVarWriter = new NetworkVariableWriter(ref writer, options);

                SyncUpdate(tick);
                SerializeNetworkVars(ref networkVarWriter);

                if (networkVarWriter.DirtyMask == 0)
                {
                    return false;
                }
                else
                {
                    maskWriter.Write(networkVarWriter.DirtyMask);
                    return true;
                }
            }
            else
            {
                _dirtyMask = 0;
                Serialize(ref writer, options);

                if (_dirtyMask == 0)
                {
                    return false;
                }
                else
                {
                    maskWriter.Write(_dirtyMask);
                    return true;
                }
            }
        }

        public long TryDeserialize(ref MessagePackReader reader, MessagePackSerializerOptions options, ushort tick)
        {
            if (NetSyncV2)
            {
                ulong dirtyMask = reader.ReadUInt64();
                if (dirtyMask == 0) return reader.Consumed;


                var networkVarReader = new NetworkVariableReader(ref reader, options, dirtyMask);

                DeserializeNetworkVars(ref networkVarReader);
                ApplySyncUpdate(tick);
                return networkVarReader.Consumed;
            }
            else
            {
                _dirtyMask = reader.ReadUInt64();
                if (_dirtyMask == 0) return reader.Consumed;

                Deserialize(ref reader, options);
                return reader.Consumed;
            }
        }

        public virtual void Serialize(ref MessagePackWriter writer, MessagePackSerializerOptions options) { }
        public virtual void Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) { }

        protected virtual void SyncUpdate(int tick) { }
        protected virtual void ApplySyncUpdate(int tick) { }
        protected virtual void SerializeNetworkVars(ref NetworkVariableWriter writer) { }
        protected virtual void DeserializeNetworkVars(ref NetworkVariableReader reader) { }
    }
}