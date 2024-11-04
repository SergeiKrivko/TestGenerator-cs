namespace MyPlugin;

public class MyPlugin : TestGenerator.Shared.Plugin
{
    public MyPlugin()
    {
        Name = "NewPlugin";
        
        MainTabs = [];
        SideTabs = [];
        
        BuildTypes = [];
        ProjectTypes = [];
    }
}