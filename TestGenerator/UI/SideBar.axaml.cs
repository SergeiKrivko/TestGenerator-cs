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
    private Dictionary<string, ToggleButton> _buttons = new();

    private string? _current;
    public string? Current { 
        get => _current;
        set
        {
            if (value != _current)
            {
                _current = value;
                foreach (var item in _buttons)
                {
                    item.Value.IsChecked = value == item.Key;
                }

                TabChanged?.Invoke(value);
            }
        }
    }
    
    public SideBar()
    {
        InitializeComponent();
    }

    public delegate void ChangeHandler(string? key);
    public event ChangeHandler? TabChanged;

    public void Add(string key, string iconData)
    {
        var button = new ToggleButton();
        var pathIcon = new PathIcon();
        pathIcon.Data = PathGeometry.Parse(iconData);
        
        button.Content = pathIcon;
        button.Width = 40;
        button.Height = 40;
        button.CornerRadius = new CornerRadius(8);
        button.Click += (obj, args) => _onClicked(key);
        _buttons[key] = button;
        StackPanel.Children.Add(button);
    }

    private void _onClicked(string key)
    {
        foreach (var item in _buttons)
        {
            if (key != item.Key)
                item.Value.IsChecked = false;
        }

        Current = _buttons[key].IsChecked == true ? key : null;
        
        
        TabChanged?.Invoke(key);
    }
}