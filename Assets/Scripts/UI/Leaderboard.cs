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
    private static float wheelDiameter = 78 * 2.54f / 100000;
    private static float rpmRatio = 1f;
    int rpm = 0;
    int numplayers = 0;
    public static string playerName;
    public static int place;
    public static float distance;

    Dictionary <int, float> players = new Dictionary<int, float>();

    private void Awake()
    {
        RealTimeClient.Instance.StatsUpdate += UpdateLiveStats;
        RealTimeClient.Instance.PlayerDisconnect += DecrementPlayerSize;
        RealTimeClient.Instance.CustomizationUpdate += IncrementPlayerSize;
    }

    private void Start()
    {        
        entryContainter = transform.Find("LeaderboardEntryContainer");
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
    }

    // Update is called once per frame
    void Update()
    {
        // Double check number of players currently in game
        //numplayers = ;
        int currentDistance;
        int nextDistance; 
        Vector3 temp;

        // For each player, check the player for ditance
        for (int i = 1; i < numplayers; i++)
        {
            // Check if next player has higher distance. If so move this one down and next up
            //if(players[i] < players[i+1])
            //{
                temp = entryContainter.Find("Player" + i).transform.position;
                entryContainter.Find("Player" + i).transform.position = entryContainter.Find("Player" + (i+1)).transform.position;
                entryContainter.Find("Player" + (i+1)).transform.position = temp;
                updatePlace(i, i+1);
            //}
        }
    }

    private void initializeBoard(int i)
    {
        players.Add(i,0);
        entryTemplate.Find("Place").GetComponent<Text>().text = i.ToString();
        entryTemplate.Find("Username").GetComponent<Text>().text = "Player" + i;
        entryTemplate.Find("Distance").GetComponent<Text>().text = players[i].ToString();
    }

    private void updatePlace(int currentPlace, int newPlace)
    {
        entryContainter.Find("Player" + currentPlace).GetComponent<Text>().text = newPlace.ToString();
        entryContainter.Find("Player" + newPlace).GetComponent<Text>().text = currentPlace.ToString();
    }

    private void UpdateLiveStats(object sender, StatsUpdateEventArgs e)
    {
        players[e.peerId] = CalculateDistance(e.rotations);
        entryContainter.Find("Player" + e.peerId).GetComponent<Text>().text = players[e.peerId].ToString();
    }

    // When a player enters the lobby, the numplayers count will increment & their name will appear on the leaderboard.
    private void IncrementPlayerSize(object sender, CustomizationUpdateEventArgs e)
    {
        numplayers += 1;
        entryContainter.Find("Player" + e.peerId).gameObject.SetActive(true);
        entryContainter.Find("Player" + e.peerId).GetComponent<Text>().text = e.PlayerId;
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
}