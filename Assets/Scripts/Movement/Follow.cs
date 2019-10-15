using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private GameObject target = null;

    public float speed = 10f;

    private Tracker tracker;

    private void Start()
    {
        tracker = GetComponent<Tracker>();
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
}
