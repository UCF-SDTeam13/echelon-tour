using UnityEngine;

public class ChallengeDemo : MonoBehaviour
{
    // Individual challenges images
    public GameObject topSpeedImage;
    public GameObject maintainSpeedImage;
    public GameObject racePlacementImage;
    public GameObject totalDistanceImage;
    public GameObject dailiesCompletedImage;

    public TopSpeed tsc01;
    public MaintainSpeed msc01;
    public RacePlacement rpc01;
    public TotalDistance tdc01;
    public DailiesCompleted dcc01;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerPrefs.SetInt("TopSpeed1", 0);
        PlayerPrefs.SetInt("MaintainSpeed1", 0);
        PlayerPrefs.SetInt("RacePlacement1", 0);
        PlayerPrefs.SetInt("TotalDistance1", 0);
        PlayerPrefs.SetInt("DailiesCompleted", 0);

        tsc01 = new TopSpeed(topSpeedImage, 20);
        msc01 = new MaintainSpeed(maintainSpeedImage, 10, 30);
        rpc01 = new RacePlacement(racePlacementImage, 3);
        tdc01 = new TotalDistance(totalDistanceImage, 2);
        dcc01 = new DailiesCompleted(dailiesCompletedImage);
    }
}

// Challenges for reaching a certain speed
public class TopSpeed
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Speed { get; set; }
    public bool Achieved { get; set; }

    public TopSpeed(GameObject image, float speed)
    {
        this.Title = "Top Speed Challenge";
        this.Image = image;
        this.Description = "Reach a top speed of " + speed.ToString() + " mph";
        this.Speed = speed;
    }
}

// Challenges for maintaining a certain speed for a certain amount of time
public class MaintainSpeed
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Speed { get; set; }
    public float Time { get; set; }
    public bool Active { get; set; }

    public MaintainSpeed(GameObject image, float speed, float time)
    {
        this.Title = "Maintain Speed Challenge";
        this.Image = image;
        this.Description = "Maintain a speed of " + speed.ToString() + " mph or higher for " + time.ToString() + " seconds";
        this.Speed = speed;
        this.Time = time;
        this.Active = false;
    }
}

// Challenges for getting a certain rank after a race finishes
public class RacePlacement
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Rank { get; set; }

    public RacePlacement(GameObject image, int rank)
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
        this.Description = "Finish in " + rankString + " place or higher";
        this.Rank = rank;
    }
}

public class TotalDistance
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public float Distance { get; set; }

    public TotalDistance(GameObject image, float distance)
    {
        this.Title = "Total Distance Challenge";
        this.Image = image;
        this.Description = "Travel at least " + distance.ToString() + " miles in a race";
        this.Distance = distance;
    }
}

public class DailiesCompleted
{
    public GameObject Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public DailiesCompleted(GameObject image)
    {
        this.Title = "Dailies Completed Challenge";
        this.Image = image;
        this.Description = "Complete all daily challenges";
    }
}
