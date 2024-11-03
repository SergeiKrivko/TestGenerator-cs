namespace MyPlugin;

public class MyPlugin : Shared.Plugin
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