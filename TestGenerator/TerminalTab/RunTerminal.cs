using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls.Documents;
using TestGenerator.UI;

namespace TestGenerator.TerminalTab;

public class RunTerminal: Terminal
{
    public RunTerminal()
    {
        Box.IsReadOnly = true;
    }
    
    protected override string Prompt => "";

    public async Task<Process?> Run(string? command)
    {
        if (CurrentProcess == null)
        {
            Clear();
            Write(command + "\n");
            Box.IsReadOnly = false;
            var proc = await RunProcess(command);
            Box.IsReadOnly = true;
            return proc;
        }

        return null;
    }

    protected override async Task<Process?> RunProcess(string? command)
    {
        var proc = await base.RunProcess(command);
        if (proc != null)
        {
            Write($"\nProcess finished with exit code {proc.ExitCode}\n");
        }

        return proc;
    }
}