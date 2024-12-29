using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class SideBarItem : UserControl
{
    public static readonly StyledProperty<ISideItem?> ItemProperty =
        AvaloniaProperty.Register<SideBarItem, ISideItem?>(nameof(Item));
    
    public ISideItem? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public bool IsChecked
    {
        get => ToggleButton.IsChecked == true;
        set => ToggleButton.IsChecked = value;
    }

    public event Action<SideTab>? TabShow; 
    public event Action<SideTab>? TabHide; 
    public event Action<SideWindow>? WindowShow; 
    
    public SideBarItem()
    {
        InitializeComponent();
        PropertyChanged += (sender, args) =>
        {
            if (args.Property == ItemProperty)
                Update();
        };
    }

    private void Update()
    {
        SimpleButton.IsVisible = Item is SideWindow;
        ToggleButton.IsVisible = Item is SideTab;
        if (Item != null)
        {
            SimpleIcon.Data = ToggleIcon.Data = PathGeometry.Parse(Item.TabIcon);
        }
    }

    private void SimpleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Item is SideWindow window)
            WindowShow?.Invoke(window);
    }

    private void ToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Item is SideTab tab)
        {
            if (ToggleButton.IsChecked == true)
                TabShow?.Invoke(tab);
            else
                TabHide?.Invoke(tab);
        }
    }
}