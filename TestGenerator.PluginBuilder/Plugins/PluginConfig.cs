namespace Testenerator.PluginBuilder.Plugins;

public class PluginConfig
{
    public required string Key { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public required Version Version { get; set; }
    public Version? PluginLibVersion { get; set; }
}