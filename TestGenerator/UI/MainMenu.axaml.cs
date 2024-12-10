using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using TestGenerator.Settings;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class MainMenu : UserControl
{
    private readonly Dictionary<string, ToggleButton> _buttons = new();
    private readonly List<MainTab> _tabs = [];
    private string _current = "";

    public string Current
    {
        get => _current;
        set => OnTabClicked(value);
    }
    
    public static readonly RoutedEvent<RoutedEventArgs> TabChangeEvent =
        RoutedEvent.Register<MainMenu, RoutedEventArgs>("tabChangeEvent", RoutingStrategies.Bubble);
    
    public MainMenu()
    {
        InitializeComponent();
    }
    
    public event EventHandler<RoutedEventArgs>? TabChanged
    {
        add => AddHandler(TabChangeEvent, value);
        remove => RemoveHandler(TabChangeEvent, value);
    }

    public void Add(MainTab tab)
    {
        var button = new ToggleButton()
        {
            Content = tab.TabName
        };
        button.Click += (obj, args) => OnTabClicked(tab.TabKey);
        _tabs.Add(tab);
        _buttons[tab.TabKey] = button;
        SortButtons();
        if (_buttons.Count == 1)
        {
            OnTabClicked(tab.TabKey);
        }
    }

    private void SortButtons()
    {
        ButtonsPanel.Children.Clear();
        foreach (var tab in _tabs.OrderByDescending(t => t.TabPriority))
        {
            ButtonsPanel.Children.Add(_buttons[tab.TabKey]);
        }
    }

    public void Remove(string key)
    {
        var button = _buttons[key];
        ButtonsPanel.Children.Remove(button);
        _tabs.RemoveAll(t => t.TabKey == key);
        _buttons.Remove(key);
    }

    private void OnTabClicked(string key)
    {
        foreach (var btn in _buttons.Values)
        {
            btn.IsChecked = false;
        }

        _buttons[key].IsChecked = true;
        _current = key;
        
        var e = new RoutedEventArgs(TabChangeEvent);
        RaiseEvent(e);
    }

    private async void SettingsButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new SettingsWindow();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow != null)
        {
            await dialog.ShowDialog(desktop.MainWindow);
        }
    }
}