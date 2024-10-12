using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Core.Services;
using Core.Types;

namespace TestGenerator.UI;

public partial class BuildRunner : UserControl
{
    public ObservableCollection<Build> Builds { get; }
    
    public BuildRunner()
    {
        Builds = BuildsService.Instance.Builds;
        InitializeComponent();
        ComboBox.ItemsSource = Builds;
    }

    private async void RunButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var build = ComboBox.SelectedValue as Build;
        if (build != null)
            await build.ExecuteConsole();
    }
}