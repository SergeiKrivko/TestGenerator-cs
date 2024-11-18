using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;

namespace TestGenerator.Shared.Settings.Shared;

public partial class FileItem : UserControl
{
    public static readonly StyledProperty<INode> NodeProperty = AvaloniaProperty.Register<FileItem, INode>("Node");

    public INode Node
    {
        get => GetValue(NodeProperty);
        set => SetValue(NodeProperty, value);
    }
    
    
    public FileItem()
    {
        InitializeComponent();
        OnInit();
    }

    private async void OnInit()
    {
        await Task.Delay(100);
        NameBlock.Text = Node.Name;
        Node.SelectionChanged += selected => CheckBox.IsChecked = selected;
        CheckBox.IsChecked = Node.Selected;
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (CheckBox.IsChecked != Node.Selected)
            Node.Selected = CheckBox.IsChecked ?? false;
    }
}