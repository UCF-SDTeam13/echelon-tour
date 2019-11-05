
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

// Flat JSON Serializer / Deserializer to avoid Reflection for IL2CPP
// Strings Only: No integers
// Flat Only: No Objects / Arrays

public static class FlatJSON
{
    // Serialize by Building String Manually
    public static string Serialize(Dictionary<string, string> data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        foreach (KeyValuePair<string, string> kvPair in data)
        {
            sb.Append("\"");
            sb.Append(kvPair.Key);
            sb.Append("\":\"");
            sb.Append(kvPair.Value);
            sb.Append("\",");
        }
        // JavaScript / ECMAScript 5 Objects Allow Trailing Commas
        // However - It seems that isn't part of the JSON Spec
        // Remove Last Trailing Comma
        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");
        return sb.ToString();
    }
    public static HttpContent SerializeContent(Dictionary<string, string> data)
    {
        StringContent content = new StringContent(Serialize(data), Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return content;
    }
    // Deserialize using a Finite State Machine
    private enum ParseState { BeginJSON, BeginKey, EndKey, KeyValueColon, BeginValue, EndValue, FinishJSON, Validated };
    public static Dictionary<string, string> Deserialize(string json)
    {
        StringBuilder sb = new StringBuilder();
        Dictionary<string, string> d = new Dictionary<string, string>();
        ParseState state = ParseState.BeginJSON;
        string key = "";
        string value = "";

        foreach (char c in json)
        {
            switch (state)
            {
                case ParseState.BeginJSON:
                    if (c == '{')
                    {
                        // { found
                        // Beginning of JSON 
                        // -> Look for Beginning of Key "
                        state = ParseState.BeginKey;
                    }
                    break;
                case ParseState.BeginKey:
                    if (c == '"')
                    {
                        // " found
                        // Beginning of Key
                        // Clear Builder
                        sb.Clear();
                        // All Characters Until " are Part of Key
                        // -> Look for "
                        state = ParseState.EndKey;
                    }
                    break;
                case ParseState.EndKey:
                    if (c == '"')
                    {
                        // " found
                        // End of Key
                        // Emit Key String
                        key = sb.ToString();
                        // -> Look for :
                        state = ParseState.KeyValueColon;
                    }
                    else
                    {
                        // Part of Key
                        // Add to Key String
                        // Continue to Look for "
                        sb.Append(c);
                    }
                    break;
                case ParseState.KeyValueColon:
                    if (c == ':')
                    {
                        // : Found
                        // Colon Separator
                        // -> Look for "
                        state = ParseState.BeginValue;
                    }
                    break;
                case ParseState.BeginValue:
                    if (c == '"')
                    {
                        // " found
                        // Beginning of Value
                        // Clear Builder
                        sb.Clear();
                        // -> Look for "
                        state = ParseState.EndValue;
                    }
                    break;
                case ParseState.EndValue:
                    if (c == '"')
                    {
                        // " found
                        // End of Value
                        // Emit Value String
                        value = sb.ToString();
                        // Add Key and Value to Dictionary
                        d.Add(key, value);
                        // -> Look for , or }
                        state = ParseState.FinishJSON;
                    }
                    else
                    {
                        // Part of Value
                        // Add to Value String
                        // Continue to Look for "
                        sb.Append(c);
                    }
                    break;
                case ParseState.FinishJSON:
                    if (c == ',')
                    {
                        // , found
                        // New Key / Value Expected
                        // -> look for "
                        state = ParseState.BeginKey;
                    }
                    else if (c == '}')
                    {
                        // } found
                        // End of JSON
                        // -> DONE
                        state = ParseState.Validated;
                    }
                    break;
            }
        }
        return d;
    }
}