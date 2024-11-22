using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public class MainTab : UserControl
{
    public virtual string TabKey { get; protected set; } = new Guid().ToString();
    public virtual string TabName { get; protected set; } = "";

    public virtual int TabPriority { get; } = 10;
}