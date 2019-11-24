using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    // Save the player data to a path
    public static void SavePlayerData(float speed, float rpm, float calories, float distance)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "PlayerData");

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            PlayerData data = new PlayerData(speed, rpm, calories, distance);
            formatter.Serialize(stream, data);
        }
    }

    // Load the player data from a path
    public static PlayerData LoadPlayerData()
    {
        string path = Path.Combine(Application.persistentDataPath, "PlayerData");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save data not found in " + path);
            return null;
        }
    }

    // Save the challenge data to a path
    public static void SaveChallengeData(bool[] statuses, float distance)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "ChallengeData");

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            ChallengeData data = new ChallengeData(statuses, distance);
            formatter.Serialize(stream, data);
        }
    }

    // Reset the challenge data of a path
    public static void ResetChallengeData()
    {
        string path = Path.Combine(Application.persistentDataPath, "ChallengeData");
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            ChallengeData data = new ChallengeData();
            formatter.Serialize(stream, data);
        }
    }

    // Load the challenge data from a path
    public static ChallengeData LoadChallengeData()
    {
        string path = Path.Combine(Application.persistentDataPath, "ChallengeData");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                ChallengeData data = formatter.Deserialize(stream) as ChallengeData;
                return data;
            }
        }
        else
        {
            Debug.LogError("Save data not found in " + path);
            return null;
        }
    }
}
