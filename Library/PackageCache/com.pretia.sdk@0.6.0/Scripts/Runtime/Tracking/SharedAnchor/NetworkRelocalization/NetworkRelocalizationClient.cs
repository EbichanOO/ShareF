using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using PretiaArCloud.Networking;
using static PretiaArCloud.NetworkRelocalization;

namespace PretiaArCloud
{
    internal class NetworkRelocalizationClient : INetworkRelocalizationClient
    {
        private IPacketSource _packetSource;

        public NetworkRelocalizationClient(IPacketSource packetSource)
        {
            _packetSource = packetSource;
        }

        public async Task<InitializeResponse> InitializeAsync(string mapKey, byte[] config, CancellationToken cancellationToken = default)
        {
            WriteInitialize(mapKey, config);
            return await Task<InitializeResponse>.Run(() =>
            {
                byte[] packet = _packetSource.GetNextPacket();
                return Read<InitializeResponse>(Protocol.Initialize, packet);
            }, cancellationToken);
        }

        public async Task<RelocalizeResponse> RelocalizeAsync(byte[] frameData, CancellationToken cancellationToken = default)
        {
            Write(Protocol.Relocalize, frameData);
            return await Task<RelocalizeResponse>.Run(() =>
            {
                byte[] packet = _packetSource.GetNextPacket();
                return ReadRelocalizeResponse(packet);
            }, cancellationToken);
        }
        
        public async Task<IntrinsicsResponse> UpdateIntrinsicsAsync(double[] intrinsics, CancellationToken cancellationToken = default)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(intrinsics.Length * sizeof(Double));
            Buffer.BlockCopy(intrinsics, 0, buffer, 0, buffer.Length);
            Write(Protocol.UpdateIntrinsics, buffer);
            var response = await Task<IntrinsicsResponse>.Run(() =>
            {
                byte[] packet = _packetSource.GetNextPacket();
                return Read<IntrinsicsResponse>(Protocol.UpdateIntrinsics, packet);
            });
            ArrayPool<byte>.Shared.Return(buffer);
            return response;
        }

        public async Task<RelocalizationDataResponse> GetRelocalizationDataAsync(CancellationToken cancellationToken = default)
        {
            WriteHeader(Protocol.RelocalizationData);
            return await Task<RelocalizationDataResponse>.Run(() =>
            {
                byte[] packet = _packetSource.GetNextPacket();
                return ReadRelocalizationDataResponse(packet);
            }, cancellationToken);
        }

        public void Terminate()
        {
            try
            { 
                WriteHeader(Protocol.Terminate);
            }
            catch (System.Exception) {}
            _packetSource.Close();
        }

        private RelocalizationDataResponse ReadRelocalizationDataResponse(byte[] packet)
        {
            Protocol readProtocol = (Protocol)SpanBasedBitConverter.ToInt32(packet);
            if (readProtocol != Protocol.RelocalizationData)
            {
                throw new System.NotSupportedException();
            }

            var errorCodeSpan = packet.AsSpan().Slice(sizeof(Int32));
            var scoreSpan = errorCodeSpan.Slice(sizeof(Int32));
            var numKeypointsSpan = scoreSpan.Slice(sizeof(float));
            var keypointsSpan = numKeypointsSpan.Slice(sizeof(Int32));

            RelocalizationDataResponse response = new RelocalizationDataResponse();
            response.ErrorCode = SpanBasedBitConverter.ToInt32(errorCodeSpan);
            response.Score = SpanBasedBitConverter.ToFloat(scoreSpan);

            int numKeypoints = SpanBasedBitConverter.ToInt32(numKeypointsSpan);
            response.Keypoints = new float[numKeypoints];
            Buffer.BlockCopy(packet, sizeof(Int32) + sizeof(Int32) + sizeof(float) + sizeof(Int32), response.Keypoints, 0, numKeypoints * sizeof(float));

            return response;
        }

        private RelocalizeResponse ReadRelocalizeResponse(byte[] packet)
        {
            var protocolSpan = packet.AsSpan();
            Protocol readProtocol = (Protocol)SpanBasedBitConverter.ToInt32(protocolSpan);
            if (readProtocol != Protocol.Relocalize)
            {
                throw new System.NotSupportedException();
            }

            var errorCodeSpan = protocolSpan.Slice(sizeof(Int32));
            var relocStateSpan = errorCodeSpan.Slice(sizeof(Int32));
            var poseSpan = relocStateSpan.Slice(sizeof(Int32));

            RelocalizeResponse response = new RelocalizeResponse();
            response.ErrorCode = SpanBasedBitConverter.ToInt32(errorCodeSpan);
            response.RelocState = (RelocState)SpanBasedBitConverter.ToInt32(relocStateSpan);

            int numPose = 7;
            var pose = new float[numPose];
            Buffer.BlockCopy(packet, sizeof(Int32) + sizeof(Int32) + sizeof(Int32), pose, 0, numPose * sizeof(float));

            response.CameraPose = Utility.RawPoseToUnityPose(pose);

            return response;
        }

        private T Read<T>(Protocol protocol, ReadOnlySpan<byte> packet) where T : struct
        {
            Protocol readProtocol;
            try
            {
                readProtocol = (Protocol)SpanBasedBitConverter.ToInt32(packet);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new ArgumentOutOfRangeException("Unexpected network packet size", e);
            }
            var body = packet.Slice(sizeof(Int32));
            if (readProtocol == protocol)
            {
                return Utility.SpanToStruct<T>(body);
            }
            else
            {
                // TODO: handle unexpected messages
                throw new System.NotSupportedException();
            }
        }

        private void WriteHeader(Protocol protocol)
        {
            int headerSize = sizeof(Int32) + sizeof(Int32);
            byte[] headerBuffer = new byte[headerSize];

            var packetSizeSpan = headerBuffer.AsSpan();
            var protocolSpan = packetSizeSpan.Slice(sizeof(Int32));

            SpanBasedBitConverter.TryWriteBytes(packetSizeSpan, headerSize);
            SpanBasedBitConverter.TryWriteBytes(protocolSpan, (Int32)protocol);

            _packetSource.Send(headerBuffer);
        }

        private void WriteHeader(int bodySize, Protocol protocol)
        {
            int headerSize = sizeof(Int32) + sizeof(Int32);
            int packetSize = headerSize + bodySize;
            byte[] headerBuffer = new byte[headerSize];

            var packetSizeSpan = headerBuffer.AsSpan();
            var protocolSpan = packetSizeSpan.Slice(sizeof(Int32));

            SpanBasedBitConverter.TryWriteBytes(packetSizeSpan, packetSize);
            SpanBasedBitConverter.TryWriteBytes(protocolSpan, (Int32)protocol);

            _packetSource.Send(headerBuffer);
        }

        private void Write(Protocol protocol, byte[] body)
        {
            WriteHeader(body.Length, protocol);
            _packetSource.Send(body);
        }

        private void WriteInitialize(string mapKeyString, byte[] config)
        {
            if (!UInt64.TryParse(mapKeyString, out UInt64 mapKey))
            {
                throw new System.Exception($"mapKey cannot be parsed as UInt64");
            }

            int headerSize = sizeof(Int32) + sizeof(Int32) + sizeof(UInt64);
            int packetSize = headerSize + config.Length;
            byte[] headerBuffer = new byte[headerSize];

            var packetSizeSpan = headerBuffer.AsSpan();
            var protocolSpan = packetSizeSpan.Slice(sizeof(Int32));
            var mapKeySpan = protocolSpan.Slice(sizeof(Int32));

            SpanBasedBitConverter.TryWriteBytes(packetSizeSpan, packetSize);
            SpanBasedBitConverter.TryWriteBytes(protocolSpan, (Int32)Protocol.Initialize);
            SpanBasedBitConverter.TryWriteBytes(mapKeySpan, mapKey);

            _packetSource.Send(headerBuffer);
            _packetSource.Send(config);

        }
    }
}