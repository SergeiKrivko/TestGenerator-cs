using System;
using System.Threading.Tasks;
using TestGenerator.Core.Exceptions;
using TestGenerator.Core.Models;

namespace TestGenerator.Settings;

public class RemotePluginsList : PluginsList
{
    protected override async Task<RemotePlugin[]> LoadAllPlugins()
    {
        try
        {
            return await HttpService.GetAllPlugins();
        }
        catch (HttpServiceException)
        {
            return [];
        }
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