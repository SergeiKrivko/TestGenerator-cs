namespace TestGenerator.Shared.Types;

public interface IFileAction
{
    public string[]? Extensions => [];
    public string Key { get; }
    public string Name => Key;
    public string? Icon => null;
    public int Priority => 1;
    
    public Task Run(string path);

    public bool CanUse(string path) => Extensions?.Contains(Path.GetExtension(path)) ?? true;
}