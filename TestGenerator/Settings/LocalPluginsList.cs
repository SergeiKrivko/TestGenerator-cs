using System;
using System.Linq;
using System.Threading.Tasks;
using AvaluxUI.Utils;
using TestGenerator.Core.Exceptions;
using TestGenerator.Core.Models;
using TestGenerator.Core.Services;

namespace TestGenerator.Settings;

public class LocalPluginsList : PluginsList
{
    private readonly PluginsService _pluginsService = Injector.Inject<PluginsService>();
    
    protected override Task<RemotePlugin[]> LoadAllPlugins()
    {
        return Task.Run(() =>
        {
            return _pluginsService.Plugins.Values.Select(p => new RemotePlugin
            {
                Key = p.Config.Key,
                OwnerId = Guid.Empty,
                PluginId = Guid.Empty,
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