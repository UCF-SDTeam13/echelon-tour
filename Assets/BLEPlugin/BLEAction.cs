using System;
using System.Linq;

public static class BLEAction
{
    private static IBikeListener _bikeListener;

    public static void Get(byte actionCode)
    {
        BLEDebug.LogInfo("BLEAction Get");
        switch (actionCode)
        {
            case BLEProtocol.ActionCode.Acknowledge:
            case BLEProtocol.ActionCode.GetDeviceInformation:
            case BLEProtocol.ActionCode.GetErrorLog:
            case BLEProtocol.ActionCode.GetResistanceLevelRange:
            case BLEProtocol.ActionCode.GetWorkoutControlState:
            case BLEProtocol.ActionCode.GetResistanceLevel:
                BLEPlugin.Instance.SendPluginMessage(
                    BLEProtocol.ConvertBytesToString(
                        BLEProtocol.PrepareGetCommandBytes(actionCode)
                    )
                );
                break;
            default:
                BLEDebug.LogError("Error: Attempting to send invalid Get ActionCode");
                break;
        }
    }
    public static void Set(byte actionCode, int value)
    {
        BLEDebug.LogInfo("BLEAction Set");
        switch (actionCode)
        {
            case BLEProtocol.ActionCode.SetWorkoutControlState:
            case BLEProtocol.ActionCode.SetResistanceLevel:
                BLEPlugin.Instance.SendPluginMessage(
                    BLEProtocol.ConvertBytesToString(
                        BLEProtocol.PrepareSetCommandBytes(actionCode, value)
                    )
                );
                break;
            default:
                BLEDebug.LogError("Error: Attempting to send invalid Set ActionCode");
                break;
        }
    }
    public static void RegisterBikeListener(IBikeListener bikeListener)
    {
        BLEDebug.LogInfo("Registering Bike Listener");
        _bikeListener = bikeListener;
    }
    public static void ProcessReceiveData(byte[] data)
    {
        BLEDebug.LogInfo("BLEAction Processing Received Data");
        // Ignore and log invalid data
        if (!BLEProtocol.ValidateData(data))
        {
            BLEDebug.LogWarning("Warning: Receive Data could not be validated");
            return;
        }
        byte responseCode = BLEProtocol.GetResponseCode(data);
        BLEDebug.LogInfo($"Processing Response Code {responseCode}");
        switch (responseCode)
        {
            case BLEProtocol.ResponseCode.Acknowledge:
                Acknowledge();
                break;
            case BLEProtocol.ResponseCode.DeviceInformationResponse:
                ReceiveDeviceInformation(data);
                break;
            case BLEProtocol.ResponseCode.ErrorLogResponse:
                ReceiveErrorLog(data);
                break;
            case BLEProtocol.ResponseCode.ResistanceLevelRangeResponse:
                ReceiveResistanceLevelRange(data);
                break;
            case BLEProtocol.ResponseCode.ResistanceLevelResponse:
            case BLEProtocol.ResponseCode.ResistanceLevelNotification:
                ReceiveResistanceLevel(data);
                break;
            case BLEProtocol.ResponseCode.WorkoutControlStateResponse:
            case BLEProtocol.ResponseCode.WorkoutControlStateNotification:
                ReceiveWorkoutControlState(data);
                break;
            case BLEProtocol.ResponseCode.WorkoutStatusNotification:
                ReceiveWorkoutStatus(data);
                break;
            default:
                BLEDebug.LogError("Error: Invalid ResponseCode received from native plugin");
                return;
        }
    }
    private static void Acknowledge()
    {
        _bikeListener.OnAcknowledge();
    }
    private static void ReceiveDeviceInformation(byte[] data)
    {
        BLEDebug.LogInfo("Device Information Received");

        int modelID = data[BLEProtocol.Index.ModelID];
        string hardwareVersion = String.Format(
            $"{data[BLEProtocol.Index.HardwareVersion.Major]}.{data[BLEProtocol.Index.HardwareVersion.Minor]}"
        );
        string firmwareVersion = String.Format(
            $"{data[BLEProtocol.Index.FirmwareVersion.Major]}.{data[BLEProtocol.Index.FirmwareVersion.Minor]}.{data[BLEProtocol.Index.FirmwareVersion.Patch]}"
        );
        
        _bikeListener.OnReceiveDeviceInformation(modelID, hardwareVersion, firmwareVersion);
    }
    private static void ReceiveErrorLog(byte[] data)
    {
        BLEDebug.LogInfo("Error Log Received");
        int take = data.Length - (BLEProtocol.Index.ErrorLogStart + 1);
        byte[] log = data.Skip(BLEProtocol.Index.ErrorLogStart).Take(take).ToArray();
        _bikeListener.OnReceiveErrorLog(log);
    }
    private static void ReceiveResistanceLevelRange(byte[] data)
    {
        BLEDebug.LogInfo("Resistance Level Range Received");
        int minResistance = data[BLEProtocol.Index.ResistanceLevelRange.Min];
        int maxResistance = data[BLEProtocol.Index.ResistanceLevelRange.Max];
        _bikeListener.OnReceiveResistanceLevelRange(minResistance, maxResistance);
    }
    private static void ReceiveResistanceLevel(byte[] data)
    {
        BLEDebug.LogInfo("Resistance Level Received");
        int level = data[BLEProtocol.Index.ResistanceLevel];
        _bikeListener.OnReceiveResistanceLevel(level);
    }
    private static void ReceiveWorkoutControlState(byte[] data)
    {
        BLEDebug.LogInfo("Workout Control State Received");
        int controlState = data[BLEProtocol.Index.WorkoutControlState];
        _bikeListener.OnReceiveWorkoutControlState(controlState);
    }
    private static void ReceiveWorkoutStatus(byte[] data)
    {
        BLEDebug.LogInfo("Workout Status Received");
        int timeStamp = BLEProtocol.ExtractDataInt(
            BLEProtocol.Index.TimeStamp.Begin,
            BLEProtocol.Index.TimeStamp.End,
            data
        );

        int count = BLEProtocol.ExtractDataInt(
            BLEProtocol.Index.Count.Begin,
            BLEProtocol.Index.Count.End,
            data
        );

        int rpm = BLEProtocol.ExtractDataInt(
            BLEProtocol.Index.RPM.Begin,
            BLEProtocol.Index.RPM.End,
            data
        );

        int heartRate = data[BLEProtocol.Index.HeartRate];

        _bikeListener.OnReceiveWorkoutStatus(timeStamp, count, rpm, heartRate);
    }
}