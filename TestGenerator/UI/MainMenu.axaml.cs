using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using DynamicData;
using TestGenerator.Settings;

namespace TestGenerator.UI;

public partial class MainMenu : UserControl
{
    private Dictionary<string, ToggleButton> _buttons = new();
    private string _current = "";

    public string Current
    {
        get => _current;
        set => _onClicked(value);
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

    public void Add(string key, string text)
    {
        var button = new ToggleButton();
        button.Content = text;
        button.Click += (obj, args) => _onClicked(key);
        _buttons[key] = button;
        ButtonsPanel.Children.Add(button);
        if (_buttons.Count == 1)
        {
            _onClicked(key);
        }
    }

    public void Remove(string key)
    {
        var button = _buttons[key];
        ButtonsPanel.Children.Remove(button);
        _buttons.Remove(key);
    }

    private void _onClicked(string key)
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