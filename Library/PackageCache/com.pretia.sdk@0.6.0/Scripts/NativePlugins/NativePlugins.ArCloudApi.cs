using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal partial class NativePlugins
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        public static IntPtr pretiaSdkCreateNativeApplication() { return IntPtr.Zero; }

        public static void pretiaSdkDestroyNativeApplication(IntPtr arcloudApi) {}

        public static bool pretiaSdkIsSlamApiEnabled(IntPtr arcloudApi) { return false; }

        public static IntPtr pretiaSdkGetSlamApi(IntPtr arcloudApi) { return IntPtr.Zero; }

        public static IntPtr pretiaSdkGetImageProcessingApi(IntPtr arcloudApi) { return IntPtr.Zero; }

#else

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkCreateNativeApplication();

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkDestroyNativeApplication(IntPtr arcloudApi);
 
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern bool pretiaSdkIsSlamApiEnabled(IntPtr arcloudApi);
       
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkGetSlamApi(IntPtr arcloudApi);
 
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkGetImageProcessingApi(IntPtr arcloudApi);

#endif
    }
}
