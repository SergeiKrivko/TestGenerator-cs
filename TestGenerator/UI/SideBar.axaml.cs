using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace TestGenerator.UI;

public partial class SideBar : UserControl
{
    private readonly Dictionary<string, Control> _buttons = new();

    private string? _current;

    public string? Current
    {
        get => _current;
        set
        {
            if (value == _current)
                return;
            _current = value;
            foreach (var item in _buttons)
            {
                if (item.Value is ToggleButton toggleButton)
                    toggleButton.IsChecked = value == item.Key;
            }

            TabChanged?.Invoke(value);
        }
    }

    public SideBar()
    {
        InitializeComponent();
    }

    public event Action<string?>? TabChanged;
    public event Action<string>? WindowSelected;

    public void Add(string key, string iconData, bool isWindow = false)
    {
        var pathIcon = new PathIcon { Data = PathGeometry.Parse(iconData) };
        Button button;
        if (isWindow)
        {
            button = new Button
            {
                Content = pathIcon,
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(8),
            };
        }
        else
        {
            button = new ToggleButton
            {
                Content = pathIcon,
                Width = 40,
                Height = 40,
                CornerRadius = new CornerRadius(8),
            };
        }

        button.Click += (obj, args) => _onClicked(key);
        _buttons[key] = button;
        StackPanel.Children.Add(button);
    }

    public void Remove(string key)
    {
        var button = _buttons[key];
        _buttons.Remove(key);
        StackPanel.Children.Remove(button);
    }

    private void _onClicked(string key)
    {
        if (_buttons[key] is not ToggleButton toggle)
        {
            WindowSelected?.Invoke(key);
            return;
        }
        foreach (var item in _buttons)
        {
            if (key != item.Key && item.Value is ToggleButton toggleButton)
                toggleButton.IsChecked = false;
        }

        Current = toggle.IsChecked == true ? key : null;
    }
}