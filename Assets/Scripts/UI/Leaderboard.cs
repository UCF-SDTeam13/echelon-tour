using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    // Set to true when testing locally
    bool testing = false;
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

    Dictionary<int, float> players = new Dictionary<int, float>();
    Dictionary<int, int> place = new Dictionary<int, int>();

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
            initializeBoard(i + 1);
            Transform entryTransform = Instantiate(entryTemplate, entryContainter);
            entryTransform.name = "Player" + (i + 1);
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
            initializeTable(i + 1);

            Transform entryTransformHS = Instantiate(entryTemplateHS, entryContainterHS);
            entryTransformHS.name = "Player" + (i + 1);
            RectTransform entryReactTransformHS = entryTransformHS.GetComponent<RectTransform>();
            entryReactTransformHS.anchoredPosition = new Vector2(0, -templateHeightHS * i);
            entryTransformHS.gameObject.SetActive(testing);
        }
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 temp;
        int tempInt;
        string tempString;
        int currentPlace;
        int nextPlace;

        // For each player, check the player for distance
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 8; j++)
            {
                // Check if next player has higher distance. If so move this one down and next up
                if (place[i] == j)
                    if (players[place[i]] < players[place[i + 1]])
                    {   
                        // Update live leaderboard
                        temp = entryContainter.Find("Player" + (place[i])).transform.position;
                        entryContainter.Find("Player" + (place[i])).transform.position = entryContainter.Find("Player" + (place[i + 1])).transform.position;
                        entryContainter.Find("Player" + (place[i + 1])).transform.position = temp;

                        // Update final board
                        temp = entryContainterHS.Find("Player" + (place[i])).transform.position;
                        entryContainterHS.Find("Player" + (place[i])).transform.position = entryContainterHS.Find("Player" + (place[i + 1])).transform.position;
                        entryContainterHS.Find("Player" + (place[i + 1])).transform.position = temp;

                        // Update Place
                        tempString = entryContainter.Find("Player" + place[i]).Find("Place").GetComponent<Text>().text;
                        entryContainter.Find("Player" + place[i]).Find("Place").GetComponent<Text>().text = entryContainter.Find("Player" + place[i + 1]).Find("Place").GetComponent<Text>().text;
                        entryContainter.Find("Player" + place[i + 1]).Find("Place").GetComponent<Text>().text = tempString;

                        tempInt = place[i]; 
                        place[i] = place[i + 1];
                        place[i + 1] = tempInt; 
                    }
                //TESTING
                //testingTable();
            }

        }

        matchUp();
    }

    // Match up final leaderboard with everything in live leaderboard
    private void matchUp()
    {
        for(int i = 1; i < 9; i++)
        {
             // Place
            entryContainterHS.Find("Player" + i).Find("Place").GetComponent<Text>().text = entryContainter.Find("Player" + i).Find("Place").GetComponent<Text>().text;
            // Distance
            entryContainterHS.Find("Player" + i).Find("Distance").GetComponent<Text>().text = entryContainter.Find("Player" + i).Find("Distance").GetComponent<Text>().text;
        }
    }


    private void initializeBoard(int i)
    {
        players.Add(i, 0);
        place.Add(i, i);
        entryTemplate.Find("Place").GetComponent<Text>().text = i.ToString();
        entryTemplate.Find("Username").GetComponent<Text>().text = "Player" + i;
        entryTemplate.Find("Distance").GetComponent<Text>().text = players[i].ToString();
    }

    public void initializeTable(int i)
    {
        entryTemplateHS.Find("Place").GetComponent<Text>().text = i.ToString();
        entryTemplateHS.Find("Username").GetComponent<Text>().text = entryContainter.Find("Player" + place[i]).Find("Username").GetComponent<Text>().text;
        entryTemplateHS.Find("Distance").GetComponent<Text>().text = players[i].ToString();
    }

    private void UpdateLiveStats(object sender, StatsUpdateEventArgs e)
    {
        UnityMainDispatcher.Instance.QForMainThread(UpdateLiveStatsMainThread, e);
    }

    private void UpdateLiveStatsMainThread(StatsUpdateEventArgs e)
    {
        players[e.peerId] += CalculateDistance(e.rotations);
        if(e.peerId >= 1 && e.peerId <= 8)
        {
            entryContainter.Find("Player" + e.peerId).Find("Distance").GetComponent<Text>().text = players[e.peerId].ToString();
        }
    }

    // When a player enters the lobby, the numplayers count will increment & their name will appear on the leaderboard.
    private void IncrementPlayerSize(object sender, CustomizationUpdateEventArgs e)
    {
        numplayers += 1;
        UnityMainDispatcher.Instance.QForMainThread(IncrementPlayerSizeMainThread, e);
    }
    private void IncrementPlayerSizeMainThread(CustomizationUpdateEventArgs e)
    {
        entryContainter.Find("Player" + e.peerId).gameObject.SetActive(true);
        entryContainterHS.Find("Player" + e.peerId).gameObject.SetActive(true);
        entryContainter.Find("Player" + e.peerId).Find("Username").GetComponent<Text>().text = e.PlayerId;
        entryContainterHS.Find("Player" + e.peerId).Find("Username").GetComponent<Text>().text = e.PlayerId;
    }
    private void DecrementPlayerSize(object sender, PlayerEventArgs e)
    {
        numplayers -= 1;
        players[e.peerId] = -1;
        entryContainter.Find("Player" + e.peerId).gameObject.SetActive(false);
        entryContainterHS.Find("Player" + e.peerId).gameObject.SetActive(false);
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
        for (int i = 1; i < 9; i++)
        {
            entryContainter.Find("Player" + i).gameObject.SetActive(false);
        }
    }
    public void expandLeaderboard()
    {
        for (int i = 1; i < 9; i++)
        {
            entryContainter.Find("Player" + i).gameObject.SetActive(true);
        }
    }

    public void testingTable()
    {
        players[6]=30;
        entryContainter.Find("Player" + 6).Find("Distance").GetComponent<Text>().text = players[6].ToString();
        
        players[7]=20;
        entryContainter.Find("Player" + 7).Find("Distance").GetComponent<Text>().text = players[7].ToString();

        players[3]=60;
        entryContainter.Find("Player" + 3).Find("Distance").GetComponent<Text>().text = players[3].ToString();
    }
}