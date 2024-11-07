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
            .Where(f => f.EndsWith(".dll") && !f.Contains("Avalonia") && !f.Contains("Serilog") &&
                        Path.GetFileNameWithoutExtension(f) != "TestGenerator.Shared"));

        return res;
    }

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
            File.Copy(dll, Path.Join(tempPath,
                Path.GetRelativePath(Path.Join(path, "bin/Release/net8.0/publish"), dll)));
        }

        var configFile = File.CreateText(Path.Join(tempPath, "Config.json"));
        configFile.Write(JsonSerializer.Serialize(pluginConfig));
        configFile.Close();

        outPath ??= Path.Join(path, $"{pluginConfig.Key}.zip");
        if (File.Exists(outPath))
            File.Delete(outPath);
        ZipFile.CreateFromDirectory(tempPath, outPath);

        if (install)
        {
            var installedPath = Path.Join(AppDataPath, "Plugins", pluginConfig.Key);
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
}