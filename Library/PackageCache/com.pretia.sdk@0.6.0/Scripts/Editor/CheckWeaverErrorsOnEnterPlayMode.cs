using PretiaArCloud.Networking;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloudEditor
{
    public static class CheckWeaverErrorsOnEnterPlayMode
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CheckWeaverErrors()
        {
            EditorApplication.isPlaying = TypeResolver.NoCollisions;
        }
    }
}