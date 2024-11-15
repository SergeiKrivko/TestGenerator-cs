using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

namespace TestGenerator.PluginBuilder.Plugins;

public class Builder
{
    public static string AppDataPath { get; } = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko", "TestGenerator");

    private static List<string> GetDlls(string path)
    {
        var res = new List<string>();
        foreach (var directory in Directory.GetDirectories(path))
        {
            res.AddRange(GetDlls(directory));
        }

        res.AddRange(Directory.GetFiles(path)
            .Where(f => !f.EndsWith(".pdb") && !ExcludeDlls.Contains(Path.GetFileName(f))));

        return res;
    }

    private static readonly string[] ExcludeDlls =
    [
        "TestGenerator.Shared.dll",
        "Avalonia.Base.dll",
        "Avalonia.Controls.dll",
        "Avalonia.DesignerSupport.dll",
        "Avalonia.Dialogs.dll",
        "Avalonia.Markup.Xaml.dll",
        "Avalonia.Markup.dll",
        "Avalonia.Metal.dll",
        "Avalonia.MicroCom.dll",
        "Avalonia.OpenGL.dll",
        "Avalonia.Vulkan.dll",
        "Avalonia.dll",
        "Avalonia.Remote.Protocol.dll",
        "Avalonia.BuildServices.dll",
        "MicroCom.Runtime.dll",
        "Serilog.dll",
        "Serilog.Sinks.Console.dll",
        "Serilog.Sinks.File.dll"
    ];

    public static string Build(string path, string? outPath = null, bool install = false)
    {
        var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(path, "Config.json")));
        if (pluginConfig == null)
            throw new Exception("Invalid plugin config");

        Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "publish",
            WorkingDirectory = path,
        })?.WaitForExit();

        var tempPath = Path.Join(path, "bin/Release/net8.0/Plugin");
        if (Directory.Exists(tempPath))
            Directory.Delete(tempPath, recursive: true);
        Directory.CreateDirectory(tempPath);
        foreach (var dll in GetDlls(Path.Join(path, "bin/Release/net8.0/publish")))
        {
            var dir = Path.GetDirectoryName(Path.Join(tempPath,
                Path.GetRelativePath(Path.Join(path, "bin/Release/net8.0/publish"), dll)));
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            Console.WriteLine($"Copy {dll}...");
            File.Copy(dll, Path.Join(tempPath,
                Path.GetRelativePath(Path.Join(path, "bin/Release/net8.0/publish"), dll)));
        }

        Console.WriteLine("Writing config file...");
        var configFile = File.CreateText(Path.Join(tempPath, "Config.json"));
        configFile.Write(JsonSerializer.Serialize(pluginConfig));
        configFile.Close();

        outPath ??= Path.Join(path, $"{pluginConfig.Key}.zip");
        if (File.Exists(outPath))
            File.Delete(outPath);
        Console.WriteLine("Creating ZIP...");
        ZipFile.CreateFromDirectory(tempPath, outPath);

        if (install)
        {
            RemovePlugin(pluginConfig.Key);

            Console.WriteLine("Installing plugin...");
            var installedPath = Path.Join(AppDataPath, "Plugins", Guid.NewGuid().ToString());
            if (Directory.Exists(installedPath))
            {
                Directory.Delete(installedPath, recursive: true);
            }

            Directory.Move(tempPath, installedPath);
        }
        else
            Directory.Delete(tempPath, recursive: true);

        return outPath;
    }

    private static void RemovePlugin(string key)
    {
        foreach (var directory in Directory.GetDirectories(Path.Join(AppDataPath, "Plugins")))
        {
            try
            {
                var installedPluginConfig =
                    JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(directory, "Config.json")));
                if (installedPluginConfig?.Key == key)
                {
                    Console.WriteLine($"Marking {directory} as deleted...");
                    File.Create(Path.Join(directory, "IsDeleted"));
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (JsonException)
            {
            }
        }
    }
}