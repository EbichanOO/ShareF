using System;
using System.Net;
using System.Threading;

namespace PretiaArCloud.Networking
{
    // using BitConverter = SpanBasedBitConverter;

    internal class LobbyClient : IDisposable
    {
        internal enum ProtocolId
        {
            Login = 0,
            SocialMediaLogin = 1,
            AnonymousLogin = 2,
            Register = 4,
            RandomMatchRequest = 6,
            CustomMatchRequest = 7,
            Reconnection = 8,
            Logout = 10,
        }

        private IPacketSource _packetSource;
        private LobbyPacketProcessor _processor;

        internal Action<byte[], string> OnAuth = null;
        internal Action<NetworkStatusCode> OnAuthError = null;
        internal Action<IPEndPoint> OnMatch = null;
        internal Action<uint> OnDisconnect = null;
        internal Action<byte> OnLogout = null;

        internal Thread ReceiverThread { get; private set; }

        internal LobbyClient(IPacketSource packetSource)
        {
            _processor = new LobbyPacketProcessor();
            _packetSource = packetSource;
        }

        internal void StartReceiver()
        {
            ReceiverThread = new PacketReceiver(_packetSource, ProcessPacket).Start();
        }

        private void ProcessPacket(byte[] packet)
        {
            ReadOnlySpan<byte> packetSpan = new ReadOnlySpan<byte>(packet);
            switch ((ProtocolId)SpanBasedBitConverter.ToUInt16(packetSpan))
            {
                case ProtocolId.Login:
                case ProtocolId.SocialMediaLogin:
                case ProtocolId.AnonymousLogin:
                case ProtocolId.Register:
                case ProtocolId.Reconnection:
                    _processor.ParseResultCode(packetSpan.Slice(2, 1), out byte resultCode);
                    if (resultCode == 0)
                    {
                        _processor.ParseAuthResponse(packetSpan.Slice(3), out byte[] token, out string displayName);
                        if (OnAuth != null) OnAuth.Invoke(token, displayName);
                    }
                    else
                    {
                        if (OnAuthError != null) OnAuthError.Invoke((NetworkStatusCode)resultCode);
                    }
                    break;
                case ProtocolId.RandomMatchRequest:
                    _processor.ParseMatchResponse(packetSpan.Slice(2), out IPEndPoint endpoint);
                    if (OnMatch != null) OnMatch.Invoke(endpoint);
                    break;
                case ProtocolId.Logout:
                    _processor.ParseLogoutResponse(packetSpan.Slice(2), out byte result);
                    if (OnLogout != null) OnLogout.Invoke(result);
                    break;
            }
        }

        internal void SendLogin(string username, string password, string appKey)
        {
            byte[] packet = _processor.BuildLoginPacket(username, password, appKey);
            _packetSource.Send(packet);
        }

        internal void SendRegister(string username, string password, string displayName, string appKey)
        {
            byte[] packet = _processor.BuildRegisterPacket(username, password, displayName, appKey);
            _packetSource.Send(packet);
        }

        internal void SendSocialMediaLogin(string username, byte socialMediaType, uint appId)
        {
            byte[] packet = _processor.BuildSocialMediaLoginPacket(username, socialMediaType, appId);
            _packetSource.Send(packet);
        }

        internal void SendAnonymousLogin(string displayName, string appKey)
        {
            byte[] packet = _processor.BuildAnonymousLoginPacket(displayName, appKey);
            _packetSource.Send(packet);
        }

        internal void SendRandomMatchRequest()
        {
            byte[] packet = _processor.BuildRandomMatchRequestPacket();
            _packetSource.Send(packet);
        }

        internal void SendReconnection(byte[] token)
        {
            byte[] packet = _processor.BuildReconnectionPacket(token);
            _packetSource.Send(packet);
        }

        internal void SendLogout()
        {
            byte[] packet = _processor.BuildLogoutPacket();
            _packetSource.Send(packet);
        }

        public void Dispose()
        {
            _packetSource.Close();
        }
    }
}