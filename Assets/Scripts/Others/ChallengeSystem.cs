using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ChallengeSystem : MonoBehaviour
{
    // Universal Variables
    public GameObject challengePanel;
    public AudioSource challengeSound;
    public GameObject challengeTitle;
    public GameObject challengeDescription;

    // Individual challenges images
    public GameObject TopSpeedImage;
    public GameObject MaintainSpeedImage;
    public GameObject RacePlacementImage;
    public GameObject TotalDistanceImage;

    // Creation of challenges
    public TopSpeedChallenge tsc01;
    public MaintainSpeedChallenge msc01;
    public RacePlacementChallenge rpc01;
    public TotalDistanceChallenge tdc01;

    // Statuses and number of challenges
    public bool[] challengeStatuses;
    public int numChallenges;

    public GameObject spawnManager;

    private void Awake()
    {
        // Need to find out how to change certain statuses
        //ChallengeData data = SaveSystem.LoadChallengeData();
        //challengeStatuses = new bool[numChallenges];

        // Initialize the challenges with certain stats
        tsc01 = new TopSpeedChallenge(TopSpeedImage, 20);
        msc01 = new MaintainSpeedChallenge(MaintainSpeedImage, 10, 30);
        rpc01 = new RacePlacementChallenge(RacePlacementImage, 3);
        tdc01 = new TotalDistanceChallenge(TotalDistanceImage, 1); //Not sure of value

        //RealTimeClient.Instance.RaceEnd += RaceEndTrigger;
    }
    
    private void Update()
    {
        // MAY NEED TO CHECK IF MATCH IS FINISHED FIRST
        
        // Checks if the top speed is correct
        if (tsc01.Achieved == false && CalculateSpeed() >= tsc01.Speed)
        {
            StartCoroutine(TopSpeedTrigger(tsc01));
        }

        // Checks if the speed is above a certain amount
        if (msc01.Achieved == false && msc01.Active == false && CalculateSpeed() >= msc01.Speed)
        {
            float startTime = Time.time;
            msc01.Active = true;
            StartCoroutine(MaintainSpeedTracker(msc01, startTime));
        }

        /*
        if(rpc01.Achieved == false && FINALRANKPLACEHOLDER <= rpc01.Rank && raceEnd == true)
        {
            StartCoroutine(RacePlacementTrigger(rpc01));
        }
        */

        // Checks if a certain amount of distance is achieved
        if(tdc01.Achieved == false && CalculateDistance() >= tdc01.Distance)
        {
            StartCoroutine(TotalDistanceTrigger(tdc01));
        }

        /*
        if(MATCHFINISH == true)
        {
            SaveSystem.SaveChallengeData(challengeStatuses, calculateDistance());
        }
        */
    }

    private void RaceEndTrigger(object sender, EventArgs e)
    {


        if(rpc01.Achieved == false)
        {
            StartCoroutine(RacePlacementTrigger(rpc01));
        }
    }

    /*
    IEnumerator AchievedTrigger(CustomChallenge challenge)
    {
        challenge.Achieved = true;
        yield return new WaitForSeconds(5);
    }

    IEnumerator MaintainSpeedTracker(CustomChallenge challenge, float time)
    {
        float currentSpeed = calculateSpeed();

        float deltaTime = Time.time - time;

        if (deltaTime >= challenge.Time)
        {
            StartCoroutine(AchievedTrigger(challenge));
            yield break;
        }
        else if (currentSpeed >= challenge.Speed)
        {
            yield return new WaitForSeconds(1);
        }
        else
        {
            challenge.Active = false;
            yield break;
        }
    }
    */

    // DEFINITELY CAN PUT THIS ALL INTO A SINGLE CUSTOM CHALLENGE
    IEnumerator TopSpeedTrigger(TopSpeedChallenge challenge)
    {
        challenge.Achieved = true;
        challenge.Image.SetActive(true);
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challenge.Image.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }
    
    IEnumerator MaintainSpeedTrigger(MaintainSpeedChallenge challenge)
    {
        challenge.Achieved = true;
        challenge.Image.SetActive(true);
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challenge.Image.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    IEnumerator RacePlacementTrigger(RacePlacementChallenge challenge)
    {
        challenge.Achieved = true;
        challenge.Image.SetActive(true);
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challenge.Image.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    IEnumerator TotalDistanceTrigger(TotalDistanceChallenge challenge)
    {
        challenge.Achieved = true;
        challenge.Image.SetActive(true);
        challengeTitle.GetComponent<Text>().text = challenge.Title;
        challengeDescription.GetComponent<Text>().text = challenge.Description;
        challengePanel.SetActive(true);

        yield return new WaitForSeconds(7);

        challengePanel.SetActive(false);
        challenge.Image.SetActive(false);
        challengeTitle.GetComponent<Text>().text = "";
        challengeDescription.GetComponent<Text>().text = "";
    }

    // Tracks the speed every second
    IEnumerator MaintainSpeedTracker(MaintainSpeedChallenge challenge, float time)
    {
        // Loops and calculate the time that has passed
        for (float deltaTime = 0; ; deltaTime = Time.time - time)
        {
            // Calculate the current speed
            float speed = CalculateSpeed();

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

    // Calculate the speed bsaed off the bluetooth rpm
    private float CalculateSpeed()
    {
        // Trying to hard code pi
        float rpmRatio = 1;
        float wheelDiameter = (78 * 2.54f) / 100000;
        float distancePerCount = wheelDiameter * 3.14f * rpmRatio;
        float speedMultiplier = distancePerCount * 60 / 2;
        return Bike.Instance.RPM * speedMultiplier;
    }

    // Calculate the distance based off the bluetooth rotation
    private float CalculateDistance()
    {
        // Trying to hard code pi
        float wheelDiameter = (78 * 2.54f) / 100000;
        float wheelCircumference = wheelDiameter * 3.14f;
        return Bike.Instance.Count * wheelCircumference;
    }
}

/*
public class CustomChallenge
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public float Speed { get; set; }
    public float Time { get; set; }
    public float Distance { get; set; }
    public int Rank { get; set; }

    public bool Active { get; set; }
    public bool Achieved { get; set; }

    private CustomChallenge()
    {

    }

    // Top Speed
    public static CustomChallenge TopSpeed(GameObject image, float speed)
    {
        CustomChallenge challenge = new CustomChallenge();
        challenge.Title = "Top Speed Challenge";
        challenge.Image = image;
        challenge.Description = "Reach a top speed of " + speed.ToString() + " mph";
        challenge.Speed = speed;
        challenge.Achieved = false;
        return challenge;
    }

    // Maintain Speed
    public static CustomChallenge MaintainSpeed(GameObject image, float speed, float time)
    {
        CustomChallenge challenge = new CustomChallenge();
        challenge.Title = "Maintain Speed Challenge";
        challenge.Image = image;
        challenge.Description = "Maintain a speed of " + speed.ToString() + " or higher for " + time.ToString() + " seconds";
        challenge.Speed = speed;
        challenge.Time = time;
        challenge.Active = false;
        challenge.Achieved = false;
        return challenge;
    }

    // Race Placement
    public static CustomChallenge RacePlacement(GameObject image, int rank)
    {
        // Does not work with double digits right now
        string rankString = rank.ToString();
        switch (rank)
        {
            case 1:
                rankString += "st";
                break;
            case 2:
                rankString += "nd";
                break;
            case 3:
                rankString += "rd";
                break;
            default:
                rankString += "th";
                break;

        }

        CustomChallenge challenge = new CustomChallenge();
        challenge.Title = "Final Placement Challenge";
        challenge.Image = image;
        challenge.Description = "Finish in " + rankString + " place";
        challenge.Rank = rank;
        challenge.Achieved = false;
        return challenge;
    }

    // Total Distance
    public static CustomChallenge TotalDistance(GameObject image, float distance)
    {
        CustomChallenge challenge = new CustomChallenge();
        challenge.Title = "Total Distance Challenge";
        challenge.Image = image;
        challenge.Description = "Travel at least " + distance.ToString() + " miles";
        challenge.Distance = distance;
        challenge.Achieved = false;
        return challenge;
    }
}
*/

// Challenges for reaching a certain speed
public class TopSpeedChallenge
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Speed { get; set; }
    public bool Achieved { get; set; }

    public TopSpeedChallenge(GameObject image, float speed)
    {
        this.Title = "Top Speed Challenge";
        this.Image = image;
        this.Description = "Reach a top speed of " + speed.ToString() + " mph";
        this.Speed = speed;
        this.Achieved = false;
    }
}

// Challenges for maintaining a certain speed for a certain amount of time
public class MaintainSpeedChallenge
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Speed { get; set; }
    public float Time { get; set; }
    public bool Active { get; set; }
    public bool Achieved { get; set; }

    public MaintainSpeedChallenge(GameObject image, float speed, float time)
    {
        this.Title = "Maintain Speed Challenge";
        this.Image = image;
        this.Description = "Maintain a speed of " + speed.ToString() + " or higher for " + time.ToString() + " seconds";
        this.Speed = speed;
        this.Time = time;
        this.Active = false;
        this.Achieved = false;
    }
}

// Challenges for getting a certain rank after a race finishes
public class RacePlacementChallenge
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Rank { get; set; }
    public bool Achieved { get; set; }

    public RacePlacementChallenge(GameObject image, int rank)
    {
        string rankString = "";
        switch (rank)
        {
            case 1:
                rankString = rank.ToString() + "st";
                break;
            case 2:
                rankString = rank.ToString() + "nd";
                break;
            case 3:
                rankString = rank.ToString() + "rd";
                break;
            default:
                rankString = rank.ToString() + "th";
                break; 

        }

        this.Title = "Final Placement Challenge";
        this.Image = image;
        this.Description = "Finish in " + rankString + " place";
        this.Rank = rank;
        this.Achieved = false;
    }
}

public class TotalDistanceChallenge
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Distance { get; set; }
    public bool Achieved { get; set; }

    public TotalDistanceChallenge(GameObject image, float distance)
    {
        this.Title = "Total Distance Challenge";
        this.Image = image;
        this.Description = "Travel at least " + distance.ToString() + " miles";
        this.Distance = distance;
        this.Achieved = false;
    }
}

public interface IChallenge
{
    IEnumerator AchievedTrigger();
}