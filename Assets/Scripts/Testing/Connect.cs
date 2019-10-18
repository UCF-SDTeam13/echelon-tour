using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift.Realtime.Types;
using Amazon;
public class Connect : MonoBehaviour
{
    RealTimeClient rc;
    // Start is called before the first frame update
    void Start()
    {
        BLEDebug.LogInfo("Connecting to GameLift");
        rc = new RealTimeClient("IPADDRESS", 1900, 33400, ConnectionType.RT_OVER_WEBSOCKET, "psess-PLAYERSESSION", new byte[0]);

    }

    // Update is called once per frame
    void Update()
    {
        BLEDebug.LogInfo("Connection Status:" + rc.IsConnected());
        rc.SendMessage(DeliveryIntent.Reliable, "Hello, GameLift!");
    }
}
