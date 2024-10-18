using System.Text.Json;
using Shared.Utils;

namespace Shared;

public abstract class AProject
{
    public static string TestGeneratorDir { get; } = ".TestGenerator";
    public static string DataFile { get; } = "TestGeneratorData.json";
    
    public abstract SettingsSection Settings { get; }
    public abstract SettingsSection Data { get; }

    public abstract Guid Id { get; }

    public abstract string Name { get; set; }

    public abstract string Path { get; }
    public abstract ProjectType Type { get; }
}