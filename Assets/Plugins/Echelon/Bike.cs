using System;

public sealed class Bike
{
    private static readonly Lazy<Bike>
    _Bike = new Lazy<Bike>(() => new Bike());
    public static Bike Instance => _Bike.Value;

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