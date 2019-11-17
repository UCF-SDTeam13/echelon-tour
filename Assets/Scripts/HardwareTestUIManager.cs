
using UnityEngine;

public class HardwareTestUIManager : MonoBehaviour
{
    private TestBikeListener _bikeListener;

    public void Start()
    {
        _bikeListener = new TestBikeListener();
        //RequestBLE();
        //Scan();
        //ConnectTestRealBike();
        //DiscoverServices();
    }
    public void RequestBLE()
    {
        BLEPlugin.Instance.RequestEnableBLE();
    }

    public void Scan()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.StartScan();
#else
        BLEPlugin.Instance.Scan();
#endif
    }

    public void StopScan()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.StopScan();
#else
        BLEPlugin.Instance.StopScan();
#endif
    }

    public void Connect()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.Connect();
#else
        // "E3:C5:90:DF:26:A0" Bike Simulator MAC Address
        // TODO - Dialog Based Selection
        BLEPlugin.Instance.Connect("E3:C5:90:DF:26:A0");
        // BLEPlugin.Instance.Connect("C6:00:F8:85:5E:29");
        Bike.Instance.RegisterBikeListener(_bikeListener);
#endif
    }
    public void ConnectTestMockBike()
    {
        // "E3:C5:90:DF:26:A0" Bike Simulator MAC Address
        BLEPlugin.Instance.Connect("E3:C5:90:DF:26:A0");
        Bike.Instance.RegisterBikeListener(_bikeListener);
    }

    public void ConnectTestRealBike()
    {
        // "C6:00:F8:85:5E:29" ECHEX-3-105011
        BLEPlugin.Instance.Connect("C6:00:F8:85:5E:29");
        Bike.Instance.RegisterBikeListener(_bikeListener);

    }
    public void DiscoverServices()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.DiscoverServices();
#else
        BLEPlugin.Instance.DiscoverServices();
#endif
    }
    public void SendMessage()
    {
        BLEPlugin.Instance.SendPluginMessage("Hello, world!");
    }

    public void GetDeviceInformation()
    {
        Bike.Instance.Request(BLEProtocol.ActionCode.GetDeviceInformation);
    }
    public void GetErrorLog()
    {
        Bike.Instance.Request(BLEProtocol.ActionCode.GetErrorLog);
    }
    public void GetResistanceLevelRange()
    {
        Bike.Instance.Request(BLEProtocol.ActionCode.GetResistanceLevelRange);
    }
    public void GetWorkoutControlState()
    {
        Bike.Instance.Request(BLEProtocol.ActionCode.GetWorkoutControlState);
    }
    public void GetResistanceLevel()
    {
        Bike.Instance.Request(BLEProtocol.ActionCode.GetResistanceLevel);
    }
    public void SetResistanceLevel()
    {
        // TODO - Dialog
        Bike.Instance.Set(BLEProtocol.ActionCode.SetResistanceLevel, 5);
    }
    // TODO - Consider changing interval for increase / decrease
    // - when there are 0-32 levels a one level difference is hard to notice
    public void InreaseResistanceLevel()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.IncreaseResistanceLevel();
#else
        // Add one level, property will auto clamp to valid range
        ++Workout.Instance.ResistanceLevel;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetResistanceLevel, Workout.Instance.ResistanceLevel);
#endif
    }
    public void DecreaseResistanceLevel()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.DecreaseResistanceLevel();
#else
        // Subtract one level, property will auto clamp to valid range
        --Workout.Instance.ResistanceLevel;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetResistanceLevel, Workout.Instance.ResistanceLevel);
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
    public void StartWorkout()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.StartWorkout();
#else
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Start;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
#endif
    }
    public void PauseWorkout()
    {
#if UNITY_IOS && !UNITY_EDITOR
            SwiftForUnity.PauseWorkout();
#else
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Pause;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
#endif
    }
}

class TestBikeListener : IBikeListener
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
        //BLEDebug.LogInfo($"Timestamp: {timestamp}, Count: {count}, RPM: {rpm}, Heart rate {heartrate}");
        Bike.Instance.Timestamp = timestamp;
        Bike.Instance.Count = count;
        Bike.Instance.RPM = rpm;
        Bike.Instance.Heartrate = heartrate;
        BLEDebug.LogInfo($"Timestamp: {Bike.Instance.Timestamp}, Count: {Bike.Instance.Count}, RPM: {Bike.Instance.RPM}, Heart rate {Bike.Instance.Heartrate}");
        // TODO - Feed this to workout for calculations
    }
}