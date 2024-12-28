using TestGenerator.Shared.Settings.Shared;

namespace TestGenerator.Shared.Types;

public class RunProcessArgs
{
    public enum ProcessRunProvider
    {
        Background,
        RunTab,
    }
    
    public required string Filename { get; init; }
    public string Args { get; init; } = "";
    public string? WorkingDirectory { get; init; }
    public string? Stdin { get; init; }
    public EnvironmentModel? Environment { get; init; }
}