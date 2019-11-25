using System.Collections;
using UnityEngine;

public class MatchMakingCheck : MonoBehaviour
{
    public GameObject levelLoader;

    private void Awake()
    {
        StartMatchMaking();
    }

    private void Update()
    {

    }

    private async void StartMatchMaking()
    {
        await API.Instance.CreateMatchmakingTicket();
        do
        {
            BLEDebug.LogInfo("Checking Matchmaking Ticket Status");
            await API.Instance.CheckMatchmakingTicketStatus();
        } while (API.Instance.Status == "SEARCHING" || API.Instance.Status == "PLACING");

        if (API.Instance.Status == "COMPLETED")
        {
            BLEDebug.LogInfo("Connecting to GameLift Realtime Session");
            BLEDebug.LogInfo($"DNS: {API.Instance.DnsName}");
            BLEDebug.LogInfo($"TCP Port: {API.Instance.TcpPort}");
            BLEDebug.LogInfo($"UDP Port: {API.Instance.UdpPort}");
            levelLoader.GetComponent<LevelLoader>().LoadLevel("EchelonDomeTrack");

            // StartCoroutine("CheckConnectionV2");
        }
        else
        {
            Debug.Log("API.Instance.Status != Connected");
        }
    }

    IEnumerator CheckConnection()
    {
        while (true)
        {
            if (RealTimeClient.Instance.IsConnected() == true)
            {
                levelLoader.GetComponent<LevelLoader>().LoadLevel("EchelonDomeTrack");
            }
            else
            {
                Debug.Log("RealTimeClient.Instance.IsConnected() == false");
            }

            yield return new WaitForSeconds(1);
        }
    }

    // NEED TO CHECK IF WE FAILED IF TOO MUCH TIME HAS PASSED
    IEnumerator CheckConnectionV2()
    {
        // Need to restart workout, not sure if info is reseted
        BLEPlugin.Instance.StopScan();

        yield return new WaitForSeconds(1);

        if (RealTimeClient.Instance.IsConnected() == true)
        {
            levelLoader.GetComponent<LevelLoader>().LoadLevel("EchelonDomeTrack");
        }
        else
        {
            Debug.Log("RealTimeClient.Instance.IsConnected() == false");
        }
    }
}
