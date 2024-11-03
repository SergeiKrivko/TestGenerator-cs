using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TestGenerator.Shared.Settings.Shared;

public partial class DirectoryItem : UserControl, IItem
{
    public string Path { get; set; }

    private string[] _extensions;

    public event IItem.SelectionChangeHandler? SelectionChanged;
    public event Action? Changed;

    public bool Selected => CheckBox.IsChecked ?? false;

    public string[] Current
    {
        get
        {
            var lst = new List<string>();
            foreach (var child in ChildrenPanel.Children)
            {
                foreach (var p in (child as IItem)?.Current ?? [])
                {
                    lst.Add(p);
                }
            }

            return lst.ToArray();
        }
    }

    private bool DirHasFiles(string path, string[] extensions)
    {
        foreach (var f in Directory.GetFiles(path).Where(p => extensions.Contains(System.IO.Path.GetExtension(p))))
        {
            return true;
        }

        foreach (var d in Directory.GetDirectories(path))
        {
            if (DirHasFiles(d, extensions))
                return true;
        }

        return false;
    }

    public DirectoryItem(string path, string[] extensions)
    {
        Path = path;
        _extensions = extensions;
        InitializeComponent();

        NameBlock.Text = System.IO.Path.GetFileName(path);

        foreach (var directory in Directory.GetDirectories(path))
        {
            if (DirHasFiles(directory, extensions))
            {
                var item = new DirectoryItem(directory, extensions);
                ChildrenPanel.Children.Add(item);
            }
        }

        foreach (var file in Directory.GetFiles(path).Where(p => extensions.Contains(System.IO.Path.GetExtension(p))))
        {
            var item = new FileItem(file, extensions);
            item.SelectionChanged += f => Changed?.Invoke();
            ChildrenPanel.Children.Add(item);
        }
    }

    public void Select(string[] items)
    {
        foreach (var child in ChildrenPanel.Children)
        {
            var item = child as IItem;
            if (item != null)
                item.Select(items);
        }
    }

    private void ButtonExpand_OnClick(object? sender, RoutedEventArgs e)
    {
        ButtonExpand.IsVisible = false;
        ButtonCollapse.IsVisible = true;
        ChildrenPanel.IsVisible = true;
    }

    private void ButtonCollapse_OnClick(object? sender, RoutedEventArgs e)
    {
        ButtonExpand.IsVisible = true;
        ButtonCollapse.IsVisible = false;
        ChildrenPanel.IsVisible = false;
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SelectionChanged?.Invoke(Selected);
        Changed?.Invoke();
    }
}