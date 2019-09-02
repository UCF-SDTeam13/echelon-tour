public static class BLEResponse
{
    public static byte[] PrepareDeviceInformationBytes
    (int modelID, int hardwareMajor, int hardwareMinor, int firmwareMajor, int firmwareMinor, int firmwarePatch)
    {
        byte[] preparedBytes = new byte[] {
            BLEProtocol.StartByte,
            BLEProtocol.ResponseCode.DeviceInformationResponse,
            0x06,
            (byte) modelID,
            (byte) hardwareMajor,
            (byte) hardwareMinor,
            (byte) firmwareMajor,
            (byte) firmwareMinor,
            (byte) firmwarePatch,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = BLEProtocol.CalculateChecksum(preparedBytes);
        return preparedBytes;
    }

    public static byte[] PrepareResistanceLevelRangeBytes(int min, int max)
    {
        byte[] preparedBytes = new byte[] {
            BLEProtocol.StartByte,
            BLEProtocol.ResponseCode.ResistanceLevelRangeResponse,
            0x02,
            (byte) max,
            (byte) min,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = BLEProtocol.CalculateChecksum(preparedBytes);
        return preparedBytes;
    }

    public static byte[] PrepareWorkoutControlStateBytes(int controlState)
    {
        byte[] preparedBytes = new byte[] {
            BLEProtocol.StartByte,
            BLEProtocol.ResponseCode.WorkoutControlStateResponse,
            0x01,
            (byte) controlState,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = BLEProtocol.CalculateChecksum(preparedBytes);
        return preparedBytes;
    }

    public static byte[] PrepareResistanceLevelBytes(int resistanceLevel)
    {
        byte[] preparedBytes = new byte[] {
            BLEProtocol.StartByte,
            BLEProtocol.ResponseCode.ResistanceLevelResponse,
            0x01,
            (byte) resistanceLevel,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = BLEProtocol.CalculateChecksum(preparedBytes);
        return preparedBytes;
    }
}