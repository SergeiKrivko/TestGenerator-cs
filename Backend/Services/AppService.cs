using Shared;

namespace Backend.Services;

public class AppService : IAppService
{
    private static AppService? _appService;

    public new static AppService Instance
    {
        get
        {
            if (_appService == null)
            {
                _instance = _appService = new AppService();
            }

            return _appService;
        }
    }
    
    public delegate void ShowHandler(string key);

    public event ShowHandler? OnMainTabShow;

    public delegate void CommandFunc(string key, string command, string? data);

    public event CommandFunc? OnMainTabCommand;

    public override void MainTabShow(string key)
    {
        OnMainTabShow?.Invoke(key);
    }

    public override void MainTabCommand(string key, string command, string? data = null)
    {
        OnMainTabCommand?.Invoke(key, command, data);
    }
}