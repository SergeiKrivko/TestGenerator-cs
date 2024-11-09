using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public class OpenedFile
{
    public Guid Id { get; } = Guid.NewGuid();
    public required Control Widget { get; init; }
    public required string Name { get; init; }
    public string? Icon { get; init; } = null;
    public required string Path { get; init; }
}