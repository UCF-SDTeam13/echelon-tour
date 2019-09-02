using System;

// Singleton to handle Unity <-> Simulation Communication
public sealed class SimulatorPlugin : INativePlugin
{
    private static readonly Lazy<SimulatorPlugin>
    _SimulatorPlugin = new Lazy<SimulatorPlugin>(() => new SimulatorPlugin());
    public static SimulatorPlugin Instance => _SimulatorPlugin.Value;

    public bool EnabledBLE 
    {
        get;
        private set;
    }

    // Normally we would make implementation a separate class
    // but there is no Language crossing that makes that necessary
    private SimulatorPlugin()
    { 
        
    }

    public void RequestEnableBLE()
    {
        // TODO - Prompt for Enable to Simulate Flow / Delay
        EnabledBLE = true;
        BLEDebug.LogInfo("Simulating Enable BLE");
    }

    public void Scan()
    {
        // TODO - Simulate Scan and Report Simulated Results to Unity
        BLEDebug.LogInfo("Simulating Scan");
    }

    public void StopScan()
    {
        // TODO - Simulate Scan Stop
        BLEDebug.LogInfo("Stopping Simulated Scan");
    }

    public void Connect(string address)
    {
        // TODO - Simulate Connection
        BLEDebug.LogInfo("Simulate connection to " + address);
    }

    public void DiscoverServices()
    {
        // TODO - Simulate Service Discovery
        BLEDebug.LogInfo("Simulate Service Discovery");

    }

    public void SendPluginMessage(string message)
    {
        // TODO - Simulate Message Received From Unity
        BLEDebug.LogInfo("Simulating Send Message: " + message);
        BLESimulator.Instance.ReceiveUnityMessage(message);
    }

    public void OnApplicationQuit()
    {
        // TODO - Cleanup if Necessary
        BLEDebug.LogInfo("Application Quit Received");
    }
}
