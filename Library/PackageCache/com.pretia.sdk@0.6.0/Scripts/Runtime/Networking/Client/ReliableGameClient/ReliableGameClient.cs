using System;
using System.Threading;

namespace PretiaArCloud.Networking
{
    public class ReliableGameClient : IDisposable
    {
        internal enum ProtocolId
        {
            Connection = 0,
            HostMessage = 1,
            PlayerMessage = 2,
            Reconnection = 3,
            HostAppointment = 4,
            PlayerToHostMessage = 5,
            HostToPlayerMessage = 6,
            Disconnect = 7,
            Error = 8,
        }

        private IPacketSource _packetSource;
        private IMainThreadExecutor _mainThreadExecutor;
        private GamePacketProcessor _processor;

        private ArrayBufferWriter<byte> _hostMsgBuffer;
        private ArrayBufferWriter<byte> _playerMsgBuffer;
        private ArrayBufferWriter<byte> _playerToHostMsgBuffer;
        private ArrayBufferWriter<byte> _hostToPlayerMsgBuffer;

        internal Thread ReceiverThread { get; private set; }

        internal delegate void HostMsgReceived(ReadOnlyMemory<byte> packet);
        internal delegate void PlayerMsgReceived(ReadOnlyMemory<byte> packet, uint userNumber);

        internal Action<uint, string> OnSessionConnected = null;
        internal HostMsgReceived OnHostMessage = null;
        internal PlayerMsgReceived OnPlayerMessage = null;
        internal Action OnReconnected = null;
        internal Action OnHostAppointment = null;
        internal PlayerMsgReceived OnPlayerToHostMessage = null;
        internal HostMsgReceived OnHostToPlayerMessage = null;
        internal Action<uint> OnSessionDisconnected = null;
        internal Action<NetworkStatusCode, string> OnError = null;

        public ReliableGameClient(IPacketSource packetSource, IMainThreadExecutor mainThreadExecutor = default)
        {
            _processor = new GamePacketProcessor();
            _packetSource = packetSource;
            _mainThreadExecutor = mainThreadExecutor;

            _hostMsgBuffer = new ArrayBufferWriter<byte>();
            _processor.ResetArrayBuffer(_hostMsgBuffer, (ushort)ProtocolId.HostMessage);

            _playerMsgBuffer = new ArrayBufferWriter<byte>();
            _processor.ResetArrayBuffer(_playerMsgBuffer, (ushort)ProtocolId.PlayerMessage);

            _playerToHostMsgBuffer = new ArrayBufferWriter<byte>();
            _processor.ResetArrayBuffer(_playerToHostMsgBuffer, (ushort)ProtocolId.PlayerToHostMessage);

            _hostToPlayerMsgBuffer = new ArrayBufferWriter<byte>();
            _processor.ResetArrayBuffer(_hostToPlayerMsgBuffer, (ushort)ProtocolId.HostToPlayerMessage);
        }

        internal void StartReceiver()
        {
            if (_mainThreadExecutor == null)
            {
                ReceiverThread = new PacketReceiver(_packetSource, ProcessPacket).Start();
            }
            else
            {
                ReceiverThread = new PacketReceiver(_packetSource, packet =>
                    _mainThreadExecutor.Execute(() => ProcessPacket(packet))
                ).Start();
            }
        }

        private void ProcessPacket(byte[] packet)
        {
            var bodyMemory = packet.AsMemory().Slice(2);
            var bodySpan = bodyMemory.Span;
            switch ((ProtocolId)SpanBasedBitConverter.ToUInt16(packet))
            {
                case ProtocolId.Connection:
                    if (OnSessionConnected != null)
                    {
                        _processor.ParseConnection(bodySpan, out uint userNumber, out string displayName);
                        OnSessionConnected.Invoke(userNumber, displayName);
                    }
                    break;

                case ProtocolId.HostMessage:
                    if (OnHostMessage != null) OnHostMessage.Invoke(bodyMemory);
                    break;

                case ProtocolId.PlayerMessage:
                    if (OnPlayerMessage != null)
                    {
                        _processor.ParsePlayerMessage(bodyMemory, out ReadOnlyMemory<byte> msg, out uint userNumber);
                        OnPlayerMessage.Invoke(msg, userNumber);
                    }
                    break;

                case ProtocolId.Reconnection:
                    if (OnReconnected != null) OnReconnected.Invoke();
                    break;

                case ProtocolId.HostAppointment:
                    if (OnHostAppointment != null) OnHostAppointment.Invoke();
                    break;

                case ProtocolId.PlayerToHostMessage:
                    if (OnPlayerToHostMessage != null)
                    {
                        _processor.ParsePlayerMessage(bodyMemory, out ReadOnlyMemory<byte> msg, out uint userNumber);
                        OnPlayerToHostMessage.Invoke(msg, userNumber);
                    }
                    break;

                case ProtocolId.HostToPlayerMessage:
                    if (OnHostToPlayerMessage != null) OnHostToPlayerMessage.Invoke(bodyMemory);
                    break;

                case ProtocolId.Disconnect:
                    if (OnSessionDisconnected != null)
                    {
                        _processor.ParseDisconnect(bodySpan, out uint userNumber);
                        OnSessionDisconnected.Invoke(userNumber);
                    }
                    break;

                case ProtocolId.Error:
                    if (OnError != null)
                    {
                        _processor.ParseError(bodySpan, out NetworkStatusCode errorCode, out string errorMessage);
                        OnError.Invoke(errorCode, errorMessage);
                    }
                    break;
            }
        }

        internal void SendConnection(byte[] token)
        {
            byte[] packet = _processor.BuildPacket(token, (ushort)ProtocolId.Connection);
            _packetSource.Send(packet);
        }

        internal void EnqueueHostMessage(byte[] body, ushort msgType, ushort tick)
        {
            _processor.BuildAndWriteMessagePacket(_hostMsgBuffer, body, msgType, tick);
        }

        internal void SendHostMessage(ushort tick)
        {
            if (_hostMsgBuffer.Buffer[8] > 0)
            {
                _packetSource.Send(_hostMsgBuffer.WrittenSpan);
                _processor.ResetArrayBuffer(_hostMsgBuffer, (ushort)ProtocolId.HostMessage);
            }
        }

        internal void SendHostMessage(byte[] body, ushort msgType, ushort tick)
        {
            byte[] packet = _processor.BuildMessagePacket(body, (ushort)ProtocolId.HostMessage, msgType, tick);
            _packetSource.Send(packet);
        }

        internal void WriteHeader(Span<byte> destination, ushort packetLength, ushort protocolId, ushort msgType, ushort tick)
        {
            _processor.BuildMessageHeader(destination, packetLength, protocolId, msgType, tick);
        }

        internal void SendBody(ReadOnlySpan<byte> span)
        {
            _packetSource.Send(span);
        }

        internal void EnqueuePlayerMessage(byte[] body, ushort msgType, ushort tick)
        {
            _processor.BuildAndWriteMessagePacket(_playerMsgBuffer, body, msgType, tick);
        }

        internal void SendPlayerMessage(ushort tick)
        {
            if (_playerMsgBuffer.Buffer[8] > 0)
            {
                _packetSource.Send(_playerMsgBuffer.WrittenSpan);
                _processor.ResetArrayBuffer(_playerMsgBuffer, (ushort)ProtocolId.PlayerMessage);
            }
        }

        internal void SendPlayerMessage(byte[] body, ushort msgType, ushort tick)
        {
            byte[] packet = _processor.BuildMessagePacket(body, (ushort)ProtocolId.PlayerMessage, msgType, tick);
            _packetSource.Send(packet);
        }

        internal void SendReconnection(byte[] token)
        {
            byte[] packet = _processor.BuildPacket(token, (ushort)ProtocolId.Reconnection);
            _packetSource.Send(packet);
        }

        internal void EnqueuePlayerToHostMessage(byte[] body, ushort msgType, ushort tick)
        {
            _processor.BuildAndWriteMessagePacket(_playerToHostMsgBuffer, body, msgType, tick);
        }

        internal void SendPlayerToHostMessage(byte[] body, ushort msgType, ushort tick)
        {
            byte[] packet = _processor.BuildMessagePacket(body, (ushort)ProtocolId.PlayerToHostMessage, msgType, tick);
            _packetSource.Send(packet);
        }

        internal void SendPlayerToHostMessage(ushort tick)
        {
            _packetSource.Send(_playerToHostMsgBuffer.WrittenSpan);
            _processor.ResetArrayBuffer(_playerToHostMsgBuffer, (ushort)ProtocolId.PlayerToHostMessage);
        }

        internal void EnqueueHostToPlayerMessage(byte[] body, ushort msgType, ushort tick)
        {
            _processor.BuildAndWriteMessagePacket(_hostToPlayerMsgBuffer, body, msgType, tick);
        }

        internal void SendHostToPlayerMessage(uint userNumber, ushort tick)
        {
            _processor.AppendUserNumber(_hostToPlayerMsgBuffer, userNumber);
            _packetSource.Send(_hostToPlayerMsgBuffer.WrittenSpan);
            _processor.ResetArrayBuffer(_hostToPlayerMsgBuffer, (ushort)ProtocolId.HostToPlayerMessage);
        }

        internal void SendHostToPlayerMessage(byte[] body, ushort msgType, uint userNumber, ushort tick)
        {
            byte[] packet = _processor.BuildHostToPlayerMessagePacket(body, (ushort)ProtocolId.HostToPlayerMessage, msgType, userNumber, tick);
            _packetSource.Send(packet);
        }

        internal void SendDisconnect()
        {
            byte[] packet = _processor.BuildDisconnectPacket((ushort)ProtocolId.Disconnect);
            _packetSource.Send(packet);
            _packetSource.Close();
        }

        public void Dispose()
        {
            _packetSource.Close();
        }
    }
}