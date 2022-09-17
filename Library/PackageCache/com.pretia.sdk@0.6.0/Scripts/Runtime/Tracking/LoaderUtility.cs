namespace PretiaArCloud
{
    /// <summary>
    /// Utility class to help interact with subsystems
    /// </summary>
    public sealed class LoaderUtility
    {
        /// <summary>
        /// Initialize all registered subsystems
        /// </summary>
        /// <returns>True if all subsystems initialized successfully. False otherwise</returns>
        public static bool Initialize()
        {
            try
            {
                SdkLoader.InitializeSDK();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deinitialize all registered subsystems
        /// </summary>
        /// <returns>True if all subsystems deinitialized successfully. False otherwise</returns>
        public static bool Deinitialize()
        {
            try
            {
                SdkLoader.DeinitializeSDK();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the subsystem instance of type TSubsystem if it exists
        /// </summary>
        /// <typeparam name="TSubsystem"></typeparam>
        /// <returns>The instance of registered TSubsystem. Null otherwise</returns>
        public static TSubsystem GetSubsystem<TSubsystem>()
            where TSubsystem: class, ISubsystem
        {
            return SdkLoader.GetSubsystem<TSubsystem>();
        }
    }
}