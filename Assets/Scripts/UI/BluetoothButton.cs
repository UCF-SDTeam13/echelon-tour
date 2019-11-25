using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick()
    {
        Bike.Instance.Addresses.TryGetValue(this.GetComponentInChildren<Text>().text, out string address);
        BLEPlugin.Instance.Connect(address);
    }
}
