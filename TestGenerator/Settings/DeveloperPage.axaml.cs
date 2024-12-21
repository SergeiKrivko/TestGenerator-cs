using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGenerator.Core.Services;
using TestGenerator.MainTabs.Code;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.Settings;

public partial class DeveloperPage : UserControl
{
    private readonly SettingsSection _section = AAppService.Instance.GetSettings("Developer");
    
    public DeveloperPage()
    {
        InitializeComponent();
        ShowAllTasksBox.IsChecked = _section.Get("showAllTasks", false);
        AllowKillAllTasksBox.IsChecked = _section.Get("allowKillAllTasks", false);
    }

    private async void ButtonOpenLogs_OnClick(object? sender, RoutedEventArgs e)
    {
        await AppService.Instance.Request<bool>("openFileWith",
            new OpenFileWithModel { Path = LogService.LogFilePath, ProviderKey = "SystemStdApp" });
    }

    private async void ButtonOpenPlugins_OnClick(object? sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsWindows())
            await AppService.Instance.RunProcess(new RunProcessArgs()
                { Filename = "explorer", Args = PluginsService.Instance.PluginsPath });
        else if (OperatingSystem.IsLinux())
            await AppService.Instance.RunProcess(new RunProcessArgs()
                { Filename = "xdg-open", Args = PluginsService.Instance.PluginsPath });
        if (OperatingSystem.IsMacOS())
            await AppService.Instance.RunProcess(new RunProcessArgs()
                { Filename = "open", Args = PluginsService.Instance.PluginsPath });
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