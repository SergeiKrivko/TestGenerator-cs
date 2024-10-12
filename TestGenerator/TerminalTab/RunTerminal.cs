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

}