using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal partial class NativePlugins
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        public static int pretiaSdkInitializeSlam(
            IntPtr slamApi,
            string deviceName,
            string vocabFilepath,
            int cols, int rows,
            double cx, double cy, double fx, double fy) { return 0; }
        
        public static int pretiaSdkLoadMapFromFile(IntPtr slamApi, string mapFilepath) { return 0; }

        public static int pretiaSdkLoadMap(IntPtr slamApi, IntPtr mapData, Int64 dataSize) { return 0; }

        public static int pretiaSdkStartRelocalization(IntPtr slamApi) { return 0; }

        public static int pretiaSdkStartMapping(IntPtr slamApi) { return 0; }

        public static int pretiaSdkTrackFrame(
            IntPtr slamApi,
            IntPtr img,
            Int64 timestampNano,
            ref int outFrameStatus,
            IntPtr outPoseCw,
            IntPtr odomPoseWc) { return 0; }

        public static int pretiaSdkTrackFrameWithCameraIntrinsic(
            IntPtr slamApi,
            IntPtr img,
            Int64 timestampNano,
            ref int outFrameStatus,
            IntPtr outPoseCw,
            IntPtr odomPoseWc,
            double fx, double fy, double cx, double cy) { return 0; }

        public static int pretiaSdkUpdateOdometryPoses(
            IntPtr slamApi, IntPtr ids, IntPtr rawPosesWc, UInt64 numKeyframes, out IntPtr deletedIds, ref UInt64 numDeletedIds, bool removeOtherKfOdom)
        {
            deletedIds = IntPtr.Zero;
            return 0;
        }

        public static int pretiaSdkGetRelocalizationScore(IntPtr slamApi, ref float outScore) { return 0; }

        public static int pretiaSdkGetFrameTrackedKeypoints(IntPtr slamApi, ref UInt64 pointsSize, out IntPtr points)
        {
            points = IntPtr.Zero;
            return 0;
        }

        public static int pretiaSdkGetFrameLandmarks(IntPtr slamApi, ref UInt64 pointsSize, out IntPtr points, bool unityCoords)
        {
            points = IntPtr.Zero;
            return 0;
        }

        public static int pretiaSdkCreateAnchorOnLandmark(IntPtr slamApi, float raycastImageX, float raycastImageY, ref UInt32 outId) { return 0; }
        
        public static int pretiaSdkGetMapAnchors(IntPtr slamApi, out IntPtr ids, out IntPtr poses, ref UInt64 numAnchors)
        {
            ids = IntPtr.Zero;
            poses = IntPtr.Zero;
            return 0;
        }

        public static int pretiaSdkRemoveAnchor(IntPtr slamApi, UInt32 id) { return 0; }

        public static int pretiaSdkLastFrameIsKeyframe(IntPtr slamApi, ref bool isKeyframe, ref UInt32 outKeyframeId) { return 0; }

        public static int pretiaSdkResetSlam(IntPtr slamApi) { return 0; }

        public static int pretiaSdkSetMappingEnabled(IntPtr slamApi, bool enabled) { return 0; }

        public static int pretiaSdkSetMapName(IntPtr slamApi, string name) { return 0; }

        public static int pretiaSdkSetLocationData(
            IntPtr slamApi,
            double latitude, double longitude, double altitude, float bearing,
            float accuracy, float verticalAccuracy,
            UInt64 timestampNano,
            string local_time) { return 0; }

        public static int pretiaSdkSaveMap(IntPtr slamApi, string mapFilepath) { return 0; }

        public static int pretiaSdkGetMap(IntPtr slamApi, out IntPtr map) { map = IntPtr.Zero; return 0; }

#else

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkInitializeSlam(
            IntPtr slamApi,
            string deviceName,
            string vocabFilepath,
            int cols, int rows,
            double cx, double cy, double fx, double fy);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkLoadMapFromFile(IntPtr slamApi, string mapFilepath);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkLoadMap(IntPtr slamApi, IntPtr mapData, Int64 dataSize);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkStartRelocalization(IntPtr slamApi);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkStartMapping(IntPtr slamApi);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkTrackFrame(
            IntPtr slamApi,
            IntPtr img,
            Int64 timestampNano,
            ref int outFrameStatus,
            IntPtr outPoseCw,
            IntPtr odomPoseWc);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkTrackFrameWithCameraIntrinsic(
            IntPtr slamApi,
            IntPtr img,
            Int64 timestampNano,
            ref int outFrameStatus,
            IntPtr outPoseCw,
            IntPtr odomPoseWc,
            double fx, double fy, double cx, double cy);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkUpdateOdometryPoses(
            IntPtr slamApi, IntPtr ids, IntPtr rawPosesWc, UInt64 numKeyframes, out IntPtr deletedIds, ref UInt64 numDeletedIds, bool removeOtherKfOdom);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkGetRelocalizationScore(IntPtr slamApi, ref float outScore);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkGetFrameTrackedKeypoints(IntPtr slamApi, ref UInt64 pointsSize, out IntPtr points);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkGetFrameLandmarks(IntPtr slamApi, ref UInt64 pointsSize, out IntPtr points, bool unityCoords);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkCreateAnchorOnLandmark(IntPtr slamApi, float raycastImageX, float raycastImageY, ref UInt32 outId);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkGetMapAnchors(IntPtr slamApi, out IntPtr ids, out IntPtr poses, ref UInt64 numAnchors);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkRemoveAnchor(IntPtr slamApi, UInt32 id);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkLastFrameIsKeyframe(IntPtr slamApi, ref bool isKeyframe, ref UInt32 outKeyframeId);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkResetSlam(IntPtr slamApi);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkSetMappingEnabled(IntPtr slamApi, bool enabled);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkSetMapName(IntPtr slamApi, string name);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkSetLocationData(
            IntPtr slamApi,
            double latitude, double longitude, double altitude, float bearing,
            float accuracy, float verticalAccuracy,
            UInt64 timestampNano,
            string local_time);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkSaveMap(IntPtr slamApi, string mapFilepath);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkGetMap(IntPtr slamApi, out IntPtr map);

#endif

    }
}
