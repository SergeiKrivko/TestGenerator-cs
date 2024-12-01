using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TestGenerator.Core.Services;
using TestGenerator.Core.Types;
using TestGenerator.Shared.Types;

namespace TestGenerator.Builds;

public partial class SubBuildDialog : Window
{
    private BuildSubprocess _subprocess;
    
    public ObservableCollection<ABuild> Builds { get; }
    
    public SubBuildDialog(BuildSubprocess subprocess)
    {
        _subprocess = subprocess;
        InitializeComponent();
        ComboBox.ItemsSource = Builds = BuildsService.Instance.Builds;
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _subprocess.BuildId = (ComboBox.SelectedValue as Build)?.Id;
        Close();
    }
}