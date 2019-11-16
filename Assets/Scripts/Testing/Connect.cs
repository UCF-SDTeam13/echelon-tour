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
    string gameSessionId = "";
    string playerSessionId = "";
    string ipAddress = "";
    string port = "";
    string message = "";
    Ping east1;
    Ping east2;

    float timer = 0.0f;
    readonly float pingInterval = 1.0f;
    int pingEast1 = 9999;
    int pingEast2 = 9999;

    // Start is called before the first frame update
    void Start()
    {
        client = new HttpClient();
        ConnectClient();
        east1 = new Ping("3.208.0.0");
        east2 = new Ping("3.14.0.0");
    }

    async void ConnectClient()
    {
        BLEDebug.LogInfo("Getting Game Session");
        Dictionary<string, string> param = new Dictionary<string, string>();

        HttpContent req = FlatJSON.SerializeContent(param);
        HttpResponseMessage result = await client.PostAsync("https://iddz0upx1d.execute-api.us-east-2.amazonaws.com/Prod/test/game/session", req);

        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("GS" + body);
        Dictionary<string, string> response = FlatJSON.Deserialize(body);

        response.TryGetValue("message", out message);
        response.TryGetValue("GameSessionId", out gameSessionId);
        BLEDebug.LogInfo(gameSessionId);

        BLEDebug.LogInfo("Getting Player Session");
        param = new Dictionary<string, string>();
        param.Add("GameSessionId", gameSessionId);

        req = FlatJSON.SerializeContent(param);
        result = await client.PostAsync("https://iddz0upx1d.execute-api.us-east-2.amazonaws.com/Prod/test/player/session", req);
        req.Dispose();

        body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("PS" + body);
        response = FlatJSON.Deserialize(body);

        response.TryGetValue("message", out message);
        response.TryGetValue("PlayerSessionId", out playerSessionId);
        response.TryGetValue("IpAddress", out ipAddress);
        response.TryGetValue("Port", out port);
        BLEDebug.LogInfo(playerSessionId);

        int iPort = 1900;

        Int32.TryParse(port, out iPort);
        BLEDebug.LogInfo(ipAddress);
        rc = new RealTimeClient(ipAddress, iPort, 33400, ConnectionType.RT_OVER_WEBSOCKET, playerSessionId, new byte[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (rc != null && rc.IsConnected())
        {
            //BLEDebug.LogInfo("Connection Status:" + rc.IsConnected());
            //rc.SendMessage(DeliveryIntent.Reliable, "Hello, GameLift!");
        }
        timer += Time.deltaTime;

        if (east1.isDone)
        {
            pingEast1 = east1.time;
            BLEDebug.LogInfo("East1 Ping" + pingEast1);
        }
        if (east2.isDone)
        {
            pingEast2 = east2.time;
            BLEDebug.LogInfo("East2 Ping" + pingEast2);
        }

        if (timer >= pingInterval)
        {
            east1 = new Ping("3.208.0.0");
            east2 = new Ping("3.14.0.0");
            timer = 0.0f;
        }
    }
}
