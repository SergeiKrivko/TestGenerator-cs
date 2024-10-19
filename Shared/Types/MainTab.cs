using Avalonia;
using Avalonia.Controls;

namespace Shared;

public class MainTab : UserControl
{
    public string TabKey { get; protected set; } = new Guid().ToString();
    public string TabName { get; protected set; } = "";
}