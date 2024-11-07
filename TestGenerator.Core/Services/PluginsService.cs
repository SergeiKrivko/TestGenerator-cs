using System.Reflection;
using System.Text.Json;
using TestGenerator.Core.Types;
using TestGenerator.Shared;

namespace TestGenerator.Core.Services;

public class PluginsService
{
    private static PluginsService? _instance;

    public static PluginsService Instance
    {
        get
        {
            _instance ??= new PluginsService();
            return _instance;
        }
    }

    public Dictionary<string, Plugin> Plugins { get; } = [];
    
    public delegate void PluginLoadedHandler(Plugin plugin);
    public event PluginLoadedHandler? OnPluginLoaded;

    public void Load()
    {
        foreach (var directory in Directory.GetDirectories(Path.Join(AppService.Instance.AppDataPath, "Plugins")))
        {
            try
            {
                LoadPlugin(directory);
            }
            catch (Exception e)
            {
                LogService.Logger.Error($"Fail to load plugin '{directory}': {e}");
            }
        }
    }

    public void LoadPlugin(string pluginPath)
    {
        var config = JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(pluginPath, "Config.json")));
        if (config == null)
            throw new Exception("Invalid plugin: config not found");
        var pluginAssembly = _getPluginAssembly(Path.Join(pluginPath, config.Assembly));
        foreach (var type in pluginAssembly.GetTypes())
        {
            if (typeof(Plugin).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var plugin = (Plugin)instance;
                    Plugins.Add(config.Key, plugin);
                    LogService.Logger.Debug($"Plugin '{config.Key}' loaded");
                    OnPluginLoaded?.Invoke(plugin);
                }
            }
        }
    }

    private Assembly _getPluginAssembly(string relativePath)
    {
        var root = Path.Join(AppService.Instance.AppDataPath, "Plugins");

        var pluginLocation =
            Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        LogService.Logger.Debug($"Loading plugin from: {pluginLocation}");
        var loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }
}