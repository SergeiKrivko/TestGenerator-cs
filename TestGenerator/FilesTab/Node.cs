using System.Collections.ObjectModel;

namespace TestGenerator.FilesTab;

internal abstract class Node
{
    public ObservableCollection<Node> SubNodes { get; }
    public string Title { get; }
    public string Path { get; }
    public string Icon { get; set; } = "";

    protected Node(string path, string title)
    {
        Path = path;
        Title = title;
        SubNodes = new ObservableCollection<Node>();
    }

    protected Node(string path, string title, ObservableCollection<Node> subNodes)
    {
        Path = path;
        Title = title;
        SubNodes = subNodes;
    }

    public abstract void Update();

    public abstract bool Exists { get; }
}