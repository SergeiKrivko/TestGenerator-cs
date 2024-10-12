using Avalonia.Controls.Documents;

namespace Shared;

public interface ITerminalController
{
    public string Command { get; }
    public Task<int> Run();
    
    public string? WorkingDirectory { get; }
}