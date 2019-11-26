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
    private static readonly Lazy<RealTimeClient>
    _RealTimeClient = new Lazy<RealTimeClient>(() => new RealTimeClient());
    public static RealTimeClient Instance => _RealTimeClient.Value;
    public Aws.GameLift.Realtime.Client Client { get; private set; }

    // An opcode defined by client and your server script that represents a custom message type
    private const int OP_CODE_PLAYER_ACCEPTED = 113;
    private const int OP_CODE_PLAYER_DISCONNECTED = 114;
    private const int OP_CODE_RACE_START = 115;
    private const int OP_CODE_RACE_END = 116;
    private const int OP_CODE_TIME_TILL_TERMINATE = 117;
    private const int OP_CODE_STATS_UPDATE = 118;
    private const int OP_CODE_CUSTOMIZATION_UPDATE = 119;
    private string PlayerId;

    private int _peerId = 0;
    public int peerId
    {
        get
        {
            return _peerId;
        }
    }

    public event EventHandler RaceStart;
    public event EventHandler RaceEnd;
    public event EventHandler<PlayerEventArgs> PlayerConnect;
    public event EventHandler<PlayerEventArgs> PlayerDisconnect;
    public event EventHandler<CustomizationUpdateEventArgs> CustomizationUpdate;
    public event EventHandler<StatsUpdateEventArgs> StatsUpdate;

    public event EventHandler<NotifyTimeTillTerminateEventArgs> NotifyTimeTillTerminate;

    /// Initialize a client for GameLift Realtime and connect to a player session.
    /// <param name="endpoint">The DNS name that is assigned to Realtime server</param>
    /// <param name="remoteTcpPort">A TCP port for the Realtime server</param>
    /// <param name="listeningUdpPort">A local port for listening to UDP traffic</param>
    /// <param name="connectionType">Type of connection to establish between client and the Realtime server</param>
    /// <param name="playerSessionId">The player session ID that is assiged to the game client for a game session </param>
    /// <param name="connectionPayload">Developer-defined data to be used during client connection, such as for player authentication</param>

    private RealTimeClient()
    {

    }
    public void Connect(string _PlayerId, string endpoint, int remoteTcpPort, int listeningUdpPort, ConnectionType connectionType,
                 string playerSessionId, byte[] connectionPayload)
    {
        PlayerId = _PlayerId;
        // Create a client configuration to specify a secure or unsecure connection type
        // Best practice is to set up a secure connection using the connection type RT_OVER_WSS_DTLS_TLS12.
        ClientConfiguration clientConfiguration = new ClientConfiguration();
        /*
        {
            ConnectionType = connectionType
        };
        */

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
        if (Client != null)
        {
            return Client.Connected;
        }
        else
        {
            return false;
        }

    }
    public void UpdateCustomization(string characterModelId)
    {
        BLEDebug.LogInfo("Sending Customization Update");
        FlatJSON fJSON = new FlatJSON();
        fJSON.Add("PlayerId", PlayerId);
        fJSON.Add("characterModelId", characterModelId);

        Client.SendMessage(Client.NewMessage(OP_CODE_CUSTOMIZATION_UPDATE)
            .WithDeliveryIntent(DeliveryIntent.Reliable)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(fJSON.ToString())));
    }
    public void UpdateStats(int rotations, int rpm, float[] playerPosition, float progressDistance)
    {
        BLEDebug.LogInfo("Sending Stats Update");
        FlatJSON fJSON = new FlatJSON();
        fJSON.Add("rotations", rotations);
        fJSON.Add("rpm", rpm);
        fJSON.Add("playerPosition", playerPosition);
        fJSON.Add("progressDistance", progressDistance);

        Client.SendMessage(Client.NewMessage(OP_CODE_STATS_UPDATE)
            .WithDeliveryIntent(DeliveryIntent.Reliable)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(fJSON.ToString())));
    }

    /**
     * Handle connection open events
     */
    private void OnOpenEvent(object sender, EventArgs e)
    {
    }

    /**
     * Handle connection close events
     */
    private void OnCloseEvent(object sender, EventArgs e)
    {
    }

    /**
     * Handle Group membership update events 
     */
    private void OnGroupMembershipUpdate(object sender, GroupMembershipEventArgs e)
    {
    }

    /**
     *  Handle data received from the Realtime server 
     */
    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        FlatJSON fJSON = new FlatJSON();

        BLEDebug.LogInfo("OPCODE " + e.OpCode + " Received, Data:" + BytesToString(e.Data));
        switch (e.OpCode)
        {
            // handle message based on OpCode
            case OP_CODE_PLAYER_ACCEPTED:
                _peerId = e.Sender;
                OnPlayerAccepted(peerId);
                break;

            case OP_CODE_PLAYER_DISCONNECTED:
                Int32.TryParse(BytesToString(e.Data), out _peerId);
                OnPlayerDisconnected(peerId);
                break;

            case OP_CODE_RACE_START:
                OnRaceStart();
                break;

            case OP_CODE_RACE_END:
                OnRaceEnd();
                break;

            case OP_CODE_TIME_TILL_TERMINATE:
                int time;
                Int32.TryParse(BytesToString(e.Data), out time);
                OnNotifyTimeTillTerminate(time);
                break;

            case OP_CODE_STATS_UPDATE:
                fJSON.Deserialize(BytesToString(e.Data));
                fJSON.TryGetIntValue("rotations", out int rotations);
                fJSON.TryGetIntValue("rpm", out int rpm);
                fJSON.TryGetFloatArray("playerPosition", out float[] playerPosition);
                fJSON.TryGetFloatValue("progressDistance", out float progressDistance);
                OnStatsUpdate(e.Sender, rotations, rpm, playerPosition, progressDistance);
                break;
            case OP_CODE_CUSTOMIZATION_UPDATE:
                fJSON.Deserialize(BytesToString(e.Data));
                fJSON.TryGetStringValue("PlayerId", out string customplayer);
                fJSON.TryGetStringValue("CharacterModelId", out string characterModel);
                OnCustomizationUpdate(e.Sender, customplayer, characterModel);
                break;
            default:
                BLEDebug.LogWarning("Unknown OPCODE Received");
                break;
        }
    }

    /**
     * Helper method to simplify task of sending/receiving payloads.
     */
    private static byte[] StringToBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    /**
     * Helper method to simplify task of sending/receiving payloads.
     */
    private static string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }
    private void OnPlayerAccepted(int peerId)
    {
        PlayerConnect?.Invoke(this, new PlayerEventArgs(peerId));
    }
    private void OnPlayerDisconnected(int peerId)
    {
        PlayerDisconnect?.Invoke(this, new PlayerEventArgs(peerId));
    }
    private void OnRaceStart()
    {
        RaceStart?.Invoke(this, null);

    }
    private void OnRaceEnd()
    {
        RaceEnd?.Invoke(this, null);
    }
    private void OnNotifyTimeTillTerminate(int time)
    {
        NotifyTimeTillTerminate?.Invoke(this, new NotifyTimeTillTerminateEventArgs(time));
    }
    private void OnStatsUpdate(int peerId, int rotations, int rpm, float[] playerPosition, float progressDistance)
    {
        StatsUpdate?.Invoke(this, new StatsUpdateEventArgs(peerId, rotations, rpm, playerPosition, progressDistance));
    }
    private void OnCustomizationUpdate(int peerId, string PlayerId, string characterModelId)
    {
        CustomizationUpdate?.Invoke(this, new CustomizationUpdateEventArgs(peerId, PlayerId, characterModelId));
    }
}

public class PlayerEventArgs : EventArgs
{
    public int peerId { get; private set; }
    public PlayerEventArgs(int pId)
    {
        peerId = pId;
    }
}

public class CustomizationUpdateEventArgs : EventArgs
{
    public int peerId { get; private set; }
    public string PlayerId { get; private set; }
    public string characterModelId { get; private set; }
    public CustomizationUpdateEventArgs(int pId, string PId, string cId)
    {
        peerId = pId;
        PlayerId = PId;
        characterModelId = cId;
    }
}
public class StatsUpdateEventArgs : EventArgs
{
    public int peerId { get; private set; }
    public int rotations { get; private set; }
    public int rpm { get; private set; }
    public float[] playerPosition { get; private set; }
    public float progressDistance { get; private set; }
    public StatsUpdateEventArgs(int pId, int rot, int rm, float[] pPos, float pDis)
    {
        peerId = pId;
        rotations = rot;
        rpm = rm;
        playerPosition = pPos;
        progressDistance = pDis;
    }
}

public class NotifyTimeTillTerminateEventArgs : EventArgs
{
    public int time { get; set; }
    public NotifyTimeTillTerminateEventArgs(int t)
    {
        time = t;
    }
}