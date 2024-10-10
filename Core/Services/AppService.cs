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
        Config.AppName, "settings.xml"));

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