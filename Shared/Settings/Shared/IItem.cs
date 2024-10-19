namespace Shared.Settings.Shared;

public interface IItem
{
    public string Path { get; set; }

    public delegate void SelectionChangeHandler(bool selected);

    public event SelectionChangeHandler? SelectionChanged;
    
    public string[] Current { get; }
    
    public bool Selected { get; }

    public void Select(IEnumerable<string> items);
}