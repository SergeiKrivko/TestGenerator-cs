using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Shared;

namespace TestGenerator.Builds;

public partial class CommandDialog : Window
{
    private BuildSubprocess _subprocess;
    
    public CommandDialog(BuildSubprocess subprocess)
    {
        _subprocess = subprocess;
        InitializeComponent();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _subprocess.Command = TextBox.Text;
        Close();
    }
}