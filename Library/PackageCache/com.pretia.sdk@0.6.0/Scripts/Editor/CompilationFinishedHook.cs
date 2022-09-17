using UnityEditor;
using UnityEditor.Compilation;

namespace PretiaArCloud.Networking.Weaver
{
    public class CompilationFinishedHook
    {
        [InitializeOnLoadMethod]
        public static void OnInitializeOnLoadMethod()
        {
            CompilationPipeline.assemblyCompilationFinished += Weaver.OnCompilationFinished;
        }
    }
}