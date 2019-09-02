using System;
using System.Linq;
public static class BLEProtocol
{
    public struct Index
    {
        // General Index
        public const int
        Preamble = 0,
        ActionCode = 1,
        FrameLength = 2,
        FrameStart = 3,
        // Error Log
        ErrorLogStart = 3;
        // Reistance Level Range
        public struct ResistanceLevelRange
        {
            public const int
            Min = 4,
            Max = 3;
        }
        // Resistance Level
        public const int ResistanceLevel = 3;
        // Device Information
        public const int ModelID = 3;
        public struct HardwareVersion
        {
            public const int
            Major = 4,
            Minor = 5;
        }
        public struct FirmwareVersion
        {
            public const int
            Major = 6,
            Minor = 7,
            Patch = 8;
        }
        // Workout Control State
        public const int WorkoutControlState = 3;
        // Workout Status
        public struct TimeStamp
        {
            public const int
            Begin = 3,
            End = 4;
        }
        public struct Count
        {
            public const int
            Begin = 5,
            End = 8;
        }
        public struct RPM
        {
            public const int
            Begin = 9,
            End = 10;
        }
        public const int HeartRate = 11;
    }

    public struct ActionCode
    {
        public const byte
        Acknowledge = 0xA0,
        GetDeviceInformation = 0xA1,
        GetErrorLog = 0xA2,
        GetResistanceLevelRange = 0xA3,
        GetWorkoutControlState = 0xA4,
        GetResistanceLevel = 0xA5,
        SetWorkoutControlState = 0xB0,
        SetResistanceLevel = 0xB1;
    }

    public struct ResponseCode
    {
        public const byte
        Acknowledge = ActionCode.Acknowledge,
        DeviceInformationResponse = ActionCode.GetDeviceInformation,
        ErrorLogResponse = ActionCode.GetErrorLog,
        ResistanceLevelRangeResponse = ActionCode.GetResistanceLevelRange,
        WorkoutControlStateResponse = ActionCode.GetWorkoutControlState,
        ResistanceLevelResponse = ActionCode.GetResistanceLevel,
        WorkoutControlStateNotification = 0xD0,
        WorkoutStatusNotification = 0xD1,
        ResistanceLevelNotification = 0xD2;
    }
    public struct WorkoutControlState
    {
        public const int
        Stop = 0,
        Start = 1,
        Pause = 2;
    }
    public const byte StartByte = 0xF0;
    public const int MinimumCommandLength = 4;
    // Frame Layout:
    // |Preamble|ActionCode/ResponseCode|FrameLength|FrameData|Checksum

    public static bool ValidateData(byte[] data)
    {
        if (data.Length < MinimumCommandLength)
        {
            //Not long enough to read header and checksum - invalid
            return false;
        }
        int frameLength = data[Index.FrameLength];

        if (MinimumCommandLength + frameLength != data.Length)
        {
            return false;
        }

        return ValidateChecksum(data);
    }
    public static byte GetActionCode(byte[] data)
    {
        return data[Index.ActionCode];
    }
    public static byte GetResponseCode(byte[] data)
    {
        // ResponseCode "should" be in the same location as ActionCode
        return GetActionCode(data);
    }
    public static byte CalculateChecksum(byte[] data)
    {
        byte checksumExpected = 0;
        for (int i = 0; i < data.Length - 1; ++i)
        {
            checksumExpected += data[i];
        }

        // Bytes in C# / Kotlin are unsigned, but are signed in Java
        //checksumExpected &= 0xff;

        return checksumExpected;
    }
    private static bool ValidateChecksum(byte[] data)
    {
        byte checksum = CalculateChecksum(data);
        return (checksum == data[data.Length - 1]);
    }

    public static byte[] PrepareGetCommandBytes(byte actionCode)
    {
        byte[] preparedBytes = new byte[] {
            StartByte,
            actionCode,
            0x00,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = CalculateChecksum(preparedBytes);
        return preparedBytes;
    }

    public static byte[] PrepareSetCommandBytes(byte actionCode, int value)
    {
        byte[] preparedBytes = new byte[] {
            StartByte,
            actionCode,
            0x01,
            (byte) value,
            0x00
        };
        // Calculate and set checksum
        preparedBytes[preparedBytes.Length - 1] = CalculateChecksum(preparedBytes);
        return preparedBytes;
    }

    public static string ConvertBytesToString(byte[] data)
    {
        string message = System.Convert.ToBase64String(data);
        BLEDebug.LogInfo("Converted Byte to String: " + message);
        return message;
    }

    public static byte[] ConvertStringToBytes(string message)
    {
        return System.Convert.FromBase64String(message);
    }

    public static int ExtractDataInt(int beginIndex, int endIndex, byte[] data)
    {
        data = data.Skip(beginIndex).Take(endIndex - beginIndex + 1).Reverse().ToArray();
        if (data.Length == 4)
        {
            return BitConverter.ToInt32(data, 0);
        }
        else
        {
            return BitConverter.ToInt16(data, 0);
        }
    }
    
    public static byte[] ExtractIntData(int value, int beginIndex, int endIndex)
    {
        int length = endIndex - beginIndex + 1;
        if (length == 4)
        {
            return BitConverter.GetBytes((Int32)value).Reverse().ToArray();
        }
        else
        {
            return BitConverter.GetBytes((Int16)value).Reverse().ToArray();
        }
    }
}