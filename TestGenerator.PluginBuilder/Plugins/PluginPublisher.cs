using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Testenerator.PluginBuilder.Plugins;

public class PluginPublisher
{
    // public const string Url = "http://localhost:5255/api/v1/plugins/releases";
    public const string Url = "https://testgenerator-api.nachert.art/api/v1/plugins/releases";

    private static readonly HttpClient Client = new();

    public static async Task PublishByUrl(PluginConfig config, string url, string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var resp = await Client.PostAsync(Url, JsonContent.Create(new PluginRequestBody
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