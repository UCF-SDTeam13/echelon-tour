using System;
using System.Text;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime.Types;
using System.Collections.Generic;
using UnityEngine;

/**
 * An example client that wraps the GameLift Realtime client SDK
 * 
 * You can redirect logging from the SDK by setting up the LogHandler as such:
 * ClientLogger.LogHandler = (x) => Console.WriteLine(x);
 *
 */
public class RealTimeClient
{
    public Aws.GameLift.Realtime.Client Client { get; private set; }

    // An opcode defined by client and your server script that represents a custom message type
    private const int OP_CODE_PLAYER_ACCEPTED = 113;
    private const int OP_CODE_PLAYER_DISCONNECTED = 114;
    private const int OP_CODE_RACE_START = 115;
    private const int OP_CODE_RACE_END = 116;
    private const int OP_CODE_TIME_TILL_TERMINATE = 117;
    private const int OP_CODE_STATS_UPDATE = 118;
    private const int OP_CODE_CUSTOMIZATION_UPDATE = 119;
    private string PlayerId = "";
    private IGameListener gameListener;

    /// Initialize a client for GameLift Realtime and connect to a player session.
    /// <param name="endpoint">The DNS name that is assigned to Realtime server</param>
    /// <param name="remoteTcpPort">A TCP port for the Realtime server</param>
    /// <param name="listeningUdpPort">A local port for listening to UDP traffic</param>
    /// <param name="connectionType">Type of connection to establish between client and the Realtime server</param>
    /// <param name="playerSessionId">The player session ID that is assiged to the game client for a game session </param>
    /// <param name="connectionPayload">Developer-defined data to be used during client connection, such as for player authentication</param>
    public RealTimeClient(string _PlayerId, string endpoint, int remoteTcpPort, int listeningUdpPort, ConnectionType connectionType,
                 string playerSessionId, byte[] connectionPayload)
    {
        PlayerId = _PlayerId;
        // Create a client configuration to specify a secure or unsecure connection type
        // Best practice is to set up a secure connection using the connection type RT_OVER_WSS_DTLS_TLS12.
        ClientConfiguration clientConfiguration = new ClientConfiguration(
         // C# notation to set the field ConnectionType in the new instance of ClientConfiguration
         connectionType
        );

        // Create a Realtime client with the client configuration            
        Client = new Client(clientConfiguration);

        // Initialize event handlers for the Realtime client
        Client.ConnectionOpen += OnOpenEvent;
        Client.ConnectionClose += OnCloseEvent;
        Client.GroupMembershipUpdated += OnGroupMembershipUpdate;
        Client.DataReceived += OnDataReceived;

        // Create a connection token to authenticate the client with the Realtime server
        // Player session IDs can be retrieved using AWS SDK for GameLift
        ConnectionToken connectionToken = new ConnectionToken(playerSessionId, connectionPayload);

        // Initiate a connection with the Realtime server with the given connection information
        Client.Connect(endpoint, remoteTcpPort, listeningUdpPort, connectionToken);

        // Register Placeholder GameListener
        gameListener = new PlaceholderGameListener();
    }

    public void Disconnect()
    {
        if (Client.Connected)
        {
            Client.Disconnect();
        }
    }

    public bool IsConnected()
    {
        return Client.Connected;
    }

    /// <summary>
    /// Example of sending to a custom message to the server.
    /// 
    /// Server could be replaced by known peer Id etc.
    /// </summary>
    /// <param name="intent">Choice of delivery intent ie Reliable, Fast etc. </param>
    /// <param name="payload">Custom payload to send with message</param>
    /*
    public void SendMessage(DeliveryIntent intent, string payload)
    {
        
        Client.SendMessage(Client.NewMessage(OP_CODE_CUSTOM_OP1)
            .WithDeliveryIntent(intent)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(payload)));
    }
    */
    public void UpdateCustomization(string characterModelId)
    {
        FlatJSON fJSON = new FlatJSON();
        fJSON.Add("PlayerId", PlayerId);
        fJSON.Add("characterModelId", characterModelId);

        Client.SendMessage(Client.NewMessage(OP_CODE_CUSTOMIZATION_UPDATE)
            .WithDeliveryIntent(DeliveryIntent.Reliable)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(fJSON.ToString())));
    }
    public void UpdateStats(int rotations, int rpm, float playerX, float playerY, float playerZ, float targetX, float targetY, float targetZ)
    {
        StatUpdate su = new StatUpdate()
        {
            rotations = rotations,
            rpm = rpm,
            playerX = playerX,
            playerY = playerY,
            playerZ = playerZ,
            targetX = targetX,
            targetY = targetY,
            targetZ = targetZ
        };

        Client.SendMessage(Client.NewMessage(OP_CODE_STATS_UPDATE)
            .WithDeliveryIntent(DeliveryIntent.Reliable)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(JsonUtility.ToJson(su))));
    }

    /**
     * Handle connection open events
     */
    public void OnOpenEvent(object sender, EventArgs e)
    {
    }

    /**
     * Handle connection close events
     */
    public void OnCloseEvent(object sender, EventArgs e)
    {
    }

    /**
     * Handle Group membership update events 
     */
    public void OnGroupMembershipUpdate(object sender, GroupMembershipEventArgs e)
    {
    }

    public void RegisterGameListener(IGameListener gListener)
    {
        gameListener = gListener;
    }

    /**
     *  Handle data received from the Realtime server 
     */
    public void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        FlatJSON fJSON = new FlatJSON();

        BLEDebug.LogInfo("OPCODE " + e.OpCode + " Received, Data:" + BytesToString(e.Data));
        switch (e.OpCode)
        {
            // handle message based on OpCode
            case OP_CODE_PLAYER_ACCEPTED:
                int peerId;
                Int32.TryParse(BytesToString(e.Data), out peerId);
                gameListener.OnPlayerAccepted(peerId);
                break;

            case OP_CODE_PLAYER_DISCONNECTED:
                Int32.TryParse(BytesToString(e.Data), out peerId);
                gameListener.OnPlayerDisconnected(peerId);
                break;

            case OP_CODE_RACE_START:
                gameListener.OnRaceStart();
                break;

            case OP_CODE_RACE_END:
                gameListener.OnRaceEnd();
                break;

            case OP_CODE_TIME_TILL_TERMINATE:
                int time;
                Int32.TryParse(BytesToString(e.Data), out time);
                gameListener.NotifyTimeTillTerminate(time);
                break;

            case OP_CODE_STATS_UPDATE:
                StatUpdate su = JsonUtility.FromJson<StatUpdate>(BytesToString(e.Data));
                gameListener.OnStatsUpdate(su.rotations, su.rpm);
                break;
            case OP_CODE_CUSTOMIZATION_UPDATE:
                fJSON.Deserialize(BytesToString(e.Data));
                string player, characterModel;
                fJSON.TryGetStringValue("PlayerId", out player);
                fJSON.TryGetStringValue("CharacterModelId", out characterModel);
                gameListener.OnCustomizationUpdate(player, characterModel);
                break;
            default:
                BLEDebug.LogWarning("Unknown OPCODE Received");
                break;
        }
    }

    /**
     * Helper method to simplify task of sending/receiving payloads.
     */
    public static byte[] StringToBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    /**
     * Helper method to simplify task of sending/receiving payloads.
     */
    public static string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
}

public interface IGameListener
{
    void OnPlayerAccepted(int peerId);
    void OnPlayerDisconnected(int peerId);
    void OnRaceStart();
    void OnRaceEnd();
    void NotifyTimeTillTerminate(int time);
    void OnStatsUpdate(int rotations, int rpm);
    void OnCustomizationUpdate(string PlayerId, string characterModelId);
}

[Serializable]
public class StatUpdate
{
    public string username;
    public int rotations;
    public int rpm;
    public float playerX;
    public float playerY;
    public float playerZ;
    public float targetX;
    public float targetY;
    public float targetZ;
}

class PlaceholderGameListener : IGameListener
{
    public void OnPlayerAccepted(int peerId)
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
    public void OnPlayerDisconnected(int peerId)
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
    public void OnRaceStart()
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
    public void OnRaceEnd()
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
    public void NotifyTimeTillTerminate(int time)
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
    public void OnStatsUpdate(int rotations, int rpm)
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
    public void OnCustomizationUpdate(string PlayerId, string characterModelId)
    {
        BLEDebug.LogWarning("Warning - GameListener Not Registered");
    }
}