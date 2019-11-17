using System;
using UnityEngine;

//Use BLEPlugin.Instance 
public sealed class BLEPlugin : MonoBehaviour
{
    private static INativePlugin _nativePluginInstance;

    // Hacky but needed due to MonoBehavior / SendMessage behavior
    private static BLEPlugin _instance;
    public static BLEPlugin Instance => _instance;

    public void Awake()
    {
        _instance = this;
        // Inject proper plugin based on platform
#if UNITY_ANDROID
        _nativePluginInstance = AndroidPlugin.Instance;
#elif UNITY_IOS
        _nativePluginInstance = iOSPlugin.Instance;
#else
        _nativePluginInstance = SimulatorPlugin.Instance;
#endif
    }

    public void RequestEnableBLE()
    {
        if (!_nativePluginInstance.EnabledBLE)
        {
            _nativePluginInstance.RequestEnableBLE();
        }
        else
        {
            BLEDebug.LogWarning("BLE Already Enabled, Ignoring Request");
        }

        RequestEnableLocation();
    }
    public void RequestEnableLocation() {
        _nativePluginInstance.RequestEnableLocation();
    }
    public void Scan()
    {
        _nativePluginInstance.Scan();
    }

    public void StopScan()
    {
        _nativePluginInstance.StopScan();
    }

    public void Connect(string address)
    {
        _nativePluginInstance.Connect(address);
    }

    public void DiscoverServices()
    {
        _nativePluginInstance.DiscoverServices();
    }

    public void ReceivePluginMessage(string message)
    {
        BLEDebug.LogInfo("Plugin Received Message: " + message);
        BLEAction.ProcessReceiveData(BLEProtocol.ConvertStringToBytes(message));
    }
    public void ReceiveMatch (string matchRecord) {
        string[] sep = {"|"};
        string[] matchSplit = matchRecord.Split(sep, 2, StringSplitOptions.RemoveEmptyEntries);

        
        Bike.Instance.Matches.Add(matchSplit[0]);
        if (!Bike.Instance.Addresses.ContainsKey(matchSplit[0])) {
            Bike.Instance.Addresses.Add(matchSplit[0], matchSplit[1]);
        }
        // Test Value Was Added
        string s = "";
        Bike.Instance.Addresses.TryGetValue(matchSplit[0], out s);
        BLEDebug.LogInfo("Match Received: " + matchSplit[0] + " " + s);
    }
    public void SendPluginMessage(string message)
    {
        BLEDebug.LogInfo("Plugin Sending Message: " + message);
        _nativePluginInstance.SendPluginMessage(message);
    }

    public void OnApplicationQuit()
    {
        _nativePluginInstance.OnApplicationQuit();
    }
}

interface INativePlugin
{
    bool EnabledBLE { get; }

    void RequestEnableBLE();
    void RequestEnableLocation();
    void Scan();
    void StopScan();
    void Connect(string address);
    void DiscoverServices();
    // NOTE: This should be a Base64 encoded byte string
    void SendPluginMessage(string message);
    void OnApplicationQuit();
}

interface IPluginListener
{
    void ReceivePluginMessage();
}