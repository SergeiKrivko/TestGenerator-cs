using System;
using System.Text.Json.Serialization;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public class OpenedFileModel
{
    [JsonIgnore] public Guid Id { get; } = Guid.NewGuid();
    [JsonIgnore] public OpenedFile? File { get; init; }
    public required string Path { get; init; }
    public required string Provider { get; init; }
}