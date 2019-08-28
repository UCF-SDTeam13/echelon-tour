#if UNITY_ANDROID
using System;
using UnityEngine;
using UnityEngine.Android;

// Singleton to Handle Unity<->Android Communication from the Unity Side
public sealed class AndroidPlugin : INativePlugin
{
    // Lazy Initalize Instance of AndroidPlugin
    private static readonly Lazy<AndroidPlugin>
    _AndroidPlugin = new Lazy<AndroidPlugin>(() => new AndroidPlugin());
    // Use AndroidPlugin.Instance for Accessing Methods / Properties
    public static AndroidPlugin Instance => _AndroidPlugin.Value;

    public bool EnabledBLE => _BLEManagerClass.CallStatic<bool>("isBLEEnabled");

    // NOTE: Kotlin objects are actual instances called "INSTANCE", so their methods are not static
    // However, we specify functions as @JVMStatic so that they appear as typical Java static
    // since Unity's Android JNI wrapper doesn't seem to like using INSTANCE 

    private AndroidPlugin()
    {
        // Get Unity Android Context / Activity
        _unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _currentActivity = _unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        _currentApplicationContext = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        // Pass them to BLEManager for Initialization
        _BLEManagerClass = new AndroidJavaClass("org.echelon.bleplugin.BLEManager");
        _BLEManagerClass.CallStatic("initialize", _currentApplicationContext, _currentActivity);
    }

    // Android Plugin Native Class
    private readonly AndroidJavaClass _BLEManagerClass;
    // Android UnityPlayer
    private readonly AndroidJavaClass _unityPlayerClass;
    // Android UnityPlayer Current Activity
    private readonly AndroidJavaObject _currentActivity;
    // Android UnityPlayer Application Context
    private readonly AndroidJavaObject _currentApplicationContext;

    public void RequestEnableBLE()
    {
        // Ask for BLE to be Enabled
        _BLEManagerClass.CallStatic("requestEnableBLE", _currentActivity);

        // TODO - Separate Out Location Permissions and Use a Dialog to Explain
        // (Bluetooth Permission reasoning is fairly obvious but Location is Not)
        // Need Location Permission for BLE Scanning - Coarse Because We Don't Care About Actual Location
        Permission.RequestUserPermission(Permission.CoarseLocation);
    }

    // Scan for BLE Devices
    public void Scan()
    {
        _BLEManagerClass.CallStatic("scan");
    }

    // Stop Scanning for BLE Devices
    public void StopScan()
    {
        _BLEManagerClass.CallStatic("stopScan");
    }

    // Connect to BLE Device by Address
    public void Connect(string address)
    {
        _BLEManagerClass.CallStatic("connect", _currentApplicationContext, address);
    }

    // Discover Services on Connected BLE Device
    public void DiscoverServices()
    {
        _BLEManagerClass.CallStatic("discoverServices");
    }

    // Send BLE Message to Bike
    public void SendPluginMessage(string message)
    {
        _BLEManagerClass.CallStatic("receiveUnityMessage", message);
    }

    // Notify Android Side for Cleanup During Quit
    public void OnApplicationQuit()
    {
        _BLEManagerClass.CallStatic("applicationQuit", _currentActivity);
    }
}

#endif