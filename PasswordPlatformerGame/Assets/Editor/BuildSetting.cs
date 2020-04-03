using UnityEditor;
using UnityEngine;

public class BuildSetting
{
    [MenuItem("WebGL/Enable Embedded Resources")]
    public static void EnableErrorMessageTesting()
    {
        Debug.Log("Hello!");
        PlayerSettings.WebGL.useEmbeddedResources = true;
        // PlayerSettings.SetPropertyBool("useEmbeddedResources", true, BuildTargetGroup.WebGL);
    }
}