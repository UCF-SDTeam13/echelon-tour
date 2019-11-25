using UnityEngine;

public class SoloSpawnManager : MonoBehaviour
{
    public GameObject maleModel1;
    public GameObject femaleModel1;

    public GameObject playerCam = null;
    public GameObject minimapCam = null;

    private void Awake()
    {
        string modelString = PlayerPrefs.GetString("Model");
        GameObject model;

        switch (modelString)
        {
            case "MaleModel1":
                model = maleModel1;
                break;
            case "FemaleModel1":
                model = femaleModel1;
                break;
            default:
                model = maleModel1;
                break;
        }

        model.SetActive(true);
        playerCam.GetComponent<CameraPivot>().SetTarget(model);
        minimapCam.GetComponent<MinimapFollow>().target = model;
    }
}
