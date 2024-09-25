namespace Shared;

public class Plugin
{
    public string Name { get; private set; }
    public List<MainTab> MainTabs { get; } = new();

    public Plugin(
        string name,
        List<MainTab>? mainTabs = null)
    {
        Name = name;
        if (mainTabs != null)
            MainTabs = mainTabs;
    }
}