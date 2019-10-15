using UnityEngine;

public class AnimationController : MonoBehaviour
{
    // Threshold values to determine which animations
    [SerializeField] private float low = 0f;
    [SerializeField] private float middle = 10f;
    [SerializeField] private float high = 20f;

    [SerializeField] private float speed;

    private Animator animator;
    private Follow follow;

    private void Start()
    {
        // Get components to access values
        // Longer one is used as animation is faulty
        //animator = GetComponentInChildren<Animator>();
        animator = transform.GetChild(0).GetComponentInChildren<Animator>();
        follow = GetComponentInParent<Follow>();
    }

    private void LateUpdate()
    {
        // Depending on how fast, the player's animation will change
        speed = follow.speed;

        // Ranges of speed, else no animation
        if (speed > low && speed <= middle)
        {
            animator.SetFloat("SpeedY", .34f);
        }
        else if (speed > middle && speed <= high)
        {
            animator.SetFloat("SpeedY", .67f);
        }
        else if (speed > high)
        {
            animator.SetFloat("SpeedY", 1);
        }
        else
        {
            animator.SetFloat("SpeedY", 0);
        }
    }
}
