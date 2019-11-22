using System;
using System.Collections.Generic;

public sealed class Bike
{
    private static readonly Lazy<Bike>
    _Bike = new Lazy<Bike>(() => new Bike());
    public static Bike Instance => _Bike.Value;

    public HashSet<string> Matches {get; } = new HashSet<string>();
    public Dictionary<string,  string> Addresses {get; } = new Dictionary<string, string>();

    private Bike()
    {

    }

    public void Request(byte actionCode)
    {
        BLEAction.Get(actionCode);
    }

    public void Set(byte actionCode, int value)
    {
        BLEAction.Set(actionCode, value);
    }

    public void RegisterBikeListener(IBikeListener bikeListener)
    {
        BLEAction.RegisterBikeListener(bikeListener);
    }

    public int ModelID { get; set; } = 0;
    public string HardwareVersion { get; set; } = "00.00";
    public string FirmwareVersion { get; set; } = "00.00.00";
    public int ResistanceMin { get; set; } = 0;
    public int ResistanceMax { get; set; } = 32;
    public int ResistanceLevel { get; set; } = 0;
    public int ControlState { get; set; } = 0;
    public int Timestamp { get; set; } = 0;
    public int Count { get; set; } = 0;
    public int RPM { get; set; } = 1;
    public int Heartrate { get; set; } = 0;
}

public interface IBikeListener
{
    void OnAcknowledge();
    void OnReceiveDeviceInformation(int modelID, string hardwareVersion, string firmwareVersion);
    void OnReceiveErrorLog(byte[] errorCode);
    void OnReceiveResistanceLevelRange(int min, int max);
    void OnReceiveResistanceLevel(int resistanceLevel);
    void OnReceiveWorkoutControlState(int controlState);
    void OnReceiveWorkoutStatus(int timestamp, int count, int rpm, int heartrate);
}