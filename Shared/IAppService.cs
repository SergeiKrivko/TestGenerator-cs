namespace Shared;

public abstract class IAppService
{
    protected static IAppService? _instance;

    public static IAppService Instance
    {
        get
        {
            if (_instance == null)
                throw new Exception("App Service not initialized");
            return _instance;
        }
    }

    public abstract void MainTabShow(string key);

    public abstract void MainTabCommand(string key, string command, string? data = null);
}