using System.Reflection;
using TestGenerator.Shared;

namespace Core.Services;

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

    public void LoadPlugin(string pluginPath)
    {
        Assembly pluginAssembly = _getPluginAssembly(pluginPath);
        foreach (Type type in pluginAssembly.GetTypes())
        {
            if (typeof(Plugin).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type);
                if (instance != null)
                {
                    var plugin = (Plugin)instance;
                    Plugins.Add(plugin.Name, plugin);
                    LogService.Logger.Debug($"Plugin '{plugin.Name}' loaded");
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