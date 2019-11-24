using UnityEngine;

public class MapSelectCheck : MonoBehaviour
{
    public void SetMap(string map)
    {
        PlayerPrefs.SetString("MapName", map);
    }
}
