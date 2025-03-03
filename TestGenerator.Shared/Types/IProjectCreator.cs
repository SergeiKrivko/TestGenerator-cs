using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public interface IProjectCreator
{
    public string Key { get; }
    public string Name { get; }
    public string? Icon { get; }

    public Control GetControl();

    public string GetPath(Control control);
    
    public Task Initialize(IProject project, Control control, IBackgroundTask backgroundTask, CancellationToken cancellationToken);
}