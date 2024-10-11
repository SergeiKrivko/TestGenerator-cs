using Avalonia;
using Avalonia.Controls;

namespace Shared;

public class SideTab : UserControl
{
    public string TabKey { get; protected set; } = new Guid().ToString();
    public string TabName { get; protected set; } = "";
    
    public virtual void Command(string command, string? data)
    {
    }
}