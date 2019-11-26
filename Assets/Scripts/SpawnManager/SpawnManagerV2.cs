using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift.Realtime.Types;

public class SpawnManagerV2 : MonoBehaviour
{
    [SerializeField] private GameObject maleModel1 = null;
    [SerializeField] private GameObject femaleModel1 = null;

    [SerializeField] private int numRoutes = 8;

    // Used to create the spawning line
    [SerializeField] private Transform lineStart = null;
    [SerializeField] private Transform lineEnd = null;

    [SerializeField] private GameObject playerCam = null;
    [SerializeField] private GameObject minimapCam = null;

    //NEED TO CHECK TO LIST
    //[SerializeField] private GameObject[] players;
    [SerializeField] private Dictionary<int, GameObject> players;

    [SerializeField] private GameObject multiCircuit;

    [SerializeField] private bool isMultiplayer;

    private void Awake()
    {
        //players = new GameObject[numRoutes];
        players = new Dictionary<int, GameObject>();
        RealTimeClient.Instance.CustomizationUpdate += SpawnPlayer;
        RealTimeClient.Instance.PlayerDisconnect += DespawnPlayer;
        RealTimeClient.Instance.CustomizationUpdate += SpawnPlayer;
        BLEDebug.LogInfo($"Realtime PlayerId: {API.Instance.PlayerId}, PlayerSessionId: {API.Instance.PlayerSessionId}");
        RealTimeClient.Instance.Connect(API.Instance.PlayerId, API.Instance.DnsName, API.Instance.TcpPort, API.Instance.UdpPort, ConnectionType.RT_OVER_WEBSOCKET, API.Instance.PlayerSessionId, new byte[0]);
    }

    private void Start()
    {
        StartCoroutine("SpawnCoroutine");
    }

    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSecondsRealtime(10.0f);
        RealTimeClient.Instance.UpdateCustomization(PlayerPrefs.GetString("Model"));
        BLEDebug.LogInfo($"peerId :{RealTimeClient.Instance.peerId}");
        Spawn(API.Instance.CharacterModelId, RealTimeClient.Instance.peerId);
        playerCam.GetComponent<CameraPivot>().SetTarget(players[RealTimeClient.Instance.peerId]);
        minimapCam.GetComponent<MinimapFollow>().target = players[RealTimeClient.Instance.peerId];
    }
    private void SpawnPlayer(object sender, CustomizationUpdateEventArgs e)
    {
        Spawn(e.characterModelId, e.peerId);
    }

    private void DespawnPlayer(object sender, PlayerEventArgs e)
    {
        Despawn(e.peerId);
    }

    // Spawn function (needs to get adjusted)
    private void Spawn(string model, int index)
    {
        // Ranges between 2 points
        float xRange = lineEnd.position.x - lineStart.position.x;
        float yRange = lineEnd.position.y - lineStart.position.y;
        float zRange = lineEnd.position.z - lineStart.position.z;

        // Left to Right Index
        // Index value determine what type of partition
        float partition = 1f / (numRoutes - 1);
        float value;

        value = partition * (index - 1);
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
                prefab = maleModel1;
                Debug.Log("Model does not exist");
                break;
        }

        // Instantiate Prefab Before Changing It
        GameObject iModel = Instantiate(prefab, spawnLocation, spawnRotation);

        iModel.GetComponent<BezierTracker>().circuit = multiCircuit.GetComponent<BezierMultiCircuitController>().SetTrack(index - 1);

        if (isMultiplayer == true)
        {
            Debug.Log("Multiplayer Detected");
            iModel.GetComponent<BezierFollowV2>().isMultiplayer = true;
            iModel.GetComponent<BezierTracker>().isMultiplayer = true;
            iModel.GetComponent<PlayerStats>().peerId = index;
        }

        iModel.SendMessage("StartTracking");
        //players[index] = Instantiate(prefab, spawnLocation, spawnRotation);
        players.Add(index, iModel);
        //GameObject spawnInstance = Instantiate(prefab, spawnLocation, spawnRotation);
    }

    // Despawn
    private void Despawn(int index)
    {
        // Set active to false and remove the reference
        players[index].SetActive(false);
        players.Remove(index);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lineStart.position, lineEnd.position);

        Gizmos.DrawCube(lineStart.position, Vector3.one);
        Gizmos.DrawCube(lineEnd.position, Vector3.one);
    }
}
