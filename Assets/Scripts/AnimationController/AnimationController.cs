using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private Follow follow;
    private Tracker tracker;

    public float speed;
    public Vector3 next;
    public float amount;
    public float dirNum;
    public float angle;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        follow = GetComponentInParent<Follow>();
        tracker = GetComponentInParent<Tracker>();
    }

    void Update()
    {
        // Temporary speed values
        speed = follow.speed;
        if (speed > 0 && speed <= 5f)
        {
            animator.SetFloat("SpeedY", .34f);
        }
        else if (speed > 5f && speed <= 10f)
        {
            animator.SetFloat("SpeedY", .67f);
        }
        else if (speed > 10f)
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
