using System.Collections;
using UnityEngine;

public class BezierFollow : MonoBehaviour
{
    [SerializeField] private Transform[] bezierRoutes;

    private int routeToGo = 0;
    public float tParam;
    private Vector3 position;
    public float speedModifier = 0;
    private bool coroutineAllowed = true;
    public int currentRoute = 0;
    public int currentPoint = 0;

    private BezierCurve[] bezierCurves;
    public Vector3 currentTarget;

    private void Start()
    {
        bezierCurves = new BezierCurve[bezierRoutes.Length];

        for (int i = 0; i < bezierRoutes.Length; i++)
        {
            bezierCurves[i] = bezierRoutes[i].GetComponent<BezierCurve>();
        }

        // Initial target (end of straight)
        currentTarget = bezierCurves[currentRoute].positions[1];
    }

    private void FixedUpdate()
    {
        // Debug.Log(currentRoute + " " + currentPoint);
        // Debug.Log(transform.position + " " + currentTarget);

        if (currentPoint == bezierCurves[currentRoute].numPoints + 1)
        {
            currentRoute++;
            currentRoute = currentRoute % bezierRoutes.Length;

            currentPoint = 0;
        }

        if (transform.position == currentTarget)
        {
            currentPoint++;
            currentTarget = bezierCurves[currentRoute].positions[currentPoint];
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speedModifier * Time.deltaTime);

        /*
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
        */
    }

    /*
    private IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineAllowed = false;
        BezierCurve bezierCurve = bezierRoutes[routeNumber].GetComponent<BezierCurve>();

        if (bezierCurve.bezierType == BezierCurve.Bezier.Linear)
        {
            p0 = bezierRoutes[routeNumber].GetChild(0).position;
            p1 = bezierRoutes[routeNumber].GetChild(1).position;

            while(tParam < 1)
            {
                tParam += Time.deltaTime * speedModifier;
                position = bezierCurve.CalculateLinearBezierPoint(tParam, p0, p1);
                transform.position = position;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (bezierCurve.bezierType == BezierCurve.Bezier.Quadratic)
        {
            p0 = bezierRoutes[routeNumber].GetChild(0).position;
            p1 = bezierRoutes[routeNumber].GetChild(1).position;
            p2 = bezierRoutes[routeNumber].GetChild(2).position;

            while (tParam < 1)
            {
                tParam += Time.deltaTime * speedModifier;
                position = bezierCurve.CalculateQuadraticBezierPoint(tParam, p0, p1, p2);
                transform.position = position;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            p0 = bezierRoutes[routeNumber].GetChild(0).position;
            p1 = bezierRoutes[routeNumber].GetChild(1).position;
            p2 = bezierRoutes[routeNumber].GetChild(2).position;
            p3 = bezierRoutes[routeNumber].GetChild(3).position;

            while (tParam < 1)
            {
                tParam += Time.deltaTime * speedModifier;
                position = bezierCurve.CalculateCubicBezierPoint(tParam, p0, p1, p2, p3);
                transform.position = position;
                yield return new WaitForEndOfFrame();
            }
        }

        tParam = 0;
        routeToGo += 1;
        if(routeToGo > bezierRoutes.Length - 1)
        {
            routeToGo = 0;
        }
        coroutineAllowed = true;
    }
    */
}
