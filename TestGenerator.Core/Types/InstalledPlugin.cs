using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using TestGenerator.Shared;

namespace TestGenerator.Core.Types;

public class InstalledPlugin : IDisposable
{
    public required Guid Id { get; init; }
    public required PluginConfig Config { get; init; }
    public required Plugin Plugin { get; init; }
    public required string Path { get; init; }
    public required Assembly Assembly { get; init; }
    public required AssemblyLoadContext Context { get; init; }

    public static InstalledPlugin Load(string pluginPath)
    {
        var config =
            JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(System.IO.Path.Join(pluginPath, "Config.json")));
        if (config == null)
            throw new Exception("Invalid plugin: config not found");

        var context = new AssemblyLoadContext(System.IO.Path.GetFileName(pluginPath), true);
        var assembly = context.LoadFromAssemblyPath(System.IO.Path.Join(pluginPath, config.Assembly));

        var pluginClass = assembly.GetTypes().First(t => typeof(Plugin).IsAssignableFrom(t));
        var pluginInstance = Activator.CreateInstance(pluginClass) as Plugin ??
                             throw new Exception("Failed to instantiate plugin");

        return new InstalledPlugin
        {
            Id = Guid.Parse(System.IO.Path.GetFileName(pluginPath)),
            Config = config,
            Path = pluginPath,
            Assembly = assembly,
            Context = context,
            Plugin = pluginInstance
        };
    }

    public void Dispose()
    {
        Context.Unload();
        Console.WriteLine(string.Join(';', Context.Assemblies));
    }
}