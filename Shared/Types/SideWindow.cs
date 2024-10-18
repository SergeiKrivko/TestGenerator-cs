using Avalonia.Controls;

namespace Shared;

public class SideWindow
{
    public string TabKey { get; init; } = new Guid().ToString();
    public string TabName { get; init; } = "";
    public string TabIcon { get; init; } = "";

    public delegate Window WindowFunc();

    public required WindowFunc Window { get; init; }
}