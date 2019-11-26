using System;
using System.Net.Http;
using UnityEngine;
using Aws.GameLift.Realtime.Types;


public class Connect : MonoBehaviour
{
    private HttpClient client = new HttpClient();
    UnityEngine.Ping east1;
    UnityEngine.Ping east2;

    float timer = 0.0f;
    readonly float pingInterval = 1.0f;
    int pingEast1 = 9999;
    int pingEast2 = 9999;

    // Start is called before the first frame update
    public void Start()
    {
        client = new HttpClient();
        ConnectClient();
        east1 = new UnityEngine.Ping("3.208.0.0");
        east2 = new UnityEngine.Ping("3.14.0.0");
    }

    async void ConnectClient()
    {
        await API.Instance.Login("test", "password");
        await API.Instance.GetCustomization();
        BLEDebug.LogInfo(API.Instance.CharacterModelId);
        API.Instance.CharacterModelId = "model2";
        await API.Instance.SetCustomization();
        BLEDebug.LogInfo(API.Instance.CharacterModelId);
        API.Instance.CharacterModelId = "model1";
        await API.Instance.SetCustomization();
        BLEDebug.LogInfo(API.Instance.CharacterModelId);

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
            BLEDebug.LogInfo($"PlayerSessionId: {API.Instance.PlayerSessionId}");
            RealTimeClient.Instance.Connect(API.Instance.PlayerId, API.Instance.DnsName, API.Instance.TcpPort, API.Instance.UdpPort, ConnectionType.RT_OVER_WEBSOCKET, API.Instance.PlayerSessionId, new byte[0]);
        }

    }

    // Update is called once per frame
    public void Update()
    {
        timer += Time.deltaTime;

        if (east1.isDone)
        {
            pingEast1 = east1.time;

        }
        if (east2.isDone)
        {
            pingEast2 = east2.time;

        }
        if (timer >= pingInterval)
        {
            if (RealTimeClient.Instance.IsConnected())
            {
                BLEDebug.LogInfo("Connected");
                float[] playerPos = { 1, 2, 3 };
                float[] targetPos = { 4, 5, 6 };
                RealTimeClient.Instance.UpdateStats(0, 0, playerPos, 0.0f);
            }
            //BLEDebug.LogInfo("East1 Ping" + pingEast1);
            //BLEDebug.LogInfo("East2 Ping" + pingEast2);
            east1 = new UnityEngine.Ping("3.208.0.0");
            east2 = new UnityEngine.Ping("3.14.0.0");
            timer = 0.0f;
        }
    }
}
