using System.Collections.ObjectModel;

namespace TestGenerator.Shared.Settings.Shared;

public interface INode
{
    public string Path { get; set; }
    public string Name { get; }

    public ObservableCollection<INode> Children { get; }
    
    public delegate void SelectionChangeHandler(bool selected);

    public event SelectionChangeHandler? SelectionChanged;
    
    public string[] Current { get; }
    
    public bool Selected { get; set; }

    public void Select(string[] items);
}