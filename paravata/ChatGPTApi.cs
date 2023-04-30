using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;


public class ChatGPTApi
{
private const string API_URL = "https://api.openai.com/v1/";
private const string API_KEY = "your-api-key-here";

private RestClient client;
private List<string> conversationHistory;

public ChatGPTApi()
{
    client = new RestClient(API_URL);
    client.AddDefaultHeader("Authorization", $"Bearer {API_KEY}");
    conversationHistory = new List<string>();
}

public string GenerateText(string prompt)
{
    // Add the user's message to the conversation history
    conversationHistory.Add("User: " + prompt);

    // Prepare the conversation history for the API call
    string conversationContext = string.Join("\n", conversationHistory) + "\n";
    Console.WriteLine($"Conversation context: {conversationContext}"); // Debugging line

    var request = new RestRequest("completions", Method.Post);
    request.AddJsonBody(new
    {
        prompt = conversationContext,
        max_tokens = 2000,
        n = 1,
        temperature = 0.7,
        model = "text-davinci-002"
    });

    var response = client.Execute(request);
    if (response.IsSuccessful)
    {
        var content = JObject.Parse(response.Content);
        var generatedText = content["choices"][0]["text"].ToString().Trim();

        // Add the generated response to the history
        conversationHistory.Add("ChatGPT: " + generatedText);

        return generatedText;
    }
    else
    {
        throw new Exception($"Failed to generate text: {response.ErrorMessage}, Status Code: {response.StatusCode}, Content: {response.Content}");
    }
}
}

