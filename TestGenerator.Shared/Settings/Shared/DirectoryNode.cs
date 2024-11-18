using System.Collections.ObjectModel;

namespace TestGenerator.Shared.Settings.Shared;

internal class DirectoryNode : INode
{
    public string Path { get; set; }

    public string Name => System.IO.Path.GetFileName(Path);

    public event INode.SelectionChangeHandler? SelectionChanged;
    public event Action? Changed;

    private bool _ignoreChildren = false;

    private bool _selected;

    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            _ignoreChildren = true;
            foreach (var child in Children)
            {
                child.Selected = value;
            }
            _ignoreChildren = false;
            SelectionChanged?.Invoke(value);
            Changed?.Invoke();
        }
    }

    public ObservableCollection<INode> Children { get; } = [];

    public string[] Current
    {
        get
        {
            var lst = new List<string>();
            foreach (var child in Children)
            {
                lst.AddRange(child.Current);
            }

            return lst.ToArray();
        }
    }

    private bool DirHasFiles(string path, string[] extensions)
    {
        if (Directory.GetFiles(path).FirstOrDefault(p => extensions.Contains(System.IO.Path.GetExtension(p))) != null)
        {
            return true;
        }

        return Directory.GetDirectories(path).FirstOrDefault(d => DirHasFiles(d, extensions)) != null;
    }

    public DirectoryNode(string path, string[] extensions)
    {
        Path = path;

        foreach (var directory in Directory.GetDirectories(path))
        {
            if (DirHasFiles(directory, extensions))
            {
                var item = new DirectoryNode(directory, extensions);
                item.Changed += OnChildSelectionChanged;
                Children.Add(item);
            }
        }

        foreach (var file in Directory.GetFiles(path).Where(p => extensions.Contains(System.IO.Path.GetExtension(p))))
        {
            var item = new FileNode(file);
            item.SelectionChanged += f => OnChildSelectionChanged();
            Children.Add(item);
        }
    }

    public void Select(string[] items)
    {
        foreach (var child in Children)
        {
            child.Select(items);
        }
    }

    private void OnChildSelectionChanged()
    {
        if (!_ignoreChildren)
        {
            if (Children.All(c => c.Selected) != _selected)
            {
                _selected = !_selected;
                SelectionChanged?.Invoke(_selected);
            }
        }

        Changed?.Invoke();
    }
}