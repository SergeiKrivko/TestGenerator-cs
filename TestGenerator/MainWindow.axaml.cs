using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaluxUI.Utils;
using TestGenerator.Core.Services;
using TestGenerator.Shared;
using TestGenerator.Builds;
using TestGenerator.FilesTab;
using TestGenerator.Shared.Types;
using TestGenerator.TerminalTab;
using TestGenerator.UI;

namespace TestGenerator;

public partial class MainWindow : Window
{
    private readonly AppService _appService = Injector.Inject<AppService>();
    private readonly PluginsService _pluginsService = Injector.Inject<PluginsService>();
    private readonly ProjectsService _projectsService = Injector.Inject<ProjectsService>();
    private readonly ProjectTypesService _projectTypesService = Injector.Inject<ProjectTypesService>();

    private readonly Dictionary<string, MainTab> _mainTabs = [];

    public MainWindow()
    {
        _appService.OnMainTabShow += ShowMainTab;
        _appService.OnSideTabShow += ShowSideTab;
        _pluginsService.OnPluginLoaded += AddPlugin;
        _pluginsService.OnPluginUnloaded += RemovePlugin;

        InitializeComponent();
        LoadWindowBounds();

        _mainTabs.Add("Code", CodeTab);
        MainMenu.Add(CodeTab);

        SideBar.AttachPanel(SideTabsPanel);
        BottomSideBar.AttachPanel(BottomSideTabsPanel);

        AddSideTab(new FilesTab.FilesTab());
        AddSideTab(new SideWindow
        {
            TabKey = "Builds", TabName = "Сценарии запуска", Window = () => new BuildsWindow(), TabIcon =
                "M13.133 11.383L11.909 10.158C11.6285 9.87766 11.2485 9.71975 10.852 9.71882C10.4555 9.71788 10.0747 9.87399 9.79297 10.153L1.28297 18.006C0.972969 18.298 0.769969 18.67 0.750969 19.086C0.741861 19.2928 0.775527 19.4993 0.849859 19.6926C0.924191 19.8858 1.03759 20.0616 1.18297 20.209L3.01097 22.057C3.29377 22.3406 3.67744 22.5006 4.07797 22.502H4.13297C4.54997 22.486 4.92297 22.291 5.21997 21.978L13.127 13.508C13.2671 13.369 13.3785 13.2036 13.4546 " +
                "13.0215C13.5308 12.8394 13.5702 12.644 13.5707 12.4465C13.5711 12.2491 13.5326 12.0535 13.4573 11.871C13.382 11.6885 13.2715 11.5227 13.132 11.383H13.133Z M22.969 8.906L22.954 8.892L21.348 7.302C21.2538 7.20803 21.142 7.13366 21.0189 7.08319C20.8958 7.03273 20.764 7.00717 20.631 7.008C20.416 7.008 20.206 7.076 20.031 7.202C20.031 7.182 20.035 7.162 20.036 7.145C20.057 6.84 20.09 6.38 19.792 5.963C19.4371 5.48456 19.0474 5.033 18.626 4.612C18.002 3.997 16.643 " +
                "2.839 14.561 1.99C13.7715 1.66596 12.9263 1.4995 12.073 1.5C10.593 1.5 9.46897 2.167 9.03797 2.563C8.79395 2.79586 8.57138 3.05022 8.37297 3.323C8.27825 3.45375 8.22826 3.61158 8.23041 3.77302C8.23257 3.93446 8.28677 4.09089 8.38495 4.21906C8.48313 4.34724 8.62005 4.44031 8.77535 4.48443C8.93066 4.52856 9.09606 4.5214 9.24697 4.464C9.37897 4.415 9.51297 4.374 9.64997 4.341C9.93397 4.281 10.225 4.263 10.514 4.287C11.133 4.338 11.864 4.645 12.187 4.901C12.736 " +
                "5.341 13 5.936 13.043 6.827C13.052 7.006 12.681 7.677 12.106 8.443C11.9962 8.5876 11.9425 8.76713 11.955 8.94827C11.9675 9.12941 12.0453 9.29987 12.174 9.428L13.787 11.04C13.9224 11.1759 14.1047 11.2548 14.2965 11.2604C14.4883 11.2659 14.6749 11.1978 14.818 11.07C15.274 10.662 15.968 10.048 16.212 9.898C16.572 9.676 16.83 9.633 16.901 9.626C17.0845 9.60786 17.2692 9.64661 17.43 9.737C17.43 9.745 17.43 9.753 17.427 9.761C17.4244 9.76841 17.4203 9.77522 17.415 " +
                "9.781L17.33 9.863L17.316 9.876C17.2215 9.96985 17.1466 10.0815 17.0955 10.2044C17.0445 10.3274 17.0183 10.4593 17.0185 10.5924C17.0187 10.7256 17.0452 10.8574 17.0966 10.9802C17.148 11.103 17.2233 11.2144 17.318 11.308L18.924 12.898C19.0181 12.9916 19.1298 13.0656 19.2527 13.1157C19.3756 13.1658 19.5072 13.1911 19.64 13.19C19.907 13.19 20.163 13.086 20.354 12.9L22.956 10.33C23.142 10.1405 23.2473 9.88619 23.2498 9.62062C23.2522 9.35505 23.1515 9.09889 22.969 8.906Z"
        });
        AddSideTab(new RunTab());
        AddSideTab(new TerminalTab.TerminalTab());

        Title = $"TestGenerator {_appService.AppVersion}";

        MainMenu.Margin = OperatingSystem.IsMacOS() ? new Thickness(100, 0, 0, 0) : new Thickness(0, 0, 125, 0);

        _pluginsService.Initialize();
        _projectsService.Load();
        _projectsService.TerminateProjectTasksFunc = TerminateTasksWindow.TerminateProjectTasks;
    }

    private void ShowMainTab(string key)
    {
        if (key != MainMenu.Current)
            MainMenu.Current = key;
    }

    private void ShowSideTab(string key)
    {
        if (SideBar.OpenTab(key) || BottomSideBar.OpenTab(key))
            return;
        LogService.Logger.Warning($"Side tab '{key}' not found");
    }

    private void MainMenu_OnTabChanged(object? sender, RoutedEventArgs e)
    {
        foreach (var tab in _mainTabs.Values)
        {
            tab.IsVisible = false;
        }

        _mainTabs[MainMenu.Current].IsVisible = true;
    }

    private void AddSideTab(ISideItem tab)
    {
        switch (tab.PreferredPlacement)
        {
            case ISideItem.Placement.Left:
                SideBar.Add(tab);
                break;
            case ISideItem.Placement.Bottom:
                BottomSideBar.Add(tab);
                break;
            default:
                LogService.Logger.Warning("Tab doesn't added: unknown preferred placement");
                break;
        }
    }

    private void RemoveSideTab(string key)
    {
        SideBar.Remove(key);
    }

    private void AddPlugin(Plugin plugin)
    {
        foreach (var tab in plugin.MainTabs)
        {
            _mainTabs.Add(tab.TabKey, tab);
            MainTabsPanel.Children.Add(tab);
            tab.IsVisible = false;
            MainMenu.Add(tab);
        }

        foreach (var tab in plugin.SideItems.Where(i => i is SideTab))
        {
            AddSideTab(tab);
        }

        foreach (var tab in plugin.SideItems.Where(i => i is SideTab))
        {
            AddSideTab(tab);
        }

        foreach (var provider in plugin.EditorProviders)
        {
            CodeTab.Providers.Add(provider);
        }

        foreach (var creator in plugin.FileCreators)
        {
            FilesTab.FilesTab.FileCreators.Add(creator);
        }

        foreach (var creator in plugin.FileActions)
        {
            FilesTab.FilesTab.FileActions.Add(creator);
        }

        foreach (var item in plugin.FileIcons)
        {
            FileNode.FileIcons[item.Key] = item.Value;
        }

        foreach (var item in plugin.RegexFileIcons)
        {
            Node.RegexIcons[item.Key] = item.Value;
        }

        foreach (var buildType in plugin.ProjectTypes)
        {
            _projectTypesService.Types.Add(buildType.Key, buildType);
        }
    }

    private void RemovePlugin(Plugin plugin)
    {
        foreach (var tab in plugin.MainTabs)
        {
            _mainTabs.Remove(tab.TabKey);
            MainTabsPanel.Children.Remove(tab);
            MainMenu.Remove(tab.TabKey);
        }

        foreach (var tab in plugin.SideItems.Where(i => i is SideTab))
        {
            RemoveSideTab(tab.TabKey);
        }

        foreach (var tab in plugin.SideItems.Where(i => i is SideTab))
        {
            RemoveSideTab(tab.TabKey);
        }

        foreach (var provider in plugin.EditorProviders)
        {
            CodeTab.Providers.Remove(provider);
        }

        foreach (var creator in plugin.FileCreators)
        {
            FilesTab.FilesTab.FileCreators.Remove(creator);
        }

        foreach (var creator in plugin.FileActions)
        {
            FilesTab.FilesTab.FileActions.Remove(creator);
        }

        foreach (var buildType in plugin.ProjectTypes)
        {
            _projectTypesService.Types.Remove(buildType.Key);
        }
    }

    private void LoadWindowBounds()
    {
        if (OperatingSystem.IsMacOS())
            return;

        Width = _appService.Settings.Get<double>("windowWidth");
        Height = _appService.Settings.Get<double>("windowHeight");
        WindowState = _appService.Settings.Get("windowMaximized", false)
            ? WindowState.Maximized
            : WindowState.Normal;
        try
        {
            Position = PixelPoint.Parse(_appService.Settings.Get<string>("windowPosition", ""));
        }
        catch (FormatException)
        {
        }
    }

    private bool _forceQuit;

    private async void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (!_forceQuit)
        {
            e.Cancel = true;
        }

        if (!OperatingSystem.IsMacOS())
        {
            _appService.Settings.Set("windowWidth", Width);
            _appService.Settings.Set("windowHeight", Height);
            _appService.Settings.Set("windowMaximized", WindowState == WindowState.Maximized);
            _appService.Settings.Set("windowPosition", Position.ToString());
        }

        if (!_forceQuit && await TerminateTasksWindow.TerminateAllTasks())
        {
            _forceQuit = true;
            Close();
        }
    }

    private void SideBar_OnCurrentChanged(string? obj)
    {
        SplitView.IsPaneOpen = obj != null;
    }

    private void BottomSideBar_OnCurrentChanged(string? obj)
    {
        BottomPane.IsVisible = obj != null;
    }
}