#if UNITY_IOS

using System;
using UnityEngine;

public sealed class iOSPlugin : INativePlugin
{
    private static readonly Lazy<iOSPlugin>
    _iOSPlugin = new Lazy<iOSPlugin> (() => new iOSPlugin());
    public static iOSPlugin Instance => _iOSPlugin.Value;

    public bool EnabledBLE => false;

    private iOSPlugin()
    {

    }

    public void RequestEnableBLE()
    {

    }

    public void Scan()
    {
        SwiftForUnity.StartScan();
    }
    public void StopScan()
    {
        SwiftForUnity.StopScan();
    }
    public void Connect(string message)
    {
        SwiftForUnity.ConnectWithIdentifier(message);
    }
    public void DiscoverServices()
    {
        SwiftForUnity.DiscoverServices();
    }

    public void SendPluginMessage()
    {

    }
    public void OnApplicationQuit()
    {
        
    }

    public void SendPluginMessage(string message)
    {
        Instance.SendPluginMessage(message);
    }

    public void RequestEnableLocation()
    {

    }
}

#endif
