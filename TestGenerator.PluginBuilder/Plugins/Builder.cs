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

    public static string Build(string path, string? outPath = null, bool install = false, string? runtime = null)
    {
        var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(path, "Config.json")));
        if (pluginConfig == null)
            throw new Exception("Invalid plugin config");
        var netDir = "bin/Release/net8.0";
        if (runtime != null)
            netDir += "/" + runtime;

        Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "publish" + (runtime == null ? "" : $" -r {runtime}"),
            WorkingDirectory = path,
        })?.WaitForExit();

        var tempPath = Path.Join(path, $"{netDir}/Plugin");
        if (Directory.Exists(tempPath))
            Directory.Delete(tempPath, recursive: true);
        Directory.CreateDirectory(tempPath);
        foreach (var dll in GetDlls(Path.Join(path, $"{netDir}/publish")))
        {
            var dir = Path.GetDirectoryName(Path.Join(tempPath,
                Path.GetRelativePath(Path.Join(path, $"{netDir}/publish"), dll)));
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            Console.WriteLine($"Copy {dll}...");
            File.Copy(dll, Path.Join(tempPath,
                Path.GetRelativePath(Path.Join(path, $"{netDir}/publish"), dll)));
        }

        Console.WriteLine("Writing config file...");
        File.WriteAllText(Path.Join(tempPath, "Config.json"), JsonSerializer.Serialize(pluginConfig));

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