using System.Collections.ObjectModel;

namespace TestGenerator.Shared.Settings.Shared;

internal class FileNode : INode
{
    public string Path { get; set; }

    public string Name => System.IO.Path.GetFileName(Path);

    public event INode.SelectionChangeHandler? SelectionChanged;

    public ObservableCollection<INode> Children => [];

    private bool _selected;

    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            SelectionChanged?.Invoke(Selected);
        }
    }

    public string[] Current => Selected ? [Path] : [];

    public FileNode(string path)
    {
        Path = path;
    }

    public void Select(string[] items)
    {
        Selected = items.Contains(System.IO.Path.GetFullPath(Path));
    }
}