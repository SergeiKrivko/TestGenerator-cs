using Avalonia.Input;

namespace TestGenerator.Shared.Types;

public interface IEditorProvider
{
    public string[]? Extensions => [];
    public string Key { get; }
    public string Name => Key;
    public string? Icon => null;
    public int Priority => 1;
    
    public OpenedFile? Open(string path);

    public bool CanOpen(string path) => Extensions?.Contains(Path.GetExtension(path)) ?? true;
}