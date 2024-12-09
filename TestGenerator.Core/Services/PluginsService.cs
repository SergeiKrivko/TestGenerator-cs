using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.Json;
using TestGenerator.Core.Exceptions;
using TestGenerator.Core.Types;
using TestGenerator.Shared;

namespace TestGenerator.Core.Services;

public class PluginsService
{
    private static PluginsService? _instance;
    private readonly HttpClient _httpClient = new();

    public static PluginsService Instance
    {
        get
        {
            _instance ??= new PluginsService();
            return _instance;
        }
    }

    public Dictionary<string, InstalledPlugin> Plugins { get; } = [];

    public delegate void PluginLoadedHandler(Plugin plugin);

    public event PluginLoadedHandler? OnPluginLoaded;
    public event PluginLoadedHandler? OnPluginUnloaded;

    public void Load()
    {
        var path = Path.Join(AppService.Instance.AppDataPath, "Plugins");
        Directory.CreateDirectory(path);
        foreach (var directory in Directory.GetDirectories(path))
        {
            if (File.Exists(Path.Join(directory, "IsDeleted")))
            {
                try
                {
                    Directory.Delete(directory, recursive: true);
                }
                catch (UnauthorizedException e)
                {
                    LogService.Logger.Warning($"Cannot delete directory '{directory}': {e.Message}");
                }
            }
            else
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
    }

    private void LoadPlugin(string pluginPath)
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
                    Plugins.Add(config.Key,
                        new InstalledPlugin { Config = config, Plugin = plugin, Path = pluginPath });
                    LogService.Logger.Debug($"Plugin '{config.Key}' loaded");
                    OnPluginLoaded?.Invoke(plugin);
                    AppService.Instance.RunBackgroundTask($"Инициализация плагина {config.Name}", async () =>
                    {
                        await plugin.Init();
                        return 0;
                    });
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

    private void UnloadPlugin(string key)
    {
        var plugin = Plugins[key];
        OnPluginUnloaded?.Invoke(plugin.Plugin);
        Plugins.Remove(key);
    }

    public async Task InstallPlugin(string url)
    {
        var installedPath = Path.Join(AppService.Instance.AppDataPath, "Plugins", Guid.NewGuid().ToString());
        var stream = await _httpClient.GetStreamAsync(url);
        await Task.Run(() => ZipFile.ExtractToDirectory(stream, installedPath));
        LoadPlugin(installedPath);
    }

    public async Task RemovePlugin(string key)
    {
        if (!Plugins.ContainsKey(key))
        {
            LogService.Logger.Warning($"Plugin '{key}' not found");
            return;
        }

        var plugin = Plugins[key];
        await plugin.Plugin.Destroy();
        UnloadPlugin(key);
        File.Create(Path.Join(plugin.Path, "IsDeleted"));
        LogService.Logger.Information($"Plugin '{key}' was unloaded and marked as deleted");
    }

    public string GetPluginKeyByAssembly(Assembly assembly)
    {
        var path = Path.GetFullPath(assembly.Location);
        var dirPath = Path.GetFullPath(Path.Join(AppService.Instance.AppDataPath, "Plugins"));
        if (!path.StartsWith(dirPath))
            throw new Exception("Call not from plugin");
        while (!string.IsNullOrEmpty(path = Path.GetDirectoryName(path)))
        {
            if (File.Exists(Path.Join(path, "Config.json")))
            {
                var config = JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(path, "Config.json")));
                if (config != null)
                    return config.Key;
            }
        }
        throw new Exception("Plugin config not found");
    }
}