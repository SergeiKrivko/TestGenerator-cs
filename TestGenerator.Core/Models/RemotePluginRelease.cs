using System.Text.Json.Serialization;

namespace TestGenerator.Core.Models;

public class RemotePluginRelease
{
    [JsonPropertyName("pluginReleaseId")] public required Guid Id { get; init; }
    [JsonPropertyName("pluginId")] public required Guid PluginId { get; init; }
    [JsonPropertyName("publisherId")] public required Guid PublisherId { get; init; }
    // [JsonPropertyName("key")] public required string Key { get; init; }
    [JsonPropertyName("name")] public required string Name { get; init; }
    [JsonPropertyName("description")] public string? Description { get; init; }
    [JsonPropertyName("version")] public required Version Version { get; init; }
    [JsonPropertyName("runtime")] public required string? Runtime { get; init; }
    [JsonPropertyName("url")] public required string Url { get; init; }
}