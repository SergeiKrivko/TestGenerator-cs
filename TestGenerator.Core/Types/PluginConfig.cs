namespace TestGenerator.Core.Types;

public class PluginConfig
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = "";
    public required Version Version { get; init; }
    public required string Assembly { get; init; }
}