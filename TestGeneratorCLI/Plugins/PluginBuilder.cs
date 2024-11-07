using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using TestGenerator.Core.Services;

namespace TestGeneratorCLI.Plugins;

public class PluginBuilder
{
    private static List<string> GetDlls(string path)
    {
        var res = new List<string>();
        foreach (var directory in Directory.GetDirectories(path))
        {
            res.AddRange(GetDlls(directory));
        }

        res.AddRange(Directory.GetFiles(path)
            .Where(f => !f.StartsWith("Avalonia.") && Path.GetFileNameWithoutExtension(f) != "TestGenerator.Shared"));

        return res;
    }

    public static void Build(string path, string? outPath = null, bool install = false)
    {
        var pluginConfig = JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(path, "Config.json")));
        if (pluginConfig == null)
            throw new Exception("Invalid plugin config");

        Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build",
            WorkingDirectory = path,
        })?.WaitForExit();

        var tempPath = Path.Join(path, "bin/Debug/net8.0/Plugin");
        if (Directory.Exists(tempPath))
            Directory.Delete(tempPath, recursive: true);
        Directory.CreateDirectory(tempPath);
        foreach (var dll in GetDlls(Path.Join(path, "bin/Debug/net8.0")))
        {
            File.Copy(dll, Path.Join(tempPath,
                Path.GetRelativePath(Path.Join(path, "bin/Debug/net8.0"), dll)));
        }

        pluginConfig.PluginLibVersion = TestGenerator.Shared.Config.Version;
        var configFile = File.CreateText(Path.Join(tempPath, "Config.json"));
        configFile.Write(JsonSerializer.Serialize(pluginConfig));
        configFile.Close();

        outPath ??= $"{pluginConfig.Key}.zip";
        if (File.Exists(outPath))
            File.Delete(outPath);
        ZipFile.CreateFromDirectory(tempPath, outPath);

        if (install)
        {
            var installedPath = Path.Join(AppService.Instance.AppDataPath, "Plugins", pluginConfig.Key);
            if (Directory.Exists(installedPath))
            {
                Directory.Delete(installedPath, recursive: true);
            }
            Directory.Move(tempPath, installedPath);
        }
        else
            Directory.Delete(tempPath, recursive: true);
    }
}