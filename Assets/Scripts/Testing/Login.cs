using UnityEngine;


public class Login : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        Authentication.Instance.Login("test", "password");
    }
}
