using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Examples
    public void SavePlayerData(float speed, float rpm, float calories)
    {
        SaveSystem.SavePlayerData(speed, rpm, calories);
    }

    public PlayerData LoadPlayerData()
    {
        return SaveSystem.loadPlayerData();
    }

    public void SaveChallengeData(bool[] statuses)
    {
        SaveSystem.SaveChallengeData(statuses);
    }

    public ChallengeData LoadChallengeData()
    {
        return SaveSystem.loadChallengeData();
    }
}
