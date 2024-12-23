using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public interface IProjectCreator
{
    public string Key { get; }
    public string Name { get; }
    public string? Icon { get; }

    public Control GetControl();

    public string Path { get; }
    
    public Task Initialize(AProject project, Control control);
}