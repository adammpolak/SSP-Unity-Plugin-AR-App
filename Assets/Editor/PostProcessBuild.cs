using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class PostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.iOS)
        {
            // Create a PBXProject object to manipulate the Xcode project
            string projPath = PBXProject.GetPBXProjectPath(path);
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);

            // Get the UnityFramework target GUID
            string unityFrameworkTargetGuid = proj.TargetGuidByName("UnityFramework");

            // Add the VideoToolbox framework to the UnityFramework target
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "VideoToolbox.framework", false);

            // Add the libbz2.tbd library to the UnityFramework target
            proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libbz2.tbd", "libbz2.tbd", PBXSourceTree.Sdk));

            // Write the changes to the Xcode project file
            proj.WriteToFile(projPath);
        }
    }
}
