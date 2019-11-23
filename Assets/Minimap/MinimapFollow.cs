using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public GameObject target;

    // Update is called once per frame
    private void LateUpdate()
    {
        if(target != null)
        {
            transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, -target.transform.eulerAngles.y);
        }
    }

    private void SetPlayer(GameObject player)
    {
        target = player;
    }
}
