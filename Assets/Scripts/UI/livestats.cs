using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class livestats : MonoBehaviour
{

    public Text rpmText;
    public Text speedText;
    public Text resistanceText;
    public Text wattsText;
    public Text distanceText;
    public Text totalOutputText;

    int rpm = 0;
    float speed = 0f;
    float resistance = 1f;
    float watts = 0f;
    float distance = 0f;
    float totalOutput = 0f;

    private static float wheelDiameter = 78 * 2.54f / 100000;
    private static float rpmRatio = 1f;

    private static float distancePerCount = wheelDiameter * Mathf.PI * rpmRatio;

    private static float speedMultiplier = distancePerCount * 60;

    // Start is called before the first frame update
    void Start()
    {
        BLEPlugin.Instance.StartWorkout();

        resistanceText.text = resistance.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        updateUIText();
    }

    public void updateUIText()
    {
        rpm = Bike.Instance.RPM;

        speed = rpm * speedMultiplier;
        distance += rpm * wheelDiameter * 3.14f;

        rpmText.text = rpm.ToString();
        speedText.text = speed.ToString();
        resistanceText.text = resistance.ToString();
        wattsText.text = watts.ToString();
        distanceText.text = distance.ToString("F2");
        totalOutputText.text = totalOutput.ToString();
    }
}