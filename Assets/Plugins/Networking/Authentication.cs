using System;
using System.Net.Http;
using System.Collections.Generic;

public sealed class Authentication
{
    private static readonly Lazy<Authentication>
    _Authentication = new Lazy<Authentication>(() => new Authentication());
    public static Authentication Instance => _Authentication.Value;

    private readonly HttpClient client;

    private Authentication()
    {
        client = new HttpClient();
    }

    public async void Login(string username, string password)
    {
        Dictionary<string, string> credentials = new Dictionary<string, string>();
        credentials.Add("username", username);
        credentials.Add("password", password);

        HttpContent content = FlatJSON.SerializeContent(credentials);
        HttpResponseMessage result = await client.PostAsync("https://66hlxirzx1.execute-api.us-east-2.amazonaws.com/Prod/auth/login", content);
        content.Dispose();

        string body = await result.Content.ReadAsStringAsync();
        BLEDebug.LogInfo(body);
        Dictionary<string, string> response = FlatJSON.Deserialize(body);
        string message = "";
        string token = "";
        response.TryGetValue("token", out message);
        response.TryGetValue("token", out token);
        BLEDebug.LogInfo(message);
        BLEDebug.LogInfo(token);
    }
}
