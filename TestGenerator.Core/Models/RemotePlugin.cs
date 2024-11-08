using System.Text.Json.Serialization;

namespace TestGenerator.Core.Models;

public class RemotePlugin
{
    [JsonPropertyName("pluginId")] public required Guid PluginId { get; init; }
    [JsonPropertyName("ownerId")] public required Guid OwnerId { get; init; }
    [JsonPropertyName("key")] public required string Key { get; init; }
}
