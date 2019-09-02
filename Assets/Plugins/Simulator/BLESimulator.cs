
using UnityEngine;
using System;

public sealed class BLESimulator
{
    private static readonly Lazy<BLESimulator>
    _BLESimulator = new Lazy<BLESimulator>(() => new BLESimulator());
    public static BLESimulator Instance => _BLESimulator.Value;

    private readonly GameObject BLEPluginInstance;

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

    private BLESimulator()
    {
        BLEPluginInstance = GameObject.Find("BLEPlugin");
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