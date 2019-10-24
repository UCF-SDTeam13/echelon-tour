using UnityEngine;

public class BezierCircuit : MonoBehaviour
{
    public BezierCurve[] bezierCurves;
    public float[] lengths;

    private float[][] distances;
    private float currentProportion;

    private void Start()
    {
        // Initialize the distances and lengths
        distances = new float[bezierCurves.Length][];
        lengths = new float[bezierCurves.Length];
        float accumulativeDistances = 0;

        // Loop through all the lengths of each curve
        for(int i = 0; i < bezierCurves.Length; i++)
        {
            // Get the number of points that is set for each curve
            int numPoints = bezierCurves[i].numPoints;

            // Initialize and loop through the number of points to save the distances
            distances[i] = new float[numPoints];
            for(int j = 0; j < numPoints; j++)
            {
                // Save the accumulative distances
                accumulativeDistances += (bezierCurves[i].positions[j] - bezierCurves[i].positions[j + 1]).magnitude;
                distances[i][j] = accumulativeDistances;
            }

            // Save the max distance for the given curve
            lengths[i] = distances[i][numPoints - 1];
        }
    }

    public TrackPoint GetTrackPoint(float currentDistance)
    {
        Vector3 p1 = GetTrackPosition(currentDistance);
        Vector3 p2 = GetTrackPosition(currentDistance + 0.1f);
        Vector3 delta = p2 - p1;
        return new TrackPoint(p1, delta.normalized);
    }

    public struct TrackPoint
    {
        public Vector3 position;
        public Vector3 direction;

        public TrackPoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }

    // Used to get the current position on the track based off a distance
    public Vector3 GetTrackPosition(float currentDistance)
    {
        // Mod the distances to make sure it is under the max distance
        currentDistance = Mathf.Repeat(currentDistance, lengths[bezierCurves.Length - 1]);

        // Get the current Index to determine which curve is going to be used
        int currentIndex = 0;
        for (int i = 0; i < bezierCurves.Length; i++)
        {
            // Set the index when it is less than that curve's length
            if (currentDistance < lengths[i])
            {
                currentIndex = i;
                break;
            }
        }

        // Checks if we are on the first curve or the proceeding curves
        if (currentIndex == 0)
        {
            currentProportion = currentDistance / lengths[0];
        }
        else
        {
            // Minus the previous length to make sure it goes from 0-1 as that tells you where your position is
            currentProportion = (currentDistance - lengths[currentIndex - 1]) / (lengths[currentIndex] - lengths[currentIndex - 1]);
        }
        
        // Initialize a new point and the current curve
        Vector3 newPoint = new Vector3(0, 0, 0);
        BezierCurve currentBezier = bezierCurves[currentIndex];

        // Calculations based off which type of bezier curve
        switch (currentBezier.bezierType)
        {
            case BezierCurve.Bezier.Linear:
                newPoint = currentBezier.CalculateLinearBezierPoint(currentProportion,
                                currentBezier.point0.position, currentBezier.point1.position);
                break;
            case BezierCurve.Bezier.Quadratic:
                newPoint = currentBezier.CalculateQuadraticBezierPoint(currentProportion,
                                currentBezier.point0.position, currentBezier.point1.position,
                                    currentBezier.point2.position);
                break;
            case BezierCurve.Bezier.Cubic:
                newPoint = currentBezier.CalculateCubicBezierPoint(currentProportion,
                                currentBezier.point0.position, currentBezier.point1.position,
                                    currentBezier.point2.position, currentBezier.point3.position);
                break;
            default:
                Debug.Log("Not a bezier curve for some reason");
                break;
        }

        // return the point of the current curve
        return newPoint;
    }
}
