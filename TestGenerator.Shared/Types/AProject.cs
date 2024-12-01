using System.Text.Json;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Shared.Types;

public abstract class AProject
{
    public static string TestGeneratorDir { get; } = ".TestGenerator";
    
    public abstract SettingsFile Settings { get; }
    public abstract SettingsFile Data { get; }

    public abstract SettingsSection GetSettings();
    public abstract SettingsSection GetSettings(string key);

    public abstract SettingsSection GetData();
    public abstract SettingsSection GetData(string key);

    public abstract Guid Id { get; }

    public abstract string Name { get; set; }

    public abstract string Path { get; }
    public abstract ProjectType Type { get; }
}