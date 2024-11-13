using System.IO;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.FilesTab;

public class FileCreator : IFileCreator
{
    public string Key => "CreateFile";
    public string Name => "Файл";
    public int Priority => 100;
    public string? Icon => null;

    public SettingsControl? GetSettingsControl()
    {
        var settingsControl = new SettingsControl();
        settingsControl.Add(new StringField{Key = "Name", FieldName = "Имя файла"});
        return settingsControl;
    }

    public void Create(string root, SettingsSection options)
    {
        var filename = options.Get<string>("Name");
        if (!string.IsNullOrWhiteSpace(filename))
            File.Create(Path.Join(root, filename.Trim()));
    }
}