using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float turnSpeed = 1;
    [SerializeField] private float rollSpeed = 0.2f;
    [SerializeField] private float spinTurnLimit = 90;

    private float lastFlatAngle;
    private float currentTurnAmount;
    private float turnSpeedVelocityChange;
    private Vector3 rollUp = Vector3.up;

    private void FixedUpdate()
    {
        if(target != null)
        {
            FollowTarget(Time.fixedDeltaTime);
        }
    }

    public void SetTarget(GameObject player)
    {
        target = player.transform;
    }

    private void FollowTarget(float deltaTime)
    {
        if(deltaTime <= 0 || target == null)
        {
            return;
        }

        Vector3 targetForward = target.forward;
        Vector3 targetUp = target.up;

        float currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z) * Mathf.Rad2Deg;
        if(spinTurnLimit > 0)
        {
            float targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(lastFlatAngle, currentFlatAngle)) / deltaTime;
            float desiredTurnAmount = Mathf.InverseLerp(spinTurnLimit, spinTurnLimit * 0.75f, targetSpinSpeed);
            float turnReactSpeed = (currentTurnAmount > desiredTurnAmount ? .1f : 1f);
            if (Application.isPlaying)
            {
                currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, desiredTurnAmount,
                                                                 ref turnSpeedVelocityChange, turnReactSpeed);
            }
            else
            {
                currentTurnAmount = desiredTurnAmount;
            }

            lastFlatAngle = currentFlatAngle;
        }

        transform.position = Vector3.Lerp(transform.position, target.position, moveSpeed * deltaTime);

        targetForward.y = 0;
        if(targetForward.sqrMagnitude < float.Epsilon)
        {
            targetForward = target.forward;
        }

        Quaternion rollRotation = Quaternion.LookRotation(targetForward, rollUp);

        rollUp = rollSpeed > 0 ? Vector3.Slerp(rollUp, targetUp, rollSpeed * deltaTime) : Vector3.up;
        transform.rotation = Quaternion.Lerp(transform.rotation, rollRotation, turnSpeed * currentTurnAmount * deltaTime);
    }
}
