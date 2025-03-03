using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.MainTabs.Code;
using TestGenerator.Shared.Types;

namespace TestGenerator.Settings;

public partial class DeveloperPage : UserControl
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly PluginsService _pluginsService = Injector.Inject<PluginsService>();
    private readonly ISettingsSection _section;

    public DeveloperPage()
    {
        _section = _appService.GetSettings("Developer");

        InitializeComponent();
        ShowAllTasksBox.IsChecked = _section.Get("showAllTasks", false);
        AllowKillAllTasksBox.IsChecked = _section.Get("allowKillAllTasks", false);
    }

    private async void ButtonOpenLogs_OnClick(object? sender, RoutedEventArgs e)
    {
        await _appService.Request<bool>("openFileWith",
            new OpenFileWithModel { Path = LogService.LogFilePath, ProviderKey = "SystemStdApp" });
    }

    private async void ButtonOpenPlugins_OnClick(object? sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsWindows())
            await _appService.RunProcess(new RunProcessArgs
                { Filename = "explorer", Args = _pluginsService.PluginsPath });
        else if (OperatingSystem.IsLinux())
            await _appService.RunProcess(new RunProcessArgs
                { Filename = "xdg-open", Args = _pluginsService.PluginsPath });
        if (OperatingSystem.IsMacOS())
            await _appService.RunProcess(new RunProcessArgs
                { Filename = "open", Args = _pluginsService.PluginsPath });
    }

    private void ShowAllTasksBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        _section.Set("showAllTasks", ShowAllTasksBox.IsChecked);
    }

    private void AllowKillAllTasksBox_OnClick(object? sender, RoutedEventArgs e)
    {
        _section.Set("allowKillAllTasks", AllowKillAllTasksBox.IsChecked);
    }
}