namespace TestGenerator.Shared.Types;

public interface ITerminalController
{
    public string Command { get; }
    public Task<int> RunAsync();
    
    public string? WorkingDirectory { get; }
}