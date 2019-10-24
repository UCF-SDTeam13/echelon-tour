using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public enum Bezier
    {
        Linear = 0,
        Quadratic = 1,
        Cubic = 2
    }

    public Transform point0, point1, point2, point3;
    public Bezier bezierType;
    public int numPoints = 50;
    public Vector3[] positions;

    private void Awake()
    {
        // Initialize the positions
        positions = new Vector3[numPoints + 1];

        // Checks which curve we are drawing
        switch (bezierType)
        {
            case Bezier.Linear:
                DrawLinearCurve();
                break;
            case Bezier.Quadratic:
                DrawQuadraticCurve();
                break;
            case Bezier.Cubic:
                DrawCubicCurve();
                break;
            default:
                Debug.Log("Not a bezier type for some reason.");
                return;
        }
    }

    // Linear Bezier Curve
    private void DrawLinearCurve()
    {
        // Initial position
        positions[0] = CalculateLinearBezierPoint(0, point0.position, point1.position);
        
        // Loop through points and calculate the positions
        for(int i = 1; i <= numPoints; i++)
        {
            float t = i / (float) numPoints;
            positions[i] = CalculateLinearBezierPoint(t, point0.position, point1.position);
        }
    }

    // Quadratic Bezier Curve
    private void DrawQuadraticCurve()
    {
        // Initial position
        positions[0] = CalculateQuadraticBezierPoint(0, point0.position, point1.position, point2.position);

        // Loop through points and calculate the positions
        for (int i = 1; i <= numPoints; i++)
        {
            float t = i / (float) numPoints;
            positions[i] = CalculateQuadraticBezierPoint(t, point0.position, point1.position, point2.position);
        }
    }

    // Cubic Bezier Curve
    private void DrawCubicCurve()
    {
        // Initial position
        positions[0] = CalculateCubicBezierPoint(0, point0.position, point1.position, point2.position, point3.position);

        // Loop through points and calculate the positions
        for (int i = 1; i <= numPoints; i++)
        {
            float t = i / (float) numPoints;
            positions[i] = CalculateCubicBezierPoint(t, point0.position, point1.position, point2.position, point3.position);
        }
    }

    // Linear Bezier Point
    public Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        return p0 + (t * (p1 - p0));
    }

    // Quadratic Bezier Point
    public Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return (Mathf.Pow((1 - t), 2) * p0) + (2 * (1 - t) * t * p1) + (Mathf.Pow(t, 2) * p2);
    }

    // Cubic Bezier Point
    public Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (Mathf.Pow((1 - t), 3) * p0) + (3 * Mathf.Pow((1 - t), 2) * t * p1) + (3 * (1 - t) * Mathf.Pow(t, 2) * p2) + (Mathf.Pow(t, 3) * p3); 
    }

    private void OnDrawGizmos()
    {
        positions = new Vector3[numPoints + 1];

        // Checks if there are no points
        // Draw a curve based off which type of bezier
        if (numPoints == 0)
        {
            return;
        }
        else if (bezierType == Bezier.Linear)
        {
            // Checks if any of the 2 points are null
            if (point0 == null || point1 == null) return;
            DrawLinearCurve();
            Gizmos.color = Color.black;
            Gizmos.DrawLine(point0.position, point1.position);
        }
        else if (bezierType == Bezier.Quadratic)
        {
            // Checks if any of the 3 points are null
            if (point0 == null || point1 == null || point2 == null) return;
            DrawQuadraticCurve();
            Gizmos.color = Color.black;
            Gizmos.DrawLine(point0.position, point1.position);
            Gizmos.DrawLine(point1.position, point2.position);
        }
        else
        {
            // Checks if any of the 4 points are null
            if (point0 == null || point1 == null || point2 == null || point3 == null) return;
            DrawCubicCurve();
            Gizmos.color = Color.black;
            Gizmos.DrawLine(point0.position, point1.position);
            Gizmos.DrawLine(point2.position, point3.position);
        }

        // Draw a line from each position for editor visuals
        Gizmos.color = Color.blue;
        for(int i = 0; i < numPoints; i++)
        {
            Gizmos.DrawWireSphere(positions[i], .3f);
            Gizmos.DrawLine(positions[i], positions[i + 1]);
        }
    }
}
