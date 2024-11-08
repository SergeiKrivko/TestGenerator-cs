using TestGenerator.Shared;

namespace TestGenerator.Core.Types;

public class InstalledPlugin
{
    public required PluginConfig Config { get; init; }
    public required Plugin Plugin { get; init; }
    public required string Path { get; init; }
}