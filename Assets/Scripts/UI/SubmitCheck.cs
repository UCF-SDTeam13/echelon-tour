using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubmitCheck : MonoBehaviour
{
    public GameObject login;
    public GameObject submit;
    public GameObject errorDialogBox;
    public GameObject levelLoader;

    public GameObject username;
    public GameObject password;

    //PREVENT PEOPLE FROM CHANGING INPUT TEXT AND PRESSING BUTTON
    public async void SubmitLoginInformation()
    {
        TMP_InputField usernameInputField = username.GetComponent<TMP_InputField>();
        TMP_InputField passwordInputField = password.GetComponent<TMP_InputField>();
        Button submitButton = submit.GetComponent<Button>();

        if(usernameInputField.text == "" || passwordInputField.text == "")
        {
            StartCoroutine("MissingDialogBox");
            return;
        }

        usernameInputField.interactable = false;
        passwordInputField.interactable = false;
        submitButton.interactable = false;

        await API.Instance.Login(usernameInputField.text, passwordInputField.text);

        if(API.Instance.CurrentLoginStatus == API.LoginStatus.LOGGED_IN)
        {
            usernameInputField.interactable = true;
            passwordInputField.interactable = true;
            submitButton.interactable = true;

            // If statement added here later when we keep it stored in playerpref
            levelLoader.GetComponent<LevelLoader>().LoadLevel("MainMenu");

        }
        else
        {
            // Need to set active a dialog box, may need to move the interactable somewhere else
            StartCoroutine("CredentialDialogBox");
            usernameInputField.interactable = true;
            passwordInputField.interactable = true;
            submitButton.interactable = true;
            Debug.Log("Login fail");
        }
    }

    IEnumerator CredentialDialogBox()
    {
        errorDialogBox.GetComponent<Text>().text = "Could not connect with current credentials";
        errorDialogBox.SetActive(true);

        yield return new WaitForSeconds(7);

        errorDialogBox.SetActive(false);
    }

    IEnumerator MissingDialogBox()
    {
        errorDialogBox.GetComponent<Text>().text = "Username or password cannot be empty";
        errorDialogBox.SetActive(true);

        yield return new WaitForSeconds(7);

        errorDialogBox.SetActive(false);
    }
}
