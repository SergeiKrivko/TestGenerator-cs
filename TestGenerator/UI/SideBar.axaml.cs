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
    public string? Current { get; private set; }
    
    public static readonly RoutedEvent<RoutedEventArgs> TabChangeEvent =
        RoutedEvent.Register<MainMenu, RoutedEventArgs>("tabChangeEvent", RoutingStrategies.Bubble);
    
    public SideBar()
    {
        InitializeComponent();
    }
    
    public event EventHandler<RoutedEventArgs>? TabChanged
    {
        add => AddHandler(TabChangeEvent, value);
        remove => RemoveHandler(TabChangeEvent, value);
    }

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
        
        var e = new RoutedEventArgs(TabChangeEvent);
        RaiseEvent(e);
    }
}