using System.Collections;
using UnityEngine;

public class SoloSpawnManager : MonoBehaviour
{
    public GameObject maleModel1;
    public GameObject femaleModel1;

    public GameObject playerCam = null;
    public GameObject minimapCam = null;

    public GameObject circuit = null;

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

        GameObject iModel = Instantiate(model, transform.position, transform.rotation);

        iModel.GetComponent<BezierTracker>().circuit = circuit.GetComponent<BezierCircuit>();
        iModel.SendMessage("StartTracking");
        iModel.SendMessage("StartFollowing");
        iModel.GetComponentInChildren<BezierTiltController>().SendMessage("StartTilting");

        playerCam.GetComponent<CameraPivot>().SetTarget(iModel);
        minimapCam.GetComponent<MinimapFollow>().target = iModel;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
