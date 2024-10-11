using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Core.Services;
using Shared;
using TestGenerator.Builds;

namespace TestGenerator;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, MainTab> _mainTabs = new();
    private readonly Dictionary<string, Control> _sideTabs = new();

    public MainWindow()
    {
        InitializeComponent();
        _mainTabs.Add("Code", CodeTab);
        MainMenu.Add("Code", "Код");

        _sideTabs.Add("Files", FilesTabControl);
        _sideTabs.Add("Builds", new BuildsWindow());
        _sideTabs.Add("Terminal", TerminalTabControl);
        SideBar.Add("Files",
            "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z");
        SideBar.Add("Builds",
            "M13.133 11.383L11.909 10.158C11.6285 9.87766 11.2485 9.71975 10.852 9.71882C10.4555 9.71788 10.0747 9.87399 9.79297 10.153L1.28297 18.006C0.972969 18.298 0.769969 18.67 0.750969 19.086C0.741861 19.2928 0.775527 19.4993 0.849859 19.6926C0.924191 19.8858 1.03759 20.0616 1.18297 20.209L3.01097 22.057C3.29377 22.3406 3.67744 22.5006 4.07797 22.502H4.13297C4.54997 22.486 4.92297 22.291 5.21997 21.978L13.127 13.508C13.2671 13.369 13.3785 13.2036 13.4546 " +
            "13.0215C13.5308 12.8394 13.5702 12.644 13.5707 12.4465C13.5711 12.2491 13.5326 12.0535 13.4573 11.871C13.382 11.6885 13.2715 11.5227 13.132 11.383H13.133Z M22.969 8.906L22.954 8.892L21.348 7.302C21.2538 7.20803 21.142 7.13366 21.0189 7.08319C20.8958 7.03273 20.764 7.00717 20.631 7.008C20.416 7.008 20.206 7.076 20.031 7.202C20.031 7.182 20.035 7.162 20.036 7.145C20.057 6.84 20.09 6.38 19.792 5.963C19.4371 5.48456 19.0474 5.033 18.626 4.612C18.002 3.997 16.643 " +
            "2.839 14.561 1.99C13.7715 1.66596 12.9263 1.4995 12.073 1.5C10.593 1.5 9.46897 2.167 9.03797 2.563C8.79395 2.79586 8.57138 3.05022 8.37297 3.323C8.27825 3.45375 8.22826 3.61158 8.23041 3.77302C8.23257 3.93446 8.28677 4.09089 8.38495 4.21906C8.48313 4.34724 8.62005 4.44031 8.77535 4.48443C8.93066 4.52856 9.09606 4.5214 9.24697 4.464C9.37897 4.415 9.51297 4.374 9.64997 4.341C9.93397 4.281 10.225 4.263 10.514 4.287C11.133 4.338 11.864 4.645 12.187 4.901C12.736 " +
            "5.341 13 5.936 13.043 6.827C13.052 7.006 12.681 7.677 12.106 8.443C11.9962 8.5876 11.9425 8.76713 11.955 8.94827C11.9675 9.12941 12.0453 9.29987 12.174 9.428L13.787 11.04C13.9224 11.1759 14.1047 11.2548 14.2965 11.2604C14.4883 11.2659 14.6749 11.1978 14.818 11.07C15.274 10.662 15.968 10.048 16.212 9.898C16.572 9.676 16.83 9.633 16.901 9.626C17.0845 9.60786 17.2692 9.64661 17.43 9.737C17.43 9.745 17.43 9.753 17.427 9.761C17.4244 9.76841 17.4203 9.77522 17.415 " +
            "9.781L17.33 9.863L17.316 9.876C17.2215 9.96985 17.1466 10.0815 17.0955 10.2044C17.0445 10.3274 17.0183 10.4593 17.0185 10.5924C17.0187 10.7256 17.0452 10.8574 17.0966 10.9802C17.148 11.103 17.2233 11.2144 17.318 11.308L18.924 12.898C19.0181 12.9916 19.1298 13.0656 19.2527 13.1157C19.3756 13.1658 19.5072 13.1911 19.64 13.19C19.907 13.19 20.163 13.086 20.354 12.9L22.956 10.33C23.142 10.1405 23.2473 9.88619 23.2498 9.62062C23.2522 9.35505 23.1515 9.09889 22.969 8.906Z");
        SideBar.Add("Terminal",
            "M3.05178e-05 4C3.05178e-05 4 3.05176e-05 7.53815e-06 4.00003 2.31943e-06H20C20 2.31943e-06 24 2.38419e-06 24 4V14C24 14 24 18 20 18H4.00003C4.00003 18 3.05178e-05 18 3.05178e-05 14V4ZM4.00003 1C4.00003 1 1 1 1.00003 4V14C1 17 4.00003 17 4.00003 17H20C20 17 23 17 23 14.5V4C23 1 20 1 20 1H4.00003ZM9 10.5C10.4 9 9 7.5 9 7.5L5 3L3.5 4.5L7.5 9L3.5 13.5L5 15L9 10.5ZM10 15H20V13H10V15Z");

        AppService.Instance.OnMainTabShow += ShowMainTab;
        AppService.Instance.OnMainTabCommand += MainTabCommand;
        PluginsService.Instance.OnPluginLoaded += _addPlugin;

        PluginsService.Instance.LoadPlugin("TestPlugin.dll");
    }

    private void ShowMainTab(string key)
    {
        if (key != MainMenu.Current)
            MainMenu.Current = key;
    }

    private void MainTabCommand(string key, string command, string? data)
    {
        _mainTabs[key].Command(command, data);
    }

    private void MainMenu_OnTabChanged(object? sender, RoutedEventArgs e)
    {
        foreach (var tab in _mainTabs.Values)
        {
            tab.IsVisible = false;
        }

        _mainTabs[MainMenu.Current].IsVisible = true;
    }

    private async void SideBar_OnTabChanged(object? sender, RoutedEventArgs e)
    {
        foreach (var tab in _sideTabs.Values)
        {
            tab.IsVisible = false;
        }

        if (SideBar.Current is not null)
        {
            var tab = _sideTabs[SideBar.Current];
            if (tab is Window)
            {
                LogService.Logger.Debug($"Opening side dialog '{SideBar.Current}'");
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                    desktop.MainWindow != null)
                {
                    await ((Window)tab).ShowDialog(desktop.MainWindow);
                }
            }
            else
            {
                LogService.Logger.Debug($"Opening side tab '{SideBar.Current}'");
                SplitView.IsPaneOpen = true;
                tab.IsVisible = true;
            }
        }
        else
        {
            SplitView.IsPaneOpen = false;
        }
    }

    private void _addPlugin(Plugin plugin)
    {
        foreach (var tab in plugin.MainTabs)
        {
            _mainTabs.Add(tab.TabKey, tab);
            MainTabsPanel.Children.Add(tab);
            tab.IsVisible = false;
            MainMenu.Add(tab.TabKey, tab.TabName);
        }
    }
}