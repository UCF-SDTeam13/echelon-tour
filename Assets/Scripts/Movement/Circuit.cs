using UnityEngine;

public class Circuit : MonoBehaviour
{
    // Used for the two outer routes (actual nodes)
    [SerializeField] private Transform[][] outerNodes = null;

    // Used to cache the points of both outer and inner nodes
    [SerializeField] private Vector3[][] points = null;

    // Used for the distances between nodes
    [SerializeField] private float[][] distances = null;

    // Used to determine the overall number of "nodes"
    [SerializeField] private float substeps = 100;

    // Used for the number of possible routes (inner routes)
    [SerializeField] private int numRoutes = 2;

    // Used for the current length and number of physical nodes in a route
    public float[] length = null;

    private int numPoints;

    // Value holders for curve smoothing
    private int p0n, p1n, p2n, p3n;
    private Vector3 p0, p1, p2, p3;
    private float i;

    // Initialized all the positions and distances
    private void Awake()
    {
        numPoints = transform.GetChild(0).childCount;
        outerNodes = new Transform[transform.childCount][];
        length = new float[numRoutes];

        for (int i = 0; i < transform.childCount; i++)
        {
            outerNodes[i] = new Transform[numPoints];

            for (int j = 0; j < numPoints; j++)
            {
                outerNodes[i][j] = transform.GetChild(i).GetChild(j);
            }
        }
        
        SetPositionsAndDistances();
    }

    // Two functions for the tracker
    public RoutePoint GetRoutePoint(float dist, int index)
    {
        // Get the position of two nodes
        Vector3 p1 = GetRoutePosition(dist, index);
        Vector3 p2 = GetRoutePosition(dist + 0.1f, index);
        Vector3 delta = p2 - p1;
        return new RoutePoint(p1, delta.normalized);
    }

    // Return the position and direction of two nodes
    public struct RoutePoint
    {
        public Vector3 position;
        public Vector3 direction;

        public RoutePoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    // Function to set the position of nodes and the overall distances
    public void SetPositionsAndDistances()
    {
        // Total distance traveled
        float[] accumulativeDistances = new float[numRoutes];

        // Distance incremented by one for curve creation (repeated number) (initializing arrays)
        points = new Vector3[numRoutes][];
        distances = new float[numRoutes][];

        for(int i = 0; i < numRoutes; i++)
        {
            points[i] = new Vector3[numPoints];
            distances[i] = new float[numPoints + 1];
        }

        // Save the positions of all the nodes into an array for lookup
        // Partition for inner routes
        float partition = 1f / (numRoutes - 1);

        for (int i = 0; i < numRoutes; i++)
        {
            for(int j = 0; j < numPoints; j++)
            {
                // Check if physical nodes, if not physical, get the location between the physical nodes
                Vector3 point = i < transform.childCount ? outerNodes[i][j].position :
                                        Vector3.Lerp(outerNodes[0][j].position, outerNodes[1][j].position, partition);

                points[i][j] = point;
            }
            
            // Increment parition if not physical nodes
            if(i >= transform.childCount)
            {
                partition += 1f / (numRoutes - 1);
            }
        }

        // Save the accumulated distances between all the nodes into an array for lookup
        for (int i = 0; i < numRoutes; i++)
        {
            for (int j = 0; j < numPoints + 1; ++j)
            {
                Vector3 p1 = points[i][j % numPoints];
                Vector3 p2 = points[i][(j + 1) % numPoints];
                distances[i][j] = accumulativeDistances[i];
                accumulativeDistances[i] += (p1 - p2).magnitude;
            }
        }
    }

    // Function to create the curve
    public Vector3 GetRoutePosition(float dist, int index)
    {
        int point = 0;

        // Makes sure length is not 0
        if (length[index] == 0)
        {
            length[index] = distances[index][numPoints];
        }

        // Loops dist without it being greater than length and lower than 0 (similar to modulo) 
        dist = Mathf.Repeat(dist, length[index]);

        while (distances[index][point] < dist)
        {
            point++;
        }

        // Get two points that wraps around from start to end
        p1n = ((point - 1) + numPoints) % numPoints;
        p2n = point;

        // Find interpolation value between 2 points
        i = Mathf.InverseLerp(distances[index][p1n], distances[index][p2n], dist);

        p0n = ((point - 2) + numPoints) % numPoints;
        p3n = (point + 1) % numPoints;
        
        // Checks to make sure it is not the last point (dupe of first)
        p2n = p2n % numPoints;

        // 4 points for catmull-rom equation
        p0 = points[index][p0n];
        p1 = points[index][p1n];
        p2 = points[index][p2n];
        p3 = points[index][p3n];

        return CatmullRom(p0, p1, p2, p3, i);
    }

    // Catmull-rom equation https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline
    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
    {
        return 0.5f *
               ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
    }

    private void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }

    // Draws the lines in the editor when selected
    public void DrawGizmos(bool selected)
    {
        // Checks if there are 2 physical routes, both routes does at least 1 node, and if they have the same number of nodes
        if (transform.childCount == 2 && transform.GetChild(0).childCount != 0 && transform.GetChild(1).childCount != 0 &&
                        transform.GetChild(0).childCount == transform.GetChild(1).childCount)
        {
            // Initializing values
            numPoints = transform.GetChild(0).childCount;
            outerNodes = new Transform[transform.childCount][];
            length = new float[numRoutes];

            // Initializing physical nodes
            for (int i = 0; i < transform.childCount; i++)
            {
                outerNodes[i] = new Transform[numPoints];

                for (int j = 0; j < numPoints; j++)
                {
                    outerNodes[i][j] = transform.GetChild(i).GetChild(j);
                }
            }
            
            // Setting colors, positions, and distances for drawing
            Gizmos.color = Color.blue;
            SetPositionsAndDistances();

            // Initializing partition based of number of routes
            float partition = 1f / (numRoutes - 1);

            for (int i = 0; i < numRoutes; i++)
            {
                length[i] = distances[i][numPoints];

                // Save an initial position (starting node)
                Vector3 prev = i < transform.childCount ? outerNodes[i][0].position :
                                    Vector3.Lerp(outerNodes[0][0].position, outerNodes[1][0].position, partition);

                Vector3 temp = GetRoutePosition(1, i);

                // Loop through nodes based off overall "nodes" (substeps)
                for (float dist = 0; dist < length[i]; dist += (length[i] / substeps))
                {
                    // Get a position in front of the current distance
                    Vector3 next = GetRoutePosition(dist + 1, i);
                    Gizmos.DrawWireSphere(next, .1f);

                    // Determine what is the initial starting location
                    Vector3 firstNode = i < transform.childCount ? outerNodes[i][0].position :
                                            Vector3.Lerp(outerNodes[0][0].position, outerNodes[1][0].position, partition);

                    // Draw the line as long as it is not the first node
                    if (prev != firstNode)
                    {
                        Gizmos.DrawLine(prev, next);
                    }

                    // Change what is the previous node
                    prev = next;
                }

                // Draw a missing line
                Gizmos.DrawLine(prev, temp);

                // Increment partition as long as it is not a physical route
                if(i >= transform.childCount)
                {
                    partition = partition + (1f / (numRoutes - 1));
                }
            }
        } 
    }
}
