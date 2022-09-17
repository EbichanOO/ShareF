using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal partial class NativePlugins
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        public static void pretiaSdkDestroyMap(IntPtr mapPtr) { }

        public static IntPtr pretiaSdkMapGetName(IntPtr mapPtr) { return IntPtr.Zero; }
        
        public static IntPtr pretiaSdkMapGetTime(IntPtr mapPtr) { return IntPtr.Zero; }
        
        public static UInt64 pretiaSdkMapGetNumGpsData(IntPtr mapPtr) { return 0; }

        public static void pretiaSdkMapGetGpsData(
            IntPtr mapPtr, UInt64 index,
            ref double latitude,
            ref double longitude,
            ref double altitude,
            ref float bearing,
            ref UInt64 timestampNs,
            ref float accuracy,
            ref float verticalAccuracy,
            ref int numSats) {}
        
        public static IntPtr pretiaSdkMapGetVersion(IntPtr mapPtr) { return IntPtr.Zero; }
        
        public static IntPtr pretiaSdkMapGetData(IntPtr mapPtr, ref int size) { return IntPtr.Zero; }
        
        public static int pretiaSdkMapGetAnchorCount(IntPtr mapPtr) { return 0; }
        
        public static int pretiaSdkMapGetAllAnchors(IntPtr mapPtr, int maxNumAnchors, IntPtr ids, IntPtr poses) { return 0; }
        
#else

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkDestroyMap(IntPtr mapPtr);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkMapGetName(IntPtr mapPtr);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkMapGetTime(IntPtr mapPtr);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern UInt64 pretiaSdkMapGetNumGpsData(IntPtr mapPtr);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkMapGetGpsData(
            IntPtr mapPtr, UInt64 index,
            ref double latitude,
            ref double longitude,
            ref double altitude,
            ref float bearing,
            ref UInt64 timestampNs,
            ref float accuracy,
            ref float verticalAccuracy,
            ref int numSats);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkMapGetVersion(IntPtr mapPtr);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern IntPtr pretiaSdkMapGetData(IntPtr mapPtr, ref int size);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkMapGetAnchorCount(IntPtr mapPtr);
        
        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern int pretiaSdkMapGetAllAnchors(IntPtr mapPtr, int maxNumAnchors, IntPtr ids, IntPtr poses);

#endif

    }
}