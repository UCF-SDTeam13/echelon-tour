using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Examples
    public void SavePlayerData(float speed, float rpm, float calories, float distance)
    {
        SaveSystem.SavePlayerData(speed, rpm, calories, distance);
    }

    public PlayerData LoadPlayerData()
    {
        return SaveSystem.LoadPlayerData();
    }

    public void SaveChallengeData(bool[] statuses, float distance)
    {
        SaveSystem.SaveChallengeData(statuses, distance);
    }

    public void ResetChallengeData()
    {
        SaveSystem.ResetChallengeData();
    }

    public ChallengeData LoadChallengeData()
    {
        return SaveSystem.LoadChallengeData();
    }
}
