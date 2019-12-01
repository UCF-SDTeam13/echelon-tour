using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    // Universal Variables
    public GameObject challengePanel;
    public GameObject challengeImage;
    public GameObject challengeTitle;
    public GameObject challengeDescription;

    public ChallengeDemo challenges;

    private void Start()
    {
        RealTimeClient.Instance.RaceEnd += RaceEnded;
        challenges = GameObject.FindGameObjectWithTag("Challenges").GetComponent<ChallengeDemo>();
    }

    private void Update()
    {
        // Checks if the top speed is correct
        if (PlayerPrefs.GetInt("TopSpeed1") != 1 && WorkoutCalculations.CalculateSpeed(Bike.Instance.RPM) >= challenges.tsc01.Speed)
        {
            StartCoroutine(TopSpeedTrigger(challenges.tsc01));
        }

        // Checks if the speed is above a certain amount
        if (PlayerPrefs.GetInt("MaintainSpeed1") != 1 && challenges.msc01.Active == false && WorkoutCalculations.CalculateSpeed(Bike.Instance.RPM) >= challenges.msc01.Speed)
        {
            float startTime = Time.time;
            challenges.msc01.Active = true;
            StartCoroutine(MaintainSpeedTracker(challenges.msc01, startTime));
        }

        // Checks if a certain amount of distance is achieved
        if (PlayerPrefs.GetInt("TotalDistance1") != 1 && WorkoutCalculations.CalculateDistance(Bike.Instance.Count) >= challenges.tdc01.Distance)
        {
            StartCoroutine(TotalDistanceTrigger(challenges.tdc01));
        }
    }

    private void RaceEnded(object sender, EventArgs e)
    {
        if(PlayerPrefs.GetInt("RacePlacement1") != 1) //need to find placement
        {
            StartCoroutine(RacePlacementTrigger(challenges.rpc01));
        }

        if (PlayerPrefs.GetInt("TopSpeed1") == 1 && PlayerPrefs.GetInt("MaintainSpeed1") == 1 &&
                PlayerPrefs.GetInt("RacePlacement1") == 1 && PlayerPrefs.GetInt("DistanceTraveled1") == 1)
        {
            StartCoroutine(DailiesCompletedTrigger(challenges.dcc01));
        }
    }

    IEnumerator TopSpeedTrigger(TopSpeed challenge)
    {
        PlayerPrefs.SetInt("TopSpeed1", 1);
        challengeImage.GetComponent<RawImage>().texture = challenge.Image.GetComponent<RawImage>().texture;
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    IEnumerator MaintainSpeedTrigger(MaintainSpeed challenge)
    {
        PlayerPrefs.SetInt("MaintainSpeed1", 1);
        challengeImage = challenge.Image;
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    IEnumerator RacePlacementTrigger(RacePlacement challenge)
    {
        PlayerPrefs.SetInt("RacePlacement1", 1);
        challengeImage = challenge.Image;
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    IEnumerator TotalDistanceTrigger(TotalDistance challenge)
    {
        PlayerPrefs.SetInt("TotalDistance1", 1);
        challengeImage = challenge.Image;
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    IEnumerator DailiesCompletedTrigger(DailiesCompleted challenge)
    {
        PlayerPrefs.SetInt("DailiesCompleted", 1);
        challengeImage = challenge.Image;
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    // Tracks the speed every second
    IEnumerator MaintainSpeedTracker(MaintainSpeed challenge, float time)
    {
        // Loops and calculate the time that has passed
        for (float deltaTime = 0; ; deltaTime = Time.time - time)
        {
            // Calculate the current speed
            float speed = WorkoutCalculations.CalculateSpeed(Bike.Instance.RPM);

            // Check if the speed is correct and the time is achieved
            if (deltaTime >= challenge.Time && speed >= challenge.Speed)
            {
                StartCoroutine(MaintainSpeedTrigger(challenge));
                yield break;
            }
            else if (speed >= challenge.Speed)
            {
                // speed is correct, time is not achieved, wait for a second and check again
                yield return new WaitForSeconds(1);
            }
            else
            {
                // Resets the challenges as the player failed
                challenge.Active = false;
                yield break;
            }
        }
    }
}


