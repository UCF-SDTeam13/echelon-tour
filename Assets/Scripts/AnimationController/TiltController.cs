using UnityEngine;

public class TiltController : MonoBehaviour
{
    // Tracker and angle value varies a lot so there needs to be a leeway to make sure they are upright
    [SerializeField] private float leeway = 15.0f;
    [SerializeField] private float angle;

    private Tracker tracker;
    private Vector3 initialEuler;

    private void Start()
    {
        // Get the component and save the initial euler angles
        tracker = GetComponentInParent<Tracker>();
        initialEuler = transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        // Calculate angle for tilting value
        Vector3 up = new Vector3(0, 1, 0);
        angle = Vector3.Dot(Vector3.Cross(transform.forward, tracker.targetForward), up);
        angle = (angle / 2.0f);

        // Checks if between leeway to tilt object to upright or not
        if(angle < leeway && angle > -leeway)
        {
            // Lerp to where the player is upright again
            transform.localEulerAngles = AngleLerp(transform.localEulerAngles, initialEuler, Time.deltaTime);
        }
        else
        {
            // Lerp between the current euler to the new adjusted euler
            transform.localEulerAngles = AngleLerp(transform.localEulerAngles,
                                                    new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle), Time.deltaTime);
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
}
