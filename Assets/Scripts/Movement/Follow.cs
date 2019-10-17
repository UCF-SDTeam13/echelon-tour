using System.Collections;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private GameObject target = null;
    [SerializeField] private float speed = 0;
    [SerializeField] private float multiplier = 1;

    private Tracker tracker;

    private void Start()
    {
        tracker = GetComponent<Tracker>();
        multiplier = tracker.GetSpeedMultiplier();
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        Vector3 targetDirection = target.transform.position - transform.position;
        if (targetDirection != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(targetDirection);

            //Vector3 lookAhead = tracker.target.position + tracker.target.forward;
            //Vector3 lookAhead = tracker.targetForward;
            //float angle = Vector3.Angle(lookAhead, transform.forward);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection), angle * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection), 1);
        }
        else
        {
            Debug.Log("Vector3 is zero, don't know why.");
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed * multiplier;
    }

    public float GetSpeed()
    {
        return speed / multiplier;
    }
}
