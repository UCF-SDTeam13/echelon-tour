using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(float speed, float rpm, float calories)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "PlayerStatistics");
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(speed, rpm, calories);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData loadData()
    {
        string path = Path.Combine(Application.persistentDataPath, "PlayerStatistics");
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save data not found in " + path);
            return null;
        }
    }
}
