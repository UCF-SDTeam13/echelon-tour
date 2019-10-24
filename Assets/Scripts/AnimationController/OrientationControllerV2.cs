using UnityEngine;

public class OrientationControllerV2 : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 2;
    [SerializeField] private float floatValue = 0.1f;

    [SerializeField] private GameObject cyclist = null;

    // Update is called once per frame
    private void LateUpdate()
    {
        // Raycast to determine the location of where the player
        RaycastHit hit;

        // transform.TransformDirection to get the spot above the y direction of the player
        Vector3 currentPosition = cyclist.transform.position + cyclist.transform.TransformDirection(transform.up);
        Physics.Raycast(currentPosition, -cyclist.transform.TransformDirection(transform.up), out hit);

        // Debug to see where the raycast is
        Debug.DrawRay(currentPosition, hit.normal * 10, Color.cyan);

        // Save the previous euler angles (other LateUpdate can cause issues) (need more testing or may need to combine)
        Vector3 oldEuler = cyclist.transform.eulerAngles;

        // Save the rotation and lerp it accordingly
        Quaternion fromToRotation = Quaternion.FromToRotation(cyclist.transform.up, hit.normal) * cyclist.transform.rotation;
        cyclist.transform.rotation = Quaternion.Lerp(cyclist.transform.rotation, fromToRotation, rotateSpeed * Time.deltaTime);

        // Realign the rest euler angles
        Quaternion rotation = cyclist.transform.rotation;
        rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, oldEuler.y, oldEuler.z);
        cyclist.transform.rotation = rotation;

        // Change position of model to match the ground
        if (hit.collider != null)
        {
            float distance = Vector3.Distance(hit.point, cyclist.transform.position);
            Vector3 newPosition = new Vector3(cyclist.transform.position.x,
                                                (cyclist.transform.position.y - distance) + floatValue, cyclist.transform.position.z);
            cyclist.transform.position = newPosition;
        }
        else
        {
            Debug.Log("Raycast is null, likely the mesh does not have a collider.");
        }
    }
}
