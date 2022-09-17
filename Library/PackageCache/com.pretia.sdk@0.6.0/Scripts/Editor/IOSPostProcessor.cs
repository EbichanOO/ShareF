#if UNITY_EDITOR_OSX

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

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

        AddFramework(project, targetGuid, "arc_ndk_ios_dynamic.framework");
        AddFramework(project, targetGuid, "libdbow2.framework");
        AddFramework(project, targetGuid, "libfeature_extraction.framework");
        AddFramework(project, targetGuid, "libg2o_core.framework");
        AddFramework(project, targetGuid, "libg2o_csparse_extension.framework");
        AddFramework(project, targetGuid, "libg2o_ext_csparse.framework");
        AddFramework(project, targetGuid, "libg2o_solver_csparse.framework");
        AddFramework(project, targetGuid, "libg2o_solver_dense.framework");
        AddFramework(project, targetGuid, "libg2o_solver_eigen.framework");
        AddFramework(project, targetGuid, "libg2o_solver_pcg.framework");
        AddFramework(project, targetGuid, "libg2o_solver_slam2d_linear.framework");
        AddFramework(project, targetGuid, "libg2o_solver_structure_only.framework");
        AddFramework(project, targetGuid, "libg2o_stuff.framework");
        AddFramework(project, targetGuid, "libg2o_types_data.framework");
        AddFramework(project, targetGuid, "libg2o_types_icp.framework");
        AddFramework(project, targetGuid, "libg2o_types_sba.framework");
        AddFramework(project, targetGuid, "libg2o_types_sclam2d.framework");
        AddFramework(project, targetGuid, "libg2o_types_sim3.framework");
        AddFramework(project, targetGuid, "libg2o_types_slam2d.framework");
        AddFramework(project, targetGuid, "libg2o_types_slam2d_addons.framework");
        AddFramework(project, targetGuid, "libg2o_types_slam3d.framework");
        AddFramework(project, targetGuid, "libg2o_types_slam3d_addons.framework");
        AddFramework(project, targetGuid, "libopenvslam.framework");
        AddFramework(project, targetGuid, "libyaml-cpp.framework");
        AddFramework(project, targetGuid, "opencv2.framework");

        project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
        project.WriteToFile(projectPath);
    }

    private static void AddFramework(PBXProject project, string targetGuid, string frameworkName)
    {
        string frameworkDirectory = "Frameworks/PretiaSDK/Plugins/iOS";
        bool isPackage = Directory.Exists(Path.Combine("Packages", "com.pretia.sdk"));
        if (isPackage)
        {
            frameworkDirectory = "Frameworks/com.pretia.sdk/Plugins/iOS";
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

#endif // UNITY_EDITOR_OSX
