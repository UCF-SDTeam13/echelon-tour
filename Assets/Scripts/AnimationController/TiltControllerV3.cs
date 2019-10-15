using UnityEngine;

public class TiltControllerV3 : MonoBehaviour
{
    [SerializeField] private float leeway = 2;
    [SerializeField] private float tiltValue = 75;
    [SerializeField] private float turnAngle;
    [SerializeField] private float tiltAngle;
    [SerializeField] private Vector3 inverseTransformPoint;

    //[SerializeField] private GameObject cyclist = null;
    [SerializeField]
    private GameObject helper = null;

    private Tracker tracker;

    public Quaternion lookRotation;
    public Quaternion currentRotation;
    public Quaternion newRotation;

    private Vector3 initialEuler;

    private void Start()
    {
        tracker = GetComponentInParent<Tracker>();
        initialEuler = transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        CalculateTurn();
        CalculateTilt();
    }

    private void CalculateTurn()
    {
        Vector3 targetPosition = new Vector3(tracker.targetFarther.x, 0, tracker.targetFarther.z);
        Vector3 helperPosition = new Vector3(helper.transform.position.x, 0, helper.transform.position.z);
        Vector3 targetDirection = targetPosition - helperPosition;

        Vector3 helperForward = new Vector3(helper.transform.forward.x, 0, helper.transform.forward.z);

        turnAngle = Mathf.Acos(Vector3.Dot(helperForward.normalized, targetDirection.normalized));

        /////////////////////////////////////////////////////////////////////////////////////////////////

        // Calculate direction
        lookRotation = Quaternion.LookRotation(targetDirection);

        // Get the current rotation and lerp for the final rotation
        //currentRotation = transform.rotation;
        currentRotation = helper.transform.rotation;
        newRotation = Quaternion.Lerp(currentRotation, lookRotation, 1);

        // Use x (+ means target on right) (- means target on left)
        inverseTransformPoint = transform.InverseTransformPoint(targetPosition);

        // Get the turn angle amount based off the differences in lerp
        // if statement is opposite because if the object is on the right side (+) then it needs to tilt (-)
        turnAngle = Mathf.Abs(turnAngle) * (inverseTransformPoint.x > 0 ? -1 : 1);
    }

    // Cyclist transform
    private void CalculateTilt()
    {
        tiltAngle = turnAngle * tiltValue;
        Vector3 cyclistEuler = transform.localEulerAngles;

        if (tiltAngle < -leeway || tiltAngle > leeway)
        {
            // Lerp between the current euler to the new adjusted euler
            transform.localEulerAngles = AngleLerp(cyclistEuler,
                                                    new Vector3(cyclistEuler.x, cyclistEuler.y, tiltAngle), Time.deltaTime);
        }
        else
        {
            // Lerp to where the player is upright again
            transform.localEulerAngles = AngleLerp(cyclistEuler, initialEuler, Time.deltaTime);
        }
    }

    // New Lerp function as using negative values for rotation causes errors
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
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, tracker.targetFarther);
        }
    }
}
