using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;

[Serializable]
public class Credentials
{
    public string username;
    public string password;

    // Messy - but necessary because we can't use reflection
    public override string ToString()
    {
        return "{\n" + "\"username\":\"" + username + "\",\n" + "\"password\":\"" + password + "\"\n" + "}";
    }
}

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
        Credentials cred = new Credentials();
        cred.username = username;
        cred.password = password;

        string json = cred.ToString();
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        BLEDebug.LogInfo(json);

        var result = await client.PostAsync("https://66hlxirzx1.execute-api.us-east-2.amazonaws.com/Prod/auth/login", content);
        BLEDebug.LogInfo(await result.Content.ReadAsStringAsync());
    }
}
