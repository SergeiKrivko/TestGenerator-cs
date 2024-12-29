using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class SideBar : UserControl
{
    private readonly Dictionary<string, SideBarItem> _buttons = new();
    private readonly Dictionary<string, SideTab> _tabs = new();

    private string? _current;

    public string? Current
    {
        get => _current;
        set
        {
            if (value == _current)
                return;
            _current = value;
            foreach (var item in _tabs)
            {
                _buttons[item.Key].IsChecked = value == item.Key;
                item.Value.IsVisible = value == item.Key;
            }

            CurrentChanged?.Invoke(value);
        }
    }

    public event Action<string?>? CurrentChanged;

    private Panel? _panel;

    public SideBar()
    {
        InitializeComponent();
    }

    public void AttachPanel(Panel panel)
    {
        if (_panel != null)
            throw new Exception("Panel already attached");
        _panel = panel;
    }

    public void Add(ISideItem item)
    {
        var button = new SideBarItem { Item = item };

        switch (item)
        {
            case SideTab tab:
                if (_panel == null)
                    throw new Exception("Panel not attached");
                _tabs.Add(tab.TabKey, tab);
                _panel.Children.Add(tab);
                tab.IsVisible = false;
                break;
        }

        button.WindowShow += ShowWindow;
        button.TabShow += tab => Current = tab.TabKey;
        button.TabHide += tab => Current = null;
        
        _buttons[item.TabKey] = button;
        StackPanel.Children.Add(button);
    }

    public void Remove(string key)
    {
        var button = _buttons[key];
        _buttons.Remove(key);
        StackPanel.Children.Remove(button);
        _panel?.Children.Remove(_tabs[key]);
        _tabs.Remove(key);
    }

    public bool OpenTab(string key)
    {
        if (!_tabs.ContainsKey(key))
            return false;
        Current = key;
        return true;
    }

    private static async void ShowWindow(SideWindow window)
    {
        if (Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime lifetime &&
            lifetime.MainWindow != null)
        {
            var dialog = window.Window();
            await dialog.ShowDialog(lifetime.MainWindow);
        }
    }
}