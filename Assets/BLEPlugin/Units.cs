
//Usage: multiply by ratios
public static class Units
{
    public struct Ratio
    {
        public const double MinutesToSeconds = 60;
        public const double SecondsToMinutes = 1 / 60;
        public const double RPMToSpeedLevel = 1 / 10;
    }

    public struct Min
    {
        public const int ResistanceLevel = 0;
        public const int SpeedLevel = 0;
    }
    public struct Max
    {
        public const int ResistanceLevel = 32;
        public const int SpeedLevel = 10;
    }
    public static int ClampValueToRange(int value, int rangeStart, int rangeEnd)
    {
        if (value > rangeEnd)
        {
            value = rangeEnd;
        }
        else if (value < rangeStart)
        {
            value = rangeStart;
        }
        return value;
    }
}