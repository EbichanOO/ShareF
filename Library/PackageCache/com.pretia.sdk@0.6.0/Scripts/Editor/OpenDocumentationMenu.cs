using System.IO;
using UnityEditor;
using UnityEngine;

namespace PretiaArCloudEditor
{
    internal class OpenDocumentationMenu
    {
        [MenuItem("Pretia/Open API Documentation")]
        static void OpenDocumentation()
        {
            var docAbsolutePath = Path.GetFullPath("Packages/com.pretia.sdk/Documentation~/html/index.html");
            if (File.Exists(docAbsolutePath))
            {
                Application.OpenURL($"file://{docAbsolutePath}");
            }
            else
            {
                docAbsolutePath = Path.GetFullPath("Assets/PretiaSDK/Documentation~/html/index.html");
                if (File.Exists(docAbsolutePath))
                {
                    Application.OpenURL($"file://{docAbsolutePath}");
                }
                else
                {
                    Debug.LogError("Unable to open documentation.");
                }
            }
        }
    }
}