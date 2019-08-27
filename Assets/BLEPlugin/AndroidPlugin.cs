#if UNITY_ANDROID
using System;
using UnityEngine;
using UnityEngine.Android;

public sealed class AndroidPlugin : INativePlugin
{
    private static readonly Lazy<AndroidPlugin>
    _AndroidPlugin = new Lazy<AndroidPlugin>(() => new AndroidPlugin());
    public static AndroidPlugin Instance => _AndroidPlugin.Value;

    // TODO fix this
    //public bool EnabledBLE => _BLEManagerClass.CallStatic<bool>("getBLEStatus");
    public bool EnabledBLE => true;

    // NOTE: Kotlin objects are actual instances called "INSTANCE", so their methods are not static
    // NOTE2: However, we specify functions as @JVMStatic so that they appear as typical Java static
    // since Unity's Android JNI wrapper doesn't seem to like INSTANCE 

    private AndroidPlugin()
    {
        // Get Unity Android Context / Activity and Pass to BLEManager
        _unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _currentActivity = _unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        _currentApplicationContext = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        _BLEManagerClass = new AndroidJavaClass("org.echelon.bleplugin.BLEManager");
        //_BLEManagerInstance = _BLEManagerClass.GetStatic<AndroidJavaObject>("INSTANCE");
        //_BLEManagerInstance = new AndroidJavaObject("org.echelon.bleplugin.BLEManager", _currentApplicationContext, _currentActivity);
        _BLEManagerClass.CallStatic("initialize", _currentApplicationContext, _currentActivity);
    }

    // Android Plugin Native Class
    private readonly AndroidJavaClass _BLEManagerClass;
    // Android Plugin Native Instance
    //private readonly AndroidJavaObject _BLEManagerInstance;
    // Android UnityPlayer
    private readonly AndroidJavaClass _unityPlayerClass;
    // Android UnityPlayer Current Activity
    private readonly AndroidJavaObject _currentActivity;
    // Android UnityPlayer Activity Context
    private readonly AndroidJavaObject _currentApplicationContext;

    public void RequestEnableBLE()
    {
        _BLEManagerClass.CallStatic("requestEnableBLE", _currentActivity);
        Permission.RequestUserPermission(Permission.CoarseLocation);
    }

    public void Scan()
    {
        _BLEManagerClass.CallStatic("scan");
    }

    public void StopScan()
    {
        _BLEManagerClass.CallStatic("stopScan");
    }

    public void Connect(string address)
    {
        _BLEManagerClass.CallStatic("connect", _currentApplicationContext, address);
    }

    public void DiscoverServices()
    {
        _BLEManagerClass.CallStatic("discoverServices");
    }

    public void SendPluginMessage(string message)
    {
        _BLEManagerClass.CallStatic("receiveUnityMessage", message);
    }

    public void OnApplicationQuit()
    {
        _BLEManagerClass.CallStatic("applicationQuit", _currentActivity);
    }
}

#endif