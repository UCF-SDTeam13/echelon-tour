using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChallengeUI : MonoBehaviour
{
    public GameObject topSpeed;
    public GameObject maintainSpeed;
    public GameObject racePlacement;
    public GameObject totalDistance;
    public GameObject dailiesCompleted;

    public ChallengeDemo challenges;

    // Start is called before the first frame update
    void Awake()
    {
        challenges = GameObject.FindGameObjectWithTag("Challenges").GetComponent<ChallengeDemo>();
    }

    void Start()
    {
        CheckTopSpeed();
        CheckMaintainSpeed();
        CheckRacePlacement();
        CheckTotalDistance();
        CheckDailiesCompleted();
    }

    private void CheckTopSpeed()
    {
        if (PlayerPrefs.GetInt("TopSpeed1") != 1)
        {
            topSpeed.GetComponentInChildren<TMP_Text>().text = challenges.tsc01.Title + ": " + challenges.tsc01.Description;
        }
        else
        {
            topSpeed.GetComponentInChildren<TMP_Text>().text = challenges.tsc01.Title + ": COMPLETED";
        }
    }

    private void CheckMaintainSpeed()
    {
        if (PlayerPrefs.GetInt("MaintainSpeed1") != 1)
        {
            maintainSpeed.GetComponentInChildren<TMP_Text>().text = challenges.msc01.Title + ": " + challenges.msc01.Description;
        }
        else
        {
            topSpeed.GetComponentInChildren<TMP_Text>().text = challenges.msc01.Title + ": COMPLETED";
        }
    }

    private void CheckRacePlacement()
    {
        if (PlayerPrefs.GetInt("RacePlacement1") != 1)
        {
            racePlacement.GetComponentInChildren<TMP_Text>().text = challenges.rpc01.Title + ": " + challenges.rpc01.Description;
        }
        else
        {
            topSpeed.GetComponentInChildren<TMP_Text>().text = challenges.rpc01.Title + ": COMPLETED";
        }
    }

    private void CheckTotalDistance()
    {
        if (PlayerPrefs.GetInt("TotalDistance1") != 1)
        {
            totalDistance.GetComponentInChildren<TMP_Text>().text = challenges.tdc01.Title + ": " + challenges.tdc01.Description;
        }
        else
        {
            topSpeed.GetComponentInChildren<TMP_Text>().text = challenges.tdc01.Title + ": COMPLETED";
        }
    }

    private void CheckDailiesCompleted()
    {
        if (PlayerPrefs.GetInt("DailiesCompleted") != 1)
        {
            dailiesCompleted.GetComponentInChildren<TMP_Text>().text = challenges.dcc01.Title + ": " + challenges.dcc01.Description;
        }
        else
        {
            topSpeed.GetComponentInChildren<TMP_Text>().text = challenges.dcc01.Title + ": COMPLETED";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
