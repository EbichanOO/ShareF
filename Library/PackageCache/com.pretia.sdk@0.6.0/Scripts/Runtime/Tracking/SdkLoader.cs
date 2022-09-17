using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

namespace PretiaArCloud
{
    internal sealed class SdkLoader 
    {
        public delegate void DefaultSubsystemsRegistered(List<ISubsystem> subsystems);
        public static event DefaultSubsystemsRegistered OnDefaultSubsystemsRegistered;
        
        private static List<ISubsystem> _subsystems;

        /// <summary>
        /// Attempt to initialize the SDK before splash screen if the InitOnStart flag is set to true
        /// This will also add a Deinitialize callback when the application is quitting
        /// for automatic lifecycle management
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        internal static void AttemptInitializeSDKOnLoad()
        {
            var sdkSettings = PretiaSDKProjectSettings.Instance;

            // Add new subsystem implementation to this list
            // e.g., Mapping, LocalRelocalization, Networking
            _subsystems = new List<ISubsystem>();
            _subsystems.Add(SharedAnchorSubsystem.Factory.Create(sdkSettings));
            OnDefaultSubsystemsRegistered?.Invoke(_subsystems);

            if (PretiaSDKProjectSettings.Instance.InitializeOnStartup)
            {
                InitializeSDK();
                Application.quitting += DeinitializeSDK;
            }
        }

        /// <summary>
        /// Initialize all registered subsystems
        /// </summary>
        internal static void InitializeSDK()
        {
            _subsystems.ForEach(s => s.Initialize());
        }

        /// <summary>
        /// Deinitialize all registered subsystems
        /// </summary>
        internal static void DeinitializeSDK()
        {
            _subsystems.ForEach(s => s.Destroy());
        }

        /// <summary>
        /// Get the registered subsystem instance of type TSubsystem
        /// </summary>
        /// <typeparam name="TSubsystem">The subsystem instance that to get</typeparam>
        /// <returns>The instance of registered TSubsystem if it exists. Null otherwise</returns>
        internal static TSubsystem GetSubsystem<TSubsystem>()
            where TSubsystem : class, ISubsystem
        {
            return _subsystems.Find(s => s is TSubsystem) as TSubsystem;
        }
    }
}