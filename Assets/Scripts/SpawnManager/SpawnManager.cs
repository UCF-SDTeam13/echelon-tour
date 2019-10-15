using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject femalePrefab = null;
    [SerializeField] private GameObject malePrefab = null;

    [SerializeField] private int numRoutes = 8;

    // Used to create the spawning line
    [SerializeField] private Transform lineStart = null;
    [SerializeField] private Transform lineEnd = null;

    [SerializeField] private GameObject cam = null;

    [SerializeField] private GameObject[] players;

    private void Start()
    {
        // Testing values, needs to be adjusted depending on Gamelift
        players = new GameObject[numRoutes];
        Spawn(1, 0);
        Spawn(0, 1);
        Spawn(1, 2);
        Spawn(0, 3);
        Spawn(1, 4);
        Spawn(0, 5);
        Spawn(1, 6);
        Spawn(0, 7);
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
        // 0 2 3 4 5 6 7 1
        float partition = 1f / (numRoutes - 1);
        float value;

        // Index value determine what type of partition (weird adjustments due to tracker script)
        if(index == 0)
        {
            value = 0;
        }
        else if(index == 1)
        {
            value = partition * 7;
        }
        else
        {
            value = partition * (index - 1);
        }

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
