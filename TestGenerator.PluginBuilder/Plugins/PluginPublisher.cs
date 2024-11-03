using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Testenerator.PluginBuilder.Plugins;

public class PluginPublisher
{
    public const string Url = "http://localhost:5255/api/v1/plugins/releases";

    private static HttpClient _client = new();

    public static async Task PublishByUrl(PluginConfig config, string url, string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var resp = await _client.PostAsync(Url, JsonContent.Create(new PluginRequestBody
        {
            Key = config.Key,
            Name = config.Name,
            Description = config.Description,
            Version = config.Version,
            Url = url,
        }));
        Console.WriteLine($"{resp.StatusCode}: {await resp.Content.ReadAsStringAsync()}");
        // if (!resp.IsSuccessStatusCode)
        // {
        //     Console.WriteLine($"Error: {await resp.Content.ReadAsStringAsync()}");
        // }
    }
}