using UnityEngine;
using TMPro;

public class SubmitCheck : MonoBehaviour
{
    public GameObject login;
    public GameObject mainMenu;

    public GameObject username;
    public GameObject password;

    //PREVENT PEOPLE FROM CHANGING INPUT TEXT AND PRESSING BUTTON
    public async void SubmitLoginInformation()
    {
        string usernameString = username.GetComponent<TMP_InputField>().text;
        string passwordString = password.GetComponent<TMP_InputField>().text;
        
        await API.Instance.Login(usernameString, passwordString);

        if(API.Instance.CurrentLoginStatus == API.LoginStatus.LOGGED_IN)
        {
            mainMenu.SetActive(true);
            login.SetActive(false);
        }
        else
        {
            Debug.Log("WE FAILED TO LOG IN YO");
        }
    }
}
