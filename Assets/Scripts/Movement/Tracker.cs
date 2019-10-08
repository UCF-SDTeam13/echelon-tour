using UnityEngine;

public class Tracker : MonoBehaviour
{
    // Reference to the circuit it should follow
    public Circuit circuit = null;

    // Offset ahead that the object should aim for
    [SerializeField] private float lookAheadForTargetOffset = 5;

    // Multiplier to adjust distance ahead that is based off speed
    [SerializeField] private float lookAheadForTargetFactor = .1f;

    // Offset ahead for speed adjustments (used with lerp for the target's rotation)
    [SerializeField] private float lookAheadForSpeedOffset = 10f;

    // Multiplier to adjust distance for speed
    [SerializeField] private float lookAheadForSpeedFactor = .2f;

    // Target for object
    public Transform target;

    // Possible variables for animations
    public Vector3 targetPosition;
    public Vector3 targetForward;

    [SerializeField] private float speed;
    [SerializeField] private int index = 0;

    public Circuit.RoutePoint progressPoint;
    private float progressDistance;
    private Vector3 lastPosition;

    private void Start()
    {
        // Gets the circuit of the current level
        circuit = GameObject.FindGameObjectWithTag("Circuit").GetComponent<Circuit>();
        Reset();
    }

    // Makes sure the progress distance is 0
    public void Reset()
    {
        progressDistance = 0;
    }

    private void Update()
    {
        if (Time.deltaTime > 0)
        {
            speed = Mathf.Lerp(speed, (lastPosition - transform.position).magnitude / Time.deltaTime,
                               Time.deltaTime);
        }

        // Change the position of the target in front of the object
        target.position =
            circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor * speed, index)
                .position;

        // Change the rotation of the target in front of the object
        target.rotation =
            Quaternion.LookRotation(
                circuit.GetRoutePoint(progressDistance + lookAheadForSpeedOffset + lookAheadForSpeedFactor * speed, index)
                       .direction);

        targetPosition = target.position;
        targetForward = target.position + target.forward;

        progressPoint = circuit.GetRoutePoint(progressDistance, index);
        Vector3 progressDelta = progressPoint.position - transform.position;
        if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
        {
            progressDistance += progressDelta.magnitude * 0.5f;
        }

        lastPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw a line to the target front position
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);

            // Draw a line for the look ahead
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, target.position + target.forward);
        }
    }
}
