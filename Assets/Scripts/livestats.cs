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
    public Text caloriesText;
    public Text distanceText;
    public Text totalOutputText;

    float rpm = 0f;
    float speed = 0f;
    float resistance = 0f;
    float watts = 0f;
    float calories = 0f;
    float distance = 0f;
    float totalOutput = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateUIText();
    }

    public void updateUIText()
    {
        rpmText.text = rpm.ToString();
        speedText.text = speed.ToString();
        resistanceText.text = resistance.ToString();
        wattsText.text = watts.ToString();
        caloriesText.text = calories.ToString();
        distanceText.text = distance.ToString();
        totalOutputText.text = totalOutput.ToString();
    }
}