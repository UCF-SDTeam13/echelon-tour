using UnityEngine;

public class TiltController : MonoBehaviour
{
    // Tracker and angle value varies a lot so there needs to be a leeway to make sure they are upright
    [SerializeField] private float lowLeeway = 10;
    [SerializeField] private float highLeeWay = 20;
    [SerializeField] private float angle;
    [SerializeField] private float divNum = 2;

    private Tracker tracker;
    private Vector3 initialEuler;

    public Vector3 refForward;
    public Vector3 refRight;
    public Vector3 newDirection;

    public float mathAngle;

    private void Start()
    {
        // Get the component and save the initial euler angles
        tracker = GetComponentInParent<Tracker>();
        initialEuler = transform.localEulerAngles;
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Calculate angle for tilting value
        //angle = Vector3.Dot(Vector3.Cross(transform.forward, tracker.targetForward), Vector3.up);
        //angle = (angle / divNum);

        //angle = Vector3.SignedAngle(transform.forward, tracker.targetForward - transform.position, Vector3.up);

        /*
        Vector3 refForward = transform.forward;
        refForward.y = 0;
        Vector3 refRight = Vector3.Cross(Vector3.up, refForward);
        refRight.y = 0;
        Vector3 newDirection = tracker.targetForward - transform.position;
        newDirection.y = 0;
        float angleVal = Vector3.Angle(newDirection, refForward);
        float sign = Mathf.Sign(Vector3.Dot(newDirection, refRight));
        angle = sign * angleVal;
        */

        CalculateAngle();

        /*
        // Checks if between leeway to tilt object to upright or not
        if((angle > -highLeeway && angle < -lowLeeway) || (angle > lowLeeway && angle < highLeeway))
        {
            // Lerp between the current euler to the new adjusted euler
            transform.localEulerAngles = AngleLerp(transform.localEulerAngles,
                                                    new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle), Time.deltaTime);
        }
        else
        {
            // Lerp to where the player is upright again
            transform.localEulerAngles = AngleLerp(transform.localEulerAngles, initialEuler, Time.deltaTime);
        }
        */
    }

    public Quaternion lookRotation;
    public Quaternion lookRotation2;
    public Quaternion initialRotation;
    public Quaternion currRot;
    public Quaternion newRot;
    public GameObject target;
    public Vector3 inverseTransformPoint;
    private int index = 0;

    private void CalculateAngle()
    {
        //angle = Mathf.Acos(Vector3.Dot(transform.forward.normalized, tracker.targetForward.normalized));

        // I fucking hate this shit
        //transform.InverseTransformPoint(tracker.targetForward);

        /*
        Vector3 forward = transform.forward;
        Vector3 direction = tracker.targetFarther - transform.position;

        forward.y = 0;
        direction.y = 0;
        angle = Mathf.Acos(Vector3.Dot(forward.normalized, direction.normalized));
        */

        //Still fucking hate this shit
        Vector3 targetPosition = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        Vector3 transformPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetDirection = targetPosition - transformPosition;
        lookRotation = Quaternion.LookRotation(targetDirection);
        lookRotation2 = Quaternion.LookRotation(target.transform.position - transform.position);
        currRot = transform.rotation;

        Quaternion currRotation = transform.rotation;
        newRot = Quaternion.Lerp(currRotation, lookRotation, 1f);
        // Use x (+ means target on right) (- means target on left)
        inverseTransformPoint = transform.InverseTransformPoint(target.transform.position);
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
            //Gizmos.DrawLine(transform.position, tracker.targetForward);
            Gizmos.DrawLine(transform.position, target.transform.position);

            Gizmos.color = Color.green;
            //Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}
