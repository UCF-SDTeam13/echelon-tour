using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public sealed class API
{
    private static readonly Lazy<API>
    _API = new Lazy<API>(() => new API());
    public static API Instance => _API.Value;

    private readonly HttpClient client;
    private string idToken = "";
    private string refreshToken = "";
    private string accessToken = "";
    private string TicketId = "";
    private string _PlayerId = "";
    public string PlayerId
    {
        get
        {
            return _PlayerId;
        }
    }
    private string _Status = "";
    public string Status
    {
        get
        {
            return _Status;
        }
    }

    private string _GameSessionArn;
    public string GameSession
    {
        get
        {
            return _GameSessionArn;
        }
    }

    private string _IpAddress;
    public string IpAddress
    {
        get
        {
            return _IpAddress;
        }
    }
    private string _Port;
    public string Port
    {
        get
        {
            return _Port;
        }
    }
    private string _PlayerSessionId;
    public string PlayerSessionId
    {
        get
        {
            return _PlayerSessionId;
        }
    }

    private string _CharacterModelId;
    public string CharacterModelId
    {
        get
        {
            return _CharacterModelId;
        }
        set
        {
            _CharacterModelId = value;
        }
    }
    private API()
    {
        client = new HttpClient();
    }

    public async Task Login(string username, string password)
    {
        BLEDebug.LogInfo("Logging In");
        FlatJSON fJSON = new FlatJSON();
        fJSON.Add("username", username);
        fJSON.Add("password", password);
        HttpContent req = fJSON.SerializeContent();

        var result = await client.PostAsync("https://echelon.3pointlabs.org/auth/login", req);
        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("Auth " + body);
        fJSON.Deserialize(body);
        fJSON.TryGetStringValue("username", out _PlayerId);
        fJSON.TryGetStringValue("idToken", out idToken);
        fJSON.TryGetStringValue("refreshToken", out refreshToken);
        fJSON.TryGetStringValue("accessToken", out accessToken);

        // Set Token for Authentication
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
    }

    public async Task CreateMatchmakingTicket()
    {
        BLEDebug.LogInfo("Creating MatchMaking Ticket");
        FlatJSON fJSON = new FlatJSON();

        HttpContent req = fJSON.SerializeContent();
        HttpResponseMessage result = await client.PostAsync("https://echelon.3pointlabs.org/matchmaking/ticket", req);

        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("Ticket" + body);
        fJSON.Deserialize(body);

        fJSON.TryGetStringValue("TicketId", out TicketId);
        fJSON.TryGetStringValue("Status", out _Status);
    }

    public async Task CheckMatchmakingTicketStatus()
    {
        BLEDebug.LogInfo("Checking MatchMaking Ticket Status");
        FlatJSON fJSON = new FlatJSON();
        fJSON.Add("TicketId", TicketId);
        HttpContent req = fJSON.SerializeContent();
        HttpResponseMessage result = await client.PostAsync("https://echelon.3pointlabs.org/matchmaking/status", req);

        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("Ticket " + body);
        fJSON.Deserialize(body);

        fJSON.TryGetStringValue("Status", out _Status);

        if (Status == "COMPLETED")
        {
            fJSON.TryGetStringValue("GameSessionArn", out _GameSessionArn);
            fJSON.TryGetStringValue("IpAddress", out _IpAddress);
            fJSON.TryGetStringValue("Port", out _Port);
            fJSON.TryGetStringValue("PlayerSessionId", out _PlayerSessionId);
        }
    }

    public async Task GetCustomization()
    {
        BLEDebug.LogInfo("Getting CharacterModelId");
        FlatJSON fJSON = new FlatJSON();
        HttpResponseMessage result = await client.GetAsync("https://echelon.3pointlabs.org/profile/customization");

        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("CharacterModel " + body);
        fJSON.Deserialize(body);

        fJSON.TryGetStringValue("characterModelId", out _CharacterModelId);
    }

    public async Task SetCustomization()
    {
        BLEDebug.LogInfo("Setting CharacterModelId");
        FlatJSON fJSON = new FlatJSON();
        fJSON.Add("characterModelId", _CharacterModelId);
        HttpContent req = fJSON.SerializeContent();
        HttpResponseMessage result = await client.PutAsync("https://echelon.3pointlabs.org/profile/customization", req);

        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo("CharacterModel " + body);
        fJSON.Deserialize(body);

        fJSON.TryGetStringValue("characterModelId", out _CharacterModelId);
    }
}
