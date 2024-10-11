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

    public async void Run(string? command)
    {
        if (CurrentProcess == null)
        {
            Clear();
            Write(command + "\n");
            Box.IsReadOnly = false;
            await RunProcess(command);
            Box.IsReadOnly = true;
        }
    }

}