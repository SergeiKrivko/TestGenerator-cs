using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGenerator.Core.Services;
using TestGenerator.MainTabs.Code;
using TestGenerator.Shared.Types;

namespace TestGenerator.Settings;

public partial class DeveloperPage : UserControl
{
    public DeveloperPage()
    {
        InitializeComponent();
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
}