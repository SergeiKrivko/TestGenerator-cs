using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Octokit;
using TestGenerator.PluginBuilder.Args;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace TestGenerator.PluginBuilder.Plugins;

public class PluginPublisher
{
    // public const string Url = "http://localhost:5255/api/v1/plugins/releases";
    private const string Url = "https://testgenerator-api.nachert.art/api/v1/plugins/releases";

    private readonly HttpClient _client = new();

    public async Task PublishByUrl(PluginConfig config, string url, string token)
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

    public async Task<string> PublishOnGithub(PluginConfig config, string githubUser, string githubRepo,
        string githubToken, string path)
    {
        var client = new GitHubClient(new ProductHeaderValue("SergeiKrivko"));
        client.Credentials = new Credentials(githubToken);

        var tag = $"{config.Key}-{config.Version}";
        Release? release = null;
        try
        {
            release = await client.Repository.Release.Get(githubUser, githubRepo, tag);
        }
        catch (NotFoundException)
        {
        }

        release ??= await client.Repository.Release.Create(githubUser, githubRepo,
            new NewRelease(tag));

        var asset = await client.Repository.Release.UploadAsset(release,
            new ReleaseAssetUpload(config.Key + ".zip", "application/zip", File.OpenRead(path),
                TimeSpan.FromMinutes(5)));
        return asset.BrowserDownloadUrl;
    }

    public async Task Publish(PluginPublishOptions options)
    {
        var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(
            await File.ReadAllTextAsync(Path.Join(options.Path ?? Directory.GetCurrentDirectory(), "Config.json")));
        if (pluginConfig == null)
            throw new Exception("Invalid config");

        var zipPath = Builder.Build(options.Path ?? Directory.GetCurrentDirectory());
        Console.WriteLine(zipPath);

        if (options.Github)
        {
            if (options.GithubUser == null || options.GithubRepo == null || options.GithubToken == null)
                throw new Exception("Invalid Github credentials");
            var url = await PublishOnGithub(pluginConfig, options.GithubUser, options.GithubRepo, options.GithubToken,
                zipPath);
            await PublishByUrl(pluginConfig, url, options.Token);
        }
    }
}