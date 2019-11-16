using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    // Adjustments need to be made later
    public static void SavePlayerData(float speed, float rpm, float calories)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "PlayerData");

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            PlayerData data = new PlayerData(speed, rpm, calories);
            formatter.Serialize(stream, data);
        }
    }

    public static PlayerData loadPlayerData()
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

    public static void SaveChallengeData(bool[] statuses)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "ChallengeData");

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            ChallengeData data = new ChallengeData(statuses);
            formatter.Serialize(stream, data);
        }
    }

    public static ChallengeData loadChallengeData()
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
