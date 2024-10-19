using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Shared.Settings.Shared;

public partial class FileItem : UserControl, IItem
{
    public string Path { get; set; }

    private string[] _extensions;

    public event IItem.SelectionChangeHandler? SelectionChanged;

    public bool Selected => CheckBox.IsChecked ?? false;

    public string[] Current => Selected ? [Path] : [];

    public FileItem(string path, string[] extensions)
    {
        Path = path;
        _extensions = extensions;
        InitializeComponent();
        NameBlock.Text = System.IO.Path.GetFileName(path);
    }

    public void Select(IEnumerable<string> items)
    {
        CheckBox.IsChecked = items.Contains(Path);
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SelectionChanged?.Invoke(Selected);
    }
}