using UnityEngine;

public class TimeSelectCheck : MonoBehaviour
{
    public void SetTime(float time)
    {
        PlayerPrefs.SetFloat("MatchTime", time);
    }
}
