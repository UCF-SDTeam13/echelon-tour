using UnityEngine;

public class Tracker : MonoBehaviour
{
    // Reference to the circuit it should follow
    [SerializeField] private Circuit circuit = null;

    // Target for object
    [SerializeField] private Transform target = null;

    // Offset ahead that the object should aim for
    [SerializeField] private float lookAheadForTargetOffset = 5;

    // Multiplier to adjust distance ahead that is based off speed
    [SerializeField] private float lookAheadForTargetFactor = .1f;

    // Offset ahead for speed adjustments (used with lerp for the target's rotation)
    [SerializeField] private float lookAheadForSpeedOffset = 10f;

    // Multiplier to adjust distance for speed
    [SerializeField] private float lookAheadForSpeedFactor = .2f;

    // Index for the current player
    [SerializeField] private int index = 0;

    // Possible variables for animations
    public Vector3 targetLookAhead;
    public float lookAhead = 1;

    private float speed;

    public Circuit.RoutePoint progressPoint;
    private float progressDistance;
    private Vector3 lastPosition;

    private void Awake()
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

        // Get the target look ahead for animation purposes
        targetLookAhead = 
            circuit.GetRoutePoint(progressDistance + lookAheadForTargetOffset + lookAheadForTargetFactor + lookAhead * speed, index)
                .position;

        progressPoint = circuit.GetRoutePoint(progressDistance, index);
        Vector3 progressDelta = progressPoint.position - transform.position;
        if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
        {
            progressDistance += progressDelta.magnitude * 0.5f;
        }

        lastPosition = transform.position;
    }

    // Set the index for the current track
    public void SetIndex(int index)
    {
        this.index = index;
    }

    // Get the multiplier from the circuit
    public float GetSpeedMultiplier()
    {
        return circuit.GetMultiplier(index);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // Draw a line to the target front position
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, targetLookAhead);
        }
    }
}
