namespace PretiaArCloud
{
    public class ApiConstants
    {
    #if UNITY_IOS
            internal const string PRETIA_SDK_NATIVE_LIB = "__Internal";
    #elif UNITY_ANDROID
            internal const string PRETIA_SDK_NATIVE_LIB = "arc_native";
    #endif
    }
}