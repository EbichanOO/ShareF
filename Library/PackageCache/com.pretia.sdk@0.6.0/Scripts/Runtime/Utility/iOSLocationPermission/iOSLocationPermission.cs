using System.Runtime.InteropServices;

namespace PretiaArCloud
{
    internal class IOSLocationPermission
    {
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern int getiOSLocationPermissionAuthorizationStatus();
#else
        private static int getiOSLocationPermissionAuthorizationStatus() => 0;
#endif

        public enum Status
        {
            NotDetermined = 0,
            AuthorizedAlways = 1,
            AuthorizedWhenInUse = 2,
            Restricted = 3,
            Denied = 4,
        }

        public static Status GetAuthorizationStatus()
        {
            return (Status)getiOSLocationPermissionAuthorizationStatus();
        }
    }
}
