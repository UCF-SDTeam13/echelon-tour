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
    public CustomChallenge challenge01;

    public GameObject MaintainSpeedImage;
    public CustomChallenge challenge02;

    private void Start()
    {
        // Values can be obtained and adjusted later
        float topSpeed = 50;
        float maintainSpeed = 30;
        float time = 60;

        challenge01 = new CustomChallenge(TopSpeedImage, topSpeed);
        challenge02 = new CustomChallenge(MaintainSpeedImage, maintainSpeed, time);
    }
    
    private void Update()
    {
        float speed = calculateSpeed();

        if (challenge01.Achieved == false && challenge01.Active == true && speed >= challenge01.Speed)
        {
            StartCoroutine(ChallengerTrigger(challenge01));
        }

        if(challenge02.Achieved == false && challenge02.Active == false && speed >= challenge02.Speed)
        {
            float time = Time.time;
            challenge02.Active = true;
            StartCoroutine(Challenge02Tracker(time));
        }
    }

    IEnumerator ChallengerTrigger(CustomChallenge challenge)
    {
        challenge.Active = false;
        challenge.Achieved = true;
        yield return new WaitForSeconds(5);
    }

    IEnumerator Challenge02Tracker(float time)
    {
        float deltaTime = Time.time - time;
        float speed = calculateSpeed();
        
        if(deltaTime >= challenge02.Time)
        {
            StartCoroutine(ChallengerTrigger(challenge02));
            yield break;
        }
        else if (speed >= challenge02.Speed)
        {
            yield return new WaitForSeconds(1);
        }
        else
        {
            challenge02.Active = false;
            yield break;
        }
    }

    // May want to put this in one spot
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

public class CustomChallenge
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public float Speed { get; set; }
    public float Time { get; set; }
    public bool Active { get; set; }
    public bool Achieved { get; set; }

    public CustomChallenge(GameObject image, float speed)
    {
        this.Title = "Top Speed Challenge";
        this.Image = image;
        this.Description = "Reach a top speed of " + speed.ToString();
        this.Speed = speed;
        this.Active = true;
        this.Achieved = false;
    }

    public CustomChallenge(GameObject image, float speed, float time)
    {
        this.Title = "Maintain Speed Challenge";
        this.Image = image;
        this.Description = "Maintain a speed of " + speed.ToString() + " for " + time.ToString() + " seconds";
        this.Speed = speed;
        this.Time = time;
        this.Active = false;
        this.Achieved = false;
    }
}
