using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Core.Services;
using Shared;

namespace TestGenerator;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, MainTab> _mainTabs = new ();
    private readonly Dictionary<string, Control> _sideTabs = new ();
    
    public MainWindow()
    {
        InitializeComponent();
        _mainTabs.Add("Code", CodeTab);
        MainMenu.Add("Code", "Код");
        
        _sideTabs.Add("Files", FilesTabControl);
        _sideTabs.Add("Terminal", TerminalTabControl);
        SideBar.Add("Files", "M2.34225e-05 2.50001C5.01254e-05 1.50001 0.999996 2.61779e-05 2.50002 6.2865e-06H6.50002C7.5 -8.86825e-06 8.5 1.50001 9.50002 1.50001H20C22 1.50001 22.5 3.50001 22.5 3.50001V15.5C22.5 17.5 20.5 18 20 18H2.50002C0.500011 18 5.88746e-05 16 2.34225e-05 15.5V2.50001ZM2.50002 1.50001C2.50002 1.50001 1.5 1.5 1.5 2.5V5.5H21V4C21 4 20.8246 3 20 3H9.50002C8 3 7 1.5 6.50002 1.50001H2.50002ZM21 7H1.5V15.5C1.5 16.5 2.5 16.5 2.5 16.5H20C21 16.5 21 15.5 21 15.5V7Z");
        SideBar.Add("Terminal", "M3.05178e-05 4C3.05178e-05 4 3.05176e-05 7.53815e-06 4.00003 2.31943e-06H20C20 2.31943e-06 24 2.38419e-06 24 4V14C24 14 24 18 20 18H4.00003C4.00003 18 3.05178e-05 18 3.05178e-05 14V4ZM4.00003 1C4.00003 1 1 1 1.00003 4V14C1 17 4.00003 17 4.00003 17H20C20 17 23 17 23 14.5V4C23 1 20 1 20 1H4.00003ZM9 10.5C10.4 9 9 7.5 9 7.5L5 3L3.5 4.5L7.5 9L3.5 13.5L5 15L9 10.5ZM10 15H20V13H10V15Z");

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

    private void SideBar_OnTabChanged(object? sender, RoutedEventArgs e)
    {
        foreach (var tab in _sideTabs.Values)
        {
            tab.IsVisible = false;
        }

        if (SideBar.Current is not null)
        {
            SplitView.IsPaneOpen = true;
            _sideTabs[SideBar.Current].IsVisible = true;
        }
        else
        {
            SplitView.IsPaneOpen = false;
        }
        
        Console.WriteLine(FilesTabControl.TreeView.Items);
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