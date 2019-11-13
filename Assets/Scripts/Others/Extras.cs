using UnityEngine;

public class Extras : MonoBehaviour
{
    // docs.unity3d.com/ScriptReference/PlayerPrefs.html
    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("Volume");
    }

    public void SetBluetoothDevice(string name)
    {
        PlayerPrefs.SetString("BluetoothDevice", name);
    }

    public string GetBluetoothDevice()
    {
        return PlayerPrefs.GetString("BluetoothDevice");
    }

    // docs.unity3d.com/Manual/class-QualitySettings.html
    public void SetQuality(int value)
    {
        QualitySettings.SetQualityLevel(value, false);
    }

    // assetstore.unity.com/packages/tools/input-management/easy-save-the-complete-save-load-asset-768
    // another way to save data compared to the other 3 scripts
}
