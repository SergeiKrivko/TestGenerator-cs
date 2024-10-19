using System.Threading.Tasks;
using Shared;

namespace TestGenerator.TerminalTab;

public class TerminalController : ITerminalController
{
    private RunTerminal _terminal;
    public string Command { get; }
    
    public string? WorkingDirectory { get; init; }

    public TerminalController(RunTerminal terminal, string command)
    {
        _terminal = terminal;
        Command = command;
    }

    public async Task<int> RunAsync()
    {
        if (WorkingDirectory != null)
            _terminal.CurrentDirectory = WorkingDirectory;
        var proc = await _terminal.Run(Command);
        return proc?.ExitCode ?? -1;
    }
}