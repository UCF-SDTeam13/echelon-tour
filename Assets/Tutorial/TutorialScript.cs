using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    public GameObject tutorial1;
    public GameObject tutorial2;
    public GameObject tutorial3;
    public GameObject tutorial4;

    public int oldIndex = 0;
    public int newIndex = 0;

    public GameObject[] tutorialArray;

    public void Awake()
    {
        tutorialArray = new GameObject[] { tutorial1, tutorial2, tutorial3, tutorial4 };
    }

    public void LeftButtonChange()
    {
        newIndex = Mod(oldIndex - 1, 4);
        tutorialArray[newIndex].SetActive(true);
        tutorialArray[oldIndex].SetActive(false);
        oldIndex = newIndex;
    }

    public void RightButtonChange()
    {
        newIndex = Mod(oldIndex + 1, 4);
        tutorialArray[newIndex].SetActive(true);
        tutorialArray[oldIndex].SetActive(false);
        oldIndex = newIndex;
    }

    private int Mod(int a, int b)
    {
        return (a % b + b) % b;
    }
}
