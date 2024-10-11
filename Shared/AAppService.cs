using Shared.Utils;

namespace Shared;

public abstract class AAppService
{
    protected static AAppService? _instance;

    public static AAppService Instance
    {
        get
        {
            if (_instance == null)
                throw new Exception("App Service not initialized");
            return _instance;
        }
    }
    
    public abstract SettingsFile Settings { get; }

    public abstract void MainTabShow(string key);

    public abstract void MainTabCommand(string key, string command, string? data = null);

    public abstract void SideTabShow(string key);

    public abstract void SideTabCommand(string key, string command, string? data = null);

    public abstract Logger GetLogger(string name);
}