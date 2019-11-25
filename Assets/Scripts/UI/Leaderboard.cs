using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    // Set to true when testing locally
    bool testing = true;
    private Transform entryContainter;
    private Transform entryTemplate;
    private Transform entryContainterHS;
    private Transform entryTemplateHS;
    private static float wheelDiameter = 78 * 2.54f / 100000;
    private static float rpmRatio = 1f;
    int rpm = 0;
    int numplayers = 0;
    public static string playerName;
    public static float distance;

    Dictionary <int, float> players = new Dictionary<int, float>();
    Dictionary <int, int> place = new Dictionary<int, int>();

    private void Awake()
    {
        RealTimeClient.Instance.StatsUpdate += UpdateLiveStats;
        RealTimeClient.Instance.PlayerDisconnect += DecrementPlayerSize;
        RealTimeClient.Instance.CustomizationUpdate += IncrementPlayerSize;
    }

    private void Start()
    {        
        entryContainter = transform.Find("Leaderboard/LeaderboardEntryContainer");
        entryTemplate = entryContainter.Find("LeaderboardEntryTemplate");
        
        entryTemplate.gameObject.SetActive(false);

        float templateHeight = 20f;
        for (int i = 0; i < 8; i++)
        {
            initializeBoard(i+1);
            Transform entryTransform = Instantiate(entryTemplate, entryContainter);
            entryTransform.name = "Player" + (i+1);
            RectTransform entryReactTransform = entryTransform.GetComponent<RectTransform>();
            entryReactTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(testing); 
        }

        // HighScore Table
        entryContainterHS = transform.Find("HighScoreTable/HighScoreEntryContainer");
        entryTemplateHS = entryContainterHS.Find("HighScoreEntryTemplate");
        entryTemplateHS.gameObject.SetActive(false);

        float templateHeightHS = 30f;
        for (int i = 0; i < 8; i++)
        {
            highScoreTable(i + 1);

            Transform entryTransformHS = Instantiate(entryTemplateHS, entryContainterHS);
            RectTransform entryReactTransformHS = entryTransformHS.GetComponent<RectTransform>();
            entryReactTransformHS.anchoredPosition = new Vector2(0, -templateHeightHS * i);
            entryTransformHS.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 temp;
        int tempInt;
        int currentPlace;
        int nextPlace;

        // For each player, check the player for ditance
        for (int i = 1; i < 8; i++)
        {
            for(int j = 1; j < 8; j++)
            {
                // Check if next player has higher distance. If so move this one down and next up
                if(place[i] == j)
                    if(players[place[i]] < players[place[i+1]])
                    {
                        temp = entryContainter.Find("Player" + (place[i])).transform.position;
                        entryContainter.Find("Player" + (place[i])).transform.position = entryContainter.Find("Player" + (place[i + 1])).transform.position;
                        entryContainter.Find("Player" + (place[i +1])).transform.position = temp;

                        currentPlace = place[i];
                        nextPlace = place [i + 1];

                        // Update Place
                        tempInt = place[i];
                        place[i] = place [i + 1];
                        place[i + 1] = tempInt;

                        entryContainter.Find("Player" + currentPlace).Find("Place").GetComponent<Text>().text = (tempInt + 1).ToString();
                        entryContainter.Find("Player" + nextPlace).Find("Place").GetComponent<Text>().text = place[i + 1].ToString();
                    }
                //TESTING
                //if(i==6)
                //{
                //    players[6]=30;
                //    entryContainter.Find("Player" + 6).Find("Distance").GetComponent<Text>().text = players[6].ToString();
                //}
            }
    
        }
    }

    private void initializeBoard(int i)
    {
        players.Add(i,0);
        place.Add(i,i);
        entryTemplate.Find("Place").GetComponent<Text>().text = i.ToString();
        entryTemplate.Find("Username").GetComponent<Text>().text = "Player" + i;
        entryTemplate.Find("Distance").GetComponent<Text>().text = players[i].ToString();
    }

    private void UpdateLiveStats(object sender, StatsUpdateEventArgs e)
    {
        players[e.peerId] += CalculateDistance(e.rotations);
        entryContainter.Find("Player" + e.peerId).Find("Distance").GetComponent<Text>().text = players[e.peerId].ToString();
    }

    // When a player enters the lobby, the numplayers count will increment & their name will appear on the leaderboard.
    private void IncrementPlayerSize(object sender, CustomizationUpdateEventArgs e)
    {
        numplayers += 1;
        entryContainter.Find("Player" + e.peerId).gameObject.SetActive(true);
        entryContainter.Find("Player" + e.peerId).Find("Username").GetComponent<Text>().text = e.PlayerId;
    }

    private void DecrementPlayerSize(object sender, PlayerEventArgs e)
    {
        numplayers -= 1;
    }

    // Calculate the distance based off the bluetooth rotation
    private float CalculateDistance(int rotations)
    {
        // Trying to hard code pi
        float wheelDiameter = (78 * 2.54f) / 100000;
        float wheelCircumference = wheelDiameter * 3.14f;
        return rotations * wheelCircumference;
    }

    public void collapseLeaderboard()
    {
        for(int i = 1; i < 9; i++)
        {
            entryContainter.Find("Player" + i).gameObject.SetActive(false);
        }
    }
    public void expandLeaderboard()
    {
        for(int i = 1; i < 9; i++)
        {
            entryContainter.Find("Player" + i).gameObject.SetActive(true);
        }
    }

    public void highScoreTable(int i)
    {
        string placeString;

        switch(place[i])
        {
            case 1:  placeString = "1ST"; break;
            case 2:  placeString = "2ND"; break;
            case 3:  placeString = "3RD"; break;
            default: placeString = place[i] + "TH"; break;
        }
        
        entryTemplateHS.Find("Place").GetComponent<Text>().text = placeString;
        entryTemplateHS.Find("Username").GetComponent<Text>().text = entryContainter.Find("Player" + place[i]).Find("Username").GetComponent<Text>().text;
        entryTemplateHS.Find("Distance").GetComponent<Text>().text = players[i].ToString();
    }
}