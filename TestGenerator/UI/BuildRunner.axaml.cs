using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Core.Services;
using TestGenerator.Shared.Types;

namespace TestGenerator.UI;

public partial class BuildRunner : UserControl
{
    public ObservableCollection<ABuild> Builds { get; }
    
    public BuildRunner()
    {
        Builds = BuildsService.Instance.Builds;
        InitializeComponent();
        ComboBox.ItemsSource = Builds;
    }

    private async void RunButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ComboBox.SelectedValue is ABuild build)
        {
            AppService.Instance.ShowSideTab("Run");
            await build.ExecuteConsole();
        }
    }
}