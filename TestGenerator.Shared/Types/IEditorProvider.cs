namespace TestGenerator.Shared.Types;

public interface IEditorProvider
{
    public string[]? Extensions => [];
    public string Name { get; }
    public string? Icon => null;
    public int Priority => 1;
    
    public OpenedFile Open(string path);

    public bool CanOpen(string path) => Extensions?.Contains(path) ?? true;
}