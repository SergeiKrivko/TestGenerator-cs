namespace TestGenerator.PluginBuilder.Plugins;

public class PluginConfig
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = "";
    public required Version Version { get; init; }
    public required string Assembly { get; init; }
    public bool PlatformSpecific { get; init; } = false;
    public string? Runtime { get; set; } = null;
}