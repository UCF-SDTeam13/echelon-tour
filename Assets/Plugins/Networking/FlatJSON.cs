
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

// Flat JSON Serializer / Deserializer to avoid Reflection for IL2CPP
// Flat Only: No Child Objects
// Supports Strings / Arrays of Strings (No Numbers)

public class FlatJSON
{
    private readonly Dictionary<string, string> stringValues = new Dictionary<string, string>();
    private readonly Dictionary<string, string[]> stringArrays = new Dictionary<string, string[]>();
    private readonly StringBuilder sb = new StringBuilder();

    // Serialize by Building String
    override public string ToString()
    {
        sb.Clear();
        sb.Append("{");
        // Add Each String Value
        foreach (KeyValuePair<string, string> kvPair in stringValues)
        {
            sb.Append("\"");
            sb.Append(kvPair.Key);
            sb.Append("\":\"");
            sb.Append(kvPair.Value);
            sb.Append("\",");
        }
        // Add Each String Array
        foreach (KeyValuePair<string, string[]> kvPair in stringArrays)
        {
            sb.Append("\"");
            sb.Append(kvPair.Key);
            sb.Append("\":\"[");
            foreach (string v in kvPair.Value)
            {
                sb.Append("\"");
                sb.Append(v);
                sb.Append("\",");
            }
            // Remove Trailing Comma if There Is One
            if (stringArrays.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("],");
        }
        // JavaScript / ECMAScript 5 Objects Allow Trailing Commas
        // However - It seems that isn't part of the JSON Spec
        // Remove Last Trailing Comma
        // However, Don't Remove the Initial '{' If There Is No Trailing Comma
        if (sb.Length > 1)
        {
            sb.Remove(sb.Length - 1, 1);
        }
        sb.Append("}");
        return sb.ToString();
    }

    public HttpContent SerializeContent()
    {
        StringContent content = new StringContent(ToString(), Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return content;
    }
    // Deserialize using a Finite State Machine
    private enum ParseSearchState { BeginJSON, BeginKey, EndKey, KeyValueColon, BeginArrayOrValue, BeginArrayValue, EndArrayValue, EndValue, EndArray, EndJSON, Validated };
    public void Deserialize(string json)
    {
        Clear();
        ParseSearchState state = ParseSearchState.BeginJSON;
        string key = "";
        string value;
        List<string> stringArray = new List<string>();
        foreach (char c in json)
        {
            switch (state)
            {
                case ParseSearchState.BeginJSON:
                    if (c == '{')
                    {
                        // { found
                        // Beginning of JSON 
                        // -> Look for Beginning of Key "
                        state = ParseSearchState.BeginKey;
                    }
                    break;
                case ParseSearchState.BeginKey:
                    if (c == '"')
                    {
                        // " found
                        // Beginning of Key
                        // Clear Builder
                        sb.Clear();
                        // All Characters Until " are Part of Key
                        // -> Look for "
                        state = ParseSearchState.EndKey;
                    }
                    break;
                case ParseSearchState.EndKey:
                    if (c == '"')
                    {
                        // " found
                        // End of Key
                        // Emit Key String
                        key = sb.ToString();
                        // -> Look for :
                        state = ParseSearchState.KeyValueColon;
                    }
                    else
                    {
                        // Part of Key
                        // Add to Key String
                        // Continue to Look for "
                        sb.Append(c);
                    }
                    break;
                case ParseSearchState.KeyValueColon:
                    if (c == ':')
                    {
                        // : Found
                        // Colon Separator
                        // -> Look for " or [
                        state = ParseSearchState.BeginArrayOrValue;
                    }
                    break;
                case ParseSearchState.BeginArrayOrValue:
                    if (c == '"')
                    {
                        // " found
                        // Beginning of Value
                        // Clear Builder
                        sb.Clear();
                        // -> Look for "
                        state = ParseSearchState.EndValue;
                    }
                    else if (c == '[')
                    {
                        // [ found
                        // Beginning of Array
                        // -> Look for "
                        state = ParseSearchState.EndArrayValue;
                    }
                    break;
                case ParseSearchState.BeginArrayValue:
                    if (c == '"')
                    {
                        // " found
                        // Beginning of Array Value
                        // Clear Builder
                        sb.Clear();
                        // -> Look for "
                        state = ParseSearchState.EndArrayValue;
                    }
                    break;
                case ParseSearchState.EndArrayValue:
                    if (c == '"')
                    {
                        // " found
                        // End of Value
                        // Emit Value String
                        value = sb.ToString();
                        // Add Value to Array
                        stringArray.Add(value);
                        // -> Look for , or ]
                        state = ParseSearchState.EndArray;
                    }
                    else
                    {
                        // Part of Value
                        // Add to Value String
                        // Continue to Look for "
                        sb.Append(c);
                    }
                    break;
                case ParseSearchState.EndArray:
                    if (c == ',')
                    {
                        // , found
                        // -> Look for "
                        state = ParseSearchState.BeginArrayValue;
                    }
                    else if (c == ']')
                    {
                        // ] found
                        // End of Array
                        // Emit Array
                        stringArrays.Add(key, stringArray.ToArray());
                        // -> Look for , or }
                        state = ParseSearchState.EndJSON;
                    }
                    break;
                case ParseSearchState.EndValue:
                    if (c == '"')
                    {
                        // " found
                        // End of Value
                        // Emit Value String
                        value = sb.ToString();
                        // Add Key and Value to Dictionary
                        stringValues.Add(key, value);
                        // -> Look for , or }
                        state = ParseSearchState.EndJSON;
                    }
                    else
                    {
                        // Part of Value
                        // Add to Value String
                        // Continue to Look for "
                        sb.Append(c);
                    }
                    break;
                case ParseSearchState.EndJSON:
                    if (c == ',')
                    {
                        // , found
                        // New Key / Value Expected
                        // -> look for "
                        state = ParseSearchState.BeginKey;
                    }
                    else if (c == '}')
                    {
                        // } found
                        // End of JSON
                        // -> DONE
                        state = ParseSearchState.Validated;
                    }
                    break;
            }
        }
    }
    public void TryGetStringValue(string k, out string v)
    {
        stringValues.TryGetValue(k, out v);
    }
    public void TryGetStringArray(string k, out string[] v)
    {
        stringArrays.TryGetValue(k, out v);
    }
    public void Add(string k, string v)
    {
        stringValues.Add(k, v);
    }
    public void Add(string k, string[] v)
    {
        stringArrays.Add(k, v);
    }
    public void Clear()
    {
        stringValues.Clear();
        stringArrays.Clear();
    }
}