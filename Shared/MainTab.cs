using Avalonia;
using Avalonia.Controls;

namespace Shared;

public class MainTab : UserControl
{
    public string TabKey { get; set; } = new Guid().ToString();
    public string TabName { get; set; } = "";
    
    public virtual void Command(string command, string? data)
    {
    }
}