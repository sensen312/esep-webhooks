using System.Text;
using System;
using System.Net.Http;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook;

public class Function
{
    /// <summary>
    /// A function that handles the GitHub webhook payload and posts a message to Slack
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process (GitHub webhook payload).</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>The response from the Slack API.</returns>
    public async Task<string> FunctionHandler(object input, ILambdaContext context)
    {
        dynamic json = JsonConvert.DeserializeObject<dynamic>(input.ToString());
        
        string payload = $"{{'text':'Issue Created: {json.issue.html_url}'}}";
        
        var client = new HttpClient();

        var webRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(webRequest);

        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            
        return await reader.ReadToEndAsync();
    }
}