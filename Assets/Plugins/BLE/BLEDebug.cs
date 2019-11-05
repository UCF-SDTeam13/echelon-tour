using UnityEngine;

public static class BLEDebug
{
    public static void LogInfo(string message)
    {
        //if (Debug.isDebugBuild)
        //{
        Debug.Log(message);
        //}
    }
    public static void LogWarning(string message)
    {
        //if (Debug.isDebugBuild)
        //{
        Debug.LogWarning(message);
        //}
    }
    public static void LogError(string message)
    {
        Debug.LogError(message);
    }
}