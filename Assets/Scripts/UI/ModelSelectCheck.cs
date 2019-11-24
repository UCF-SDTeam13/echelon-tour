using UnityEngine;

public class ModelSelectCheck : MonoBehaviour
{
    // Eventually can save the string in the database instead and use the binary
    // formatter to keep it a little secured
    public void MaleModel1Selected()
    {
        PlayerPrefs.SetString("Model", "MaleModel1");
    }

    public void FemaleModel1Selected()
    {
        PlayerPrefs.SetString("Model", "FemaleModel1");
    }
}
