using System.Linq;
using PretiaArCloud;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace PretiaArCloudEditor
{
    public class PretiaPreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 5;

        public void OnPreprocessBuild(BuildReport report)
        {
            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (!preloadedAssets.Contains(PretiaSDKProjectSettings.Instance))
            {
                AddPretiaSettings(preloadedAssets);
            }
        }

        private void AddPretiaSettings(UnityEngine.Object[] preloadedAssets)
        {
            var list = preloadedAssets.ToList();
            list.Add(PretiaSDKProjectSettings.Instance);
            PlayerSettings.SetPreloadedAssets(list.ToArray());
        }
    }
}