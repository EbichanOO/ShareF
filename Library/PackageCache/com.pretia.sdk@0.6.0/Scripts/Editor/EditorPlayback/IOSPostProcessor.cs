#if UNITY_EDITOR_OSX

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace PretiaArCloud.RecordingPlayback
{
    public class IOSPostProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            string targetGuid = project.GetUnityMainTargetGuid();

            AddFramework(project, targetGuid, "mediacodec.framework");
            AddFramework(project, targetGuid, "VideoToolbox.framework");

            project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
            project.WriteToFile(projectPath);
        }

        private static void AddFramework(PBXProject project, string targetGuid, string frameworkName)
        {
            string frameworkDirectory = "Frameworks/ARRP/Plugins/iOS";
            bool isPackage = Directory.Exists(Path.Combine("Packages", "com.pretia.ar-recording-playback"));
            if (isPackage)
            {
                frameworkDirectory = "Frameworks/com.pretia.ar-recording-playback/Plugins/iOS";
            }

            string frameworkPath = Path.Combine(frameworkDirectory, frameworkName);
            string frameworkGuid = project.FindFileGuidByProjectPath(frameworkPath);

            if (frameworkGuid == null)
            {
                return;
            }

            PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, frameworkGuid);
        }
    }
}


#endif // UNITY_EDITOR_OSX
