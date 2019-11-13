using UnityEngine;

public class SpawnManagerV2 : MonoBehaviour
{
    [SerializeField] private GameObject femalePrefab = null;
    [SerializeField] private GameObject malePrefab = null;

    [SerializeField] private int numRoutes = 8;

    // Used to create the spawning line
    [SerializeField] private Transform lineStart = null;
    [SerializeField] private Transform lineEnd = null;

    [SerializeField] private GameObject cam = null;

    [SerializeField] private GameObject[] players;

    [SerializeField] private GameObject multiCircuit;

    private void Start()
    {
        // Testing values, needs to be adjusted depending on Gamelift
        players = new GameObject[numRoutes];
        for (int i = 0; i < numRoutes; i++)
        {
            int j = i % 2;
            Spawn(j, i);
        }
        cam.GetComponent<CameraPivot>().SetTarget(players[1]);
    }

    // Spawn function (needs to get adjusted)
    private void Spawn(int gender, int index)
    {
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
        GameObject prefab = gender == 0 ? malePrefab : femalePrefab;
        prefab.GetComponent<BezierTracker>().circuit = multiCircuit.GetComponent<BezierMultiCircuitController>().SetTrack(index);
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
