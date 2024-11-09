using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public partial class OpenedFileTab : UserControl
{
    public OpenedFile File { get; }

    public bool IsSelected
    {
        get => RootButton.IsChecked ?? false;
        set => RootButton.IsChecked = value;
    }
    
    public OpenedFileTab(OpenedFile file)
    {
        File = file;
        InitializeComponent();

        NameBlock.Text = file.Name;
    }

    public delegate void OpenedFileHandler(OpenedFile file);

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