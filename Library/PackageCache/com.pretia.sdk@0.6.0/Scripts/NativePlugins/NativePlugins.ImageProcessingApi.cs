using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal partial class NativePlugins
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        public static int pretiaSdkInitializeORBFeatureExtraction(
            IntPtr imageApi,
            string deviceName,
            int cols, int rows,
            double cx, double cy, double fx, double fy) { return 0; }

        public static int pretiaSdkGetYamlConfiguration(
            IntPtr imageApi,
            string appKey,
            out IntPtr outConfig,
            ref UInt64 outConfigSize) { outConfig = IntPtr.Zero; return 0; }
        
        public static int pretiaSdkExtractAndPackORBFeatures(
            IntPtr imageApi,
            IntPtr img,
            out IntPtr outFeatures,
            ref UInt64 outFeaturesSize) { outFeatures = IntPtr.Zero; return 0; }

#else

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkInitializeORBFeatureExtraction(
            IntPtr imageApi,
            string deviceName,
            int cols, int rows,
            double cx, double cy, double fx, double fy);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkGetYamlConfiguration(
            IntPtr imageApi,
            string appKey,
            out IntPtr outConfig,
            ref UInt64 outConfigSize);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkExtractAndPackORBFeatures(
            IntPtr imageApi,
            IntPtr img,
            out IntPtr outFeatures,
            ref UInt64 outFeaturesSize);

#endif
    }
}