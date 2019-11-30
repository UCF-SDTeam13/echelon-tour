using UnityEngine;

public class ModelSelectCheck : MonoBehaviour
{
    // Eventually can save the string in the database instead and use the binary
    // formatter to keep it a little secured
    async public void MaleModel1Selected()
    {
        await API.Instance.SetCustomization("MaleModel1");
        PlayerPrefs.SetString("Model", "MaleModel1");

    }

    async public void FemaleModel1Selected()
    {
        await API.Instance.SetCustomization("FemaleModel1");
        PlayerPrefs.SetString("Model", "FemaleModel1");

    }
}
