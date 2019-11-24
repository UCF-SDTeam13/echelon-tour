using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene("BikeAnimationTestScene");
    }

    public void BikeTest()
    {
        SceneManager.LoadScene("HardwareTestScene");
    }

}
