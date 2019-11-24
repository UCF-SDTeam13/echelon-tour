using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubmitCheck : MonoBehaviour
{
    public GameObject login;
    public GameObject modelSelect;
    public GameObject submit;
    public GameObject errorDialogBox;

    public GameObject username;
    public GameObject password;

    //PREVENT PEOPLE FROM CHANGING INPUT TEXT AND PRESSING BUTTON
    public async void SubmitLoginInformation()
    {
        TMP_InputField usernameInputField = username.GetComponent<TMP_InputField>();
        TMP_InputField passwordInputField = password.GetComponent<TMP_InputField>();
        Button submitButton = submit.GetComponent<Button>();

        usernameInputField.interactable = false;
        passwordInputField.interactable = false;
        submitButton.interactable = false;

        await API.Instance.Login(usernameInputField.text, passwordInputField.text);

        if(API.Instance.CurrentLoginStatus == API.LoginStatus.LOGGED_IN)
        {
            // If statement added here later when we keep it stored in playerpref
            modelSelect.SetActive(true);
            login.SetActive(false);
        }
        else
        {
            // Need to set active a dialog box, may need to move the interactable somewhere else
            usernameInputField.interactable = true;
            passwordInputField.interactable = true;
            submitButton.interactable = true;
            Debug.Log("Login fail");
        }
    }
}
