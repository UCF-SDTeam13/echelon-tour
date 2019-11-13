using UnityEngine;

public class OrientationController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 2.0f;

    [SerializeField] private float floatValue = 0.1f;

    // Update is called once per frame
    private void LateUpdate()
    {
        // Raycast to determine the location of where the player
        RaycastHit hit;

        // transform.TransformDirection to get the spot above the y direction of the player
        Vector3 currentPosition = transform.position + transform.TransformDirection(transform.up);
        Physics.Raycast(currentPosition, -transform.TransformDirection(transform.up), out hit);

        // Debug to see where the raycast is
        Debug.DrawRay(currentPosition, hit.normal * 10, Color.cyan);

        // Save the previous euler angles (other LateUpdate can cause issues) (need more testing or may need to combine)
        Vector3 oldEuler = transform.eulerAngles;

        // Save the rotation and lerp it accordingly
        Quaternion rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);

        // Realign the rest euler angles
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(q.eulerAngles.x, oldEuler.y, oldEuler.z);
        transform.rotation = q;

        // Change position of model to match the ground
        if (hit.collider != null)
        {
            float distance = Vector3.Distance(hit.point, transform.position);
            Vector3 newPosition = new Vector3(transform.position.x, (transform.position.y - distance) + floatValue, transform.position.z);
            transform.position = newPosition;
        }
        else
        {
            Debug.Log("Raycast is null, likely the mesh does not have a collider.");
        }
    }
}
