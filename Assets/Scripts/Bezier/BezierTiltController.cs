using UnityEngine;

public class BezierTiltController : MonoBehaviour
{
    [SerializeField] private float leeway = 2;
    [SerializeField] private float tiltValue = 100;
    [SerializeField] private float turnAngle;
    [SerializeField] private float tiltAngle;
    [SerializeField] private Transform helper;

    private Vector3 inverseTransformPoint;
    public BezierTracker tracker;
    private Vector3 initialEuler;
    public bool startTilt = false;

    public void StartTilting()
    {
        tracker = GetComponentInParent<BezierTracker>();
        initialEuler = transform.localEulerAngles;
        startTilt = true;
    }

    private void LateUpdate()
    {
        if(startTilt == false)
        {
            return;
        }

        // Calculate the turn and then get the tilt
        CalculateTurn();
        CalculateTilt();
    }

    private void CalculateTurn()
    {
        // Save the vectors to be on a single plane (y = 0)
        Vector3 targetPosition = new Vector3(tracker.targetLookAhead.x, 0, tracker.targetLookAhead.z);
        Vector3 helperPosition = new Vector3(helper.transform.position.x, 0, helper.transform.position.z);

        // Get the direction between the helper and target
        Vector3 targetDirection = targetPosition - helperPosition;

        // Save the forward of the helper and put it on a single plane
        Vector3 helperForward = new Vector3(helper.transform.forward.x, 0, helper.transform.forward.z);

        // Calculate the angle
        turnAngle = Mathf.Acos(Vector3.Dot(helperForward.normalized, targetDirection.normalized));

        // Get a point based off the target position
        inverseTransformPoint = transform.InverseTransformPoint(targetPosition);

        // Calculate the angle and whether it is positive or negative
        turnAngle = Mathf.Abs(turnAngle) * (inverseTransformPoint.x > 0 ? -1 : 1);
    }

    private void CalculateTilt()
    {
        // Multiple the angle by a set amount and get the current eulers
        tiltAngle = turnAngle * tiltValue;
        Vector3 cyclistEuler = transform.localEulerAngles;

        // Check if it is pass the leeway (small changes means it should stay the same)
        if (tiltAngle < -leeway || tiltAngle > leeway)
        {
            // Lerp the angle to it's new angle
            transform.localEulerAngles = AngleLerp(cyclistEuler,
                                                    new Vector3(cyclistEuler.x, cyclistEuler.y, tiltAngle), Time.deltaTime);
        }
        else
        {
            // Lerp back to the initial anglesB
            transform.localEulerAngles = AngleLerp(cyclistEuler, initialEuler, Time.deltaTime);
        }
    }

    // Lerp function as negative numbers causes issues in Unity
    Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if(startTilt == false)
            {
                return;
            }
            // Draw a line from the player to the look head
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, tracker.targetLookAhead);
        }
    }
}
