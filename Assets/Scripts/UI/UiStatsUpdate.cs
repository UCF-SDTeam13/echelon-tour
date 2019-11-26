using System;
using UnityEngine;

public class UiStatsUpdate : MonoBehaviour
{
    private readonly float[][] PowerOutputTable = new float[][]
    {
        new float[] {0,  7.1f, 13.5f,  25.0f,  28.5f,  39.5f,  51.5f,  65.5f,  78.5f,  92.0f, 105.6f}, // Resistance Level 0
        new float[] {0,  7.1f, 13.5f,  25.0f,  28.5f,  39.5f,  51.5f,  65.5f,  78.5f,  92.0f, 105.6f},
        new float[] {0,  7.6f, 14.0f,  26.0f,  29.0f,  40.0f,  52.3f,  66.0f,  79.0f,  92.5f, 106.3f},
        new float[] {0,  8.1f, 14.5f,  27.0f,  29.6f,  40.5f,  53.0f,  66.8f,  80.3f,  94.1f, 107.7f},
        new float[] {0,  8.7f, 15.0f,  27.8f,  30.1f,  41.3f,  54.0f,  67.6f,  82.6f,  97.5f, 111.4f},
        new float[] {0,  9.1f, 15.5f,  28.7f,  31.0f,  42.6f,  55.7f,  70.7f,  85.7f, 100.5f, 115.4f},
        new float[] {0,  9.6f, 16.0f,  29.0f,  31.7f,  44.4f,  57.5f,  73.0f,  88.3f, 103.8f, 119.3f},
        new float[] {0,  0.2f, 16.5f,  30.0f,  33.5f,  45.9f,  60.6f,  75.8f,  92.1f, 108.0f, 124.0f},
        new float[] {0, 10.8f, 17.0f,  30.5f,  35.0f,  47.3f,  62.2f,  79.1f,  96.0f, 113.0f, 129.6f},
        new float[] {0, 11.3f, 17.5f,  32.0f,  35.5f,  48.8f,  64.2f,  81.5f,  99.1f, 116.0f, 134.0f},
        new float[] {0, 11.9f, 18.0f,  33.0f,  36.2f,  50.6f,  66.4f,  88.2f, 102.8f, 120.5f, 138.7f}, // Resistance Level 10
        new float[] {0, 12.3f, 18.5f,  34.0f,  38.5f,  52.3f,  68.8f,  91.1f, 107.3f, 125.0f, 144.0f},
        new float[] {0, 12.9f, 19.1f,  35.2f,  39.0f,  54.4f,  71.7f,  95.6f, 111.1f, 130.7f, 150.0f},
        new float[] {0, 13.4f, 19.6f,  37.0f,  40.6f,  59.5f,  75.1f,  99.7f, 114.8f, 136.0f, 157.0f},
        new float[] {0, 14.1f, 20.2f,  38.5f,  42.6f,  62.2f,  78.5f, 104.7f, 121.5f, 138.0f, 158.0f},
        new float[] {0, 14.8f, 20.8f,  40.0f,  45.0f,  65.5f,  82.5f, 105.6f, 122.0f, 143.2f, 164.2f},
        new float[] {0, 15.4f, 21.8f,  41.5f,  45.9f,  68.9f,  86.5f, 111.2f, 124.0f, 150.8f, 172.7f},
        new float[] {0, 16.2f, 22.5f,  44.0f,  46.5f,  72.3f,  90.1f, 117.1f, 128.5f, 158.6f, 181.5f},
        new float[] {0, 17.1f, 23.5f,  45.5f,  47.3f,  74.5f,  92.5f, 124.1f, 135.2f, 165.5f, 189.8f},
        new float[] {0, 17.9f, 23.9f,  47.2f,  49.0f,  76.8f,  97.1f, 131.5f, 142.0f, 176.5f, 201.0f},
        new float[] {0, 18.5f, 24.1f,  49.7f,  51.0f,  82.5f, 102.7f, 140.7f, 150.8f, 185.0f, 212.0f}, // Resistance Level 20
        new float[] {0, 19.1f, 24.8f,  53.8f,  54.4f,  84.6f, 109.7f, 151.0f, 159.0f, 198.0f, 226.3f},
        new float[] {0, 20.1f, 26.0f,  56.2f,  57.0f,  87.8f, 117.8f, 165.0f, 169.9f, 205.5f, 241.0f},
        new float[] {0, 21.4f, 27.5f,  57.0f,  58.5f,  92.9f, 125.4f, 188.0f, 181.5f, 212.3f, 256.0f},
        new float[] {0, 22.3f, 29.5f,  60.8f,  62.5f, 101.6f, 135.9f, 199.0f, 193.8f, 225.4f, 274.0f},
        new float[] {0, 23.5f, 31.5f,  64.8f,  66.2f, 109.6f, 147.5f, 223.2f, 209.0f, 241.5f, 293.5f},
        new float[] {0, 24.7f, 33.5f,  70.0f,  71.5f, 119.7f, 159.9f, 245.9f, 224.4f, 256.5f, 312.8f},
        new float[] {0, 26.6f, 36.5f,  75.3f,  77.8f, 133.1f, 174.3f, 278.3f, 239.0f, 276.4f, 348.5f},
        new float[] {0, 28.1f, 40.0f,  84.8f,  83.9f, 144.2f, 192.5f, 306.9f, 267.5f, 308.5f, 381.0f},
        new float[] {0, 29.7f, 44.5f,  92.1f,  93.5f, 163.8f, 219.7f, 334.1f, 295.4f, 365.7f, 422.2f},
        new float[] {0, 31.6f, 50.0f, 105.8f, 103.5f, 181.6f, 242.8f, 358.7f, 330.0f, 420.0f, 465.0f}, // Resistance Level 30
        new float[] {0, 33.1f, 57.0f, 117.5f, 114.8f, 207.4f, 280.4f, 375.2f, 367.2f, 470.4f, 528.7f},
        new float[] {0, 36.7f, 62.0f, 125.1f, 126.9f, 239.8f, 320.2f, 406.4f, 425.0f, 520.2f, 600.0f}
    };

    private readonly float[] EnergyTable = new float[]
    {
        0.072f, // Resistance Level 0
        0.072f,
        0.084f,
        0.096f,
        0.118f,
        0.136f,
        0.162f,
        0.185f,
        0.210f,
        0.230f,
        0.253f, // Resistance Level 10
        0.278f,
        0.305f,
        0.330f,
        0.360f,
        0.391f,
        0.433f,
        0.475f,
        0.517f,
        0.559f,
        0.601f, // Resistance Level 20
        0.643f,
        0.685f,
        0.727f,
        0.769f,
        0.811f,
        0.853f,
        0.895f,
        0.937f,
        0.979f,
        1.021f, // Resistance Level 30
        1.063f,
        1.105f
    };

    // The NEED THE f
    public struct Ratio
    {
        public const float MinutesToSeconds = 60;
        public const float SecondsToMinutes = 1 / 60f;
        public const float RPMToSpeedLevel = 1 / 10f;
        public const float KMPHToMPH = 0.62137f;
    }

    public struct Min
    {
        public const int ResistanceLevel = 1;
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

    public float PowerOutput { get; private set; } = 0;

    private int _resistanceLevel = 1;
    public int ResistanceLevel
    {
        get
        {
            return _resistanceLevel;
        }
        set
        {
            // Clamp resistance level to 0 - 32
            _resistanceLevel = ClampValueToRange(value,
                Min.ResistanceLevel, Max.ResistanceLevel);
        }
    }

    private int _speedLevel = 0;
    public int SpeedLevel
    {
        get
        {
            return _speedLevel;
        }
        private set
        {
            _speedLevel = ClampValueToRange(value, Min.SpeedLevel, Max.SpeedLevel);
        }
    }

    public float CalculatePowerOutput()
    {
        SpeedLevel = Mathf.RoundToInt(Bike.Instance.RPM * Ratio.RPMToSpeedLevel);
        return PowerOutputTable[ResistanceLevel][SpeedLevel];
    }

    public float CalculateCalories()
    {
        return EnergyTable[ResistanceLevel] * 60;
    }

    public float CalculateSpeed()
    {
        float rpmRatio = 1;
        float wheelDiameter = (78 * 2.54f) / 100000;
        float distancePerCount = wheelDiameter * Mathf.PI * rpmRatio;
        float speedMultiplier = distancePerCount * 60;
        return Bike.Instance.RPM * speedMultiplier * Ratio.KMPHToMPH;
    }

    public float CalculateDistance()
    {
        float wheelDiameter = (78 * 2.54f) / 100000;
        float wheelCircumference = wheelDiameter * Mathf.PI;
        return Bike.Instance.Count * wheelCircumference;
    }

    public int CalculateRPM()
    {
        return Bike.Instance.RPM;
    }

    public int CalculateResistance()
    {
        return Bike.Instance.ResistanceLevel;
    }
}
