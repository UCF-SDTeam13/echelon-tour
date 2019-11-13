[System.Serializable]
public class PlayerData
{
    public float avgSpeed;
    public float avgRPM;
    public float calories;
    // Other things depending on what personal stats we want to show

    public PlayerData(float speed, float rpm, float calories)
    {
        avgSpeed = speed;
        avgRPM = rpm;
        this.calories = calories;
    }
}
