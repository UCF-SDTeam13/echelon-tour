using UnityEngine;

public class BezierFollowV2 : MonoBehaviour
{
    [SerializeField] private Transform target;

    private BezierTracker tracker;

    public float speed;

    private void Start()
    {
        // Find the tracker component
        tracker = GetComponent<BezierTracker>();
    }

    private void FixedUpdate()
    {
        // Calculate speed (echelon way)
        calculateSpeed();

        // Move the object position towards the target at a given speed
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);

        // Get the direction and magnitude from the player to the target
        Vector3 targetDirection = target.transform.position - transform.position;

        // Check if the targetDirection is zero
        if (targetDirection != Vector3.zero)
        {
            // Lerp the object rotation towards the direction of the target
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection), speed * Time.fixedDeltaTime);
        }
        else
        {
            Debug.Log("Vector3 is zero, don't know why.");
        }
    }

    private void calculateSpeed()
    {
        float rpmRatio = 1;
        float wheelDiameter = (78 * 2.54f) / 100000;
        float distancePerCount = wheelDiameter * Mathf.PI * rpmRatio;
        float speedMultiplier = distancePerCount * 60;
        speed = Bike.Instance.RPM * speedMultiplier;
    }
}
