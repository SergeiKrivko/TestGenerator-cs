using Avalonia.Controls;

namespace TestGenerator.Shared.Settings;

public class SettingsNode
{
    public string Path { get; }
    public bool IsVisible => _isVisibleFunc == null || _isVisibleFunc();

    public delegate bool IsVisibleFunction();

    private readonly IsVisibleFunction? _isVisibleFunc;
    
    public Control Control { get; }

    public SettingsNode(string path, Control control, IsVisibleFunction? isVisibleFunction = null)
    {
        Path = path;
        Control = control;
        _isVisibleFunc = isVisibleFunction;
    }
}