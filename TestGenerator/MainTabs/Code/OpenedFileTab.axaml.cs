using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public partial class OpenedFileTab : UserControl
{
    public OpenedFileModel File { get; }

    public bool IsSelected
    {
        get => RootButton.IsChecked ?? false;
        set => RootButton.IsChecked = value;
    }
    
    public OpenedFileTab(OpenedFileModel file)
    {
        File = file;
        InitializeComponent();

        NameBlock.Text = file.File?.Name;
        Icon.IsVisible = !string.IsNullOrEmpty(file.File?.Icon);
        Icon.Data = PathGeometry.Parse(file.File?.Icon ?? "");
    }

    public delegate void OpenedFileHandler(OpenedFileModel file);

    public event OpenedFileHandler? Selected;
    public event OpenedFileHandler? Deselected;
    public event OpenedFileHandler? CloseRequested;

    private void RootButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (RootButton.IsChecked == true)
            Selected?.Invoke(File);
        else 
            Deselected?.Invoke(File);
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(File);
    }
}