using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class livestats : MonoBehaviour
{
    public GameObject uiStatsUpdate;
    private UiStatsUpdate getUpdatedStats;
    
    public Text rpmText;
    public Text speedText;
    public Text resistanceText;
    public Text wattsText;
    public Text distanceText;
    public Text caloriesText;

    int rpm = 0;
    float speed = 0f;
    int resistance = 1;
    float watts = 0f;
    float distance = 0f;
    float calories = 0f;

    // Start is called before the first frame update
    void Start()
    {
        getUpdatedStats = uiStatsUpdate.GetComponent<UiStatsUpdate>();
        BLEPlugin.Instance.StartWorkout();
    }

    // Update is called once per frame
    void Update()
    {
        updateUIText();
    }

    public void updateUIText()
    {
        rpm = getUpdatedStats.CalculateRPM();
        rpmText.text = rpm.ToString();

        speed = getUpdatedStats.CalculateSpeed();
        speedText.text = speed.ToString();

        resistance = getUpdatedStats.CalculateResistance();
        resistanceText.text = resistance.ToString();

        watts = getUpdatedStats.CalculatePowerOutput();
        wattsText.text = watts.ToString();

        distance = getUpdatedStats.CalculateDistance();
        distanceText.text = distance.ToString("F2");

        // Need to be event for this one
        calories += getUpdatedStats.CalculateCalories();
        caloriesText.text = "TBD"; //calories.ToString();
    }
}