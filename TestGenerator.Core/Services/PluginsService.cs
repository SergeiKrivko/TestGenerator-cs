﻿using System.Diagnostics;
using System.Reflection;
using TestGenerator.Core.Exceptions;
using TestGenerator.Core.Types;
using TestGenerator.Shared;
using TestGenerator.Shared.Types;

namespace TestGenerator.Core.Services;

public class PluginsService(IAppService appService)
{
    private readonly HttpClient _httpClient = new();

    public Dictionary<string, InstalledPlugin> Plugins { get; } = [];

    public delegate void PluginLoadedHandler(Plugin plugin);

    public event PluginLoadedHandler? OnPluginLoaded;
    public event PluginLoadedHandler? OnPluginUnloaded;

    public string PluginsPath { get; } = Path.Join(appService.AppDataPath, "Plugins");

    public void Initialize()
    {
        Directory.CreateDirectory(PluginsPath);
        foreach (var directory in Directory.GetDirectories(PluginsPath))
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
                    Load(directory);
                }
                catch (Exception e)
                {
                    LogService.Logger.Error($"Fail to load plugin '{directory}': {e}");
                }
            }
        }
    }

    private void Load(string pluginPath)
    {
        var plugin = InstalledPlugin.Load(pluginPath);
        Plugins.Add(plugin.Config.Key, plugin);
        LogService.Logger.Debug($"Plugin '{plugin.Config.Key}' loaded");
        OnPluginLoaded?.Invoke(plugin.Plugin);
        appService.RunBackgroundTask($"Инициализация плагина {plugin.Config.Name}", async token =>
        {
            await plugin.Plugin.Init(token);
            return 0;
        });
    }

    private void Unload(string key)
    {
        var plugin = Plugins[key];
        OnPluginUnloaded?.Invoke(plugin.Plugin);
        plugin.Dispose();
        Plugins.Remove(key);
    }

    public async Task Install(string url)
    {
        var id = Guid.NewGuid();

        var tempPath = Path.GetTempFileName();
        await using (var stream = await _httpClient.GetStreamAsync(url))
        {
            await using var file = File.OpenWrite(tempPath);
            await stream.CopyToAsync(file);
        }

        await RunInstaller($"install {tempPath} {id} --clear-deleted --clear-duplicates");
        Load(Path.Join(PluginsPath, id.ToString()));
    }

    public async Task Update(string key, string url)
    {
        Guid oldId;
        {
            if (!Plugins.TryGetValue(key, out var plugin))
            {
                LogService.Logger.Warning($"Plugin '{key}' not found");
                return;
            }

            plugin = Plugins[key];
            await plugin.Plugin.Destroy();
            Unload(key);
            oldId = plugin.Id;
        }

        var newId = Guid.NewGuid();

        var tempPath = Path.GetTempFileName();
        await using (var stream = await _httpClient.GetStreamAsync(url))
        {
            await using var file = File.OpenWrite(tempPath);
            await stream.CopyToAsync(file);
        }

        await RunInstaller($"update {oldId} {tempPath} {newId} --clear-deleted --clear-duplicates");
        Load(Path.Join(PluginsPath, newId.ToString()));
    }

    public async Task Remove(string key)
    {
        Guid id;
        {
            if (!Plugins.TryGetValue(key, out var plugin))
            {
                LogService.Logger.Warning($"Plugin '{key}' not found");
                return;
            }

            plugin = Plugins[key];
            await plugin.Plugin.Destroy();
            Unload(key);
            id = plugin.Id;
        }
        await RunInstaller($"remove {id} --clear-deleted --clear-duplicates");
        LogService.Logger.Information($"Plugin '{key}' was unloaded and deleted");
    }

    private static async Task RunInstaller(string args)
    {
        var installerPath = Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location),
            "TestGenerator.PluginInstaller");
        Process? process = null;
        if (OperatingSystem.IsWindows())
        {
            process = Process.Start(new ProcessStartInfo
            {
                FileName = installerPath + ".exe",
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = true,
                Verb = "runas",
            });
        }
        else if (OperatingSystem.IsLinux())
        {
            process = Process.Start(new ProcessStartInfo
            {
                FileName = "gnome-terminal",
                Arguments = $"-- \"{installerPath}\" {args}",
            });
        }
        else if (OperatingSystem.IsMacOS())
        {
            process = Process.Start(new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"-a Terminal \"{installerPath}\""
            });
        }

        if (process == null)
            return;

        await process.WaitForExitAsync();
    }
}