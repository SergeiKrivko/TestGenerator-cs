using System;
using System.Linq;
using System.Threading.Tasks;
using TestGenerator.Core.Exceptions;
using TestGenerator.Core.Models;
using TestGenerator.Core.Services;

namespace TestGenerator.Settings;

public class LocalPluginsList : PluginsList
{
    protected override Task<RemotePlugin[]> LoadAllPlugins()
    {
        return Task.Run(() =>
        {
            return PluginsService.Instance.Plugins.Values.Select(p => new RemotePlugin
            {
                Key = p.Config.Key,
                OwnerId = default,
                PluginId = default,
            }).ToArray();
        });
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