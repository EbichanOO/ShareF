using UnityEngine.XR.Management;

namespace PretiaArCloud
{
    public static class XRGeneralSettingsExtensions
    {
        public static bool TryGetSubsystem<TSubsystem>(this XRGeneralSettings xrGeneralSettings, out TSubsystem subsystem)
            where TSubsystem : class, UnityEngine.ISubsystem
        {
            subsystem = default;

            if (xrGeneralSettings != null && xrGeneralSettings.Manager != null)
            {
                var loader = xrGeneralSettings.Manager.activeLoader;
                if (loader != null)
                    subsystem = loader.GetLoadedSubsystem<TSubsystem>();
            }

            return subsystem != null;
        }
    }
}
