using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private Follow newFollow;
    private Tracker newTracker;

    public float speed;
    public Vector3 next;
    public float amount;
    public float dirNum;
    public float angle;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        newFollow = GetComponentInParent<Follow>();
        newTracker = GetComponentInParent<Tracker>();
    }

    void Update()
    {
        // Temporary speed values
        speed = newFollow.speed;
        if (speed > 0 && speed <= 0.1f)
        {
            animator.SetFloat("SpeedY", .34f);
        }
        else if (speed > 0.1f && speed <= 0.2f)
        {
            animator.SetFloat("SpeedY", .67f);
        }
        else if (speed > 0.2f)
        {
            animator.SetFloat("SpeedY", 1);
        }
        else
        {
            animator.SetFloat("SpeedY", 0);
        }

        //Ignoring animation tilting
        /*
        Vector3 heading = newTracker.targetPosition - transform.position;
        dirNum = Vector3.Dot(Vector3.Cross(transform.forward, heading), transform.up) * 100;

        if((dirNum > -1f && dirNum < 1f) || animator.GetFloat("SpeedY") == 0)
        {
            animator.SetFloat("SpeedX", 0);
        }
        else
        {
            animator.SetFloat("SpeedX", dirNum);
        }
        */
    }
}
