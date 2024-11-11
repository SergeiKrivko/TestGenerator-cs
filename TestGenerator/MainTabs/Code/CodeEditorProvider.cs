using System.IO;
using TestGenerator.Shared.Types;

namespace TestGenerator.MainTabs.Code;

public class CodeEditorProvider : IEditorProvider
{
    public string Key => "CodeEditor";
    public string Name => " Редактор кода";
    public string[]? Extensions => null;
    public int Priority => 5;

    public OpenedFile Open(string path)
    {
        var widget = new CodeEditor();
        widget.Open(path);
        return new OpenedFile { Name = Path.GetFileName(path), Widget = widget, Path = path };
    }
}