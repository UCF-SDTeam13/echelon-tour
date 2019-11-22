using System.IO;
using UnityEditor;
class Autobuilder
{
    [MenuItem("AutoBuilder/Android")]
    public static void PerformAndroidBuild()
    {
        string[] scenes = { "Assets/Scenes/NetworkTestScene.unity" };

        string buildPath = "../../../Build/Android";

        // Create build folder if it does not exist
        //Directory.CreateDirectory(buildPath);

        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.Android, BuildOptions.Development);
    }
}