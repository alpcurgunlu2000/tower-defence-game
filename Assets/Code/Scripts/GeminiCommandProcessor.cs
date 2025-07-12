using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GeminiCommandProcessor : MonoBehaviour
{
    private string geminiApiKey = "AIzaSyCKkt2TNBhzTHl1kLVYwy3iOZ4gQZEkYpg";
    private string geminiApiUrl;
    private string commandsJson;

    private void Awake()
    {
        geminiApiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={geminiApiKey}";

        // Load commands.json from StreamingAssets
        string path = Path.Combine(Application.streamingAssetsPath, "commands.json");
        if (File.Exists(path))
        {
            commandsJson = File.ReadAllText(path);
            Debug.Log($"Loaded commands.json: {commandsJson.Substring(0, Mathf.Min(100, commandsJson.Length))}...");
        }
        else
        {
            Debug.LogError("commands.json not found in StreamingAssets!");
            commandsJson = "{}";
        }
    }

    public async Task<string> GetCommand(string sentence)
    {
        // log sentence
        using (var client = new HttpClient())
        {
            var payload = new
            {
                contents = new[] {
                    new {
                        parts = new[] {
                            new {
                                text = $"Here is the commands.json file: {commandsJson}. Based on the sentence: '{sentence}', generate a command using the information from the JSON. The generated answer must start with '--COMMAND--<command_name>+<parameter_name1>:<parameter1>+<parameter_name2>:<parameter2> etc.'"
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.5,
                    maxOutputTokens = 50
                }
            };

            string jsonPayload = JsonUtility.ToJson(payload).Replace("\"temperature\":0.5", "\"temperature\":0.5").Replace("\"maxOutputTokens\":50", "\"maxOutputTokens\":50");
            // fix nested objects issue by using Newtonsoft.Json (or do manual building)
            jsonPayload = "{\"contents\":[{\"parts\":[{\"text\":\"Here is the commands.json file: "
    + commandsJson.Replace("\"", "\\\"")
    + ". Based on the sentence: '"
    + sentence
    + "'. Always generate a command using the JSON. "
    + "If the sentence does not exactly match, find the closest command by similar words. "
    + "Never return empty. Always start your answer with '--COMMAND--<command_name>+<parameter_name1>:<parameter1>+<parameter_name2>:<parameter2> etc.'. "
    + "Do NOT explain, do NOT add anything else.\"}]}],\"generationConfig\":{\"temperature\":0.5,\"maxOutputTokens\":50}}";


            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(geminiApiUrl, content);
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    string command = ExtractCommand(result);
                    Debug.Log("Extracted command: " + command);
                    return command;
                }
                else
                {
                    Debug.LogError($"Gemini API error: {response.StatusCode} - {result}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("HTTP error: " + ex.Message);
                return null;
            }
        }
    }

    private string ExtractCommand(string json)
    {
        try
        {
            var parsed = SimpleJSON.JSONNode.Parse(json);
            string text = parsed["candidates"][0]["content"]["parts"][0]["text"];
            return text;
        }
        catch
        {
            return "";
        }
    }
}
