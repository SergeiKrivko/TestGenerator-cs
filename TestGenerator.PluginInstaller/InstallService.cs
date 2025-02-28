using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text.Json;

namespace TestGenerator.PluginInstaller;

public class InstallService
{
    private string AppDataPath { get; } = Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko", "TestGenerator",
        "Plugins");

    public void Initialize()
    {
        Directory.CreateDirectory(AppDataPath);
        PermissionsService.SetPermissions(AppDataPath);
    }

    private string GetInstallPath(Guid? id = null)
    {
        id ??= Guid.NewGuid();
        return Path.Join(AppDataPath, id.ToString());
    }

    public void Install(string zipPath, Guid? id = null)
    {
        ZipFile.ExtractToDirectory(zipPath, GetInstallPath(id));
        File.Delete(zipPath);
    }

    private const int RemoveReties = 5;

    private static void SafeDeleteDirectory(string path, bool markIfFail = false)
    {
        for (var i = 0; i < RemoveReties; i++)
        {
            try
            {
                Directory.Delete(path, true);
                return;
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }
        }

        if (markIfFail && Directory.Exists(path))
            File.Create(Path.Join(path, "IsDeleted")).Close();
    }

    public void Remove(Guid id)
    {
        Console.WriteLine($"Removing {GetInstallPath(id)}...");
        SafeDeleteDirectory(GetInstallPath(id), true);
    }

    public void Update(Guid oldId, string zipPath, Guid? newId = null)
    {
        Install(zipPath, newId);
        Remove(oldId);
        Directory.SetLastWriteTime(GetInstallPath(newId), DateTime.Now);
    }

    public void ClearDeleted()
    {
        foreach (var directory in Directory.GetDirectories(AppDataPath)
                     .Where(d => File.Exists(Path.Join(d, "IsDeleted"))))
        {
            Console.WriteLine($"Deleting {directory} as marked as deleted...");
            SafeDeleteDirectory(directory, true);
        }

        foreach (var directory in Directory.GetDirectories(AppDataPath)
                     .Where(d => !File.Exists(Path.Join(d, "Config.json"))))
        {
            Console.WriteLine($"Deleting {directory} as invalid...");
            SafeDeleteDirectory(directory, true);
        }
    }

    private record PluginConfig(string Key);

    private static string? GetKey(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<PluginConfig>(File.ReadAllText(Path.Join(path, "Config.json")))?.Key;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void ClearDuplicates()
    {
        foreach (var group in Directory.GetDirectories(AppDataPath)
                     .Where(d => !File.Exists(Path.Join(d, "IsDeleted")))
                     .GroupBy(GetKey))
        {
            foreach (var directory in group.OrderBy(Directory.GetLastWriteTime).SkipLast(1))
            {
                Console.WriteLine($"Deleting {directory} as duplicate...");
                SafeDeleteDirectory(directory, true);
            }
        }
    }
}