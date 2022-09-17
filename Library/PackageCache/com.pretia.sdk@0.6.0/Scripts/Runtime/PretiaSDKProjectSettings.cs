using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace PretiaArCloud
{
    public sealed class PretiaSDKProjectSettings : ScriptableObject
    {
        private const string PATH = "Assets/Pretia/PretiaSDKProjectSettings.asset";

        private static PretiaSDKProjectSettings _instance;
        public static PretiaSDKProjectSettings Instance
        {
#if UNITY_EDITOR
            get
            {
                if (_instance == null)
                {
                    _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<PretiaSDKProjectSettings>(PATH);
                }

                return _instance;
            }
#else
            get
            {
                return _instance;
            }
#endif
        }

        [SerializeField]
        private string _appKey;
        public string AppKey => _appKey;

        [SerializeField]
        private bool _initializeOnStartup = true;
        public bool InitializeOnStartup => _initializeOnStartup;

        [HideInNormalInspector]
        public string LobbyServerAddress = "arc-lobby-server.pretiaar.com";
        [HideInNormalInspector]
        public int LobbyServerPort = 4321;

        [HideInNormalInspector]
        public string RelocServerAddress = "arc-reloc.pretiaar.com"; // TODO
        public string RelocServerIPAddress
        {
            get
            {
                if(InternetConnectivity == false)
                {
                    return "ipAddress-No-Connection";
                }

                var entry = System.Net.Dns.GetHostEntry(RelocServerAddress);
                var ipAddress = entry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
                return ipAddress.ToString();
            }
        }
        [HideInNormalInspector]
        public int RelocServerPort = 80;

        [HideInNormalInspector]
        public string PublicMapServerAddress = "https://arc-map-public.pretiaar.com";

        [HideInNormalInspector]
        public string DeveloperServerAddress = "https://arc-developer.pretiaar.com";

        private static bool _internetConnectivity = false;
        public static bool InternetConnectivity
        {
            get
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    _internetConnectivity = false;
                }

                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||
                    Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    _internetConnectivity = true;
                }

                return _internetConnectivity;
            }
        }

#if !UNITY_EDITOR
        private void Awake()
        {
            _instance = this;
        }
#endif
    }
}