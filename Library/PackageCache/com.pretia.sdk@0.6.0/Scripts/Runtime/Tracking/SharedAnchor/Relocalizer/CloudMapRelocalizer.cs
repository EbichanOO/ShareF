using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using PretiaArCloud.Networking;
using UnityEngine;

namespace PretiaArCloud
{
    using static NetworkRelocalization;
    using static SharedAnchorSubsystem;

    internal class CloudMapRelocalizer : IRelocalizer
    {
        private readonly string _relocServerAddress;
        private readonly int _relocServerPort;
        private readonly string _appKey;
        private readonly SyncedCameraManager _syncedCameraManager;
        private readonly INativeApi _nativeApi;

        private string _mapKey;
        private TcpClient _tcpClient;
        private NetworkRelocalizationClient _relocClient;

        public CloudMapRelocalizer(
            string relocServerAddress,
            int relocServerPort,
            string appKey,
            SyncedCameraManager syncedCameraManager,
            INativeApi nativeApi)
        {
            _appKey = appKey;
            _relocServerAddress = relocServerAddress;
            _relocServerPort = relocServerPort;
            _syncedCameraManager = syncedCameraManager;
            _nativeApi = nativeApi;

            if (string.IsNullOrEmpty(_appKey))
            {
                throw new Exception("App Key is not set. Please set the app key in order to use network relocalization");
            }
        }

        public void SetMapKey(string mapKey)
        {
            _mapKey = mapKey;
        }

        public string GetMapKey() => _mapKey;

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_mapKey))
            {
                throw new Exception("Map key not set");
            }
            await InitializeORBFeatureExtractionAsync(cancellationToken);
            bool connected = await ConnectRelocServerAsync(cancellationToken);
            if (!connected)
            {
                throw new Exception("Unable to connect to server");
            }
            await InitializeNetworkSlamAsync(cancellationToken);
        }

        public async Task<(RelocState, RelocResult)> RelocalizeAsync(CancellationToken cancellationToken = default)
        {
            // Get camera image
            if (!_syncedCameraManager.TryGetCameraImage(out var imageData))
            {
                return (RelocState.Initializing, default(RelocResult));
            }

#if UNITY_IOS
            // Get intrinsics for iOS only
            if (!_syncedCameraManager.TryGetCameraIntrinsics(out var intrinsics))
            {
                return (RelocState.Initializing, default(RelocResult));
            }

            var intrinsicsData = new double[]
            {
                intrinsics.FocalLength.x,
                intrinsics.FocalLength.y,
                intrinsics.PrincipalPoint.x,
                intrinsics.PrincipalPoint.y,
            };

            using (var timeout =  new CancellationTokenSource(TimeSpan.FromSeconds(60)))
            using (var linkedCts =  CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken))
            {
                await _relocClient.UpdateIntrinsicsAsync(intrinsicsData, linkedCts.Token);
            }
#endif // UNITY_IOS

            // Run everything in a different thread to avoid creating additional latency
            // (extracting features is a little slow, and we want to send the data to the server immediately when ready)
            return await Task.Run(async () => {
                
                // Extract features from frame data
                var status = _nativeApi.ExtractAndPackORBFeatures(imageData, out byte[] featureData);
                if (status != NativeStatusCode.SUCCESS)
                {
                    throw new Exception($"Native frame processing failed with {status}");
                }

                // Send features to server for relocalization
                RelocalizeResponse relocResponse;
                using (var timeout =  new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                using (var linkedCts =  CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken))
                {
                    relocResponse = await _relocClient.RelocalizeAsync(featureData, linkedCts.Token);
                }

                RelocState relocStatus = relocResponse.RelocState;
                RelocResult relocResult = default;

                if (status == NativeStatusCode.SUCCESS && relocStatus == RelocState.Tracking)
                {
                    relocResult.Pose = relocResponse.CameraPose;
                }

                // Retrieve relocalization score and keypoints
                using (var timeout =  new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                using (var linkedCts =  CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken))
                {
                    var response = await _relocClient.GetRelocalizationDataAsync(linkedCts.Token);
                    relocResult.Score = response.Score;
                    relocResult.Keypoints = response.Keypoints;
                }

                return (relocStatus, relocResult);
            });
        }

        private async Task InitializeORBFeatureExtractionAsync(CancellationToken cancellationToken = default)
        {
            SyncedCameraManager.CameraIntrinsics intrinsics;
            while (!_syncedCameraManager.TryGetCameraIntrinsics(out intrinsics))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            var status = _nativeApi.InitializeORBFeatureExtraction(
                deviceName: Utility.GetDeviceName(),
                fx: intrinsics.FocalLength.x, fy: intrinsics.FocalLength.y,
                cx: intrinsics.PrincipalPoint.x, cy: intrinsics.PrincipalPoint.y,
                width: intrinsics.Resolution.x, height: intrinsics.Resolution.y);
            
            if (status != NativeStatusCode.SUCCESS)
            {
                throw new Exception($"Native initialization of network map failed with {status}");
            }
        }

        private async Task<bool> ConnectRelocServerAsync(CancellationToken cancellationToken = default)
        {
            var cancelSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => cancelSource.TrySetResult(true)))
            {
                _tcpClient = new TcpClient();
                var connectTask = _tcpClient.ConnectAsync(_relocServerAddress, _relocServerPort);
                var task = await Task.WhenAny(cancelSource.Task, connectTask);

                if (task == connectTask && _tcpClient.Connected)
                {
                    var packetSource = new RelocPacketSource(_tcpClient.GetStream());
                    _relocClient = new NetworkRelocalizationClient(packetSource);
                    return true;
                }
                else
                {
                    _tcpClient.Close();
                    return false;
                }
            }
        }

        private async Task InitializeNetworkSlamAsync(CancellationToken cancellationToken = default)
        {
            var status = _nativeApi.GetYamlConfiguration(_appKey, out byte[] config);
            if (status != NativeStatusCode.SUCCESS)
            {
                throw new Exception($"Failed to retrieve native configuration: {status}");
            }

            using (var timeout =  new CancellationTokenSource(TimeSpan.FromSeconds(60)))
            using (var linkedCts =  CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken))
            {
                var response = await _relocClient.InitializeAsync(_mapKey, config, linkedCts.Token);
                if (response.ErrorCode != 0)
                {
                    throw new Exception("Unable to initialize network relocalization");
                }
            }
        }

        public void Cleanup()
        {
            if (_relocClient != null)
            {
                _relocClient.Terminate();
            }
        }

        public void Reset()
        {
            _mapKey = null;
        }

        public void Dispose()
        {
            _mapKey = null;
            if (_tcpClient != null)
            {
                if (_tcpClient.Connected)
                    _tcpClient.Close();
            }
        }
    }
}