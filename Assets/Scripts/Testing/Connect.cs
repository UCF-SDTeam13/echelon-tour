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
    // Start is called before the first frame update
    void Start()
    {
        client = new HttpClient();
        ConnectClient();
    }

    async void ConnectClient()
    {
        BLEDebug.LogInfo("Getting Game Session");
        Dictionary<string, string> param = new Dictionary<string, string>();

        HttpContent req = FlatJSON.SerializeContent(param);
        HttpResponseMessage result = await client.PostAsync("https://66hlxirzx1.execute-api.us-east-2.amazonaws.com/Prod/test/game/session", req);

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
        result = await client.PostAsync("https://66hlxirzx1.execute-api.us-east-2.amazonaws.com/Prod/test/player/session", req);
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
    }
}
