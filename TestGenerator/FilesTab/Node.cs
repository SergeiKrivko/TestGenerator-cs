using System.Collections.ObjectModel;

namespace TestGenerator.FilesTab;

internal class Node
{
    public ObservableCollection<Node> SubNodes { get; }
    public string Title { get; }
    public string Path { get; }
    public string Icon { get; set; } = "";

    public Node(string path, string title)
    {
        Path = path;
        Title = title;
        SubNodes = new ObservableCollection<Node>();
    }

    public Node(string path, string title, ObservableCollection<Node> subNodes)
    {
        Path = path;
        Title = title;
        SubNodes = subNodes;
    }
}