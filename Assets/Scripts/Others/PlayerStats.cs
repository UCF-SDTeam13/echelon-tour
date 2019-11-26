using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int peerId;
    public int rpm;
    public int count;
    public float distance;

    private void Start()
    {
        // NOTE: Moved to StartTracking
    }
    public void StartTracking()
    {
        RealTimeClient.Instance.StatsUpdate += UpdatePlayerStats;
    }
    private void UpdatePlayerStats(object sender, StatsUpdateEventArgs e)
    {
        rpm = e.rpm;
        count = e.rotations;
        distance = CalculateDistance(count);
    }

    private float CalculateDistance(int count)
    {
        float wheelDiameter = (78 * 2.54f) / 100000;
        float wheelCircumference = wheelDiameter * 3.14f;
        return wheelCircumference * count;
    }
}
