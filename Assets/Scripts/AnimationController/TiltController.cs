using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltController : MonoBehaviour
{
    private Tracker tracker;
    private Vector3 initialEuler;

    public float leeway = 15.0f;
    public float angle;
    public float z;

    private void Start()
    {
        tracker = GetComponentInParent<Tracker>();
        initialEuler = transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        // Calculate angle for tilting value
        Vector3 up = new Vector3(0, 1, 0);
        angle = Vector3.Dot(Vector3.Cross(transform.forward, tracker.targetForward), up);

        // Checks if between leeway to tilt object to upright or not
        if(angle < leeway && angle > -leeway)
        {
            transform.localEulerAngles = AngleLerp(transform.localEulerAngles, initialEuler, Time.deltaTime);
        }
        else
        {
            transform.localEulerAngles = AngleLerp(transform.localEulerAngles,
                                                    new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle), Time.deltaTime);
        }
    }

    /*
    // -Left +Right
    private void OnDrawGizmos()
    {
        Vector3 heading = cube.transform.position;
        Vector3 up = new Vector3(0, 1, 0);
        dirNum = Vector3.Dot(Vector3.Cross(-transform.forward, heading), up);
        z = dirNum * 15.0f;

        Debug.Log("-transform.forward: " + -transform.forward);
        Debug.Log("heading: " + heading);
        Debug.Log("transform.up: " + transform.up);
        Debug.Log("Cross: " + Vector3.Cross(-transform.forward, heading));
        Debug.Log("Dot: " + Vector3.Dot(Vector3.Cross(-transform.forward, heading), transform.up));

        //Vector3 euler = transform.localEulerAngles;
        //euler.z = Mathf.Lerp(euler.z, z, Time.deltaTime);
        //transform.localEulerAngles = euler;
        
        transform.localEulerAngles = AngleLerp(transform.localEulerAngles,
                                                    new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z), Time.deltaTime);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, heading);
    }
    */

    Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }
}
