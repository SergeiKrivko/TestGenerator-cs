using System.ComponentModel;

namespace Shared;

public class AppService
{
    private static AppService? _instance;

    public static AppService Instance
    {
        get
        {
            _instance ??= new AppService();
            return _instance;
        }
    }

    public delegate void ShowHandler(string key);
    public event ShowHandler? OnMainTabShow;
    
    public delegate void CommandFunc(string key, string command, string? data);
    public event CommandFunc? OnMainTabCommand;

    public void MainTabShow(string key)
    {
        OnMainTabShow?.Invoke(key);
    }
    
    public void MainTabCommand(string key, string command, string? data = null)
    {
        OnMainTabCommand?.Invoke(key, command, data);
    }
}