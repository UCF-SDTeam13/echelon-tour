using System;

public class Workout
{
    private static readonly Lazy<Workout>
    _Workout = new Lazy<Workout>(() => new Workout());
    public static Workout Instance => _Workout.Value;

    //Note: I'm pretty sure calculating calories this way is wrong but it's how the factory does it
    //Energy Consumed in Calories during a period of 1 minute at Resistance Level
    private readonly double[] EnergyTable = new double[]
    {
        0.072, // Resistance Level 0
        0.072,
        0.084,
        0.096,
        0.118,
        0.136,
        0.162,
        0.185,
        0.210,
        0.230,
        0.253, // Resistance Level 10
        0.278,
        0.305,
        0.330,
        0.360,
        0.391,
        0.433,
        0.475,
        0.517,
        0.559,
        0.601, // Resistance Level 20
        0.643,
        0.685,
        0.727,
        0.769,
        0.811,
        0.853,
        0.895,
        0.937,
        0.979,
        1.021, // Resistance Level 30
        1.063,
        1.105
    };

    //TODO: Increase accuracy with multiple linear regression (or 32 linear)
    // though i'm not sure if that will work because E=1/2Ir^2
    //Power Output in Watts for a given Resistance level and RPM level
    private readonly double[][] PowerOutputTable = new double[][]
    {
        new double[] {0,  7.1, 13.5,  25.0,  28.5,  39.5,  51.5,  65.5,  78.5,  92.0, 105.6}, // Resistance Level 0
        new double[] {0,  7.1, 13.5,  25.0,  28.5,  39.5,  51.5,  65.5,  78.5,  92.0, 105.6},
        new double[] {0,  7.6, 14.0,  26.0,  29.0,  40.0,  52.3,  66.0,  79.0,  92.5, 106.3},
        new double[] {0,  8.1, 14.5,  27.0,  29.6,  40.5,  53.0,  66.8,  80.3,  94.1, 107.7},
        new double[] {0,  8.7, 15.0,  27.8,  30.1,  41.3,  54.0,  67.6,  82.6,  97.5, 111.4},
        new double[] {0,  9.1, 15.5,  28.7,  31.0,  42.6,  55.7,  70.7,  85.7, 100.5, 115.4},
        new double[] {0,  9.6, 16.0,  29.0,  31.7,  44.4,  57.5,  73.0,  88.3, 103.8, 119.3},
        new double[] {0,  0.2, 16.5,  30.0,  33.5,  45.9,  60.6,  75.8,  92.1, 108.0, 124.0},
        new double[] {0, 10.8, 17.0,  30.5,  35.0,  47.3,  62.2,  79.1,  96.0, 113.0, 129.6},
        new double[] {0, 11.3, 17.5,  32.0,  35.5,  48.8,  64.2,  81.5,  99.1, 116.0, 134.0},
        new double[] {0, 11.9, 18.0,  33.0,  36.2,  50.6,  66.4,  88.2, 102.8, 120.5, 138.7}, // Resistance Level 10
        new double[] {0, 12.3, 18.5,  34.0,  38.5,  52.3,  68.8,  91.1, 107.3, 125.0, 144.0},
        new double[] {0, 12.9, 19.1,  35.2,  39.0,  54.4,  71.7,  95.6, 111.1, 130.7, 150.0},
        new double[] {0, 13.4, 19.6,  37.0,  40.6,  59.5,  75.1,  99.7, 114.8, 136.0, 157.0},
        new double[] {0, 14.1, 20.2,  38.5,  42.6,  62.2,  78.5, 104.7, 121.5, 138.0, 158.0},
        new double[] {0, 14.8, 20.8,  40.0,  45.0,  65.5,  82.5, 105.6, 122.0, 143.2, 164.2},
        new double[] {0, 15.4, 21.8,  41.5,  45.9,  68.9,  86.5, 111.2, 124.0, 150.8, 172.7},
        new double[] {0, 16.2, 22.5,  44.0,  46.5,  72.3,  90.1, 117.1, 128.5, 158.6, 181.5},
        new double[] {0, 17.1, 23.5,  45.5,  47.3,  74.5,  92.5, 124.1, 135.2, 165.5, 189.8},
        new double[] {0, 17.9, 23.9,  47.2,  49.0,  76.8,  97.1, 131.5, 142.0, 176.5, 201.0},
        new double[] {0, 18.5, 24.1,  49.7,  51.0,  82.5, 102.7, 140.7, 150.8, 185.0, 212.0}, // Resistance Level 20
        new double[] {0, 19.1, 24.8,  53.8,  54.4,  84.6, 109.7, 151.0, 159.0, 198.0, 226.3},
        new double[] {0, 20.1, 26.0,  56.2,  57.0,  87.8, 117.8, 165.0, 169.9, 205.5, 241.0},
        new double[] {0, 21.4, 27.5,  57.0,  58.5,  92.9, 125.4, 188.0, 181.5, 212.3, 256.0},
        new double[] {0, 22.3, 29.5,  60.8,  62.5, 101.6, 135.9, 199.0, 193.8, 225.4, 274.0},
        new double[] {0, 23.5, 31.5,  64.8,  66.2, 109.6, 147.5, 223.2, 209.0, 241.5, 293.5},
        new double[] {0, 24.7, 33.5,  70.0,  71.5, 119.7, 159.9, 245.9, 224.4, 256.5, 312.8},
        new double[] {0, 26.6, 36.5,  75.3,  77.8, 133.1, 174.3, 278.3, 239.0, 276.4, 348.5},
        new double[] {0, 28.1, 40.0,  84.8,  83.9, 144.2, 192.5, 306.9, 267.5, 308.5, 381.0},
        new double[] {0, 29.7, 44.5,  92.1,  93.5, 163.8, 219.7, 334.1, 295.4, 365.7, 422.2},
        new double[] {0, 31.6, 50.0, 105.8, 103.5, 181.6, 242.8, 358.7, 330.0, 420.0, 465.0}, // Resistance Level 30
        new double[] {0, 33.1, 57.0, 117.5, 114.8, 207.4, 280.4, 375.2, 367.2, 470.4, 528.7},
        new double[] {0, 36.7, 62.0, 125.1, 126.9, 239.8, 320.2, 406.4, 425.0, 520.2, 600.0}
    };
    private double _WheelDiameter = 78;

    // Wheel Diameter in Inches
    public double WheelDiameter
    {
        get
        {
            return _WheelDiameter;
        }
        set
        {
            _WheelDiameter = value;
        }
    }

    //Rotation per Minute - Read Only
    public double RPM { get; private set; } = 0;
    // Energy Burned in Calories - Read Only
    public double CaloriesBurned { get; private set; } = 0;
    // Distance Traveled in Inches - Read Only
    public double DistanceTraveled { get; private set; } = 0;
    // Power Output in Watts - Read Only
    public double PowerOutput { get; private set; } = 0;

    // SpeedLevel for PowerOutput Table (Rounded RPM/10 Range of 0 - 10) - Read Only
    private int _speedLevel = 0;
    public int SpeedLevel
    {
        get
        {
            return _speedLevel;
        }
        private set
        {
            _speedLevel = Units.ClampValueToRange(value, Units.Min.SpeedLevel, Units.Max.SpeedLevel);
        }
    }

    // Resistance Level (Range of 0 - 32)
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
            _resistanceLevel = Units.ClampValueToRange(value,
                Units.Min.ResistanceLevel, Units.Max.ResistanceLevel);
        }
    }

    private int _controlState = BLEProtocol.WorkoutControlState.Stop;
    public int ControlState
    {
        get
        {
            return _controlState;
        }
        set
        {
            // Ignore and Log Invalid States
            // TODO - Do this properly since range clamping shouldn't be used for states
            _controlState = Units.ClampValueToRange(value, BLEProtocol.WorkoutControlState.Stop, BLEProtocol.WorkoutControlState.Pause);
        }
    }
    //deltaTime should be in seconds
    public void RecordRPM(double rpm, double deltaTime)
    {
        RPM = rpm;
        // Covert from Rotations per Minute to per Second
        double rps = RPM * Units.Ratio.MinutesToSeconds;
        // Rotations = RPS * Seconds
        double rotations = rps * deltaTime;
        // Circumference = 2*Pi*r = Pi*d
        // Distance = rotations * circumference
        double distance = rotations * Math.PI * WheelDiameter;
        DistanceTraveled += distance;

        CaloriesBurned += EnergyTable[ResistanceLevel] * Units.Ratio.MinutesToSeconds;
        // Calculate Speed level (RPM / 10)
        SpeedLevel = (int)Math.Round(RPM * Units.Ratio.RPMToSpeedLevel);
        // Calculate Power output using Power table
        PowerOutput = PowerOutputTable[ResistanceLevel][SpeedLevel];
    }
    public void ResetWorkout()
    {
        CaloriesBurned = 0;
        DistanceTraveled = 0;
    }
}