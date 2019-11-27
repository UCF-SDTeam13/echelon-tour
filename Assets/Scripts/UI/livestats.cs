using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class livestats : MonoBehaviour
{
    //public GameObject uiStatsUpdate;
    //private UiStatsUpdate getUpdatedStats;

    public Text rpmText;
    public Text speedText;
    public Text resistanceText;
    public Text wattsText;
    public Text distanceText;
    public Text caloriesText;

    int rpm = 0;
    float speed = 0f;
    int resistance = 0;
    float watts = 0f;
    float distance = 0f;
    float calories = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //getUpdatedStats = uiStatsUpdate.GetComponent<UiStatsUpdate>();
        BLEPlugin.Instance.StartWorkout();
        StartCoroutine("UpdateUiStats");
    }

    /*
    // Update is called once per frame
    void Update()
    {
        //updateUIText();
    }

    public void updateUIText()
    {
        //rpm = getUpdatedStats.CalculateRPM();
        rpm = WorkoutCalculations.CalculateRPM();
        rpmText.text = rpm.ToString();

        //speed = getUpdatedStats.CalculateSpeed();
        speed = WorkoutCalculations.CalculateSpeed(Bike.Instance.RPM);
        speedText.text = speed.ToString();

        //resistance = getUpdatedStats.CalculateResistance();
        resistance = WorkoutCalculations.CalculateResistance();
        resistanceText.text = resistance.ToString();

        //watts = getUpdatedStats.CalculatePowerOutput();
        watts = WorkoutCalculations.CalculatePowerOutput(Bike.Instance.RPM);
        wattsText.text = watts.ToString();

        //distance = getUpdatedStats.CalculateDistance();
        distance = WorkoutCalculations.CalculateDistance(Bike.Instance.Count);
        distanceText.text = distance.ToString("F2");

        // Need to be event for this one
        //calories += getUpdatedStats.CalculateCalories();
        caloriesText.text = "TBD"; //calories.ToString();
    }
    */
    
    IEnumerator UpdateUiStats()
    {
        while (true)
        {
            rpm = WorkoutCalculations.CalculateRPM();
            rpmText.text = rpm.ToString();

            speed = WorkoutCalculations.CalculateSpeed(Bike.Instance.RPM);
            speedText.text = speed.ToString("F2");

            resistance = WorkoutCalculations.CalculateResistance();
            resistanceText.text = resistance.ToString();

            watts = WorkoutCalculations.CalculatePowerOutput(Bike.Instance.RPM);
            wattsText.text = watts.ToString();

            if (rpm != 0)
            {
                distance = WorkoutCalculations.CalculateDistance(Bike.Instance.Count);
                distanceText.text = distance.ToString("F2");

                calories += WorkoutCalculations.CalculateCalories();
                caloriesText.text = calories.ToString();
            }

            yield return new WaitForSeconds(1);
        }
    }
    /*
    IEnumerator UpdateUiStats()
    {
        while (true)
        {
            rpm = Random.Range(1, 100);
            rpmText.text = rpm.ToString();

            speed = Random.Range(0, 100);
            speedText.text = speed.ToString("F2");

            resistance = Random.Range(0, 100);
            resistanceText.text = resistance.ToString();

            watts = Random.Range(0, 100);
            wattsText.text = watts.ToString();

            if (rpm != 0)
            {
                distance = Random.Range(0, 100);
                distanceText.text = distance.ToString("F2");

                calories += Random.Range(0, 100);
                caloriesText.text = calories.ToString();
            }

            yield return new WaitForSeconds(1);
        }
    }
    */
}