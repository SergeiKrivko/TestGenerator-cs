namespace Shared;

public class Plugin
{
    public string Name { get; private set; }
    public List<MainTab> MainTabs { get; } = [];
    public List<SideTab> SideTabs { get; } = [];
    public List<BuildType> BuildTypes { get; } = [];
    public List<ProjectType> ProjectTypes { get; } = [];

    public Plugin(
        string name,
        List<MainTab>? mainTabs = null,
        List<SideTab>? sideTabs = null,
        List<BuildType>? buildTypes = null,
        List<ProjectType>? projectTypes = null)
    {
        Name = name;
        if (mainTabs != null)
            MainTabs = mainTabs;
        if (sideTabs != null)
            SideTabs = sideTabs;
        if (buildTypes != null)
            BuildTypes = buildTypes;
        if (projectTypes != null)
            ProjectTypes = projectTypes;
    }
}