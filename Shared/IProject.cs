using System.Text.Json;

namespace Shared;

public abstract class IProject
{
    public static string TestGeneratorDir { get; } = ".TestGenerator";
    public static string DataFile { get; } = "TestGeneratorData.json";

    public abstract Guid Id { get; }

    public abstract string Name { get; set; }

    public abstract string Path { get; }
    public abstract ProjectType Type { get; }
}