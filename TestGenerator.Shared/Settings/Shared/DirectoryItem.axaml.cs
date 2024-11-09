using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TestGenerator.Shared.Settings.Shared;

public partial class DirectoryItem : UserControl, IItem
{
    public string Path { get; set; }

    private string[] _extensions;

    public event IItem.SelectionChangeHandler? SelectionChanged;
    public event Action? Changed;

    private bool _ignoreChildren = false;

    public bool Selected
    {
        get => CheckBox.IsChecked ?? false;
        set
        {
            CheckBox.IsChecked = value;
            foreach (var child in ChildrenPanel.Children)
            {
                if (child is IItem)
                {
                    ((IItem)child).Selected = value;
                }
            }
        }
    }

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
                item.Changed += OnChildSelectionChanged;
                ChildrenPanel.Children.Add(item);
            }
        }

        foreach (var file in Directory.GetFiles(path).Where(p => extensions.Contains(System.IO.Path.GetExtension(p))))
        {
            var item = new FileItem(file, extensions);
            item.SelectionChanged += f => OnChildSelectionChanged();
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

    private void OnChildSelectionChanged()
    {
        if (!_ignoreChildren)
        {
            if (ChildrenPanel.Children.All(c => (c as IItem)?.Selected ?? false))
            {
                CheckBox.IsChecked = true;
                SelectionChanged?.Invoke(Selected);
            } 
            else
            {
                CheckBox.IsChecked = false;
                SelectionChanged?.Invoke(Selected);
            }
        }
        Changed?.Invoke();
    }

    private void CheckBox_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        SelectionChanged?.Invoke(Selected);
        _ignoreChildren = true;
        foreach (var child in ChildrenPanel.Children)
        {
            if (child is IItem)
            {
                Console.WriteLine(Selected);
                ((IItem)child).Selected = Selected;
            }
        }

        _ignoreChildren = false;

        Changed?.Invoke();
    }
}