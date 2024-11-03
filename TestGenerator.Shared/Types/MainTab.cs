using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public class MainTab : UserControl
{
    public string TabKey { get; protected set; } = new Guid().ToString();
    public string TabName { get; protected set; } = "";
}