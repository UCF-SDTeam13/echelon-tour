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

    public static byte[] PrepareWorkoutStatusBytes(int timestamp, int count, int rpm, int heartrate)
    {
        byte[] timestampData = BLEProtocol.ExtractIntData(timestamp,
        BLEProtocol.Index.TimeStamp.Begin, BLEProtocol.Index.TimeStamp.End);

        byte[] rpmData = BLEProtocol.ExtractIntData(rpm,
        BLEProtocol.Index.RPM.Begin, BLEProtocol.Index.RPM.End);

        byte[] countData = BLEProtocol.ExtractIntData(count,
        BLEProtocol.Index.Count.Begin, BLEProtocol.Index.Count.End);

        byte[] preparedBytes = new byte[] {
            BLEProtocol.StartByte,
            BLEProtocol.ResponseCode.WorkoutStatusNotification,
            0x09,
            timestampData[0],
            timestampData[1],
            countData[0],
            countData[1],
            countData[2],
            countData[3],
            rpmData[0],
            rpmData[1],
            (byte) heartrate,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = BLEProtocol.CalculateChecksum(preparedBytes);
        return preparedBytes;
    }
}