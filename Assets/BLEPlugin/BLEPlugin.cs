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
        //Inject proper plugin based on platform
#if UNITY_ANDROID
        _nativePluginInstance = AndroidPlugin.Instance;
#elif UNITY_IOS
        _nativePlugin = iOSPlugin.Instance;
#else
        _nativePlugin = SimulatorPlugin.Instance;
#endif
    }

    public void RequestEnableBLE() {
        _nativePluginInstance.RequestEnableBLE();
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
    void Scan();
    void StopScan();
    void Connect(string address);
    void DiscoverServices();
    //NOTE: This should be a Base64 encoded byte string
    void SendPluginMessage(string message);
    void OnApplicationQuit();
}

interface IPluginListener
{
    void ReceivePluginMessage();
}