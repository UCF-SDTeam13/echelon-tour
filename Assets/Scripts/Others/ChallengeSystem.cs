using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeSystem : MonoBehaviour
{
    public GameObject challengeBackground;
    public AudioSource challengeSound;
    public GameObject challengeTitle;
    public GameObject challengeImage;
    public GameObject challengeDescription;

    public GameObject TopSpeedImage;
    public GameObject MaintainSpeedImage;
    public GameObject RacePlacementImage;

    public TopSpeedChallenge tsc01;
    public MaintainSpeedChallenge msc01;
    public RacePlacementChallenge rpc01;

    public bool[] challengeStatuses;
    public int numChallenges;

    private void Start()
    {
        challengeStatuses = new bool[numChallenges];
    }
    
    private void Update()
    {

    }

    IEnumerator TopSpeedTrigger(TopSpeedChallenge challenge)
    {
        challenge.Achieved = true;
        yield return new WaitForSeconds(5);
    }
    
    IEnumerator MaintainSpeedTrigger(MaintainSpeedChallenge challenge)
    {
        challenge.Active = false;
        challenge.Achieved = true;
        yield return new WaitForSeconds(5);
    }

    IEnumerator RacePlacementTrigger(RacePlacementChallenge challenge)
    {
        challenge.Achieved = true;
        yield return new WaitForSeconds(5);
    }

    IEnumerator MaintainSpeedTracker(MaintainSpeedChallenge challenge, float time)
    {
        float deltaTime = Time.time - time;
        float speed = calculateSpeed();
        
        if(deltaTime >= challenge.Time)
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

    private float calculateSpeed()
    {
        // Trying to hard code pi
        float rpmRatio = 1;
        float wheelDiameter = (78 * 2.54f) / 100000;
        float distancePerCount = wheelDiameter * 3.14f * rpmRatio;
        float speedMultiplier = distancePerCount * 60;
        return Bike.Instance.RPM * speedMultiplier;
    }
}

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
                rankString = Rank.ToString() + "nd";
                break;
            case 3:
                rankString = Rank.ToString() + "rd";
                break;
            default:
                rankString = Rank.ToString() + "th";
                break; 

        }

        this.Title = "Final Placement Challenge";
        this.Image = image;
        this.Description = "Finish in " + rankString + " place";
        this.Rank = rank;
        this.Achieved = false;
    }
}