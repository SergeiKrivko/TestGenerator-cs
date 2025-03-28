﻿namespace TestGenerator.PluginBuilder.Plugins;

public class PluginRequestBody
{
    public required string Key { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public required Version Version { get; set; }
    public string? Url { get; set; }
    public string? Runtime { get; set; }
}