using System.IO;
using TestGenerator.Shared.Settings;
using TestGenerator.Shared.Types;
using TestGenerator.Shared.Utils;

namespace TestGenerator.FilesTab;

public class DirectoryCreator : IFileCreator
{
    public string Key => "CreateDirectory";
    public string Name => "Папку";
    public int Priority => 100;
    public string? Icon => null;

    public SettingsControl? GetSettingsControl()
    {
        var settingsControl = new SettingsControl();
        settingsControl.Add(new StringField{Key = "Name", FieldName = "Имя папки"});
        return settingsControl;
    }

    public void Create(string root, SettingsSection options)
    {
        var filename = options.Get<string>("Name");
        if (!string.IsNullOrWhiteSpace(filename))
            Directory.CreateDirectory(Path.Join(root, filename.Trim()));
    }
}