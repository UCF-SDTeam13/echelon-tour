using UnityEngine;
using UnityEngine.UI;

public class DataTesting : MonoBehaviour
{
    public Text speedText;
    public Text rpmText;
    public Text caloriesText;
    public Text distanceText;

    private float speed = 0;
    private float rpm = 0;
    private float calories = 0;
    private float distance = 0;

    private void Start()
    {
        SetText();
    }

    public void RollRandom()
    {
        speed = Random.Range(0, 100);
        rpm = Random.Range(0, 100);
        calories = Random.Range(0, 100);
        distance = Random.Range(0, 100);
        SetText();
    }

    public void SetText()
    {
        speedText.text = speed.ToString();
        rpmText.text = rpm.ToString();
        caloriesText.text = calories.ToString();
        distanceText.text = distance.ToString();
    }

    public void SavePlayerData()
    {
        SaveSystem.SavePlayerData(speed, rpm, calories, distance);
    }

    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadPlayerData();

        speed = data.avgSpeed;
        rpm = data.avgRPM;
        calories = data.avgCalories;
        distance = data.totalDistance;

        SetText();
    }
}