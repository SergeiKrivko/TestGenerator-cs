using Avalonia.Controls;

namespace TestGenerator.Shared.Types;

public interface IProjectCreator
{
    public string Key { get; }
    public string Name { get; }
    public string? GroupName { get; }
    public string? Icon { get; }

    public Control GetControl();

    public string Create(Control control);
}