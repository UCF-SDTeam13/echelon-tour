using System.Collections;
using UnityEngine;

public class ChallengeTesting : MonoBehaviour
{
    public FakeBLE fakeBLE;

    public TopSpeedChallenge tsc01;
    public MaintainSpeedChallenge msc01;
    public RacePlacementChallenge rpc01;
    public TotalDistanceChallenge tdc01;

    public GameObject TopSpeedImage;
    public GameObject MaintainSpeedImage;
    public GameObject RacePlacementImage;
    public GameObject TotalDistanceImage;

    private void Awake()
    {
        fakeBLE = fakeBLE.GetComponent<FakeBLE>();
        tsc01 = new TopSpeedChallenge(TopSpeedImage, 30);
        msc01 = new MaintainSpeedChallenge(MaintainSpeedImage, 30, 20);
        tdc01 = new TotalDistanceChallenge(TotalDistanceImage, 30); //Not sure of value
    }

    private void Update()
    {
        //Debug.Log("RPM " + fakeBLE.rpm + " Count " + fakeBLE.count + " Speed " + calculateSpeed(fakeBLE.rpm) + " Distance " + calculateDistance(fakeBLE.count));
        if (tsc01.Achieved == false && calculateSpeed(fakeBLE.rpm) >= tsc01.Speed)
        {
            StartCoroutine(TopSpeedTrigger(tsc01));
        }

        if (msc01.Achieved == false && msc01.Active == false && calculateSpeed(fakeBLE.rpm) >= msc01.Speed)
        {
            Debug.Log("0");
            float startTime = Time.time;
            msc01.Active = true;
            StartCoroutine(MaintainSpeedTracker(msc01, startTime));
        }

        if (tdc01.Achieved == false && calculateDistance(fakeBLE.count) >= tdc01.Distance)
        {
            StartCoroutine(TotalDistanceTrigger(tdc01));
        }
    }

    IEnumerator TopSpeedTrigger(TopSpeedChallenge challenge)
    {
        challenge.Achieved = true;
        Debug.Log("TOPSPEEDACHIEVED");
        yield return new WaitForSeconds(5);
    }

    IEnumerator MaintainSpeedTrigger(MaintainSpeedChallenge challenge)
    {
        challenge.Achieved = true;
        Debug.Log("MAINTAINSPEEDACHIEVED");
        yield return new WaitForSeconds(5);
    }

    IEnumerator TotalDistanceTrigger(TotalDistanceChallenge challenge)
    {
        challenge.Achieved = true;
        Debug.Log("TOTALDISTANCEACHIEVED");
        yield return new WaitForSeconds(5);
    }

    IEnumerator MaintainSpeedTracker(MaintainSpeedChallenge challenge, float time)
    {
        for(float deltaTime = 0; ; deltaTime = Time.time - time)
        {
            float speed = calculateSpeed(fakeBLE.rpm);

            if (deltaTime >= challenge.Time && speed >= challenge.Speed)
            {
                StartCoroutine(MaintainSpeedTrigger(challenge));
                yield break;
            }
            else if (speed >= challenge.Speed)
            {
                Debug.Log("DeltaTime " + deltaTime);
                yield return new WaitForSeconds(1);
            }
            else
            {
                Debug.Log("Speed fail");
                challenge.Active = false;
                yield break;
            }
        }
    }

    private float calculateSpeed(int rpm)
    {
        // Trying to hard code pi
        float rpmRatio = 1;
        float wheelDiameter = (78 * 2.54f) / 100000;
        float distancePerCount = wheelDiameter * 3.14f * rpmRatio;
        float speedMultiplier = distancePerCount * 60;
        return rpm * speedMultiplier;
    }

    private float calculateDistance(int count)
    {
        // Trying to hard code pi
        float wheelDiameter = (78 * 2.54f) / 100000;
        float wheelCircumference = wheelDiameter * 3.14f;
        return count * wheelCircumference;
    }
}
