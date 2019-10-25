using UnityEngine;

public class BezierAnimationController : MonoBehaviour
{
    [SerializeField] private float lowThreshold = 0;
    [SerializeField] private float middleThreshold = 5;
    [SerializeField] private float highThreshold = 10;

    private float speed;
    private Animator animator;
    private BezierFollowV2 follow;

    private void Start()
    {
        // Get the components for the animator and follow
        animator = GetComponentInChildren<Animator>();
        follow = GetComponentInParent<BezierFollowV2>();
    }

    private void LateUpdate()
    {
        // Save the speed, can adjust if necessary
        speed = follow.speed;

        // Ranges of speed, else no animation
        if (speed > lowThreshold && speed <= middleThreshold)
        {
            animator.SetFloat("SpeedY", .34f);
        }
        else if (speed > middleThreshold && speed <= highThreshold)
        {
            animator.SetFloat("SpeedY", .67f);
        }
        else if (speed > highThreshold)
        {
            animator.SetFloat("SpeedY", 1);
        }
        else
        {
            animator.SetFloat("SpeedY", 0);
        }
    }
}
