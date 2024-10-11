using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Core.Services;
using Core.Types;

namespace TestGenerator.Builds;

public partial class BuildsWindow : Window
{
    public ObservableCollection<Build> Builds { get; }
    
    public BuildsWindow()
    {
        InitializeComponent();
        Builds = BuildsService.Instance.Builds;
        BuildsList.ItemsSource = Builds;
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        BuildsService.Instance.New("");
    }
}