using UnityEngine;

public class SpawnManagerV2 : MonoBehaviour
{
    [SerializeField] private GameObject maleModel1 = null;
    [SerializeField] private GameObject femaleModel1 = null;

    [SerializeField] private int numRoutes = 8;

    // Used to create the spawning line
    [SerializeField] private Transform lineStart = null;
    [SerializeField] private Transform lineEnd = null;

    [SerializeField] private GameObject cam = null;

    [SerializeField] private GameObject[] players;

    [SerializeField] private GameObject multiCircuit;

    [SerializeField] private GameObject minimapCam;

    private void Awake()
    {
        players = new GameObject[numRoutes];
        RealTimeClient.Instance.CustomizationUpdate += SpawnPlayer;
    }

    private void Start()
    {
        Spawn(API.Instance.CharacterModelId, RealTimeClient.Instance.peerId - 1);
        cam.GetComponent<CameraPivot>().SetTarget(players[RealTimeClient.Instance.peerId - 1]);
        minimapCam.GetComponent<MinimapFollow>().target = players[RealTimeClient.Instance.peerId - 1];

        RealTimeClient.Instance.CustomizationUpdate += SpawnPlayer;
    }

    private void SpawnPlayer(object sender, CustomizationUpdateEventArgs e)
    {
        Spawn(e.characterModelId, e.peerId - 1);
    }

    // Spawn function (needs to get adjusted)
    private void Spawn(string model, int index)
    {
        Debug.Log("THIS IS THE INDEX " + index);
        // Ranges between 2 points
        float xRange = lineEnd.position.x - lineStart.position.x;
        float yRange = lineEnd.position.y - lineStart.position.y;
        float zRange = lineEnd.position.z - lineStart.position.z;

        // Left to Right Index
        // Index value determine what type of partition
        float partition = 1f / (numRoutes - 1);
        float value;

        value = partition * index;
        // The position where the player will spawn
        Vector3 spawnLocation = new Vector3(lineStart.position.x + (xRange * value),
                                                lineStart.position.y + (yRange * value),
                                                    lineStart.position.z + (zRange * value));

        // The rotation or direction the player will be set at
        Quaternion spawnRotation = new Quaternion(transform.rotation.x,
                                                     transform.rotation.y,
                                                        transform.rotation.z,
                                                            transform.rotation.w);

        // Check which prefab needs to be used
        GameObject prefab = null;
        switch (model)
        {
            case "maleModel1":
                prefab = maleModel1;
                break;
            case "femaleModel1":
                prefab = femaleModel1;
                break;
            default:
                Debug.Log("Model does not exist");
                break;
        }

        if(prefab == null)
        {
            prefab = maleModel1;
        }

        prefab.GetComponent<BezierTracker>().circuit = multiCircuit.GetComponent<BezierMultiCircuitController>().SetTrack(index);
        prefab.GetComponent<BezierFollowV2>().isMultiplayer = true;
        players[index] = Instantiate(prefab, spawnLocation, spawnRotation);
        //GameObject spawnInstance = Instantiate(prefab, spawnLocation, spawnRotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lineStart.position, lineEnd.position);

        Gizmos.DrawCube(lineStart.position, Vector3.one);
        Gizmos.DrawCube(lineEnd.position, Vector3.one);
    }
}
