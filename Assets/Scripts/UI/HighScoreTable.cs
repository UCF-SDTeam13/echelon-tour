using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform entryContainter;
    private Transform entryTemplate;

    public static string playerName;
    public static int place;
    public static int averageSpeed;
    public static float distance;

    public static Text placeText;

    int numplayers = 8;


    private void Awake()
    {
        // Initialize the number of players that finished the race
        // numplayers = ;
        
        entryContainter = transform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainter.Find("HighScoreEntryTemplate");
        
        entryTemplate.gameObject.SetActive(false);

        float templateHeight = 30f;
        for (int i = 0; i < numplayers; i++)
        {
            getResults(i);

            Transform entryTransform = Instantiate(entryTemplate, entryContainter);
            RectTransform entryReactTransform = entryTransform.GetComponent<RectTransform>();
            entryReactTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);
        }
    }

    private void getResults(int i)
    {
        string placeString;
        // Make call to database to get playerName, place, average speed, distance and set the corresponding local variables to match
        // playerName =
        // averageSpeed = 
        // float distance =

        place = i + 1;

        switch(place)
        {
            case 1:  placeString = "1ST"; break;
            case 2:  placeString = "2ND"; break;
            case 3:  placeString = "3RD"; break;
            default: placeString = place + "TH"; break;
        }
        
        entryTemplate.Find("Place").GetComponent<Text>().text = placeString;
        entryTemplate.Find("Username").GetComponent<Text>().text = "test";
        entryTemplate.Find("AvgSpeed").GetComponent<Text>().text = "test";
        entryTemplate.Find("Distance").GetComponent<Text>().text = "test";
    }
}
