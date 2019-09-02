
using UnityEngine;
using System;

public sealed class BLESimulator : MonoBehaviour
{
    // Hacky but needed due to how MonoBehaviour works
    private static BLESimulator _BLESimulatorInstance;
    public static BLESimulator Instance => _BLESimulatorInstance;

    private GameObject BLEPluginInstance;

    // Immutable State
    private const int modelID = 1;
    private const int hardwareMajor = 2;
    private const int hardwareMinor = 3;
    private const int firmwareMajor = 4;
    private const int firmwareMinor = 5;
    private const int firmwarePatch = 6;

    private const int resistanceLevelMin = 0;
    private const int resitanceLevelMax = 32;

    // Mutable State
    private int controlState = BLEProtocol.WorkoutControlState.Stop;
    private int resistanceLevel = 0;
    private int timestamp = 0;
    private int count = 0;
    private int rpm = 0;
    private int heartrate = 0;

    public void Awake()
    {
        _BLESimulatorInstance = this;
        BLEPluginInstance = GameObject.Find("BLEPlugin");
    }

    public void Start()
    {
        InvokeRepeating("SendWorkoutStatusNotification", 1.0f, 1.0f);
    }

    public void SendWorkoutStatusNotification()
    {
        if (controlState == BLEProtocol.WorkoutControlState.Stop) {
            timestamp = 0;
            count = 0;
            rpm = 0;
            heartrate = 0;
        }
        else if (controlState == BLEProtocol.WorkoutControlState.Start) {
            ++timestamp;
            count += 2;
            rpm = 120;
            heartrate = 0;
        }

        BLEDebug.LogInfo("Sending WorkoutStatusNotification");
        BLEPluginInstance.SendMessage("ReceivePluginMessage",
            BLEProtocol.ConvertBytesToString(
                BLENotify.PrepareWorkoutStatusBytes(
                    timestamp,
                    count,
                    rpm,
                    heartrate
                )
            )
        );
    }

    public void ReceiveUnityMessage(string message)
    {
        byte[] messageData = BLEProtocol.ConvertStringToBytes(message);
        if (!BLEProtocol.ValidateData(messageData))
        {
            BLEDebug.LogError("Received Message Failed Validation - Ignoring");
        }
        byte actionCode = BLEProtocol.GetActionCode(messageData);

        switch (actionCode)
        {
            case BLEProtocol.ActionCode.Acknowledge:
                // Factory doesn't use this so we shouldn't be using it either
                BLEDebug.LogWarning("Acknowledge Received");
                break;
            case BLEProtocol.ActionCode.GetDeviceInformation:
                BLEPluginInstance.SendMessage("ReceivePluginMessage",
                    BLEProtocol.ConvertBytesToString(
                        BLEResponse.PrepareDeviceInformationBytes(
                            modelID,
                            hardwareMajor,
                            hardwareMinor,
                            firmwareMajor,
                            firmwareMinor,
                            firmwarePatch
                        )
                    )
                );
                break;
            case BLEProtocol.ActionCode.GetErrorLog:
                // TODO - Low Priority
                break;
            case BLEProtocol.ActionCode.GetResistanceLevelRange:
                BLEPluginInstance.SendMessage("ReceivePluginMessage",
                    BLEProtocol.ConvertBytesToString(
                        BLEResponse.PrepareResistanceLevelRangeBytes(
                            resistanceLevelMin,
                            resitanceLevelMax
                        )
                    )
                );
                break;
            case BLEProtocol.ActionCode.GetWorkoutControlState:
                BLEPluginInstance.SendMessage("ReceivePluginMessage",
                    BLEProtocol.ConvertBytesToString(
                        BLEResponse.PrepareWorkoutControlStateBytes(
                            controlState
                        )
                    )
                );
                break;
            case BLEProtocol.ActionCode.GetResistanceLevel:
                BLEPluginInstance.SendMessage("ReceivePluginMessage",
                    BLEProtocol.ConvertBytesToString(
                        BLEResponse.PrepareResistanceLevelBytes(
                            resistanceLevel
                        )
                    )
                );
                break;
            case BLEProtocol.ActionCode.SetWorkoutControlState:
                controlState = messageData[BLEProtocol.Index.WorkoutControlState];
                BLEPluginInstance.SendMessage("ReceivePluginMessage",
                    BLEProtocol.ConvertBytesToString(
                        BLENotify.PrepareWorkoutControlStateBytes(
                            controlState
                        )
                    )
                );
                break;
            case BLEProtocol.ActionCode.SetResistanceLevel:
                resistanceLevel = messageData[BLEProtocol.Index.ResistanceLevel];
                BLEPluginInstance.SendMessage("ReceivePluginMessage",
                    BLEProtocol.ConvertBytesToString(
                        BLENotify.PrepareResistanceLevelBytes(
                            resistanceLevel
                        )
                    )
                );
                break;
            default:
                BLEDebug.LogError("Error: Received Invalid ActionCode");
                break;
        }
    }
}