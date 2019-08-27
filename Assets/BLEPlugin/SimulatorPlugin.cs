using System;

public sealed class SimulatorPlugin : INativePlugin
{
    private static readonly Lazy<SimulatorPlugin>
    _SimulatorPlugin = new Lazy<SimulatorPlugin>(() => new SimulatorPlugin());
    public static SimulatorPlugin Instance => _SimulatorPlugin.Value;

    public bool EnabledBLE => false;

    private SimulatorPlugin()
    {

    }

    public void RequestEnableBLE()
    {

    }

    public void Scan()
    {

    }

    public void StopScan()
    {

    }

    public void Connect(string address)
    {

    }

    public void DiscoverServices()
    {

    }

    public void SendPluginMessage(string message)
    {

    }
    public void OnApplicationQuit()
    {

    }
}
