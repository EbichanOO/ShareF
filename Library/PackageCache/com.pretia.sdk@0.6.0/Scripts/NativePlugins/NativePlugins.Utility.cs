using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal partial class NativePlugins
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        public static void pretiaSdkFreeFloatBuffer(IntPtr buffer){}

        public static void pretiaSdkFreeCharBuffer(IntPtr buffer){}

        public static void pretiaSdkFreeUCharBuffer(IntPtr buffer){}

        public static void pretiaSdkFreeUIntBuffer(IntPtr buffer){}

#else

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkFreeFloatBuffer(IntPtr buffer);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkFreeCharBuffer(IntPtr buffer);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkFreeUCharBuffer(IntPtr buffer);

        [DllImport(ApiConstants.PRETIA_SDK_NATIVE_LIB)]
        public static extern void pretiaSdkFreeUIntBuffer(IntPtr buffer);

#endif
    }
}