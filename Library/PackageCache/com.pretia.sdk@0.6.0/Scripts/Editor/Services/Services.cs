using System.Net.Http;
using PretiaArCloud;
using PretiaArCloudEditor.RecordingPlayback;
using UnityEditor;

namespace PretiaArCloudEditor
{
    internal class Services
    {
        internal static DeveloperService DeveloperService;
        internal static GroupService GroupService;
        internal static MapService MapService;
        internal static PlaybackDownloader PlaybackDownloader;

        [InitializeOnLoadMethod]
        public static void OnInitializeOnLoadMethod()
        {
            HttpClient httpClient = new HttpClient();
            IJsonSerializer serializer = new Utf8JsonEditorSerializer();
            IJwtDecoder decoder = new JwtDecoder();

            DeveloperService = new DeveloperService(httpClient, decoder, serializer);

            GroupService = new GroupService(httpClient, serializer);
            GroupService.Token = EditorPrefs.GetString(Constants.TOKEN);

            MapService = new MapService(httpClient, serializer);
            MapService.Token = EditorPrefs.GetString(Constants.TOKEN);

            PlaybackDownloader = new PlaybackDownloader(serializer);
        }
    }
}