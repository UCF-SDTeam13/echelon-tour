using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject femalePrefab = null;
    [SerializeField] private GameObject malePrefab = null;
    [SerializeField] private int numRoutes = 8;

    public Transform lineStart, lineEnd;

    private void Start()
    {
        Spawn(1, 0);
        Spawn(0, 1);
        Spawn(1, 2);
        Spawn(0, 3);
        Spawn(1, 4);
        Spawn(0, 5);
        Spawn(1, 6);
        Spawn(0, 7);
    }

    private void Spawn(int gender, int index)
    {
        float xRange = lineEnd.position.x - lineStart.position.x;
        float yRange = lineEnd.position.y - lineStart.position.y;
        float zRange = lineEnd.position.z - lineStart.position.z;

        // Left to Right Index
        // 0 2 3 4 5 6 7 1
        float partition = 1f / (numRoutes - 1);
        float value;

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

        Vector3 spawnLocation = new Vector3(lineStart.position.x + (xRange * value),
                                                lineStart.position.y + (yRange * value),
                                                    lineStart.position.z + (zRange * value));

        Quaternion spawnRotation = new Quaternion(transform.rotation.x,
                                                     transform.rotation.y,
                                                        transform.rotation.z,
                                                            transform.rotation.w);

        GameObject prefab = gender == 0 ? malePrefab : femalePrefab;
        GameObject spawnInstance = Instantiate(prefab, spawnLocation, spawnRotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lineStart.position, lineEnd.position);

        Gizmos.DrawCube(lineStart.position, Vector3.one);
        Gizmos.DrawCube(lineEnd.position, Vector3.one);
    }
}
