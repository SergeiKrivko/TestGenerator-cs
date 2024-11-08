using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TestGenerator.Settings;

public partial class PluginsView : UserControl
{
    private object? _lastSelected;
    
    public PluginsView()
    {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TabControl?.SelectedContent == _lastSelected)
            return;
        (TabControl?.SelectedContent as PluginsList)?.Load();
        _lastSelected = TabControl?.SelectedContent;
    }
}