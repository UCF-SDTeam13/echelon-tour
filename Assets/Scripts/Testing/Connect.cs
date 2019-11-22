using System;
using System.Net.Http;
using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift.Realtime.Types;
using System.Threading.Tasks;

public class Connect : MonoBehaviour
{
    private HttpClient client = new HttpClient();
    RealTimeClient rc;
    Ping east1;
    Ping east2;

    float timer = 0.0f;
    readonly float pingInterval = 1.0f;
    int pingEast1 = 9999;
    int pingEast2 = 9999;

    // Start is called before the first frame update
    public void Start()
    {
        client = new HttpClient();
        ConnectClient();
        east1 = new Ping("3.208.0.0");
        east2 = new Ping("3.14.0.0");
    }

    async void ConnectClient()
    {
        await API.Instance.Login("test", "password");
        await API.Instance.CreateMatchmakingTicket();
        do
        {
            BLEDebug.LogInfo("Checking Matchmaking Ticket Status");
            await API.Instance.CheckMatchmakingTicketStatus();
        } while (API.Instance.Status == "SEARCHING" || API.Instance.Status == "PLACING");
        if (API.Instance.Status == "COMPLETED")
        {
            int iPort = 1900;

            Int32.TryParse(API.Instance.Port, out iPort);
            rc = new RealTimeClient(API.Instance.IpAddress, iPort, 33400, ConnectionType.RT_OVER_WEBSOCKET, API.Instance.PlayerSessionId, new byte[0]);

        }

    }

    // Update is called once per frame
    public void Update()
    {
        if (rc != null && rc.IsConnected())
        {
            //BLEDebug.LogInfo("Connection Status:" + rc.IsConnected());
        }
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
            if (rc != null && rc.IsConnected())
            {
                BLEDebug.LogInfo("Connection Status:" + rc.IsConnected());
                rc.UpdateStats(0, 0, 1, 2, 3, 1, 2, 3);
            }
            //BLEDebug.LogInfo("East1 Ping" + pingEast1);
            //BLEDebug.LogInfo("East2 Ping" + pingEast2);
            east1 = new Ping("3.208.0.0");
            east2 = new Ping("3.14.0.0");
            timer = 0.0f;
        }
    }
}
