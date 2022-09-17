using System;
using System.Net;

#if UNITY_EDITOR
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: InternalsVisibleTo("IntegrationTests")]
#endif

namespace PretiaArCloud.Networking
{
    internal class LobbyPacketProcessor
    {
        internal void ParseAuthResponse(ReadOnlySpan<byte> buffer, out byte[] token, out string displayName)
        {
            ushort tokenSize = SpanBasedBitConverter.ToUInt16(buffer);
            token = buffer.Slice(2, tokenSize).ToArray();
            displayName = StringEncoder.Instance.GetString(buffer.Slice(token.Length + 3));
        }

        internal void ParseResultCode(ReadOnlySpan<byte> buffer, out byte resultCode)
        {
            resultCode = buffer[0];
        }

        internal void ParseMatchResponse(ReadOnlySpan<byte> buffer, out IPEndPoint endpoint)
        {
            string content = StringEncoder.Instance.GetString(buffer.Slice(2));
            string[] splitContent = content.Split(':');

            string hostname = splitContent[0];
            int port = Int32.Parse(splitContent[1]);

            endpoint = new IPEndPoint(IPAddress.Parse(hostname), port);
        }

        internal void ParseDisconnectResponse(ReadOnlySpan<byte> buffer, out uint userNumber)
        {
            userNumber = SpanBasedBitConverter.ToUInt32(buffer);
        }

        internal void ParseLogoutResponse(ReadOnlySpan<byte> readOnlySpan, out byte result)
        {
            result = readOnlySpan[0];
        }

        internal byte[] BuildLoginPacket(string username, string password, string appKey)
        {
            if (username.Length > Constants.MAX_USERNAME_LENGTH)
            {
                throw new ArgumentOutOfRangeException("username", string.Format("username.Length({0}) is over the length limit: {1}", username.Length, Constants.MAX_USERNAME_LENGTH));
            }

            if (password.Length > Constants.MAX_PASSWORD_LENGTH)
            {
                throw new ArgumentOutOfRangeException("password", string.Format("password.Length({0}) is over the length limit: {1}", password.Length, Constants.MAX_PASSWORD_LENGTH));
            }

            if (appKey.Length > Constants.MAX_APPKEY_LENGTH)
            {
                throw new ArgumentOutOfRangeException("appKey", string.Format("appKey.Length({0}) is over the length limit: {1}", appKey.Length, Constants.MAX_APPKEY_LENGTH));
            }

            int usernameByteCount = StringEncoder.Instance.GetByteCount(username);
            int passwordByteCount = StringEncoder.Instance.GetByteCount(password);
            int appKeyByteCount = StringEncoder.Instance.GetByteCount(appKey);

            byte[] packet = new byte[7 + usernameByteCount + passwordByteCount + appKeyByteCount];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.Login);

            packet[4] = (byte)usernameByteCount;
            StringEncoder.Instance.GetBytes(username, 0, username.Length, packet, 5);

            packet[usernameByteCount + 5] = (byte)passwordByteCount;
            StringEncoder.Instance.GetBytes(password, 0, password.Length, packet, usernameByteCount + 6);

            packet[usernameByteCount + passwordByteCount + 6] = (byte)appKeyByteCount;
            StringEncoder.Instance.GetBytes(appKey, 0, appKey.Length, packet, usernameByteCount + passwordByteCount + 7);

            return packet;
        }

        internal byte[] BuildRegisterPacket(string username, string password, string displayName, string appKey)
        {
            if (username.Length > Constants.MAX_USERNAME_LENGTH)
            {
                throw new ArgumentOutOfRangeException("username", string.Format("username.Length({0}) is over the length limit: {1}", username.Length, Constants.MAX_USERNAME_LENGTH));
            }

            if (password.Length > Constants.MAX_PASSWORD_LENGTH)
            {
                throw new ArgumentOutOfRangeException("password", string.Format("password.Length({0}) is over the length limit: {1}", password.Length, Constants.MAX_PASSWORD_LENGTH));
            }

            if (displayName.Length > Constants.MAX_DISPLAYNAME_LENGTH)
            {
                throw new ArgumentOutOfRangeException("displayName", string.Format("displayName.Length({0}) is over the length limit: {1}", displayName.Length, Constants.MAX_DISPLAYNAME_LENGTH));
            }

            if (appKey.Length > Constants.MAX_APPKEY_LENGTH)
            {
                throw new ArgumentOutOfRangeException("appKey", string.Format("appKey.Length({0}) is over the length limit: {1}", appKey.Length, Constants.MAX_APPKEY_LENGTH));
            }

            byte[] packet = new byte[8 + username.Length + password.Length + displayName.Length + appKey.Length];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.Register);

            packet[4] = (byte)username.Length;
            StringEncoder.Instance.GetBytes(username, 0, username.Length, packet, 5);

            packet[username.Length + 5] = (byte)password.Length;
            StringEncoder.Instance.GetBytes(password, 0, password.Length, packet, username.Length + 6);

            packet[username.Length + password.Length + 6] = (byte)displayName.Length;
            StringEncoder.Instance.GetBytes(displayName, 0, displayName.Length, packet, username.Length + password.Length + 7);

            packet[username.Length + password.Length + displayName.Length + 7] = (byte)appKey.Length;
            StringEncoder.Instance.GetBytes(appKey, 0, appKey.Length, packet, username.Length + password.Length + displayName.Length + 8);

            return packet;
        }

        internal byte[] BuildSocialMediaLoginPacket(string username, byte socialMediaType, uint appId)
        {
            if (username.Length > Constants.MAX_USERNAME_LENGTH)
            {
                throw new ArgumentOutOfRangeException("username", string.Format("username.Length({0}) is over the length limit: {1}", username.Length, Constants.MAX_USERNAME_LENGTH));
            }

            int usernameByteCount = StringEncoder.Instance.GetByteCount(username);

            byte[] packet = new byte[10 + usernameByteCount];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.SocialMediaLogin);

            packet[4] = (byte)usernameByteCount;
            StringEncoder.Instance.GetBytes(username, 0, username.Length, packet, 5);

            packet[usernameByteCount + 5] = socialMediaType;

            SpanBasedBitConverter.TryWriteBytes(packet.AsSpan().Slice(packet.Length - 4), appId);

            return packet;
        }

        internal byte[] BuildAnonymousLoginPacket(string displayName, string appKey)
        {
            if (displayName.Length > Constants.MAX_DISPLAYNAME_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(displayName), string.Format("displayName.Length({0}) is over the length limit: {1}", displayName.Length, Constants.MAX_DISPLAYNAME_LENGTH));
            }

            if (displayName.Length > Constants.MAX_APPKEY_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(appKey), string.Format("appKey.Length({0}) is over the length limit: {1}", displayName.Length, Constants.MAX_APPKEY_LENGTH));
            }

            int appKeyByteCount = StringEncoder.Instance.GetByteCount(appKey);
            int displayNameByteCount = StringEncoder.Instance.GetByteCount(displayName);

            byte[] packet = new byte[6 +  appKeyByteCount + displayNameByteCount];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.AnonymousLogin);

            packet[4] = (byte)appKeyByteCount;
            StringEncoder.Instance.GetBytes(appKey, 0, appKey.Length, packet, 5);

            packet[appKeyByteCount + 5] = (byte)displayNameByteCount;
            StringEncoder.Instance.GetBytes(displayName, 0, displayName.Length, packet, appKeyByteCount + 6);

            return packet;
        }

        internal byte[] BuildReconnectionPacket(byte[] token)
        {
            byte[] packet = new byte[4 + token.Length];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.Reconnection);
            Array.Copy(token, 0, packet, 4, token.Length);

            return packet;
        }

        internal byte[] BuildRandomMatchRequestPacket()
        {
            byte[] packet = new byte[4];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.RandomMatchRequest);

            return packet;
        }

        internal byte[] BuildLogoutPacket()
        {
            byte[] packet = new byte[4];
            var packetSpan = packet.AsSpan();

            SpanBasedBitConverter.TryWriteBytes(packetSpan, (ushort)packet.Length);
            SpanBasedBitConverter.TryWriteBytes(packetSpan.Slice(2), (ushort)LobbyClient.ProtocolId.Logout);

            return packet;
        }
    }
}