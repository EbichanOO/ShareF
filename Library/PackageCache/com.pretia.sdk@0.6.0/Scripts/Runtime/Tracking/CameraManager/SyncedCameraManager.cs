using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections.LowLevel.Unsafe;

namespace PretiaArCloud
{
    internal class SyncedCameraManager
    {
        internal struct CameraIntrinsics
        {
            public Vector2 FocalLength;
            public Vector2 PrincipalPoint;
            public Vector2Int Resolution;
        }

        private XRCameraSubsystem _cameraSubsystem;
        private ARPoseDriver _arPoseDriver;
        private int _targetResLong;
        private int _targetResShort;
        private XRCpuImage _cpuImage = default;
        private NativeArray<byte> _imageBuffer;
        private CameraIntrinsics? _latestIntrinsics;
        private Pose? _latestCameraPose;
        private double? _latestTimestamp;
        private ScreenOrientation? _latestScreenOrientation;

        /// <summary>
        /// This camera manager fetches and stores camera data every frame so that all data are synchronized.
        /// The data are fetched during Application.onBeforeRender, because that is when the camera pose is updated.
        /// As a consequence, if data are queried from this manager during an Update cycle, the data will be from the previous frame.
        /// </summary>
        public SyncedCameraManager(
            XRCameraSubsystem cameraSubsystem,
            int targetResLongSide = 640, int targetResShortSide = 480)
        {
            _cameraSubsystem = cameraSubsystem;
            _targetResLong = targetResLongSide;
            _targetResShort = targetResShortSide;
        }

        ~SyncedCameraManager()
        {
            Clear();
        }

        public void SetARPoseDriver(ARPoseDriver arPoseDriver)
        {
            _arPoseDriver = arPoseDriver;
        }

        public void Enable()
        {
            if (_arPoseDriver == null)
            {
                _arPoseDriver = (new GameObject()).AddComponent<ARPoseDriver>();
            }
            _arPoseDriver.gameObject.SetActive(true);
            _arPoseDriver.enabled = true;
            Application.onBeforeRender -= AcquireSyncedData;
            Application.onBeforeRender += AcquireSyncedData;
        }

        /// Note: The returned pointer and the corresponding data are only valid
        ///       until the next call to TryGetCameraImage() or Clear().
        public bool TryGetCameraImage(out IntPtr imageData)
        {
            imageData = IntPtr.Zero;

            if (!_cpuImage.valid)
            {
                return false;
            }

            if (!_imageBuffer.IsCreated)
            {
                _imageBuffer = new NativeArray<byte>(_targetResLong * _targetResShort, Allocator.Persistent);
            }

            var cameraConfiguration = _cameraSubsystem.currentConfiguration.Value;
            var conversionParams = new XRCpuImage.ConversionParams(_cpuImage, TextureFormat.R8);
            var targetResolution = GetTargetResolution(cameraConfiguration.width, cameraConfiguration.height);
            conversionParams.outputDimensions = targetResolution;

            if (_cpuImage.GetConvertedDataSize(targetResolution, TextureFormat.R8) != _imageBuffer.Length)
            {
                throw new Exception("Camera image conversion error: wrong buffer size");
            }

            _cpuImage.Convert(conversionParams, _imageBuffer);

            unsafe
            {
                imageData = new IntPtr(NativeArrayUnsafeUtility.GetUnsafePtr(_imageBuffer));
            }

            return true;
        }

        public bool TryGetCameraIntrinsics(out CameraIntrinsics intrinsics)
        {
            intrinsics = _latestIntrinsics.HasValue ? _latestIntrinsics.Value : default;
            return _latestIntrinsics.HasValue;
        }

        public bool TryGetCameraPose(out Pose pose, out ScreenOrientation orientation)
        {
            pose = _latestCameraPose.HasValue ? _latestCameraPose.Value : default;
            orientation = _latestScreenOrientation.HasValue ? _latestScreenOrientation.Value : default;
            return _latestCameraPose.HasValue && _latestScreenOrientation.HasValue;
        }

        public bool TryGetCameraPoseUnsynced(out Pose pose)
        {
            pose = default;
            if (_arPoseDriver == null) { return false; }
            pose = new Pose(_arPoseDriver.transform.localPosition, _arPoseDriver.transform.localRotation);
            return true;
        }

        public bool TryGetCameraTimestamp(out double timestamp)
        {
            timestamp = _latestTimestamp.HasValue ? _latestTimestamp.Value : default;
            return _latestTimestamp.HasValue;
        }

        public void Disable()
        {
            Application.onBeforeRender -= AcquireSyncedData;
            ClearSyncedData();
        }

        public void Clear()
        {
            ClearSyncedData();
            if (_imageBuffer.IsCreated)
            {
                _imageBuffer.Dispose();
            }
        }

        private Vector2Int GetTargetResolution(int width, int height)
        {
            if (width > height)
            {
                return new Vector2Int(_targetResLong, _targetResShort);
            }
            else
            {
                return new Vector2Int(_targetResShort, _targetResLong);
            }
        }

        private void AcquireIntrinsics()
        {
            _latestIntrinsics = null;

            // Immediately returns false if it failed to get intrinsics
            // or the camera configuration is not yet populated
            bool configurationOk = _cameraSubsystem.currentConfiguration.HasValue;
            bool intrinsicsOk = _cameraSubsystem.TryGetIntrinsics(out var intrinsics);
            if (configurationOk && intrinsicsOk)
            {
                // Calculate the scaled resulution
                var cameraConfiguration = _cameraSubsystem.currentConfiguration.Value;
                var scaledResolution = GetTargetResolution(cameraConfiguration.width, cameraConfiguration.height);
                var scale = new Vector2(
                    (float)scaledResolution[0] / (float)intrinsics.resolution[0],
                    (float)scaledResolution[1] / (float)intrinsics.resolution[1]);

                // Calculate the outputs with the scale
                _latestIntrinsics = new CameraIntrinsics
                {
                    FocalLength = intrinsics.focalLength * scale,
                    PrincipalPoint = intrinsics.principalPoint * scale,
                    Resolution = scaledResolution
                };
            }
        }

        private void AcquireCameraImage()
        {
            _latestTimestamp = null;
            if (_cpuImage.valid)
            {
                _cpuImage.Dispose();
                _cpuImage = default;
            }

            if (_cameraSubsystem.TryAcquireLatestCpuImage(out _cpuImage))
            {
                _latestTimestamp = _cpuImage.timestamp;
            }
        }

        private void AcquireCameraPose()
        {
            _latestCameraPose = new Pose(_arPoseDriver.transform.localPosition, _arPoseDriver.transform.localRotation);
            _latestScreenOrientation = Screen.orientation;
        }

        private void AcquireSyncedData()
        {
            ClearSyncedData();
            AcquireIntrinsics();
            AcquireCameraImage();
            AcquireCameraPose();
        }

        private void ClearSyncedData()
        {
            _latestIntrinsics = null;
            _latestTimestamp = null;
            _latestCameraPose = null;
            _latestScreenOrientation = null;
            if (_cpuImage.valid)
            {
                _cpuImage.Dispose();
                _cpuImage = default;
            }
        }
    }
}
