using System;
using UnityEngine;

//Use BLEPlugin.Instance 
public sealed class BLEPlugin : MonoBehaviour
{
    private static INativePlugin _nativePluginInstance;

    // Hacky but needed due to MonoBehavior / SendMessage behavior
    private static BLEPlugin _instance;
    public static BLEPlugin Instance => _instance;
    private BLEBikeListener _bikeListener = new BLEBikeListener();
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
    public void RequestEnableLocation()
    {
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
        Bike.Instance.RegisterBikeListener(_bikeListener);
        _nativePluginInstance.Connect(address);

    }
    public void StartWorkout()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.StartWorkout();
#else
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Start;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
#endif
    }
    public void StopWorkout()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.StopWorkout();
#else
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Stop;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
#endif
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
    public void ReceiveMatch(string matchRecord)
    {
        string[] sep = { "|" };
        string[] matchSplit = matchRecord.Split(sep, 2, StringSplitOptions.RemoveEmptyEntries);


        Bike.Instance.Matches.Add(matchSplit[0]);
        if (!Bike.Instance.Addresses.ContainsKey(matchSplit[0]))
        {
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

class BLEBikeListener : IBikeListener
{
    public void OnAcknowledge()
    {
        // TODO -? For some reason factory implements ACKs, but then proceeeds to not use them
        // this is here just in case that changes in future firmware
        BLEDebug.LogInfo("ACK Received");
    }
    public void OnReceiveDeviceInformation(int modelID, string hardwareVersion, string firmwareVersion)
    {
        BLEDebug.LogInfo($"Device Info ModelID: {modelID}, Hardware Version: {hardwareVersion} Firmware Version: {firmwareVersion}");
        Bike.Instance.ModelID = modelID;
        Bike.Instance.HardwareVersion = hardwareVersion;
        Bike.Instance.FirmwareVersion = firmwareVersion;
    }
    public void OnReceiveErrorLog(byte[] errorCode)
    {
        BLEDebug.LogInfo("Error Log Received");
        // TODO
    }
    public void OnReceiveResistanceLevelRange(int min, int max)
    {
        BLEDebug.LogInfo($"Resistance Level Range Received: ({min},{max})");
        // TODO - Current implementation uses Units.Min/Max and ignores what the bike says
        Bike.Instance.ResistanceMin = min;
        Bike.Instance.ResistanceMax = max;
    }
    public void OnReceiveResistanceLevel(int resistanceLevel)
    {
        BLEDebug.LogInfo($"Resistance Level Received: {resistanceLevel}");
        Workout.Instance.ResistanceLevel = resistanceLevel;
        Bike.Instance.ResistanceLevel = resistanceLevel;
    }
    public void OnReceiveWorkoutControlState(int controlState)
    {
        BLEDebug.LogInfo($"Workout Control State Received: {controlState}");
        Workout.Instance.ControlState = controlState;
        Bike.Instance.ControlState = controlState;
    }
    public void OnReceiveWorkoutStatus(int timestamp, int count, int rpm, int heartrate)
    {
        BLEDebug.LogInfo($"Timestamp: {timestamp}, Count: {count}, RPM: {rpm}, Heart rate {heartrate}");
        Bike.Instance.Timestamp = timestamp;
        Bike.Instance.Count = count;
        Bike.Instance.RPM = rpm;
        Bike.Instance.Heartrate = heartrate;
        BLEDebug.LogInfo($"Timestamp: {Bike.Instance.Timestamp}, Count: {Bike.Instance.Count}, RPM: {Bike.Instance.RPM}, Heart rate {Bike.Instance.Heartrate}");
        // TODO - Feed this to workout for calculations
    }
}