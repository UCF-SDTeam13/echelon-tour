using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardV2 : MonoBehaviour
{
    Dictionary<int, PlayerStats> players = new Dictionary<int, PlayerStats>();

    public Transform[] lbTemplate;
    public Transform[] hsTemplate;

    public int maxPlayers = 8;
    public bool isActive = true;

    private float AvgSpeedTotal = 0;
    private int numSpeedMeasurements = 1;
    public bool racing = true;

    private void Awake()
    {
        RealTimeClient.Instance.StatsUpdate += UpdateLiveStats;
        RealTimeClient.Instance.PlayerDisconnect += DecrementPlayerSize;
        RealTimeClient.Instance.CustomizationUpdate += IncrementPlayerSize;
    }

    private void Start()
    {
        InitializeLeaderboard();
        InitializeHighscore();
    }
    
    private void Update()
    {
        if(isActive != true)
        {
            return;
        }

        for(int i = 1; i < maxPlayers; i++)
        {
            for(int j = 0; j < maxPlayers; j++)
            {
                if(players[i].rank == j + 1)
                {
                    if (players[players[i].rank].distance < players[players[i + 1].rank].distance)
                    {
                        lbTemplate[players[i].rank - 1].Find("Distance").GetComponent<Text>().text = players[players[i + 1].rank].distance.ToString("F2");
                        lbTemplate[players[i + 1].rank - 1].Find("Distance").GetComponent<Text>().text = players[players[i].rank].distance.ToString("F2");

                        lbTemplate[players[i].rank - 1].Find("Username").GetComponent<Text>().text = players[players[i + 1].rank].name.ToString();
                        lbTemplate[players[i + 1].rank - 1].Find("Username").GetComponent<Text>().text = players[players[i].rank].name.ToString();

                        lbTemplate[players[i].rank - 1].Find("Place").GetComponent<Text>().text = players[players[i + 1].rank].rank.ToString();
                        lbTemplate[players[i + 1].rank - 1].Find("Place").GetComponent<Text>().text = players[players[i].rank].rank.ToString();

                        int tempRank = players[i].rank;
                        players[i].rank = players[i + 1].rank;
                        players[i + 1].rank = tempRank;
                        
                        // Update final board
                        hsTemplate[players[i].rank - 1].Find("Distance").GetComponent<Text>().text = players[players[i + 1].rank].distance.ToString("F2");
                        hsTemplate[players[i + 1].rank - 1].Find("Distance").GetComponent<Text>().text = players[players[i].rank].distance.ToString("F2");

                        hsTemplate[players[i].rank - 1].Find("Username").GetComponent<Text>().text = players[players[i + 1].rank].name.ToString();
                        hsTemplate[players[i + 1].rank - 1].Find("Username").GetComponent<Text>().text = players[players[i].rank].name.ToString();

                        hsTemplate[players[i].rank - 1].Find("Place").GetComponent<Text>().text = players[players[i + 1].rank].rank.ToString();
                        hsTemplate[players[i + 1].rank - 1].Find("Place").GetComponent<Text>().text = players[players[i].rank].rank.ToString();
                    }
                }
            }
        }

        //MatchUp();
    }
    
    // Match up final leaderboard with everything in live leaderboard
    private void MatchUp()
    {
        for (int i = 1; i < maxPlayers; i++)
        {
            // Place
            hsTemplate[players[i].rank - 1].Find("Place").GetComponent<Text>().text = hsTemplate[players[i].rank].Find("Place").GetComponent<Text>().text;
            // Distance
            hsTemplate[players[i].rank - 1].Find("Distance").GetComponent<Text>().text = hsTemplate[players[i].rank].Find("Distance").GetComponent<Text>().text;
        }
    }

    // Initialize the leaderboard table
    private void InitializeLeaderboard()
    {
        for(int i = 0; i < maxPlayers; i++)
        {
            players.Add(i + 1, new PlayerStats("", 0, i + 1));
            lbTemplate[i].Find("Place").GetComponent<Text>().text = "";
            lbTemplate[i].Find("Username").GetComponent<Text>().text = "";
            lbTemplate[i].Find("Distance").GetComponent<Text>().text = "";
        }
    }

    // Initialize the highscore table
    private void InitializeHighscore()
    {
        for(int i = 0; i < maxPlayers; i++)
        {
            hsTemplate[i].Find("Place").GetComponent<Text>().text = "";
            hsTemplate[i].Find("Username").GetComponent<Text>().text = "";
            hsTemplate[i].Find("Distance").GetComponent<Text>().text = "";
        }
    }

    // Stores avg speed of the player
    private void UpdateLiveStats(object sender, StatsUpdateEventArgs e)
    {
        UnityMainDispatcher.Instance.QForMainThread(UpdateLiveStatsMainThread, e);

        if (racing && e.peerId == RealTimeClient.Instance.peerId)
        {
            AvgSpeedTotal += e.rpm;
            ++numSpeedMeasurements;
        }
    }

    // Updates the livestats of the leaderboard
    private void UpdateLiveStatsMainThread(StatsUpdateEventArgs e)
    {
        players[e.peerId].distance = WorkoutCalculations.CalculateDistance(e.rotations);
        //BLEDebug.LogInfo($"STAT: {e.peerId} {players[e.peerId]} {e.rotations}");

        if (racing && e.peerId >= 1 && e.peerId <= 8 && isActive == true)
        {
            lbTemplate[players[e.peerId].rank - 1].Find("Place").GetComponent<Text>().text = players[e.peerId].rank.ToString();
            lbTemplate[players[e.peerId].rank - 1].Find("Distance").GetComponent<Text>().text = players[e.peerId].distance.ToString("F2");

            hsTemplate[players[e.peerId].rank - 1].Find("Place").GetComponent<Text>().text = players[e.peerId].rank.ToString();
            hsTemplate[players[e.peerId].rank - 1].Find("Distance").GetComponent<Text>().text = players[e.peerId].distance.ToString("F2");
        }
    }

    // Increment PlayerSize as players join
    private void IncrementPlayerSize(object sender, CustomizationUpdateEventArgs e)
    {
        UnityMainDispatcher.Instance.QForMainThread(IncrementPlayerSizeMainThread, e);
    }

    private void IncrementPlayerSizeMainThread(CustomizationUpdateEventArgs e)
    {
        if(players.ContainsKey(e.peerId) == true)
        {
            players[e.peerId] = new PlayerStats(e.PlayerId, 0, e.peerId);
        }
        else
        {
            players.Add(e.peerId, new PlayerStats(e.PlayerId, 0, e.peerId));
        }

        lbTemplate[players[e.peerId].rank - 1].Find("Place").GetComponent<Text>().text = players[e.peerId].rank.ToString();
        lbTemplate[players[e.peerId].rank - 1].Find("Username").GetComponent<Text>().text = players[e.peerId].name;
        lbTemplate[players[e.peerId].rank - 1].Find("Distance").GetComponent<Text>().text = players[e.peerId].distance.ToString();

        hsTemplate[players[e.peerId].rank - 1].Find("Place").GetComponent<Text>().text = players[e.peerId].rank.ToString();
        hsTemplate[players[e.peerId].rank - 1].Find("Username").GetComponent<Text>().text = players[e.peerId].name;
        hsTemplate[players[e.peerId].rank - 1].Find("Distance").GetComponent<Text>().text = players[e.peerId].distance.ToString();
    }

    // Decrement PlayerSize as players leave 
    private void DecrementPlayerSize(object sender, PlayerEventArgs e)
    {
        lbTemplate[players[e.peerId].rank - 1].Find("Place").GetComponent<Text>().text = "";
        lbTemplate[players[e.peerId].rank - 1].Find("Username").GetComponent<Text>().text = "";
        lbTemplate[players[e.peerId].rank - 1].Find("Distance").GetComponent<Text>().text = "";

        hsTemplate[players[e.peerId].rank - 1].Find("Username").GetComponent<Text>().text = "";
        hsTemplate[players[e.peerId].rank - 1].Find("Distance").GetComponent<Text>().text = "";

        players.Remove(e.peerId);
    }

    public void Collapse()
    {
        isActive = false;
    }

    public void Expand()
    {
        isActive = true;
    }

    class PlayerStats
    {
        public string name { get; set; }
        public float distance { get; set; }
        public int rank { get; set; }

        public PlayerStats(string name, float distance, int rank)
        {
            this.name = name;
            this.distance = distance;
            this.rank = rank;
        }
    }
}
