using UnityEngine;

public class TiltControllerV2 : MonoBehaviour
{
    [SerializeField] private float leeway = 15;
    [SerializeField] private float tiltValue = 15;
    [SerializeField] private float turnAngle;
    [SerializeField] private float tiltAngle;
    [SerializeField] private Vector3 inverseTransformPoint;

    //[SerializeField] private GameObject cyclist = null;
    [SerializeField] private GameObject helper = null;

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
        // 0 the y to ignore height differences
        Vector3 targetPosition = new Vector3(tracker.targetLookAhead.x, 0, tracker.targetLookAhead.z);
        //Vector3 transformPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 transformPosition = new Vector3(helper.transform.position.x, 0, helper.transform.position.z);
        Vector3 targetDirection = targetPosition - transformPosition;

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
        turnAngle = Mathf.Abs(newRotation.y - currentRotation.y) * (inverseTransformPoint.x > 0 ? -1 : 1);
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
            Gizmos.DrawLine(transform.position, tracker.targetLookAhead);
        }
    }
}
