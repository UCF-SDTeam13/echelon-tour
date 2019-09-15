
using UnityEngine;

public class HardwareTestUIManager : MonoBehaviour
{
    private readonly TestBikeListener _bikeListener = new TestBikeListener();

    public void RequestBLE()
    {
        BLEPlugin.Instance.RequestEnableBLE();
    }

    public void Scan()
    {
        BLEPlugin.Instance.Scan();
    }

    public void StopScan()
    {
        BLEPlugin.Instance.StopScan();
    }

    public void Connect()
    {
        // "E3:C5:90:DF:26:A0" Bike Simulator MAC Address
        // TODO - Dialog Based Selection
        BLEPlugin.Instance.Connect("E3:C5:90:DF:26:A0");
        // BLEPlugin.Instance.Connect("C6:00:F8:85:5E:29");
        Bike.Instance.RegisterBikeListener(_bikeListener);

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
        BLEPlugin.Instance.DiscoverServices();
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
        // Add one level, propery will auto clamp to valid range
        ++Workout.Instance.ResistanceLevel;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetResistanceLevel, Workout.Instance.ResistanceLevel);
    }
    public void DecreaseResistanceLevel()
    {
        // Subtract one level, property will auto clamp to valid range
        --Workout.Instance.ResistanceLevel;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetResistanceLevel, Workout.Instance.ResistanceLevel);
    }
    public void StopWorkout()
    {
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Stop;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
    }
    public void StartWorkout()
    {
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Start;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
    }
    public void PauseWorkout()
    {
        Workout.Instance.ControlState = BLEProtocol.WorkoutControlState.Pause;
        Bike.Instance.Set(BLEProtocol.ActionCode.SetWorkoutControlState, Workout.Instance.ControlState);
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
    }
    public void OnReceiveResistanceLevel(int resistanceLevel)
    {
        BLEDebug.LogInfo($"Resistance Level Received: {resistanceLevel}");
        Workout.Instance.ResistanceLevel = resistanceLevel;
    }
    public void OnReceiveWorkoutControlState(int controlState)
    {
        BLEDebug.LogInfo($"Workout Control State Received: {controlState}");
        Workout.Instance.ControlState = controlState;
    }
    public void OnReceiveWorkoutStatus(int timestamp, int count, int rpm, int heartrate)
    {
        BLEDebug.LogInfo($"Timestamp: {timestamp}, Count: {count}, RPM: {rpm}, Heart rate {heartrate}");
        // TODO - Feed this to workout for calculations
    }
}