using UnityEngine;

public class StopWorkoutCheck : MonoBehaviour
{
    public void StopWorkout()
    {
        BLEPlugin.Instance.StopWorkout();
    }
}
