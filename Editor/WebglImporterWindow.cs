using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lajawi
{
    public class WebglImporterWindow : EditorWindow
    {
        private const string packageName = "com.github.lajawi.webgltemplates";
        private const string webglTemplatesVersionFilePath = "webglTemplatesVersion.txt";
        private const string webglTemplatesVersionSkippedFilePath = "webglTemplatesVersionSkipped.txt";

        private static ListRequest listReq;
        private static string packageVersion;
        private static bool manualImport;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            Label label = new("Another version of the WebGL Templates Package is currently installed. Do you want to update the current assets?")
            {
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                },
            };
            root.Add(label);

            Button updateButton = new()
            {
                name = "update",
                text = "Update",
            };
            updateButton.clicked += ImportWebglTemplates;
            updateButton.clicked += () => { this.Close(); };
            root.Add(updateButton);

            Button skipButton = new()
            {
                name = "skip",
                text = "Skip",
            };
            skipButton.clicked += SkipVersion;
            skipButton.clicked += () => { this.Close(); };
            root.Add(skipButton);
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            listReq = Client.List();
            manualImport = false;
            EditorApplication.update += CheckPackageVersion;
        }

        [MenuItem("Tools/WebGL Templates/Import WebGL Templates")]
        private static void InitManual()
        {
            listReq = Client.List();
            manualImport = true;
            EditorApplication.update += CheckPackageVersion;
        }

        [MenuItem("Tools/WebGL Templates/Force Reimport WebGL Templates")]
        private static void Reimport()
        {
            ImportWebglTemplates();
        }

        private static void CheckPackageVersion()
        {
            if (!listReq.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= CheckPackageVersion;

            packageVersion = listReq.Result.First(p => p.name == packageName).version;

            if (!File.Exists(webglTemplatesVersionFilePath))
            {
                Debug.Log($"No version installed, installing version {packageVersion}");
                ImportWebglTemplates();
                return;
            }

            if (File.Exists(webglTemplatesVersionSkippedFilePath) && File.ReadAllText(webglTemplatesVersionSkippedFilePath).Equals(packageVersion))
            {
                if (manualImport) Debug.Log($"WebGL Templates version {packageVersion} skipped. Use 'Force Reimport WebGL Templates' instead");
                return;
            }

            string installedVer = File.ReadAllText(webglTemplatesVersionFilePath);

            if (installedVer.Equals(packageVersion))
            {
                //Debug.Log($"WebGL Templates version {packageVersion} installed");
                File.Delete(webglTemplatesVersionSkippedFilePath);
                return;
            }

            Debug.Log($"WebGL Templates version {packageVersion} available");

            WebglImporterWindow wnd = GetWindow<WebglImporterWindow>();
            wnd.maxSize = wnd.minSize = new(350, 100);
            wnd.titleContent = new GUIContent("WebGL Templates Importer");
        }

        private static void SkipVersion()
        {
            File.WriteAllText(webglTemplatesVersionSkippedFilePath, packageVersion);
        }

        private static void ImportWebglTemplates()
        {
            Debug.Log($"Importing WebGL Templates from version {packageVersion}");

            CopyDirectory(Path.GetFullPath($"Packages/{packageName}/Assets~/WebGLTemplates"), Path.GetFullPath("Assets/WebGLTemplates"), true, true);
            File.WriteAllText(webglTemplatesVersionFilePath, packageVersion);
            File.Delete(webglTemplatesVersionSkippedFilePath);
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = false, bool recursive = false)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();

            var destDir = new DirectoryInfo(destinationDir);

            if (!destDir.Exists) Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, overwrite);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, overwrite, recursive);
                }
            }
        }
    }
}
