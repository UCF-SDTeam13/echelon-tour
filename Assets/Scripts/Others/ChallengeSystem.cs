using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeSystem : MonoBehaviour
{
    public GameObject challengePanel;
    public AudioSource challengeSound;
    public GameObject challengeTitle;
    public GameObject challengeDescription;

    public GameObject TopSpeedImage;
    public GameObject MaintainSpeedImage;
    public GameObject RacePlacementImage;
    public GameObject TotalDistanceImage;

    public TopSpeedChallenge tsc01;
    public MaintainSpeedChallenge msc01;
    public RacePlacementChallenge rpc01;
    public TotalDistanceChallenge tdc01;

    public bool[] challengeStatuses;
    public int numChallenges;

    private void Awake()
    {
        // Need to find out how to change certain statuses
        //ChallengeData data = SaveSystem.LoadChallengeData();
        //challengeStatuses = new bool[numChallenges];

        tsc01 = new TopSpeedChallenge(TopSpeedImage, 30);
        msc01 = new MaintainSpeedChallenge(MaintainSpeedImage, 30, 30);
        rpc01 = new RacePlacementChallenge(RacePlacementImage, 3);
        tdc01 = new TotalDistanceChallenge(TotalDistanceImage, 10); //Not sure of value
    }
    
    private void Update()
    {
        // MAY NEED TO CHECK IF MATCH IS FINISHED FIRST
        if (tsc01.Achieved == false && calculateSpeed() >= tsc01.Speed)
        {
            StartCoroutine(TopSpeedTrigger(tsc01));
        }

        if (msc01.Achieved == false && msc01.Active == false && calculateSpeed() >= msc01.Speed)
        {
            float startTime = Time.time;
            msc01.Active = true;
            StartCoroutine(MaintainSpeedTracker(msc01, startTime));
        }

        /*
        if(rpc01.Achieved == false && FINALRANKPLACEHOLDER <= rpc01.Rank && MATCHFINISH == true)
        {
            StartCoroutine(RacePlacementTrigger(rpc01));
        }
        */

        if(tdc01.Achieved == false && calculateDistance() >= tdc01.Distance)
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

    IEnumerator MaintainSpeedTracker(MaintainSpeedChallenge challenge, float time)
    {
        for (float deltaTime = 0; ; deltaTime = Time.time - time)
        {
            float speed = calculateSpeed();

            if (deltaTime >= challenge.Time && speed >= challenge.Speed)
            {
                StartCoroutine(MaintainSpeedTrigger(challenge));
                yield break;
            }
            else if (speed >= challenge.Speed)
            {
                yield return new WaitForSeconds(1);
            }
            else
            {
                challenge.Active = false;
                yield break;
            }
        }
    }

    private float calculateSpeed()
    {
        // Trying to hard code pi
        float rpmRatio = 1;
        float wheelDiameter = (78 * 2.54f) / 100000;
        float distancePerCount = wheelDiameter * 3.14f * rpmRatio;
        float speedMultiplier = distancePerCount * 60;
        return Bike.Instance.RPM * speedMultiplier;
    }

    private float calculateDistance()
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