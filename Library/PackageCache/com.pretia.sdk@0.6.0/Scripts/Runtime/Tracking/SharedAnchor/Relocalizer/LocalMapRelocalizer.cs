using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PretiaArCloud
{
    using static SharedAnchorSubsystem;

    internal class LocalMapRelocalizer : IRelocalizer
    {
        private readonly SyncedCameraManager _syncedCameraManager;
        private readonly INativeApi _nativeApi;

        private string _mapPath;
        private string _vocabPath;

        public LocalMapRelocalizer(
            SyncedCameraManager syncedCameraManager,
            INativeApi nativeApi)
        {
            _syncedCameraManager = syncedCameraManager;
            _nativeApi = nativeApi;

            SetMapPath();
            _vocabPath = Path.Combine(Application.streamingAssetsPath, "orb_vocab.dbow2").ToString();
        }

        public void SetMapPath(string mapPath = "")
        {
            if (string.IsNullOrEmpty(mapPath))
            {
                _mapPath = Path.Combine(Application.streamingAssetsPath, "map.msg").ToString();
            }
            else
            {
                _mapPath = mapPath;
            }
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
#if UNITY_ANDROID
            if (_vocabPath.StartsWith(Application.streamingAssetsPath))
            {
                var outputVocabPath = Path.Combine(Application.persistentDataPath, "orb_vocab.dbow2").ToString();
                await Utility.StreamingAssetsToFile(_vocabPath, outputVocabPath, cancellationToken);
                _vocabPath = outputVocabPath;
            }
            if (_mapPath.StartsWith(Application.streamingAssetsPath))
            {
                var outputMapPath = Path.Combine(Application.persistentDataPath, "map.msg").ToString();
                await Utility.StreamingAssetsToFile(_mapPath, outputMapPath, cancellationToken);
                _mapPath = outputMapPath;
            }
#endif

            if (!File.Exists(_vocabPath))
            {
                throw new ArgumentException($"Vocabulary file not found at {_vocabPath}");
            }

            if (!File.Exists(_mapPath))
            {
                throw new ArgumentException($"Map file not found at {_mapPath}");
            }

            await InitializeSlamAsync(cancellationToken);

            await Task<NativeStatusCode>.Run(() => 
            {
                var status = _nativeApi.LoadMap(_mapPath);

                if (status != NativeStatusCode.SUCCESS)
                {
                    throw new Exception($"Local map loading failed with {status}");
                }

                status = _nativeApi.StartRelocalization();

                if (status != NativeStatusCode.SUCCESS)
                {
                    throw new Exception($"Start local relocalization failed with {status}");
                }
            });
        }

        public async Task<(RelocState, RelocResult)> RelocalizeAsync(CancellationToken cancellationToken = default)
        {
            // Get camera image
            if (!_syncedCameraManager.TryGetCameraImage(out var imageData)
             || !_syncedCameraManager.TryGetCameraTimestamp(out var timestamp))
            {
                return (RelocState.Initializing, default(RelocResult));
            }

#if UNITY_IOS
            if (!_syncedCameraManager.TryGetCameraIntrinsics(out var intrinsics))
            {
                return (RelocState.Initializing, default(RelocResult));
            }
#endif

            long timestampNano = (long)(timestamp * 1e9);
            
            return await Task.Run(() => {
                int frameStatus = 0;
                float[] slamPose = new float[7];

#if UNITY_IOS
                // For iOS
                var status = _nativeApi.RelocalizeFrameWithCameraIntrinsic(imageData, timestampNano, ref frameStatus, ref slamPose, fx: intrinsics.FocalLength.x, fy: intrinsics.FocalLength.y, cx: intrinsics.PrincipalPoint.x, cy: intrinsics.PrincipalPoint.y);
#else
                // For Android
                var status = _nativeApi.RelocalizeFrame(imageData, timestampNano, ref frameStatus, ref slamPose);
#endif
                if (status != NativeStatusCode.SUCCESS)
                {
                    throw new Exception($"Frame relocalization failed with {status}");
                }

                RelocState relocStatus = (frameStatus == 0 ? RelocState.Tracking : RelocState.Lost);
                RelocResult relocResult = default;

                if (status == NativeStatusCode.SUCCESS && relocStatus == RelocState.Tracking)
                {
                    relocResult.Pose.rotation.x = slamPose[0];
                    relocResult.Pose.rotation.y = slamPose[1];
                    relocResult.Pose.rotation.z = slamPose[2];
                    relocResult.Pose.rotation.w = slamPose[3];
                    relocResult.Pose.position.x = slamPose[4];
                    relocResult.Pose.position.y = slamPose[5];
                    relocResult.Pose.position.z = slamPose[6];
                }

                status = _nativeApi.GetRelocalizationScore(ref relocResult.Score);
                if (status != NativeStatusCode.SUCCESS)
                {
                    Debug.LogError($"Unable to fetch relocalization score: {status}");
                }

                status = _nativeApi.GetRelocalizationKeypoints(out relocResult.Keypoints);
                if (status != NativeStatusCode.SUCCESS)
                {
                    Debug.LogError($"Unable to fetch relocalization keypoints: {status}");
                }

                return (relocStatus, relocResult);
            });
        }

        private async Task InitializeSlamAsync(CancellationToken cancellationToken = default)
        {
            SyncedCameraManager.CameraIntrinsics intrinsics;
            while (!_syncedCameraManager.TryGetCameraIntrinsics(out intrinsics))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            var deviceName = Utility.GetDeviceName();

            await Task.Run(() =>
            {
                var status = _nativeApi.InitializeSlam(
                    vocabPath: _vocabPath,
                    deviceName: deviceName,
                    fx: intrinsics.FocalLength.x, fy: intrinsics.FocalLength.y,
                    cx: intrinsics.PrincipalPoint.x, cy: intrinsics.PrincipalPoint.y,
                    width: intrinsics.Resolution.x, height: intrinsics.Resolution.y);

                if (status != NativeStatusCode.SUCCESS)
                {
                    throw new Exception($"Native initialization of local map failed with {status}");
                }
            });
        }

        public void Cleanup()
        {
        }

        public void Reset()
        {
        }

        public void Dispose()
        {
        }
    }
}