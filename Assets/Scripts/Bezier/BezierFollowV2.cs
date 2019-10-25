using UnityEngine;

public class BezierFollowV2 : MonoBehaviour
{
    [SerializeField] private Transform target;

    private BezierTracker tracker;
    private float rpmRatio = 1;
    private float wheelDiameter = (78 * 2.54f) / 100000;

    public float speed;

    private void Start()
    {
        // Find the tracker component
        tracker = GetComponent<BezierTracker>();
    }

    private void Update()
    {
        // Calculate speed (echelon way)
        calculateSpeed();
    }

    private void FixedUpdate()
    {
        // Save speed for that exact moment (just in case)
        float currentSpeed = speed / 2.0f;

        // Move the object position towards the target at a given speed
        //transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.fixedDeltaTime);

        // Get the direction and magnitude from the player to the target
        Vector3 targetDirection = target.transform.position - transform.position;

        // Check if the targetDirection is zero
        if (targetDirection != Vector3.zero)
        {
            // Lerp the object rotation towards the direction of the target
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection), speed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection), currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Debug.Log("Vector3 is zero, don't know why.");
        }
    }

    private void calculateSpeed()
    {
        // Trying to hard code pi
        float distancePerCount = wheelDiameter * 3.14f * rpmRatio;
        float speedMultiplier = distancePerCount * 60;
        int rpm = Bike.Instance.RPM;
        speed = rpm * speedMultiplier;
    }
}

