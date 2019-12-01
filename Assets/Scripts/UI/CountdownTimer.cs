using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public static float raceTime = 1f;
    float currentTime = 0f;
    float startingTime = raceTime * 60f;
    [SerializeField] Text countdownText;
    public GameObject highscoreTable;
    public GameObject livestats;
    public GameObject optionsButton;

    public GameObject time;
    public GameObject leaderboard;

    public GameObject challengeSystem;

    void Start()
    {
        currentTime = startingTime;  
    }

    void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "SoloRide" || currentScene == "MatchMakingLobby" || currentScene == "EchelonDomeTrack")
        {
            setUIText();
        }
    }

    // Sets the UI text
    public void setUIText()
    {
        currentTime -= 1 * Time.deltaTime;

        // Timer has run out
        if (currentTime <= 0)
        {
            finishAction();
            return;
        }

        float hours;
        float minutes;
        float seconds;
        float miniseconds;

        GetTimeValues(currentTime, out hours, out minutes, out seconds, out miniseconds);

        if (hours > 0)
            countdownText.text = string.Format("{0}:{1}:{2}", hours, minutes, seconds);
        else if (minutes > 0)
            countdownText.text = string.Format("{0}:{1}", minutes, seconds);
        else
            countdownText.text = string.Format("{0}:{1}", seconds, miniseconds);

        // Once timer reaches 10 seconds. Color of font will change to red
        if (currentTime <= 60)
        {
            countdownText.color = Color.red;
        }
    }

    // Gets time values
    public void GetTimeValues(float time, out float hours, out float minutes, out float seconds, out float miniseconds)
    {
        hours = (int)(currentTime / 3600f);
        minutes = (int)((currentTime - hours * 3600) / 60f);
        seconds = (int)((currentTime - hours * 3600 - minutes * 60));
        miniseconds = (int)((time - hours * 3600 - minutes * 60 - seconds) * 100);
    }

    public void setTimer(float buttonTime)
    {
        raceTime = raceTime * buttonTime;
    }

    public void setTimerToDefault()
    {
        raceTime = 1f;
    }

    // This cotrols what will happen when timer runs out
    public void finishAction()
    {
        // Debug.Log("Game has ended");
        countdownText.text = "00:00";

        if (SceneManager.GetActiveScene().name == "SoloRide")
        {
            highscoreTable.SetActive(true);
        }
        else
        {
            highscoreTable.SetActive(true);
            livestats.SetActive(false);
            optionsButton.SetActive(false);
            time.SetActive(false);
            leaderboard.SetActive(false);
            highscoreTable.SendMessageUpwards("ShowHighScore");
        }

        challengeSystem.GetComponent<ChallengeManager>().SendMessage("RaceEnded");

        //SceneManager.LoadSceneAsync("Loss");
        //Application.Quit();
    }
}