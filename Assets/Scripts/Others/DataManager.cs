using UnityEngine;

public class DataManager : MonoBehaviour
{
    public void SaveData(float speed, float rpm, float calories)
    {
        SaveSystem.SaveData(speed, rpm, calories);
    }

    public PlayerData LoadData()
    {
        return SaveSystem.loadData();
    }
}
