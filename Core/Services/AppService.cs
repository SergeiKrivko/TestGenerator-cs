using Shared;
using Shared.Utils;

namespace Core.Services;

public class AppService : AAppService
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

    public override SettingsFile Settings { get; } = SettingsFile.Open(Path.Join(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SergeiKrivko",
        Config.AppName, "Settings.xml"));

    public delegate void ShowHandler(string key);

    public event ShowHandler? OnMainTabShow;
    public event ShowHandler? OnSideTabShow;

    public delegate void CommandFunc(string key, string command, string? data);

    public event CommandFunc? OnMainTabCommand;
    public event CommandFunc? OnSideTabCommand;

    public override void MainTabShow(string key)
    {
        LogService.Logger.Debug($"Show main tab '{key}'");
        OnMainTabShow?.Invoke(key);
    }

    public override void MainTabCommand(string key, string command, string? data = null)
    {
        LogService.Logger.Debug($"Command to main tab '{key}'");
        OnMainTabCommand?.Invoke(key, command, data);
    }
    public override void SideTabShow(string key)
    {
        LogService.Logger.Debug($"Show side tab '{key}'");
        OnSideTabShow?.Invoke(key);
    }

    public override void SideTabCommand(string key, string command, string? data = null)
    {
        LogService.Logger.Debug($"Command to side tab '{key}'");
        OnSideTabCommand?.Invoke(key, command, data);
    }

    public override Logger GetLogger(string name) => LogService.GetLogger(name);
}