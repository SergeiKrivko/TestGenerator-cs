using TestGenerator.Core.Models;

namespace TestGenerator.Core.Services;

public class PluginsHttpService : BodyDetailHttpService
{
    public PluginsHttpService()
    {
        BaseUrl = "https://testgenerator-api.nachert.art/";
    }

    public async Task<RemotePlugin[]> GetAllPlugins()
    {
        return await Get<RemotePlugin[]>("api/v1/plugins");
    }

    public async Task<RemotePluginRelease> GetLatestRelease(string key)
    {
        var runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
        return await Get<RemotePluginRelease>($"api/v1/plugins/releases/latest?key={key}&runtime={runtime}");
    }
}