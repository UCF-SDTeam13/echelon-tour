using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothConnectUI : MonoBehaviour
{
    public Button prefabButton;
    public GameObject scrollContent;
    HashSet<string> bikesListed = new HashSet<string>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (bikesListed.Count != Bike.Instance.Matches.Count)
        {
            BLEDebug.LogInfo("BLE Button Added");
            HashSet<string> newBikes = new HashSet<string>(Bike.Instance.Matches);
            newBikes.ExceptWith(bikesListed);

            foreach (string name in newBikes)
            {
                Button b = Object.Instantiate(prefabButton, Vector3.zero, Quaternion.identity);
                RectTransform r = b.GetComponent<RectTransform>();
                r.SetParent(scrollContent.GetComponent<RectTransform>());
                b.GetComponentInChildren<Text>().text = name;
                bikesListed.Add(name);
            }
        }
    }

    void OnEnable()
    {
        BLEPlugin.Instance.RequestEnableBLE();
        BLEPlugin.Instance.Scan();
    }

    void OnDisable()
    {
        BLEPlugin.Instance.StopScan();
    }
}
