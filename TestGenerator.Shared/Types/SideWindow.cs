using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public class SideWindow : ISideItem
{
    public string TabKey { get; init; } = new Guid().ToString();
    public string TabName { get; init; } = "";
    public string TabIcon { get; init; } = "";
    public virtual ISideItem.Placement PreferredPlacement => ISideItem.Placement.Left;

    public delegate Window WindowFunc();

    public required WindowFunc Window { get; init; }
}