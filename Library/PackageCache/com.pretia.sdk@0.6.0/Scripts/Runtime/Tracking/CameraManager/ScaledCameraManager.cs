using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace PretiaArCloud
{
    internal class ScaledCameraManager : IScaledCameraManager
    {
        private XRCameraSubsystem _cameraSubsystem;
        private Texture2D _latestTexture;

        public Vector2Int LatestResolution { get; private set; } = default;

        public ScaledCameraManager(XRCameraSubsystem cameraSubsystem)
        {
            _cameraSubsystem = cameraSubsystem;
        }

        private Vector2Int GetScaledResolution(int width, int height)
        {
            Vector2Int scaledResolution = default;

            if (width > height)
            {
                scaledResolution[0] = 640;
                scaledResolution[1] = 480;
            }
            else
            {
                scaledResolution[0] = 480;
                scaledResolution[1] = 640;
            }

            return scaledResolution;
        }

        public bool TryGetIntrinsics(
            out Vector2 focalLength,
            out Vector2 principalPoint,
            out Vector2Int resolution)
        {
            // Initialize outputs with default values
            focalLength = default;
            principalPoint = default;
            resolution = default;

            // Immediately returns false if it failed to get intrinsics
            // or the camera configuration is not yet populated
            bool configurationOk = _cameraSubsystem.currentConfiguration.HasValue;
            bool intrinsicsOk = _cameraSubsystem.TryGetIntrinsics(out var intrinsics);
            if (configurationOk && intrinsicsOk)
            {
                // Calculate the scaled resulution
                var cameraConfiguration = _cameraSubsystem.currentConfiguration.Value;
                var scaledResolution = GetScaledResolution(cameraConfiguration.width, cameraConfiguration.height);
                var scale = new Vector2(
                    (float)scaledResolution[0] / (float)intrinsics.resolution[0],
                    (float)scaledResolution[1] / (float)intrinsics.resolution[1]);

                // Calculate the outputs with the scale
                focalLength = intrinsics.focalLength * scale;
                principalPoint = intrinsics.principalPoint * scale;
                resolution = scaledResolution;

                LatestResolution = resolution;

                return true;
            }

            return false;
        }

        public unsafe bool TryGetLatestCameraTexture(out Texture2D latestTexture, out double timestamp)
        {
            // Initialize output with default value
            latestTexture = default;
            timestamp = 0.0;

            // Immediately returns false if it failed to get the latest cpu image
            // or the camera configuration is not yet populated
            bool configurationOk = _cameraSubsystem.currentConfiguration.HasValue;
            bool imageOk = _cameraSubsystem.TryAcquireLatestCpuImage(out XRCpuImage image);
            if (configurationOk && imageOk)
            {
                timestamp = image.timestamp;
                
                // Setup the conversion parameters
                var cameraConfiguration = _cameraSubsystem.currentConfiguration.Value;
                var conversionParams = new XRCpuImage.ConversionParams(image, TextureFormat.R8);
                var scaledResolution = GetScaledResolution(cameraConfiguration.width, cameraConfiguration.height);
                conversionParams.outputDimensions = scaledResolution;

                // Create a new texture if it is null
                if (_latestTexture == null)
                {
                    _latestTexture = new Texture2D(
                        width: scaledResolution[0],
                        height: scaledResolution[1],
                        textureFormat: TextureFormat.R8,
                        mipChain: false);
                }

                bool convertOk = true;

                // Use Texture2D to prevent allocating a new byte array everytime
                var rawTextureData = _latestTexture.GetRawTextureData<byte>();

                try
                {
                    image.Convert(
                        conversionParams,
                        new IntPtr(rawTextureData.GetUnsafePtr()),
                        rawTextureData.Length);
                }
                catch
                {
                    convertOk = false;
                }
                finally
                {
                    image.Dispose();
                }

                _latestTexture.Apply();
                latestTexture = _latestTexture;
                LatestResolution = scaledResolution;

                return convertOk;
            }

            return false;
        }

        public unsafe IntPtr GetTextureUnsafePtr(Texture2D tex)
        {
            var rawTextureData = tex.GetRawTextureData<byte>();
            return new IntPtr(rawTextureData.GetUnsafePtr());
        }
    }
}