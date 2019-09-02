public static class BLENotify
{
    public static byte[] PrepareWorkoutControlStateBytes(int controlState)
    {
        byte[] preparedBytes = new byte[] {
            BLEProtocol.StartByte,
            BLEProtocol.ResponseCode.WorkoutControlStateNotification,
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
            BLEProtocol.ResponseCode.ResistanceLevelNotification,
            0x01,
            (byte) resistanceLevel,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = BLEProtocol.CalculateChecksum(preparedBytes);
        return preparedBytes;
    }
}