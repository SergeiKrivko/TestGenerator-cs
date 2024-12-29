namespace TestGenerator.Shared.Types;

public interface ISideItem
{
    public string TabKey { get; }
    public string TabName { get; }
    public string TabIcon { get; }

    public enum Placement
    {
        Left,
        Bottom
    }
    
    public Placement PreferredPlacement { get; }
}