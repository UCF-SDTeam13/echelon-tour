using UnityEngine;

public class ModelSelectCheck : MonoBehaviour
{
    // Eventually can save the string in the database instead and use the binary
    // formatter to keep it a little secured

    private void Start()
    {
        CheckFirstTime();
    }
    
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

    public void CheckFirstTime()
    {
        if(PlayerPrefs.HasKey("FirstTime") == true){
            Debug.Log("FirstTime exists");
            if(PlayerPrefs.GetInt("FirstTime") != 1)
            {
                PlayerPrefs.SetInt("FirstTime", 1);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
        else
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            Debug.Log("FirstTime does not exist");
        }
    }
}
