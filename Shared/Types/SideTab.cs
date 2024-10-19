using Avalonia;
using Avalonia.Controls;

namespace Shared;

public abstract class SideTab : UserControl
{
    public virtual string TabKey { get; } = new Guid().ToString();
    public abstract string TabName { get; }
    public abstract string TabIcon { get; }
}