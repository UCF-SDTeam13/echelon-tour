[System.Serializable]
public class PlayerData
{
    public float avgSpeed;
    public float avgRPM;
    public float avgCalories;
    // Other things depending on what personal stats we want to show

    public PlayerData(float speed, float rpm, float calories)
    {
        if(speed > avgSpeed)
        {
            avgSpeed = speed;
        }

        if(rpm > avgRPM)
        {
            avgRPM = rpm;
        }

        if(calories > avgCalories)
        {
            avgCalories = calories;
        }
    }
}

[System.Serializable]
public class ChallengeData
{
    public bool[] challengeStatuses;

    public ChallengeData(bool[] statuses)
    {
        // Check if the challenges array is null
        if (challengeStatuses == null)
        {
            // Create a new array
            challengeStatuses = new bool[statuses.Length];
        }

        // Loop and check if any status can be checked off
        for (int i = 0; i < statuses.Length; i++)
        {
            // Only check off if the current status is false and the incoming status is true
            if (challengeStatuses[i] == false && statuses[i] == true)
            {
                challengeStatuses[i] = true;
            }
        }
    }
}
