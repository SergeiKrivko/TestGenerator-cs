using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public class MainTab : UserControl
{
    public virtual string TabKey { get; } = new Guid().ToString();
    public virtual string TabName { get; } = "";

    public virtual int TabPriority { get; } = 10;
}