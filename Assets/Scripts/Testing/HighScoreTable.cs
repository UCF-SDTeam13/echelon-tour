using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreTable : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform entryContainter;
    private Transform entryTemplate;
    private void Awake()
    {
        entryContainter = transform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainter.Find("HighScoreEntryTemplate");
        
        entryTemplate.gameObject.SetActive(false);

        float templateHeight = 20f;
        for (int i = 0; i < 10; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainter);
            RectTransform entryReactTransform = entryTransform.GetComponent<RectTransform>();
            entryReactTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);
        }
    }
}
