using System.Threading.Tasks;
using TestGenerator.Core.Exceptions;
using TestGenerator.Core.Models;

namespace TestGenerator.Settings;

public class RemotePluginsList : PluginsList
{
    protected override async Task<RemotePlugin[]> LoadAllPlugins()
    {
        return await HttpService.GetAllPlugins();
    }

    protected override async Task<RemotePluginRelease?> LoadLatestRelease(string key)
    {
        try
        {
            return await HttpService.GetLatestRelease(key);
        }
        catch (HttpServiceException)
        {
            return null;
        }
    }
}