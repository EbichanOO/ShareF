using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.ARFoundation;

namespace PretiaArCloud
{
    internal enum NativeStatusCode : int
    {
        SUCCESS = 0,

        SLAM_ERROR_API_NOT_ENABLED = 1,

        OPENVSLAM_ERROR_CONFIGURATION = 10,
        OPENVSLAM_ERROR_NOT_INITIALIZED = 11,
        OPENVSLAM_ERROR_LOAD_MAP = 12,
        OPENVSLAM_ERROR_SAVE_MAP = 13,
        OPENVSLAM_ERROR_TRACKING = 14,
        OPENVSLAM_ERROR_CREATING_ANCHOR = 15,

        IMAGE_PROC_ERROR_CONFIGURATION = 100,
        IMAGE_PROC_ERROR_NOT_INITIALIZED = 101,
        IMAGE_PROC_ERROR_FEAT_EXTRACTION = 102,

    }
 
    internal partial interface INativeApi
    {
        NativeStatusCode InitializeSlam(string vocabPath, string deviceName, float fx, float fy, float cx, float cy, int width, int height);
        NativeStatusCode LoadMap(string mapPath);
        NativeStatusCode LoadMap(byte[] mapData);
        NativeStatusCode StartRelocalization();
        NativeStatusCode RelocalizeFrame(IntPtr img, long timestampNano, ref int outFrameStatus, ref float[] outFramePose);
        NativeStatusCode RelocalizeFrameWithCameraIntrinsic(IntPtr img, long timestampNano, ref int outFrameStatus, ref float[] outFramePose, double fx, double fy, double cx, double cy);
        NativeStatusCode GetRelocalizationScore(ref float outScore);
        NativeStatusCode GetRelocalizationKeypoints(out float[] outKeypoints);
        NativeStatusCode ResetSlam();

        NativeStatusCode InitializeORBFeatureExtraction(string deviceName, float fx, float fy, float cx, float cy, int width, int height);

        NativeStatusCode GetYamlConfiguration(string appKey, out byte[] outConfig);

        NativeStatusCode ExtractAndPackORBFeatures(IntPtr img, out byte[] outFeatures);
    }

    internal partial class NativeApi : INativeApi
    {
        protected static NativeApi _nativeApi;

        protected IntPtr ArCloudApi;
        protected IntPtr SlamApi;
        protected IntPtr ImageProcApi;

        public NativeApi()
        {
            ArCloudApi = NativePlugins.pretiaSdkCreateNativeApplication();
            SlamApi = NativePlugins.pretiaSdkGetSlamApi(ArCloudApi);
            ImageProcApi = NativePlugins.pretiaSdkGetImageProcessingApi(ArCloudApi);
        }

        ~NativeApi()
        {
            NativePlugins.pretiaSdkDestroyNativeApplication(ArCloudApi);
        }

        public static NativeApi GetNativeApi()
        {
            if (_nativeApi == null)
            {
                _nativeApi = new NativeApi();
            }
            return _nativeApi;
        }

        public NativeStatusCode ExtractAndPackORBFeatures(IntPtr img, out byte[] outFeatures)
        {
            IntPtr featuresNative;
            ulong featuresSize = 0;
            var err = NativePlugins.pretiaSdkExtractAndPackORBFeatures(ImageProcApi, img, out featuresNative, ref featuresSize);
            outFeatures = new byte[featuresSize];
            Marshal.Copy(featuresNative, outFeatures, 0, (int)featuresSize);
            NativePlugins.pretiaSdkFreeUCharBuffer(featuresNative);
            return (NativeStatusCode)err;
        }

        public NativeStatusCode GetRelocalizationKeypoints(out float[] outKeypoints)
        {
            IntPtr keypointsNative;
            ulong keypointsSize = 0;
            var status = NativePlugins.pretiaSdkGetFrameTrackedKeypoints(SlamApi, ref keypointsSize, out keypointsNative);
            outKeypoints = new float[keypointsSize];
            Marshal.Copy(keypointsNative, outKeypoints, 0, (int)keypointsSize);
            NativePlugins.pretiaSdkFreeFloatBuffer(keypointsNative);
            return (NativeStatusCode)status;
        }

        public NativeStatusCode GetRelocalizationScore(ref float outScore)
        {
            return (NativeStatusCode)NativePlugins.pretiaSdkGetRelocalizationScore(SlamApi, ref outScore);
        }

        public NativeStatusCode GetYamlConfiguration(string appKey, out byte[] outConfig)
        {
            IntPtr configNative;
            ulong configSize = 0;
            var err = NativePlugins.pretiaSdkGetYamlConfiguration(ImageProcApi, appKey, out configNative, ref configSize);
            outConfig = new byte[configSize];
            Marshal.Copy(configNative, outConfig, 0, (int)configSize);
            NativePlugins.pretiaSdkFreeUCharBuffer(configNative);
            return (NativeStatusCode)err;
        }

        public NativeStatusCode InitializeORBFeatureExtraction(string deviceName, float fx, float fy, float cx, float cy, int width, int height)
        {
            return (NativeStatusCode)NativePlugins.pretiaSdkInitializeORBFeatureExtraction(ImageProcApi, deviceName, width, height, cx, cy, fx, fy);
        }

        public NativeStatusCode InitializeSlam(string vocabPath, string deviceName, float fx, float fy, float cx, float cy, int width, int height)
        {
            return (NativeStatusCode)NativePlugins.pretiaSdkInitializeSlam(SlamApi, deviceName, vocabPath, width, height, cx, cy, fx, fy);
        }

        public NativeStatusCode LoadMap(string mapPath)
        {
            return (NativeStatusCode)NativePlugins.pretiaSdkLoadMapFromFile(SlamApi, mapPath);
        }

        public NativeStatusCode LoadMap(byte[] mapData)
        {
            GCHandle handle = GCHandle.Alloc(mapData, GCHandleType.Pinned);
            var status = NativePlugins.pretiaSdkLoadMap(SlamApi, handle.AddrOfPinnedObject(), mapData.Length);
            handle.Free();
            return (NativeStatusCode) status;
        }

        public NativeStatusCode RelocalizeFrame(IntPtr img, long timestampNano, ref int outFrameStatus, ref float[] outFramePose)
        {
            GCHandle handle = GCHandle.Alloc(outFramePose, GCHandleType.Pinned);
            var err = NativePlugins.pretiaSdkTrackFrame(SlamApi, img, timestampNano, ref outFrameStatus, handle.AddrOfPinnedObject(), IntPtr.Zero);
            handle.Free();
            return (NativeStatusCode)err;
        }

        public NativeStatusCode RelocalizeFrameWithCameraIntrinsic(IntPtr img, long timestampNano, ref int outFrameStatus, ref float[] outFramePose, double fx, double fy, double cx, double cy)
        {
            GCHandle handle = GCHandle.Alloc(outFramePose, GCHandleType.Pinned);
            var err = NativePlugins.pretiaSdkTrackFrameWithCameraIntrinsic(SlamApi, img, timestampNano, ref outFrameStatus, handle.AddrOfPinnedObject(), IntPtr.Zero, fx, fy, cx, cy);
            handle.Free();
            return (NativeStatusCode)err;
        }

        public NativeStatusCode ResetSlam()
        {
            return (NativeStatusCode)NativePlugins.pretiaSdkResetSlam(SlamApi);
        }

        public NativeStatusCode StartRelocalization()
        {
            return (NativeStatusCode)NativePlugins.pretiaSdkStartRelocalization(SlamApi);
        }
    }
}