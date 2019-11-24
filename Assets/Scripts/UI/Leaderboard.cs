using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    private Transform entryContainter;
    private Transform entryTemplate;
    private static float wheelDiameter = 78 * 2.54f / 100000;
    private static float rpmRatio = 1f;
    int rpm = 0;
    int numplayers = 8;
    public static string playerName;
    public static int place;
    public static float distance;

    private void Start()
    {
        // Initialize the number of players that joined game
        // numplayers = ;
        
        entryContainter = transform.Find("LeaderboardEntryContainer");
        entryTemplate = entryContainter.Find("LeaderboardEntryTemplate");
        
        entryTemplate.gameObject.SetActive(false);

        float templateHeight = 20f;
        for (int i = 0; i < numplayers; i++)
        {
            initializeBoard(i+1);
            Transform entryTransform = Instantiate(entryTemplate, entryContainter);
            entryTransform.name = "Player" + (i+1);
            RectTransform entryReactTransform = entryTransform.GetComponent<RectTransform>();
            entryReactTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);
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

        // For each num of players, get info from gamelift and update leaderboard accordingly
        for (int i = 1; i < numplayers; i++)
        {
            // Find gamelift player with currentPlace = i;
            //currentDistance = valueFromGameLift(withPlace(i));
            //nextDistance = valueFromGameLift(withPlace(i+1));

            // Display new Distance
            //entryContainter.Find("Player" + i).GetComponent<Text>().text = currentDistance.ToString();

            // Check if next player has higher distance. If so move this one down and next up
            //if(currentDistance < nextDistance)
            //{
                Debug.Log(entryContainter.Find("Player"+ i).name);
                temp = entryContainter.Find("Player" + i).transform.position;
                entryContainter.Find("Player" + i).transform.position = entryContainter.Find("Player" + (i+1)).transform.position;
                entryContainter.Find("Player" + (i+1)).transform.position = temp;
                updatePlace(i, i+1);
            //}
        }
    }

    private void initializeBoard(int i)
    {
        entryTemplate.Find("Place").GetComponent<Text>().text = i.ToString();
        entryTemplate.Find("Username").GetComponent<Text>().text = "test" + i;
        entryTemplate.Find("Distance").GetComponent<Text>().text = "test" + i;
    }

    private void updatePlace(int currentPlace, int newPlace)
    {
        // Use CurrentPos to determine which element to grab and then transform it
        //entryContainter.Find("Player" + currentPlace).GetComponent<Text>().text = newPlace.ToString();
        //entryContainter.Find("Player" + newPlace).GetComponent<Text>().text = currentPlace.ToString();

        // Update position of player with currPlace to newPlace and player of newPlace to currPlace
        // TODO
    }

    public void collapseLeaderboard()
    {
        for(int i = 1; i < numplayers + 1; i++)
        {
            entryContainter.Find("Player" + i).gameObject.SetActive(false);
        }
    }
    public void expandLeaderboard()
    {
        for(int i = 1; i < numplayers + 1; i++)
        {
            entryContainter.Find("Player" + i).gameObject.SetActive(true);
        }
    }
}